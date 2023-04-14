namespace IdentityService.WebAPI.Controllers.Login
{
    public record LoginByPhoneAndCodeRequest(string phoneNumber, string code);
}