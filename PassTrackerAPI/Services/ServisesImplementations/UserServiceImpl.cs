using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using System.Security.Claims;


namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class UserServiceImpl : IUserService
    {
        private readonly DataContext _context;
        private readonly IHasherService _hasherService;
        private readonly ITokenService _tokenService;

        public UserServiceImpl(DataContext context, IHasherService hasherService, ITokenService tokenService)
        {
            _context = context;
            _hasherService = hasherService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user)
        {
            await CheckEmail(user.Email);

            UserDb newUser = new UserDb
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                MiddleName = user.MiddleName,
                Group = user.Group,
                Email = user.Email,
                Password = _hasherService.HashPassword(user.Password),
                CreateTime = DateTime.Now.ToUniversalTime(),
                Roles = new List<UserRoleDb>()
            };

            UserRoleDb newRole = new UserRoleDb
            {
                User = newUser,
                Role = RoleDb.New
            };

            newUser.Roles.Add(newRole);

            _context.UserRoles.Add(newRole);
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            string token = _tokenService.CreateAccessTokenById(newUser.Id);

            return new TokenResponseDTO(token);
        }

        public async Task<TokenResponseDTO> LoginUser(UserLoginDTO user)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (foundUser == null || !_hasherService.CheckPassword(foundUser.Password, user.Password))
                throw new CredentialsException(ErrorTitles.CREDENTIALS_EXCEPTION, ErrorMessages.INVALID_CREDENTIALS);

            string token = _tokenService.CreateAccessTokenById(foundUser.Id);

            return new TokenResponseDTO(token);
        }

        public async Task<UserProfileDTO> GetUserProfileById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {

            }

            var userInfo = new UserProfileDTO
            {
                Name = user.SecondName + " " + user.FirstName + " " + user.MiddleName,
                Group = user.Group,
                Roles = new List<RoleDb>()
            };

            foreach (var elem in user.Roles)
                userInfo.Roles.Add(elem.Role);

            return userInfo;
        }

        public Task<UserProfileDTO> GetProfile(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            return GetUserProfileById(new Guid(userId));
        }

        public async Task CheckEmail(string email)
        {
            var foundUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (foundUserByEmail != null)
                throw new CredentialsException(ErrorTitles.CREDENTIALS_EXCEPTION, ErrorMessages.EMAIL_IS_ALREADY_USED);
        }
    }
}
