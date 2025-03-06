using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Repositories;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class DeaneryServiceImpl: IDeaneryService
    {
        private readonly DataContext _context;

        public DeaneryServiceImpl(DataContext context)
        {
            _context = context;
        }

        public async Task AcceptRequest(Guid requestId)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            req.StatusRequest = StatusRequestDB.Accepted;
            await _context.SaveChangesAsync();
        }

        public async Task DeclineRequest(Guid requestId, CommentToDeclinedRequest Comment)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            req.StatusRequest = StatusRequestDB.Declined;
            req.Comment = Comment.Comment;
            await _context.SaveChangesAsync();
        }
    }
}
