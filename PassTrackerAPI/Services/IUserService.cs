using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    }
}
