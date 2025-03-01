using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IRequestService
    {
        public Task<string> CreateRequest(RequestCreateDTO request, ClaimsPrincipal user);
    }
}
