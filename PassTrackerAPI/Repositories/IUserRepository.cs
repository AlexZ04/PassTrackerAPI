using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.Repositories
{
    public interface IUserRepository
    {
        public Task<UserDb> GetUserById(Guid id);
    }
}
