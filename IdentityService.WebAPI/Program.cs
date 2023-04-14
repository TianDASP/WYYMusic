using CommonInitializer;
using IdentityService.Domain;
using IdentityService.Domain.Entity;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Zack.JWT;
using Microsoft.AspNetCore;

namespace IdentityService.WebAPI
{
    public class Program
    {
        public static async Task  Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 配置 配置系统Provider 的源 为数据库中的表
            builder.ConfigureDbConfiguration();
            // 1.加载项目自定义服务 2.为所有DbContext注册连接配置 3.
            builder.ConfigureExtraServices(new InitializerOptions
            {
                EventBusQueueName = "",
                LogFilePath = "d:/temp/xxx.log",
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Identity框架的额外初始化工作
            //登录、注册的项目除了要启用WebApplicationBuilderExtensions中的初始化之外，还要如下的初始化
            //不要用AddIdentity，而是用AddIdentityCore
            //因为用AddIdentity会导致JWT机制不起作用，AddJwtBearer中回调不会被执行，因此总是Authentication校验失败
            //https://github.com/aspnet/Identity/issues/1376
            IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //不能设定RequireUniqueEmail，否则不允许邮箱为空
                //options.User.RequireUniqueEmail = true;
                //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            }
            );
            idBuilder = new IdentityBuilder(idBuilder.UserType, typeof(Domain.Entity.Role), builder.Services);
            idBuilder.AddEntityFrameworkStores<IdDbContext>().AddDefaultTokenProviders()
            //.AddRoleValidator<RoleValidator<Role>>()
                .AddRoleManager<RoleManager<Domain.Entity.Role>>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>();
            // webapi默认开启
            //builder.Services.AddOptions();
            #region Ip限流
            builder.Services.AddMemoryCache();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr");
                options.Configuration = redisConnStr;
            });

            builder.Configuration.AddJsonFile("IpRateLimiting.json", true, true);
            var x = builder.Configuration.GetSection("IpRateLimiting");
            var y = builder.Configuration.GetSection("IpRateLimitPolicies"); 
            builder.Services.Configure<IpRateLimitOptions>(x);
            builder.Services.Configure<IpRateLimitPolicies>(y); 

            //var redisCacheOptions = builder.Configuration.GetSection("RedisCacheOptions").Get<RedisCacheOptions>();

            //var csredis = new CSRedis.CSRedisClient(redisCacheOptions.ConnectionString);

            //RedisHelper.Initialization(csredis);

            builder.Services.AddInMemoryRateLimiting(); 
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion


            // 开发版本与发布版本 不同接口实现的服务注册
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddScoped<ISmsSender, MockSmsSender>();
                builder.Services.AddScoped<IEmailSender, MockEmailSender>();
            }
            else
            {
                
            }

            var app = builder.Build();

            // 启用 ipPolicyStore,执行ip限流策略
            using (var scope = app.Services.CreateScope())
            {
                // get the IpPolicyStore instance
                var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

                // seed IP data from appsettings
                await ipPolicyStore.SeedAsync();
            }
            //app.UseClientRateLimiting();
            app.UseIpRateLimiting();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseZackDefault();
            //app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}