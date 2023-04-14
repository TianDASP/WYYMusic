using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;
using WYYMusic.Domain;

namespace WYYMusic.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<WYYDomainService>();
            services.AddScoped<IWYYRepository,WYYRepository>();
        }
    }
}
