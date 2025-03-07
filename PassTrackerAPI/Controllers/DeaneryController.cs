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
    public class DeaneryContoller : ControllerBase
    {
        private readonly IDeaneryService _deaneryService;

        public DeaneryContoller(IDeaneryService deaneryService)
        {
            _deaneryService = deaneryService;
        }
        
        
        [HttpPost("accept-request/{id}"), Authorize, CheckTokenLife, AdminOrDeanery]
        public async Task<IActionResult> AcceptReq([FromRoute] Guid id)
        {
            await _deaneryService.AcceptRequest(id);
            return Ok();
        }
        
        [HttpPost("decline-request/{id}"), Authorize, CheckTokenLife, AdminOrDeanery]
        public async Task<IActionResult> DeclineReq([FromRoute] Guid id, CommentToDeclinedRequest comment)
        {
            await _deaneryService.DeclineRequest(id, comment);
            return Ok();
        }

        [HttpGet("download-requests"), Authorize, CheckTokenLife] //AdminOrDeanery
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
