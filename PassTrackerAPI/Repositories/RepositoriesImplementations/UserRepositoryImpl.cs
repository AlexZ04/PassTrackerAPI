using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.Repositories.RepositoriesImplementations
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepositoryImpl(DataContext context)
        {
            _context = context;
        }

        public async Task<UserDb> GetUserById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException();

            return user;
        }
    }
}
