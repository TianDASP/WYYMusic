using IdentityService.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Services
{
    public class MockSmsSender : ISmsSender
    {
        private readonly ILogger<MockSmsSender> logger;

        public MockSmsSender(ILogger<MockSmsSender> logger)
        {
            this.logger = logger;
        } 

        public Task SendAsync(string phoneNum, params string[] args)
        {
            logger.LogInformation("发送短信到{0}, 参数{1}", phoneNum, args);
            return Task.CompletedTask;
        }
    }
}
