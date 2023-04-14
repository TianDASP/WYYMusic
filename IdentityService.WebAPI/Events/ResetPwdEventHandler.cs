using IdentityService.Domain;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{ 
    [EventName("IdentityService.User.PasswordReset")]
    public class ResetPwdEventHandler : JsonIntegrationEventHandler<ResetPwdEvent>
    {
        private readonly ILogger<ResetPwdEventHandler> logger;
        private readonly ISmsSender smsSender;

        public ResetPwdEventHandler(ILogger<ResetPwdEventHandler> logger, ISmsSender smsSender)
        {
            this.logger = logger;
            this.smsSender = smsSender;
        }

        public override Task HandleJson(string eventName, ResetPwdEvent? eventData)
        {
            //发送密码给被用户的手机
            if(eventData.PhoneNum != null)
            {
                return smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
            } 
            return Task.CompletedTask;
        }
    }
}
