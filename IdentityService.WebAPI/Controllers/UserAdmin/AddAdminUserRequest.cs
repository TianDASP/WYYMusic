using FluentValidation;

namespace IdentityService.WebAPI.Controllers.UserAdmin
{
    public record AddAdminUserRequest(string userName, string phoneNum);
    public class AddAdminUserRequestValidator : AbstractValidator<AddAdminUserRequest>
    {
        public AddAdminUserRequestValidator()
        {
            RuleFor(x=>x.userName).NotNull().NotEmpty().MaximumLength(20).MinimumLength(2);    
            // 国内手机号
            RuleFor(x=>x.phoneNum).NotNull().NotEmpty().MaximumLength(11);
        }
    }
}