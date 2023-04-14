namespace IdentityService.WebAPI.Events
{
    public record GenerateLoginByPhoneCodeEvent(  string PhoneNum, string Code);
}
