using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IDeaneryService
    {
        public Task AcceptRequest(Guid requestId);
        public Task DeclineRequest(Guid requestId, CommentToDeclinedRequestDTO Comment);
        public Task<byte[]> DownloadRequest(StatusRequestDB? StatusRequestSort);
    }
}
