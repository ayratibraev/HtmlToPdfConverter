using Microsoft.AspNetCore.Mvc;

namespace Uploader.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Pong");
    }
}