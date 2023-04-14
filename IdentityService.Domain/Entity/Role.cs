using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace IdentityService.Domain.Entity
{
    public class Role : IdentityRole<long>
    {
        public Role()
        {

        }
        public Role(string roleName) : base(roleName)
        {
            this.Id = YitIdHelper.NextId();
            //this.Id = Guid.NewGuid();
        }
    }
}
