using IdentityService.Domain;
using IdentityService.Domain.Entity;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zack.EventBus;

namespace IdentityService.WebAPI.Controllers.UserAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UserAdminController : ControllerBase
    {
        private readonly IIdRepository idRepository;
        private readonly IEventBus eventBus;
        private readonly UserManager<User> userManager;

        public UserAdminController(IEventBus eventBus, IIdRepository idRepository, UserManager<User> userManager)
        {
            this.eventBus = eventBus;
            this.idRepository = idRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public Task<UserDTO[]> FindAllUsers()
        {
            return userManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<UserDTO> FindById(long id)
        {  //Todo 过滤软删除的用户
            var user = await userManager.FindByIdAsync(id.ToString());
            return UserDTO.Create(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddAdminUser(AddAdminUserRequest request)
        {
            (IdentityResult res, User? user, string? password) = await idRepository.AddAdminUserAsync(request.userName, request.phoneNum);
            if (!res.Succeeded)
            {
                var strs = String.Join('\n', res.Errors.Select(x => $"code={x.Code},msg={x.Description}"));
                return BadRequest(strs);
            }
            // 把生成的密码发送给对方---短信或邮件
            var userCreatedEvent = new UserCreatedEvent(user.Id, user.UserName, user.PhoneNumber, password, user.Email);
            eventBus.Publish("IdentityService.User.Created", userCreatedEvent);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAdminUser(long id)
        { 
            await idRepository.RemoveUserAsync(id);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/PasswordReset")]
        public async Task<ActionResult> ResetAdminUserPassword(long id)
        {
            (var res, var user, var password) = await idRepository.ResetPasswordAsync(id);
            if (!res.Succeeded)
            {
                var strs = String.Join('\n', res.Errors.Select(x => $"code={x.Code},msg={x.Description}"));
                return BadRequest(strs);
            }
            // 把重置的密码发送给对方---短信或邮件
            var userResetEvent = new ResetPwdEvent(user.Id, user.UserName, password, user.PhoneNumber);
            eventBus.Publish("IdentityService.User.PasswordReset", userResetEvent);
            return Ok();

        }
    }
}
