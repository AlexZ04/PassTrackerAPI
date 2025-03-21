﻿using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IUserService
    {
        public Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user);
        public Task<TokenResponseDTO> LoginUser(UserLoginDTO user);
        public Task<TokenResponseDTO> LoginWithRefreshToken(RefreshTokenDTO token);
        public Task<UserProfileDTO> GetUserProfileById(Guid id);
        public Task<UserProfileDTO> GetProfile(ClaimsPrincipal user);
        public Task<UsersPagedListDTO> GetAllUsers(int? Group, string? Name, int page, int size, bool newUsersOnly = false );
        public Task Logout(string? token, ClaimsPrincipal user);
        public Task<RoleResponseDTO> GetUserHighestRole(Guid id);
        public Task EditUserEmail(ClaimsPrincipal user, UserEditEmailDTO email);
        public Task EditUserPassword(ClaimsPrincipal user, UserEditPasswordDTO password);
            public Task<UsersPagedListDTO> GetAllUsersWithoutAdmins(int? Group, string? Name, int page, int size, bool newUsersOnly = false);
    }
}
