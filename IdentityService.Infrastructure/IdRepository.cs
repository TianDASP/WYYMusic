using IdentityService.Domain;
using IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    internal class IdRepository : IIdRepository
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<IdRepository> logger;
        private readonly IdDbContext dbContext;

        public IdRepository(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<IdRepository> logger, IdDbContext dbContext, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.dbContext = dbContext; 
        }
        public Task<IdentityResult> AccessFailedAsync(User user)
        {
            return userManager.AccessFailedAsync(user);
        }

        public async Task<(IdentityResult, User?, string?)> AddAdminUserAsync(string userName, string phoneNum)
        {
            // 先查询用户是否存在
            if(await FindByNameAsync(userName) != null)
            {
                return (ErrorResult($"已存在用户名{userName}"), null, null);
            }
            if(await FindByPhoneNumberAsync(phoneNum) != null)
            {
                return (ErrorResult($"已存在手机号{phoneNum}"), null, null);
            }
            User user = new User(userName);
            user.PhoneNumber = phoneNum;
            user.PhoneNumberConfirmed = true;
            string password = GeneratePassword();
            var result =await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (result, null, null);    
            }
            result = await AddToRoleAsync(user, "Admin");
            if(!result.Succeeded)
            {
                return (result, null, null);
            }
            return (result, user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            if(!await roleManager.RoleExistsAsync(roleName))
            {
                Role role = new Role(roleName);
                var res = await roleManager.CreateAsync(role);
                if (res.Succeeded == false)
                {
                    return res;
                }
            }
            return await userManager.AddToRoleAsync(user, roleName);
        }

        // 登录后可直接更改新密码
        public async Task<IdentityResult> ChangePasswordAsync(long userId, string newPassword)
        {
            if (newPassword.Length < 6)
            {
                IdentityError error = new IdentityError();
                error.Code = "Password Invalid";
                error.Description = "密码长度不能少于6";
                return IdentityResult.Failed(error);
            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetPwdResult = await userManager.ResetPasswordAsync(user, token, newPassword);
            return resetPwdResult; 
        }
        /// <summary>
        /// 尝试登录,如果开启lockoutOnFailure, 登录失败调用接口自动计数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure = true)
        {
            if (await userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if(success)
            {
                return SignInResult.Success;
            }
            else
            {// 对登录失败计数
                if (lockoutOnFailure)
                {
                    var res = await AccessFailedAsync(user);
                    if (!res.Succeeded)
                    {
                        throw new ApplicationException("AccessFailed failed");
                    }
                }
                return SignInResult.Failed;
            }
        }

        //// 检查验证码是否一致
        //public async Task<bool> CheckLoginByPhoneAndCodeAsync(string phoneNumber, string code)
        //{
        //    var user = await FindByPhoneNumberAsync(phoneNumber);
        //    if (user != null && !string.IsNullOrEmpty(user.Code))
        //    {
        //        if (user.Code == code)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            // 清除原来的code并保存
        //            user.RemoveLoginByPhoneCode();
        //            await _userManager.UpdateAsync(user);
        //        }
        //    }
        //    return false;
        //}

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return userManager.CreateAsync(user, password);    
        }

        public Task<User?> FindByIdAsync(long userId)
        {
            return userManager.FindByIdAsync(userId.ToString());
        }

        public Task<User?> FindByNameAsync(string userName)
        {
            return userManager.FindByNameAsync(userName);
        }
         

        public Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<string?> GenerateChangePasswordResetTokenAsync(string phoneNumber)
        {
            var user = await FindByPhoneNumberAsync(phoneNumber);
            if (user == null)
            {
                return null;
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return token??null;
        }

        //public async Task<string?> GenerateLoginByPhoneCodeAsync(string phoneNumber)
        //{
        //    var user = await FindByPhoneNumberAsync(phoneNumber);
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    // 可以把code放入redis中,就可以设置几分钟过期
        //    var code = user.GenerateLoginByPhoneCode();
        //    await _userManager.UpdateAsync(user);
        //    return code ?? null;
        //}

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> RemoveUserAsync(long userId)
        {
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(); 
            } 
            var userLoginInfos = await userManager.GetLoginsAsync(user);
            foreach (var login in userLoginInfos)
            {
                await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }
            user.SoftDelete();
            var res = await userManager.UpdateAsync(user);
            return res;
        }


        private static IdentityResult ErrorResult(string msg)
        {
            IdentityError idError = new IdentityError { Description = msg };
            return IdentityResult.Failed(idError);
        }


        private string GeneratePassword()
        {
            var options = userManager.Options.Password;
            int length = options.RequiredLength;
            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));
            return password.ToString();
        }

        public async Task<(IdentityResult, User?, string?)> ResetPasswordAsync(long id)
        {
            var user =await FindByIdAsync(id);
            if (user == null)
            {
                return (ErrorResult("用户没找到"), null, null);
            }
            string pwd = GeneratePassword();
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            var res = await userManager.ResetPasswordAsync(user, token, pwd);
            if (!res.Succeeded)
            {
                return (res, null,null);
            }
            return (res, user, pwd);
        }
    }
}
