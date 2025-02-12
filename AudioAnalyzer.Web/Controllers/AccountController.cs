using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AudioAnalyzer.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AccountController : Controller
{
    public IActionResult Login()
    {
        return Ok();
    }

    public IActionResult Logout()
    {
        return Ok();
    }
}
