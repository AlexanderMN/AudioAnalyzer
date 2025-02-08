using System.Net;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Web;



var webApplicationBuilder = WebApplication.CreateBuilder(args);

StartUp startUp = new StartUp(webApplicationBuilder);

startUp.ConfigureServices();
startUp.ConfigureHost();
startUp.Build();
startUp.ConfigureMiddleware();
startUp.MapEndpoints();
startUp.Run();