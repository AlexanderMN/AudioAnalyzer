using System.Net;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Web;



var webApplicationBuilder = WebApplication.CreateBuilder(args);

WebStartUp webStartUp = new WebStartUp(webApplicationBuilder);

webStartUp.ConfigureServices();
webStartUp.ConfigureHost();
webStartUp.Build();
webStartUp.ConfigureMiddleware();
webStartUp.Run();