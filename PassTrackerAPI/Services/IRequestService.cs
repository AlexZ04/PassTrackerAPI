using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IRequestService
    {
        public Task<string> CreateRequest(RequestCreateDTO request, ClaimsPrincipal user);
        public Task<string> ChangeRequest(RequestChangeDTO request, ClaimsPrincipal user);
        public Task<string> DeleteRequest(Guid requestId, ClaimsPrincipal user);
        //public Task<string> GetRequestInfo(Guid requestId, ClaimsPrincipal user);
    }
}
