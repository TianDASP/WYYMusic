using FluentValidation;

namespace IdentityService.WebAPI.Controllers.Login
{
    public record SendCodeByPhoneRequest(string phoneNum);

    public class SendCodeByPhoneRequestValidator : AbstractValidator<SendCodeByPhoneRequest>
    {
        public SendCodeByPhoneRequestValidator()
        {
            RuleFor(e => e.phoneNum).NotNull().NotEmpty();
        }
    }
}