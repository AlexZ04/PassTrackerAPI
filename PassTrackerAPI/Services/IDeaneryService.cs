using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IDeaneryService
    {
        public Task AcceptRequest(Guid requestId);
        public Task DeclineRequest(Guid requestId, CommentToDeclinedRequest Comment);
    }
}
