using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using PassTrackerAPI.Data.Entities;


namespace PassTrackerAPI.Controllers
{
    [ApiController]
    [Route("request")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        /// <summary>
        /// Create new request
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">User is not student</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpPost("create-request"), Authorize, CheckTokenLife]
        public async Task<IActionResult> CreateReq(RequestCreateDTO request)
        {
            await _requestService.CreateRequest(request, User);
            return Ok();
        }


        /// <summary>
        /// Change request info
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpPut("change-request/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> ChangeReq([FromRoute, Required] Guid id,  RequestChangeDTO request) 
        {
            await _requestService.ChangeRequest(id, request, User);
            return Ok();
        }


        /// <summary>
        /// Delete request by id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Request is accepted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpDelete("delete-request/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> DeleteReq([FromRoute, Required] Guid id)
        {
            await _requestService.DeleteRequest(id, User);
            return Ok();
        }

        /// <summary>
        /// Get request info (by id)
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Access to request info is denyed</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RequestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("get-requestInfo/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetReqtInfo([FromRoute, Required] Guid id)
        {
            return Ok(await _requestService.GetRequestInfo(id, User));
        }


        /// <summary>
        /// Get all requests with filters !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized (or user don't have necessary roles)</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RequestsPagedListDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("get-all-requests"), Authorize(Roles = "Admin,Deanery,Teacher"), CheckTokenLife]
        public async Task<IActionResult> GetAllReqs(
            [FromQuery] StatusRequestDB? StatusRequestSort,
            [FromQuery] DateTime? StartDate, 
            [FromQuery] DateTime? FinishDate, 
            [FromQuery] int? Group, 
            [FromQuery] string? Name, 
            [FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _requestService.GetAllRequests(StatusRequestSort, StartDate, FinishDate, Group, Name, page, size));
        }

        /// <summary>
        /// Get all user requests
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RequestsPagedListDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("get-all-user-requests"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetAllUserReqs(
            [FromQuery, Range(1, int.MaxValue)] int page = 1, 
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _requestService.GetAllUserRequests(User, page, size));
        }


        /// <summary>
        /// Get all user requests by his id !!For Admins and Deanery only
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RequestsPagedListDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UnsuccessfulRequestDTO), StatusCodes.Status500InternalServerError)]
        [HttpGet("get-all-user-requests/{id}"), Authorize(Roles = "Admin,Deanery,Teacher"), CheckTokenLife]
        public async Task<IActionResult> GetAllUsersReqsById([FromRoute, Required] Guid id,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _requestService.GetAllUsersRequestById(id, page, size));
        }
    }
}
