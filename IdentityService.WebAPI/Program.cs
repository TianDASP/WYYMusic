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
            // ���� ����ϵͳProvider ��Դ Ϊ���ݿ��еı�
            builder.ConfigureDbConfiguration();
            // 1.������Ŀ�Զ������ 2.Ϊ����DbContextע���������� 3.
            builder.ConfigureExtraServices(new InitializerOptions
            {
                EventBusQueueName = "",
                LogFilePath = "d:/temp/xxx.log",
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Identity��ܵĶ����ʼ������
            //��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
            //��Ҫ��AddIdentity��������AddIdentityCore
            //��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
            //https://github.com/aspnet/Identity/issues/1376
            IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //�����趨RequireUniqueEmail��������������Ϊ��
                //options.User.RequireUniqueEmail = true;
                //�������У���GenerateEmailConfirmationTokenAsync��֤������
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
            // webapiĬ�Ͽ���
            //builder.Services.AddOptions();
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


            // �����汾�뷢���汾 ��ͬ�ӿ�ʵ�ֵķ���ע��
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddScoped<ISmsSender, MockSmsSender>();
                builder.Services.AddScoped<IEmailSender, MockEmailSender>();
            }
            else
            {
                
            }

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