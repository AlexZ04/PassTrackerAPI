using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace PassTrackerAPI.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, Required] UserRegisterDTO user)
    {
        return Ok(await _userService.RegisterUser(user));
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody, Required] UserLoginDTO user)
    {
        return Ok(await _userService.LoginUser(user));
    }

    [HttpPost("logout")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> Logout()
    {
        await _userService.Logout(HttpContext.GetTokenAsync("access_token").Result);

        return Ok();
    }

    [HttpGet("profile/{id}")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> GetProfileById([Required] Guid id)
    {
        return Ok(await _userService.GetUserProfileById(id));
    }

    [HttpGet("profile")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await _userService.GetProfile(User));
    }
}
