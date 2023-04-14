using IdentityService.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace IdentityService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IdDomainService> ();
            services.AddScoped<IIdRepository, IdRepository>();
        }
    }
}
