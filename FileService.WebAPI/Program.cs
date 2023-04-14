using AspNetCoreRateLimit;
using CommonInitializer;
using FileService.Infrastructure.Services;
using Microsoft.Extensions.FileProviders;
using Zack.EventBus;

namespace FileService.WebAPI
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 配置 配置系统Provider 的源 为数据库中的表
            builder.ConfigureDbConfiguration();
            // 1.加载项目自定义服务 2.为所有DbContext注册连接配置 3.
            builder.ConfigureExtraServices(new InitializerOptions
            {
                EventBusQueueName = "FileService.WebAPI",
                LogFilePath = "d:/temp/WYYFileService.log",
            });
            builder.Services.Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
            //app.UseIpRateLimiting();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();//启用Cors
            app.UseStaticFiles();
            //app.UseZackDefault();
            app.UseEventBus();
            app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}