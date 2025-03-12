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


        /// <summary>
        /// Add user (by id) new role !!For Admins and Deanery only
        /// </summary>
        /// <response code="204">Success</response>
        /// <response code="400">User have this role or user is not confirmed  (or invalid role)</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpPost("deanery")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GiveUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.GiveUserRole(id, role);

            return NoContent();
        }


        /// <summary>
        /// Take a role from user user (by id) !!For Admins and Deanery only
        /// </summary>
        /// <response code="204">Success</response>
        /// <response code="400">User have this role or user is not confirmed (or invalid role)</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpDelete("deanery")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> TakeUserRole([FromQuery] Guid id, [FromQuery] RoleControlDTO role)
        {
            await _adminService.TakeUserRole(id, role);

            return NoContent();
        }


        /// <summary>
        /// Get list of all users with roles (with pagination) !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Returns list of users with pagination</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(UsersPagedListDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("users")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllUsers([FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _userService.GetAllUsers(page, size));
        }


        /// <summary>
        /// Get list of all unconfirmed users (with pagination) !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Returns list of users with pagination</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(UsersPagedListDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("unconfirmed")]
        [Authorize(Roles = "Admin,Deanery")]
        [CheckTokenLife]
        public async Task<IActionResult> GetAllNewUsers([FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _userService.GetAllUsers(page, size, true));
        }


        /// <summary>
        /// Delete user by id !!For Admins only
        /// </summary>
        /// <response code="204">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary role)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpDelete("user")]
        [Authorize(Roles = "Admin")]
        [CheckTokenLife]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            await _adminService.DeleteUser(id);

            return NoContent();
        }


        /// <summary>
        /// Confirm user and give him new role !!For Admins only
        /// </summary>
        /// <response code="204">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary role)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
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
