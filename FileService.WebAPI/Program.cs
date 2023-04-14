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
            // ���� ����ϵͳProvider ��Դ Ϊ���ݿ��еı�
            builder.ConfigureDbConfiguration();
            // 1.������Ŀ�Զ������ 2.Ϊ����DbContextע���������� 3.
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
            #region Ip����
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
                        // ���� ipPolicyStore,ִ��ip��������
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
            app.UseCors();//����Cors
            app.UseStaticFiles();
            //app.UseZackDefault();
            app.UseEventBus();
            app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}