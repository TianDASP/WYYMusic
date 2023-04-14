using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    public interface IEmailSender
    {
        // 对方地址, 主题, 邮件内容
        public Task SendAsync(string toEmail, string subject, string body);
    }
}
