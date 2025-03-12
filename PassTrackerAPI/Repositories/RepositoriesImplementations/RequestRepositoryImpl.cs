using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.Exceptions;

namespace PassTrackerAPI.Repositories.RepositoriesImplementations
{
    public class RequestRepositoryImpl : IRequestRepository
    {
        private readonly DataContext _context;

        public RequestRepositoryImpl(DataContext context)
        {
            _context = context;
        }

        public async Task<RequestDB> GetRequestById(Guid id)
        {
            var req = await _context.Requests.Include(el => el.User).FirstOrDefaultAsync(r => r.Id == id);

            if (req == null) throw new NotFoundException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST);

            return req;
        }
    }
}
