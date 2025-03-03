using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IUserService
    {
        public Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user);
        public Task<TokenResponseDTO> LoginUser(UserLoginDTO user);
        public Task<UserProfileDTO> GetUserProfileById(Guid id);
        public Task<UserProfileDTO> GetProfile(ClaimsPrincipal user);
        public Task<List<UserShortDTO>> GetAllUsers(bool newUsersOnly = false);
        public Task Logout(string? token);
        public Task<RoleResponseDTO> GetUserHighestRole(Guid id);
    }
}
