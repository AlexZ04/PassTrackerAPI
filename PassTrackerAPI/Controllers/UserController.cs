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
    [Admin]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await _userService.GetProfile(User));
    }

    [HttpGet("highest-role/{id}")]
    public async Task<IActionResult> GetUserHighestRole([FromRoute] Guid id)
    {
        return Ok(await _userService.GetUserHighestRole(id));
    }

    [HttpPatch("edit/email")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> EditUserEmail([FromQuery] Guid id, [FromBody] UserEditEmailDTO email)
    {
        await _userService.EditUserEmail(id, email);

        return NoContent();
    }

    [HttpPatch("edit/password")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> EditUserPassword([FromQuery] Guid id, [FromBody] UserEditPasswordDTO password)
    {
        await _userService.EditUserPassword(id, password);

        return NoContent();
    }
}
