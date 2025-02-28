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
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;

    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, Required] UserRegisterDTO user)
    {
        TokenResponseDTO? tokenResponse = await _userService.RegisterUser(user, ModelState);

        if (tokenResponse == null)
        {
            return ValidationProblem(); 
        }
        return Ok(tokenResponse);
        

    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody, Required] UserLoginDTO user)
    {
        return Ok(await _userService.LoginUser(user));
    }    

}
