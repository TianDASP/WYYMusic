using IdentityService.Domain;
using IdentityService.Domain.Entity;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Zack.EventBus;
using StackExchange.Redis;

namespace IdentityService.WebAPI.Controllers.Login
{
    [Route("api/[controller]/[action]")]
    [ApiController] 
    public class LoginController : ControllerBase
    {
        private readonly IIdRepository idRepository;
        private readonly IdDomainService idDomainService;
        private readonly IEventBus eventBus;
        private readonly IConnectionMultiplexer redisConn;
        public LoginController(IIdRepository idRepository, IdDomainService idDomainService, IEventBus eventBus, IConnectionMultiplexer redisConn)
        {
            this.idRepository = idRepository;
            this.idDomainService = idDomainService;
            this.eventBus = eventBus;
            this.redisConn = redisConn;
        }

        // 这个应该放在UserAdmin 用户管理中,只有管理员有权限
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateFirstAdminUser()
        {
            if (await idRepository.FindByNameAsync("admin") != null)
            { 
                return StatusCode((int)HttpStatusCode.Conflict, "初始管理员已创建");
            }
            User user = new User("admin");
            var r = await idRepository.CreateAsync(user, "123456");
            Debug.Assert(r.Succeeded);
            //
            r = await idRepository.AddToRoleAsync(user, "User");
            Debug.Assert(r.Succeeded);
            r = await idRepository.AddToRoleAsync(user, "Admin");
            Debug.Assert(r.Succeeded);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LoginByUserNameAndPwd(LoginByUserNameAndPwdRequest request)
        {
            (var signInResult, var token) = await idDomainService.LoginByUserNameAndPwdAsync(request.UserName, request.Password);
            if(signInResult.Succeeded)
            {
                return token;
            }else if (signInResult.IsLockedOut)
            {
                return StatusCode((int)HttpStatusCode.Locked, "用户已被锁定");
            }else
            {
                string msg = signInResult.ToString();
                return BadRequest(msg);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangeMyPassword(ChangeMyPasswordRequest request)
        {
            long userId = long.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var resetPwdResult = await idRepository.ChangePasswordAsync(userId, request.Password);
            if(resetPwdResult.Succeeded)
            {
                return Ok();
            }else
            {
                var strs = resetPwdResult.Errors.Select(e => $"code={e.Code},msg={e.Description}");
                return BadRequest(String.Join('\n', strs));
            }
        }

        // 手机验证码登录
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LoginByPhoneAndCode(LoginByPhoneAndCodeRequest request)
        {
            // redis方式: 先去redis验证code,然后去domainservice登录
            (var result, string? token) = await idDomainService.LoginByPhoneAndCodeAsync(request.phoneNumber, request.code);
            if (result.Succeeded)
            {
                // 先删除数据库中的code, 
                var db = redisConn.GetDatabase();
                var res = db.KeyDelete(request.phoneNumber);
                db.KeyExpire(request.phoneNumber, TimeSpan.FromSeconds(0));
                Console.WriteLine(res.ToString());
                return token!;
            }
            return BadRequest("验证失败,请重新发送验证码");
        }

        // 发送手机验证码
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SendCodeByPhone(SendCodeByPhoneRequest request)
        {
            // 可以直接在controller里面 生成code,然后存入redis里
            var code = await idDomainService.GenerateLoginByPhoneCodeAsync(request.phoneNum);
            if (code == null)
            {
                return BadRequest("输入正确的手机号");
            }
            // 发布事件
            GenerateLoginByPhoneCodeEvent codeEvent = new GenerateLoginByPhoneCodeEvent(request.phoneNum, code);
            eventBus.Publish("IdentityService.User.GenerateLoginByPhoneCode", codeEvent);
            // return
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> TestAuthorize()
        {
            return Ok("Hello World");
        }

        [HttpGet]
        public async Task<ActionResult> RedisTest()
        {
            var db = redisConn.GetDatabase();
            db.StringSet("key1", "value1", TimeSpan.FromSeconds(5));
            await Task.Delay(3000);
            Console.WriteLine($"key1:{db.StringGet("key1")}");
            await Task.Delay(3000);
            try
            { 
                var res = db.StringGet("key1");
                Console.WriteLine(res + "-------------");
            }
            catch
            {
                Console.WriteLine("获取值出错");
            }
            return Ok();
        }
    }
}
