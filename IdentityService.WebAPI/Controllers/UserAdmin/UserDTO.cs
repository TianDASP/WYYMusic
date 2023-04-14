using IdentityService.Domain.Entity;

namespace IdentityService.WebAPI.Controllers.UserAdmin
{
    public record UserDTO(long id, string UserName, string PhoneNumber, DateTime CreationTime)
    {
        public static UserDTO Create(User user)
        {
            return new UserDTO(user.Id, user.UserName, user.PhoneNumber, user.CreationTime);
        }
    }
}
