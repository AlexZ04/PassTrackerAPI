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

        [HttpPost("create-request"), Authorize, CheckTokenLife]
        public async Task<IActionResult> CreateReq( RequestCreateDTO request)
        {
            await _requestService.CreateRequest(request, User);
            return Ok();
        }

        [HttpPut("change-request/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> ChangeReq([FromRoute] Guid id,  RequestChangeDTO request) 
        {
            await _requestService.ChangeRequest(id, request, User);
            return Ok();
        }

        [HttpDelete("delete-request/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> DeleteReq([FromRoute] Guid id)
        {
            await _requestService.DeleteRequest(id, User);
            return Ok();
        }

        [HttpGet("get-requestInfo/{id}"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetReqtInfo([FromRoute] Guid id)
        {
            return Ok(await _requestService.GetRequestInfo(id, User));
        }


        //Фильтрация осуществляется по дате начала пропуска, по дате конца, по группе, по фио студента
        [HttpGet("get-all-requests"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetAllReqs([FromQuery] StatusRequestDB? StatusRequestSort,
            [FromQuery] DateTime? StartDate , [FromQuery] DateTime? FinishDate, [FromQuery] int? Group, 
            [FromQuery] string? Name, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _requestService.GetAllRequests(StatusRequestSort , StartDate, FinishDate, Group,  Name,  page,  size));
        }

        [HttpGet("get-all-user-requests"), Authorize, CheckTokenLife]
        public async Task<IActionResult> GetAllUserReqs([FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, int.MaxValue)] int size = 5)
        {
            return Ok(await _requestService.GetAllUserRequests( User, page , size));
        }
    }
}
