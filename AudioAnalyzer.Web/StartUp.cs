using AudioAnalyzer.Data.Persistence.Repositories.AudioExtensions;
using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;
using Microsoft.AspNetCore.Http.Features;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;

namespace AudioAnalyzer.Web;

public class StartUp
{
    private WebApplicationBuilder _builder;
    private WebApplication? _app;
    public StartUp(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public void ConfigureServices()
    {
        // Add services to the container.
        _builder.Services.AddControllersWithViews();
        _builder.Services.AddSingleton<HttpClient>();
        _builder.Services.AddSingleton<IAudioExtensionRepository, LocalAudioExtensionRepository>();
        _builder.Services.AddSingleton<IEndpointRepository<string>, LocalEndpointRepository<string>>();
        _builder.Services.AddSingleton<IEndpointRepository<int>, LocalEndpointRepository<int>>();
        _builder.Services.AddSingleton<IEndpointService<string>, EndpointService<string>>();
        _builder.Services.AddSingleton<IEndpointService<int>, EndpointService<int>>();
        _builder.Services.AddSingleton<IFileService, FileService>();
        _builder.Services.AddSingleton<IFileServiceCommunication, FileServiceCommunication>();
        
        _builder.Services.AddSingleton<RabbitMqMessageBroker>();

        _builder.Services.AddMvc()
                .AddSessionStateTempDataProvider();
        _builder.Services.AddSession();
        
        _builder.Services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
        });
    }

    public void ConfigureHost(string url = "https://127.0.0.1:7144", long maxFileSizeMbs = 500)
    {
        _builder.WebHost.UseUrls(url);
        
        _builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxFileSizeMbs * 1024 * 1024);
    }

    public void Build()
    {
        _app = _builder.Build();
    }

    public void ConfigureMiddleware()
    {
        // Configure the HTTP request pipeline.
        if (!_app.Environment.IsDevelopment())
        {
            _app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        _app.UseRabbitMq();
        
        _app.UseStaticFiles();

        _app.UseRouting();

        _app.UseSession();
        _app.UseAuthorization();

        _app.MapControllers();

        _app.MapDefaultControllerRoute();
    }

    public void Run()
    {
        _app.Run();
    }
}