using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using Zack.DomainCommons.Models;

namespace IdentityService.Domain.Entity
{
    public class User : IdentityUser<long>, IHasCreationTime, IHasDeletionTime, ISoftDelete
    {
        // 创建时赋值,且只能赋值一次
        public DateTime CreationTime { get; init; }

        // 封装领域的行为
        public DateTime? DeletionTime { get; private set; }

        public bool IsDeleted { get; private set; }

        // 手机登录验证码
        public string Code { get; private set; }  = string.Empty;

        public User(string userName) : base(userName)
        {
            Id = YitIdHelper.NextId();
            //Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }

        // 封装的行为
        public void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }

        public string GenerateLoginByPhoneCode()
        {
            // 创建一个Random对象
            Random rnum = new Random();
            // 调用Next方法，传入最小值和最大值（不包含）
            int randomNumber = rnum.Next(100000, 1000000);
            this.Code = randomNumber.ToString();
            return this.Code;
        }

        // 清除此账号的手机验证码
        public void RemoveLoginByPhoneCode()
        {
            this.Code = "";
        }
    }
}
