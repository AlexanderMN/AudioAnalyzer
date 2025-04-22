using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Repositories.AudioExtensions;
using AudioAnalyzer.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RabbitMqInfrastructure.Broker;
using RabbitMqInfrastructure.Ftp;
using Endpoint = AudioAnalyzer.Data.Models.Endpoint;

namespace AudioAnalyzer.Web;

public class StartUp
{
    private WebApplicationBuilder _builder;
    private WebApplication? _app;
    public StartUp(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public WebApplicationBuilder ConfigureServices()
    {
        // Add services to the container.
        
        _builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                });

        _builder.Services.AddScoped<FtpStructureBuilder>();
        _builder.Services.AddControllersWithViews();
        _builder.Services.AddRazorPages();
        
        _builder.Services.AddSingleton<HttpClient>();
        _builder.Services.AddSingleton<IAudioExtensionRepository, LocalAudioExtensionRepository>();
        _builder.Services.AddScoped<EndpointService>();
        
        //TODO add remote ftpSettings configuration
        _builder.Services.Configure<FtpSettings>(_builder.Configuration
                                                         .GetSection("RemoteEndpoints")
                                                         .GetSection("FTPServer"));
        
        _builder.Services.AddSingleton<IFtpClient, FtpClient>();
        _builder.Services.AddSingleton<AudioFileNameHandler>();
        _builder.Services.AddScoped<FileServiceCommunication>();
        _builder.Services.AddDbContext<DataBaseContext>();
        _builder.Services.AddScoped<DatabaseDbContextService>();
        _builder.Services.AddScoped<FileUploadHub>();
        _builder.Services.AddSingleton<FileUploadHubConnectionContext>();
        _builder.Services.AddSingleton<BrokerQueueCallbacks, RabbitMqQueueCallbacks>();
        _builder.Services.AddSignalR();
        
        //TODO add remote broker configuration
        _builder.Services.AddSingleton<RabbitMqSetting>(_builder.Configuration
                                                             .GetSection("RemoteEndpoints")
                                                             .GetSection("Broker").Get<RabbitMqSetting>()!);
        
        _builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqMessagePublisher>();
        _builder.Services.AddScoped<RabbitMqPostManager>();
        _builder.Services.AddMvc()
                .AddSessionStateTempDataProvider();
        _builder.Services.AddSession();
        
        _builder.Services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
        });
        
        return _builder;
    }

    public WebApplicationBuilder ConfigureHost(string url = "https://127.0.0.1:7144", long maxFileSizeMbs = 500)
    {
        _builder.WebHost.UseUrls(url);
        _builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxFileSizeMbs * 1024 * 1024);
        
        return _builder;
    }

    public void Build()
    {
        _app = _builder.Build();
    }

    public void ConfigureMiddleware()
    {
        if (_app is null)
        {
            throw new ApplicationException("The application needs to be configured before this method");
        }
        
        // Configure the HTTP request pipeline.
        if (!_app.Environment.IsDevelopment())
        {
            _app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }

        
        _app.UseStaticFiles();

        _app.UseRouting();

        _app.UseSession();
        
        _app.UseAuthentication();
        _app.UseAuthorization();
        
        
        
        //_app.UseRabbitMq();
    }

    public void MapEndpoints()
    {
        if (_app is null)
        {
            throw new ApplicationException("The application needs to be configured before this method");
        }
        
        _app.MapControllers();

        _app.MapDefaultControllerRoute();

        _app.MapHub<FileUploadHub>("/hubs/fileUpload");
        
        _app.MapRazorPages();
    }
    
    public WebApplication Run()
    {
        if (_app is null)
        {
            throw new ApplicationException("The application needs to be configured before this method");
        }
        
        _app.Lifetime.ApplicationStarted.Register(OnAppStarted);
        _app.Lifetime.ApplicationStopping.Register(OnAppStopping);
        _app.Run();
        return _app;
    }

    private void OnAppStarted()
    {
        if (_app is null)
        {
            throw new ApplicationException("Application has not been configured");
        }

        using var scope = _app.Services.CreateScope();
        InfrastructureConfiguration.ConfigureDatabase(scope);
        InfrastructureConfiguration.ConfigureFtpServer(scope);
        InfrastructureConfiguration.ConfigureBroker(scope);
    }

    private void OnAppStopping()
    {
        if (_app is null)
        {
            throw new ApplicationException("Application has not been configured");
        }
        
        RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(
            rabbitMqSetting: _app.Services.GetService<RabbitMqSetting>()!,
            brokerQueueCallbacks: _app.Services.GetService<BrokerQueueCallbacks>()!);
        
        consumer.StopAsync(CancellationToken.None).Wait();
    }
}