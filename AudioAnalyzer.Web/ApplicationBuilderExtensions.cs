using AudioAnalyzer.Infrastructure.Broker;

namespace AudioAnalyzer.Web;

public static class ApplicationBuilderExtensions
{
    private static IMessageBroker _messageBroker { get; set; }
    private static RabbitMqConfig _messageBrokerConfig { get; set; }
    public static IApplicationBuilder UseRabbitMq(this IApplicationBuilder app)
    {
        _messageBroker = app.ApplicationServices.GetService<IMessageBroker>();
        
        _messageBrokerConfig = app.ApplicationServices.GetService<RabbitMqConfig>();
        
        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        
        lifetime.ApplicationStarted.Register(OnStarted);
        lifetime.ApplicationStopping.Register(OnStopping);
        
        return app;
    }

    private async static void OnStarted()
    {
        await _messageBroker?.Start();
        _messageBrokerConfig.RegisterConsumers();
    }

    private static void OnStopping()
    {
        _messageBroker?.Stop();
    }
}
