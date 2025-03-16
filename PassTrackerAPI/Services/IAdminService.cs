using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IAdminService
    {
        public Task GiveUserRole(Guid id, RoleControlDTO role, ClaimsPrincipal user);
        public Task TakeUserRole(Guid id, RoleControlDTO role, ClaimsPrincipal user);
        public Task DeleteUser(Guid id);
        public Task ConfirmUser(Guid id, RoleControlDTO role);
    }
}
