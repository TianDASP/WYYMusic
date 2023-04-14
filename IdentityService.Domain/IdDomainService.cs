using IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Zack.JWT;

namespace IdentityService.Domain
{
    public class IdDomainService
    {
        private readonly IIdRepository repository;
        private readonly IOptions<JWTOptions> optJWT;
        private readonly ITokenService tokenService;
        private readonly IConnectionMultiplexer redisConn;

        public IdDomainService(ITokenService tokenService, IOptions<JWTOptions> optJWT, IIdRepository repository, IConnectionMultiplexer redisConn)
        {
            this.tokenService = tokenService;
            this.optJWT = optJWT;
            this.repository = repository;
            this.redisConn = redisConn;
        }

        private async Task<SignInResult> CheckUserNameAndPwdAsync(string userName, string password)
        {
            // 先验证是否有账号,再尝试登录
            var user = await repository.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var res = await repository.CheckForSignInAsync(user, password);
            return res;
        }

        private async Task<SignInResult> CheckPhoneNumAndPwdAsync(string phoneNum, string password)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNum);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var res = await repository.CheckForSignInAsync(user, password);
            return res;
        }

        public async Task<(SignInResult Result, string? Token)> LoginByPhoneAndPwdAsync(string phoneNum, string password)
        {
            // 校验通过后返回token
            var checkResult = await CheckPhoneNumAndPwdAsync(phoneNum, password);
            if (checkResult.Succeeded)
            {
                // 根据phone获取user,,根据user生成token
                var user = await repository.FindByPhoneNumberAsync(phoneNum);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }

        public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPwdAsync(string phoneNum, string password)
        {
            var checkResult = await CheckUserNameAndPwdAsync(phoneNum, password);
            if (checkResult.Succeeded)
            {
                var user = await repository.FindByNameAsync(phoneNum);
                string token = await BuildTokenAsync(user);
                return (SignInResult.Success, token);
            }
            else
            {
                return (checkResult, null);
            }
        }

        public async Task<string?> GenerateLoginByPhoneCodeAsync(string phoneNumber)
        {
            var user = await repository.FindByPhoneNumberAsync(phoneNumber);
            if (user == null)
            {
                return null;
            }
            // 可以把code放入redis中,就可以设置几分钟过期
            var code = user.GenerateLoginByPhoneCode();
            // Redis 1分钟有效期
            var db = redisConn.GetDatabase();
            db.StringSet(phoneNumber, code, TimeSpan.FromMinutes(1));
            return code;
        }

        public async Task<(SignInResult Result, string? Token)> LoginByPhoneAndCodeAsync(string phoneNum, string code)
        {
            // 
            var db = redisConn.GetDatabase();
            RedisValue codeinRedis = db.StringGet(phoneNum);

            //var checkResult = await repository.CheckLoginByPhoneAndCodeAsync(phoneNum, code);
            if (codeinRedis.HasValue && code == codeinRedis)
            {
                var user = await repository.FindByPhoneNumberAsync(phoneNum);
                if (user != null)
                {
                    string token = await BuildTokenAsync(user);
                    return (SignInResult.Success, token);
                }
                return (SignInResult.Failed, null);
            }
            else
            {
                return (SignInResult.Failed, null);
            }
        }

        private async Task<string> BuildTokenAsync(User user)
        {
            var roles = await repository.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return tokenService.BuildToken(claims, optJWT.Value);
        }
    }
}
