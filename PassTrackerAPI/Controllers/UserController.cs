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


    /// <summary>
    /// Register new user
    /// </summary>
    /// <response code="200">Returns pair of acccess and refresh tokens</response>
    /// <response code="400">Wrong credentials</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, Required] UserRegisterDTO user)
    {
        return Ok(await _userService.RegisterUser(user));
    }


    /// <summary>
    /// Login user. Returns pair of acccess and refresh tokens
    /// </summary>
    /// <response code="200">Returns pair of acccess and refresh tokens</response>
    /// <response code="400">Wrong credentials</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody, Required] UserLoginDTO user)
    {
        return Ok(await _userService.LoginUser(user));
    }


    /// <summary>
    /// Update access token with refresh token
    /// </summary>
    /// <response code="200">Returns pair of acccess and refresh tokens</response>
    /// <response code="400">Invalid refresh token</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPost("login-refresh")]
    public async Task<IActionResult> LoginWithRefreshToken([FromBody, Required] RefreshTokenDTO token)
    {
        return Ok(await _userService.LoginWithRefreshToken(token));
    }


    /// <summary>
    /// Logout from system. Disable both of actual users access and refresh tokens
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">User is not loginned</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _userService.Logout(HttpContext.GetTokenAsync("access_token").Result, User);

        return Ok();
    }


    /// <summary>
    /// Get user profile by id
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpGet("profile/{id}")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> GetProfileById([Required] Guid id)
    {
        return Ok(await _userService.GetUserProfileById(id));
    }


    /// <summary>
    /// Get user profile (for user)
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpGet("profile")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await _userService.GetProfile(User));
    }


    /// <summary>
    /// Get user highest role by id
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpGet("highest-role/{id}")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> GetUserHighestRole([FromRoute] Guid id)
    {
        return Ok(await _userService.GetUserHighestRole(id));
    }


    /// <summary>
    /// Edit users email
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid parameters</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPatch("edit/email")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> EditUserEmail([FromBody] UserEditEmailDTO email)
    {
        await _userService.EditUserEmail(User, email);

        return NoContent();
    }


    /// <summary>
    /// Edit users password
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid parameters</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
    [HttpPatch("edit/password")]
    [Authorize]
    [CheckTokenLife]
    public async Task<IActionResult> EditUserPassword([FromBody] UserEditPasswordDTO password)
    {
        await _userService.EditUserPassword(User, password);

        return NoContent();
    }
}
