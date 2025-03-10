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
        
        
        [HttpPost("accept-request/{id}"), Authorize(Roles = "Admin,Deanery"), CheckTokenLife]
        public async Task<IActionResult> AcceptReq([FromRoute] Guid id)
        {
            await _deaneryService.AcceptRequest(id);
            return Ok();
        }
        
        [HttpPost("decline-request/{id}"), Authorize(Roles = "Admin,Deanery"), CheckTokenLife]
        public async Task<IActionResult> DeclineReq([FromRoute] Guid id, CommentToDeclinedRequest comment)
        {
            await _deaneryService.DeclineRequest(id, comment);
            return Ok();
        }

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
