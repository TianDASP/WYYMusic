using CommonInitializer;

namespace WYYMusic.Admin.WebApi
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
                EventBusQueueName = "WYYMusic.Admin.WebAPI",
                LogFilePath = "d:/temp/WYYMusic.Admin.log",
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseZackDefault();  

            app.MapControllers();

            app.Run();
        }
    }
}