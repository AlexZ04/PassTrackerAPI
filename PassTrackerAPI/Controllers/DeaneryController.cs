using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.Xml.Linq;

namespace PassTrackerAPI.Controllers
{
    [ApiController]
    [Route("deanery")]
    public class DeaneryController : ControllerBase
    {
        private readonly IDeaneryService _deaneryService;

        public DeaneryController(IDeaneryService deaneryService)
        {
            _deaneryService = deaneryService;
        }


        /// <summary>
        /// Accept request by id !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpPost("accept-request/{id}"), Authorize(Roles = "Admin,Deanery"), CheckTokenLife]
        public async Task<IActionResult> AcceptReq([FromRoute] Guid id)
        {
            await _deaneryService.AcceptRequest(id);
            return Ok();
        }


        /// <summary>
        /// Deny request by id !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpPost("decline-request/{id}"), Authorize(Roles = "Admin,Deanery"), CheckTokenLife]
        public async Task<IActionResult> DeclineReq([FromRoute] Guid id, CommentToDeclinedRequestDTO comment)
        {
            await _deaneryService.DeclineRequest(id, comment);
            return Ok();
        }


        /// <summary>
        /// Get excel document of requests !!For Admins and Deanery only
        /// </summary>
        /// <response code="204">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("download-requests"), Authorize(Roles = "Admin,Deanery"), CheckTokenLife]
        public async Task<IActionResult> DownloadReqs()
        {
            return File(
            await _deaneryService.DownloadRequest(true),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "requests.xlsx" 
            );
        }

    }
}
