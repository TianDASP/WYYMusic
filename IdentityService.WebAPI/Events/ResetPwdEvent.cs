namespace IdentityService.WebAPI.Events
{
    // 发送重置后的密码给用户
    public record ResetPwdEvent(long Id, string UserName, string Password, string PhoneNum);
}
