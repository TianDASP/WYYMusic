namespace IdentityService.WebAPI.Events
{
    public record UserCreatedEvent(long id, string userName, string? phoneNum, string pwd,string? Email);

}