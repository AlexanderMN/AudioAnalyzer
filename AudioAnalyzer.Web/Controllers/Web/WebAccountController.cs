using System.Security.Claims;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AudioAnalyzer.Web.Controllers.Web;

[Route("[controller]")]
public class WebAccountController : Controller
{
    private readonly DataBaseContext _dataBaseContext;
    private readonly FtpStructureBuilder _ftpStructureBuilder;
    public WebAccountController(DataBaseContext dataBaseContext, FtpStructureBuilder ftpStructureBuilder)
    {
        _dataBaseContext = dataBaseContext;
        _ftpStructureBuilder = ftpStructureBuilder;
    }

    [HttpGet]
    [Route("Login")]
    public IActionResult Login()
    {
        return View("Login");
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var user = _dataBaseContext.Users.FirstOrDefault(u => u.UserName == model.Username 
                                                              && u.Password == model.Password);
        if (user == null) 
            return View(model);
        
        await Authenticate(user.Id);
        return Redirect("/Audio");
    }

    [HttpGet]
    [Route("Register")]
    public IActionResult Register()
    {
        return View("Register");
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var user = _dataBaseContext.Users.FirstOrDefault(u => 
                                                             u.UserName == model.Username ||
                                                             u.Email == model.Email);
        if (user == null)
        {
            user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Password = model.Password
            };
            
            _dataBaseContext.Users.Add(user);

            if (await _dataBaseContext.SaveChangesAsync() == 1)
            {
                await _ftpStructureBuilder.CreateUserFolders(user);
            }

            return View("Login");
        }
        else
        {
            var errorMessage = user.UserName == model.Username ? 
                $"Логин '{model.Username}' уже занят." :
                $"Пользователь с Email: '{model.Email}' уже существует .";
                
            return BadRequest(errorMessage);
        }
    }
    [NonAction]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
    [NonAction]
    private async Task Authenticate(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        ClaimsIdentity id = new ClaimsIdentity(claims: claims,
                                               authenticationType: "ApplicationCookie", 
                                               nameType: ClaimsIdentity.DefaultNameClaimType, 
                                               roleType: ClaimsIdentity.DefaultRoleClaimType);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
}
