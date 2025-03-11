using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;
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
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GiveUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.GiveUserRole(id, role);

            return NoContent();
        }

        [HttpDelete("deanery")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> TakeUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.TakeUserRole(id, role);

            return NoContent();
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllUsers([FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _userService.GetAllUsers(page, size));
        }

        [HttpGet("unconfirmed")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllNewUsers([FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _userService.GetAllUsers(page, size, true));
        }

        [HttpDelete("user")]
        [Authorize(Roles = "Admin")]
        [CheckTokenLife]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            await _adminService.DeleteUser(id);

            return NoContent();
        }

        [HttpPost("confirm-user")]
        [Authorize(Roles = "Admin")]
        [CheckTokenLife]
        public async Task<IActionResult> ConfirmUser([FromQuery] Guid id)
        {
            await _adminService.ConfirmUser(id);

            return NoContent();
        }
    }
}
