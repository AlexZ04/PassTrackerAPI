using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IDeaneryService
    {
        public Task<byte[]> DownloadRequest(StatusRequestDB? StatusRequestSort, ClaimsPrincipal user);
    }
}
