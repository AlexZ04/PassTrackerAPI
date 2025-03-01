using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Filters;
using PassTrackerAPI.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;



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

        [HttpPost("CreateRequest")]
        [Authorize]
        [CheckTokenLife]
        public async Task<IActionResult> CreateReq([FromBody, Required] RequestCreateDTO request)
        {
            return Ok(await _requestService.CreateRequest(request, User));
        }

    }
}
