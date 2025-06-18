using TestConnectorLibary.Implementation;
using TestConnectorLibary.Interfaces;
using TestConnectorLibary.WevSocket;

namespace TestConnectorLibary.TestAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<WebSocketClientParser>();
            builder.Services.AddSingleton<ITestConnector, TestConnector>();

            builder.Services.AddSingleton<WebSocketLogger>();

            var app = builder.Build();

            //добавлен logger в консоли asp,
            //в будущем можно вытащить этот логер в какуюто внешнию систему ELK, или просто Ѕƒ
            var eventLogger = app.Services.GetRequiredService<WebSocketLogger>();
            eventLogger.SubscribeEvents();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
