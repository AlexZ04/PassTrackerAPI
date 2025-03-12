using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.Repositories
{
    public interface IRequestRepository
    {
        public Task<RequestDB> GetRequestById(Guid id);
    }
}
