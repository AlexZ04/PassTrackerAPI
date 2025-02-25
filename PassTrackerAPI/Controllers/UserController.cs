using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Services;

namespace PassTrackerAPI.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register()
    {
        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser()
    {
        return Ok(await _userService.LoginUser());
    }    
}
