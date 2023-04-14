using IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    public interface IIdRepository
    {  
        Task<User?> FindByIdAsync(long userId);
        Task<User?> FindByNameAsync(string userName);
        Task<User?> FindByPhoneNumberAsync(string phoneNumber);

        Task<IdentityResult> CreateAsync(User user, string password); // 创建用户
        Task<IdentityResult> AccessFailedAsync(User user);// 登陆失败记录
        Task<string?> GenerateChangePasswordResetTokenAsync(string phoneNumber);
        //Task<string?> GenerateLoginByPhoneCodeAsync(string phoneNumber);
        //Task<bool> CheckLoginByPhoneAndCodeAsync(string phoneNumber, string code);
        Task<IdentityResult> ChangePasswordAsync(long userId, string newPassword); // 

        // 获取用户的角色
        Task<IList<string>> GetRolesAsync(User user);

        // 为用户user添加角色role
        Task<IdentityResult> AddToRoleAsync(User user, string roleName);

        Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockoutOnFailure = true);
        
        Task<IdentityResult> RemoveUserAsync(long userId);
        // 添加管理员
        Task<(IdentityResult, User?, string?)> AddAdminUserAsync(string userName, string phoneNum);
        //重置密码
        Task<(IdentityResult, User?, string?)> ResetPasswordAsync(long id);
    }
}
