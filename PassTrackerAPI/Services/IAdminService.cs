using PassTrackerAPI.DTO;

namespace PassTrackerAPI.Services
{
    public interface IAdminService
    {
        public Task GiveUserRole(Guid id, RoleControlDTO role);
        public Task TakeUserRole(Guid id, RoleControlDTO role);
        public Task DeleteUser(Guid id);
    }
}
