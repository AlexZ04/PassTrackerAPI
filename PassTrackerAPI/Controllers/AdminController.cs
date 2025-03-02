using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Services;
using System.Data;

namespace PassTrackerAPI.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("deanery")]
        public async Task<IActionResult> GiveUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.GiveUserRole(id, role);

            return NoContent();
        }

        [HttpDelete("deanery")]
        public async Task<IActionResult> DeleteUserDeanery([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.TakeUserRole(id, role);

            return NoContent();
        }
    }
}
