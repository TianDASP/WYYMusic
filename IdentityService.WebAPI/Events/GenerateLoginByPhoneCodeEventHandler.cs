using IdentityService.Domain;
using Zack.EventBus;

namespace IdentityService.WebAPI.Events
{
    // 处理从MQ发送来的json序列化后的string
    [EventName("IdentityService.User.GenerateLoginByPhoneCode")]
    public class GenerateLoginByPhoneCodeEventHandler : JsonIntegrationEventHandler<GenerateLoginByPhoneCodeEvent>
    {
        private readonly ISmsSender _smsSender;
        private readonly ILogger<GenerateLoginByPhoneCodeEventHandler> _logger;
        public GenerateLoginByPhoneCodeEventHandler(ISmsSender smsSender, ILogger<GenerateLoginByPhoneCodeEventHandler> logger)
        {
            _smsSender = smsSender;
            _logger = logger;
        }
        public override Task HandleJson(string eventName, GenerateLoginByPhoneCodeEvent? eventData)
        { 
            // 发送验证码到用户手机
            return _smsSender.SendAsync(eventData.PhoneNum, eventData.Code);
        }
    }
}
