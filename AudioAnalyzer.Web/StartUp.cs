using System.Configuration;
using System.Text;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Data.Persistence.Repositories.AudioExtensions;
using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;
using Microsoft.AspNetCore.Http.Features;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;
using AudioAnalyzer.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

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
        
        _builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                });
        
        _builder.Services.AddControllersWithViews();
        _builder.Services.AddRazorPages();
        
        _builder.Services.AddSingleton<HttpClient>();
        _builder.Services.AddSingleton<IAudioExtensionRepository, LocalAudioExtensionRepository>();
        _builder.Services.AddSingleton<IEndpointRepository<string>, LocalEndpointRepository<string>>();
        _builder.Services.AddSingleton<IEndpointRepository<int>, LocalEndpointRepository<int>>();
        _builder.Services.AddSingleton<IEndpointService<string>, EndpointService<string>>();
        _builder.Services.AddSingleton<IEndpointService<int>, EndpointService<int>>();
        _builder.Services.Configure<FtpSettings>(_builder.Configuration
                                                         .GetSection("RemoteEndpoints")
                                                         .GetSection("FTPServer"));
        
        _builder.Services.AddSingleton<IFtpClient, FtpClient>();
        _builder.Services.AddSingleton<AudioFileNameHandler>();
        _builder.Services.AddSingleton<IFileServiceCommunication, FileServiceCommunication>();

        var dbOptionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
        _builder.Services.AddSingleton(dbOptionsBuilder.Options);
        
        _builder.Services.AddDbContext<DataBaseContext>();
        
        
        _builder.Services.AddSingleton<IRepository<User>, DbContextUserRepository>();
        _builder.Services.AddSingleton<FileUploadHub>();
        _builder.Services.AddSingleton<BrokerQueueCallbacks, RabbitMqQueueCallbacks>();
        _builder.Services.AddSignalR();
        
        _builder.Services.Configure<RabbitMqSetting>(_builder.Configuration
                                                             .GetSection("RemoteEndpoints")
                                                             .GetSection("Broker"));
        
        _builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqMessagePublisher>();
        _builder.Services.AddHostedService<RabbitMqMessageConsumer>();

        _builder.Services.AddMvc()
                .AddSessionStateTempDataProvider();
        _builder.Services.AddSession();
        
        // var config = _builder.Configuration;
        //
        // _builder.Services.AddAuthentication(x =>
        // {
        //     x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //     x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        // }).AddJwtBearer(options =>
        // {
        //     options.TokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidIssuer = config["Jwt:Issuer"], 
        //         ValidAudience = config["Jwt:Audience"],
        //         IssuerSigningKey = new SymmetricSecurityKey
        //             (Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        //         ValidateIssuer = true,
        //         ValidateAudience = true,
        //         ValidateLifetime = true,
        //         ValidateIssuerSigningKey = true
        //     };
        // });
        //
        // _builder.Services.AddAuthorization();
        
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

        
        _app.UseStaticFiles();

        _app.UseRouting();

        _app.UseSession();
        
        _app.UseAuthentication();
        _app.UseAuthorization();
        
        
        
        //_app.UseRabbitMq();
    }

    public void MapEndpoints()
    {
        _app.MapControllers();

        _app.MapDefaultControllerRoute();

        _app.MapHub<FileUploadHub>("/hubs/fileUpload");
        
        _app.MapRazorPages();
    }
    
    public void Run()
    {
        _app.Run();
    }
}