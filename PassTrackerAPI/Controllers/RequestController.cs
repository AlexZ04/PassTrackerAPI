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

        [HttpPost("CreateRequest"), Authorize, CheckTokenLife]
        public async Task<IActionResult> CreateReq( RequestCreateDTO request)
        {
            return Ok(await _requestService.CreateRequest(request, User));
        }

        [HttpPut("ChangeRequest/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> ChangeReq([FromRoute] Guid id,  RequestChangeDTO request) 
        {
            return Ok(await _requestService.ChangeRequest(id, request, User));
        }

        [HttpDelete("DeleteRequest/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> DeleteReq([FromRoute] Guid id)
        {
            return Ok(await _requestService.DeleteRequest(id, User));
        }

        [HttpGet("GetRequestInfo/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetReqtInfo([FromRoute] Guid id)
        {
            return Ok(await _requestService.GetRequestInfo(id, User));
        }

        [HttpGet("GetAllRequests"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetAllReqs([FromQuery] StatusRequestDB? StatusRequestSort)
        {
            return Ok(await _requestService.GetAllRequests(StatusRequestSort));
        }

        [HttpGet("GetAllUserRequests"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetAllUserReqs()
        {
            return Ok(await _requestService.GetAllUserRequests( User));
        }
    }
}
