using IdentityService.Domain;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{
    [EventName("IdentityService.User.Created")]
    public class UserCreatedEventHandler : JsonIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly ISmsSender smsSender;
        private readonly IEmailSender emailSender;
        public UserCreatedEventHandler(IEmailSender emailSender, ISmsSender smsSender)
        {
            this.emailSender = emailSender;
            this.smsSender = smsSender;
        }
        public override Task HandleJson(string eventName, UserCreatedEvent? eventData)
        {
            if(!string.IsNullOrEmpty( eventData.phoneNum))
            {
                smsSender.SendAsync(eventData.phoneNum, eventData.pwd);
            }
            if (!string.IsNullOrEmpty(eventData.Email))
            {
                emailSender.SendAsync(eventData.Email, "注册初始密码", eventData.pwd);
            } 
            return Task.CompletedTask;
        }
    }
}
