using CommonInitializer;

namespace WYYMusic.Main.WebApi
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