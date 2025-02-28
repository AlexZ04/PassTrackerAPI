using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.DTO;
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

}
