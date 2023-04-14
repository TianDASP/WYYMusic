using IdentityService.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Services
{
    public class MockEmailSender : IEmailSender
    {
        private readonly ILogger<MockEmailSender> logger;

        public MockEmailSender(ILogger<MockEmailSender> logger)
        {
            this.logger = logger;
        }
        public Task SendAsync(string toEmail, string subject, string body)
        {
            logger.LogInformation("发送邮件到 {0} ,标题{1}, 内容{2}", toEmail, subject, body);
            return Task.CompletedTask;
        }
    }
}
