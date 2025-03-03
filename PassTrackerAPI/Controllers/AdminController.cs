using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.Data;

namespace PassTrackerAPI.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;

        public AdminController(IAdminService adminService, IUserService userService)
        {
            _adminService = adminService;
            _userService = userService;
        }

        [HttpPost("deanery")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> GiveUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.GiveUserRole(id, role);

            return NoContent();
        }

        [HttpDelete("deanery")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> DeleteUserDeanery([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.TakeUserRole(id, role);

            return NoContent();
        }

        [HttpGet("users")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [HttpGet("unconfirmed")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllNewUsers()
        {
            return Ok(await _userService.GetAllUsers(true));
        }

        [HttpDelete("user")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            await _adminService.DeleteUser(id);

            return NoContent();
        }
    }
}
