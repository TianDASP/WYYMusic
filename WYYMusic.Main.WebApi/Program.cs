using CommonInitializer;

namespace WYYMusic.Main.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 配置 配置系统Provider 的源 为数据库中的表
            builder.ConfigureDbConfiguration();
            // 1.加载项目自定义服务 2.为所有DbContext注册连接配置 3.
            builder.ConfigureExtraServices(new InitializerOptions
            {
                EventBusQueueName = "WYYMusic.Main.WebAPI",
                LogFilePath = "d:/temp/WYYMusic.Main.log",
            });
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseZackDefault(); ;


            app.MapControllers();

            app.Run();
        }
    }
}