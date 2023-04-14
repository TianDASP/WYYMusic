using CommonInitializer;

namespace WYYMusic.Admin.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // ���� ����ϵͳProvider ��Դ Ϊ���ݿ��еı�
            builder.ConfigureDbConfiguration();
            // 1.������Ŀ�Զ������ 2.Ϊ����DbContextע���������� 3.
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