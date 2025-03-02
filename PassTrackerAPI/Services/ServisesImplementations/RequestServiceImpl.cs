
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class RequestServiceImpl : IRequestService
    {
        private readonly DataContext _context;

        public RequestServiceImpl(DataContext context)
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


        public async Task<string> ChangeRequest(RequestChangeDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            //if (req.User.Id != new Guid(userId)) { throw new UnauthorizedAccessException(); }

            req.StartDate = request.StartDate;
            req.FinishDate = request.FinishDate;
            req.TypeRequest = request.TypeRequest;
            req.Photo = request.Photo;
            await _context.SaveChangesAsync();

            return ("Request has been changed");
        }
        public async Task<string> DeleteRequest(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            //if (req.User.Id != new Guid(userId)) { throw new UnauthorizedAccessException(); }

            if (req.StatusRequest != StatusRequestDB.Accepted)
            {
                _context.Requests.Remove(req);
                await _context.SaveChangesAsync();
                return ("Request has been deleted");
            }
            throw new CredentialsException(ErrorTitles.REQUEST_ERROR, ErrorMessages.CANNOT_DELETE_ACCEPTED_REQUEST);

        }

    }
}
