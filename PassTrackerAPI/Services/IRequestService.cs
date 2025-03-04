using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IRequestService
    {
        public Task<string> CreateRequest(RequestCreateDTO request, ClaimsPrincipal user);
        public Task<string> ChangeRequest(Guid requestId, RequestChangeDTO request, ClaimsPrincipal user);
        public Task<string> DeleteRequest(Guid requestId, ClaimsPrincipal user);
        public Task<RequestDTO> GetRequestInfo(Guid requestId, ClaimsPrincipal user);
        public Task<List<RequestShortDTO>> GetAllRequests(StatusRequestDB? StatusRequestSort); 
        public Task<List<RequestShortDTO>> GetAllUserRequests(ClaimsPrincipal user);
    }
}
