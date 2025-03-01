
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class RequestServiceImpl : IRequestService
    {
        private readonly DataContext _context;

        public RequestServiceImpl(DataContext context,  ITokenService tokenService)
        {
            _context = context;
        }

        public async Task<string> CreateRequest(RequestCreateDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var userProfile = await _context.Users
             .FirstOrDefaultAsync(u => u.Id == new Guid(userId));

            RequestDB newRequest = new RequestDB
            {
                Id = Guid.NewGuid(),
                User = userProfile,
                StartDate = request.StartDate,
                FinishDate = request.FinishDate,
                TypeRequest = request.TypeRequest,
                StatusRequest = StatusRequestDB.Pending,
                Photo = request.Photo
            };

            _context.Requests.Add(newRequest);

            await _context.SaveChangesAsync();

            return("Request has been created");
        }
    }
}
