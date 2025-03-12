using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Migrations;
using PassTrackerAPI.Repositories;
using System.Security.Claims;


namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class UserServiceImpl : IUserService
    {
        private readonly DataContext _context;
        private readonly IHasherService _hasherService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserServiceImpl(
            DataContext context,
            IHasherService hasherService,
            ITokenService tokenService,
            IUserRepository userRepository)
        {
            _context = context;
            _hasherService = hasherService;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user)
        {
            await CheckEmailIfUsed(user.Email);

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

            var refreshToken = new RefreshTokenDb
            {
                Id = Guid.NewGuid(),
                User = newUser,
                Token = _tokenService.GenerateRefreshToken(),
                Expires = DateTime.Now.AddDays(GeneralSettings.REFRESH_TOKEN_LIFE).ToUniversalTime()
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            string token = _tokenService.CreateAccessTokenById(newUser.Id, await GetUserRoles(newUser));

            return new TokenResponseDTO(token, refreshToken.Token);
        }

        public async Task<TokenResponseDTO> LoginUser(UserLoginDTO user)
        {
            var foundUser = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (foundUser == null || !_hasherService.CheckPassword(foundUser.Password, user.Password))
                throw new CredentialsException(ErrorTitles.CREDENTIALS, ErrorMessages.INVALID_CREDENTIALS);

            string token = _tokenService.CreateAccessTokenById(foundUser.Id, await GetUserRoles(foundUser));

            var refreshToken = new RefreshTokenDb
            {
                Id = Guid.NewGuid(),
                User = foundUser,
                Token = _tokenService.GenerateRefreshToken(),
                Expires = DateTime.Now.AddDays(GeneralSettings.REFRESH_TOKEN_LIFE).ToUniversalTime()
            };

            await _tokenService.HandleTokens(foundUser.Id, refreshToken.Id);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new TokenResponseDTO(token, refreshToken.Token);
        }

        public async Task<TokenResponseDTO> LoginWithRefreshToken(RefreshTokenDTO token)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .ThenInclude(u => u.Roles)
                .FirstOrDefaultAsync(r => r.Token == token.RefreshToken);

            if (refreshToken == null || refreshToken.Expires < DateTime.Now)
                throw new CredentialsException(ErrorTitles.CREDENTIALS, ErrorMessages.REFRESH_TOKEN_IS_NOT_VALID);

            string accessToken = _tokenService.CreateAccessTokenById(refreshToken.User.Id, await GetUserRoles(refreshToken.User));

            refreshToken.Token = _tokenService.GenerateRefreshToken();
            refreshToken.Expires = DateTime.Now.AddDays(GeneralSettings.REFRESH_TOKEN_LIFE).ToUniversalTime();

            await _context.SaveChangesAsync();

            return new TokenResponseDTO(accessToken, refreshToken.Token);
        }

        public async Task Logout(string? token, ClaimsPrincipal user)
        {
            await CheckToken(token);

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var blacklistToken = new BlacklistTokenDb
            {
                Token = token,
                AddTime = DateTime.Now.ToUniversalTime(),
            };

            await _tokenService.HandleTokens(new Guid(userId), Guid.Empty);

            _context.BlacklistTokens.Add(blacklistToken);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfileDTO> GetUserProfileById(Guid id)
        {
            var user = await _userRepository.GetUserById(id);

            var userInfo = new UserProfileDTO
            {
                Name = ConcatName(user.SecondName, user.FirstName, user.MiddleName),
                Group = user.Group,
                Email = user.Email,
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

        // if newUsersOnly == false then get all not-new users
        public async Task<UsersPagedListDTO> GetAllUsers(int page, int size, bool newUsersOnly = false)
        {
            var allUsersQuerable = _context.Users
                .Include(u => u.Roles);

            List<UserDb> allUsers;
            
            if (newUsersOnly)
                allUsers = await allUsersQuerable
                    .Where(u => u.Roles.Select(u => u.Role).Contains(RoleDb.New))
                    .ToListAsync();
            else
                allUsers = await allUsersQuerable
                    .Where(u => !u.Roles.Select(u => u.Role).Contains(RoleDb.New))
                    .ToListAsync();

            List<UserShortDTO> res = new List<UserShortDTO>();

            foreach (var user in allUsers)
            {
                res.Add(new UserShortDTO
                {
                    Id = user.Id,
                    Name = ConcatName(user.SecondName, user.FirstName, user.MiddleName),
                    Group = user.Group
                });
            }

            var paged = res.Skip((page - 1) * size).Take(size).ToList();
            UsersPagedListDTO response = new UsersPagedListDTO
            {
                Requests = paged,
                Pagination = new PageInfoDTO
                {
                    size = size,
                    count = (int)Math.Ceiling((decimal)res.Count() / size),
                    current = page
                }
            };

            return response;
        }

        public async Task<RoleResponseDTO> GetUserHighestRole(Guid id)
        {
            var user = await _userRepository.GetUserById(id);

            var userRoles = await _context.UserRoles
                .Where(u => u.User.Equals(user))
                .Select(u => u.Role)
                .OrderByDescending(u => (int) u)
                .ToListAsync();

            if (userRoles.Count() == 0) userRoles.Add(RoleDb.New);

            return new RoleResponseDTO
            {
                Role = userRoles[0]
            };
        }

        public async Task EditUserEmail(ClaimsPrincipal user, UserEditEmailDTO email)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var foundUser = await _userRepository.GetUserById(new Guid(userId));

            foundUser.Email = email.Email;

            await _context.SaveChangesAsync();
        }

        public async Task EditUserPassword(ClaimsPrincipal user, UserEditPasswordDTO password)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var foundUser = await _userRepository.GetUserById(new Guid(userId));

            foundUser.Password = _hasherService.HashPassword(password.Password);

            await _context.SaveChangesAsync();
        }

        // private helpful functions
        private async Task CheckEmailIfUsed(string email)
        {
            var foundUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (foundUserByEmail != null)
                throw new CredentialsException(ErrorTitles.CREDENTIALS, ErrorMessages.EMAIL_IS_ALREADY_USED);
        }

        private async Task CheckToken(string? token)
        {
            if (token == null || await _context.BlacklistTokens.FindAsync(token) != null)
                throw new UnauthorizedAccessException();
        }

        private string ConcatName(string secondName, string firstName, string thirdName)
        {
            return secondName + " " + firstName + " " + thirdName; 
        }

        private async Task<List<string>> GetUserRoles(UserDb user)
        {
            List<string> roles = new List<string>();

            foreach (var role in user.Roles)
            {
                switch (role.Role)
                {
                    case RoleDb.New:
                        roles.Add("New");
                        break;

                    case RoleDb.Student:
                        roles.Add("Student");
                        break;

                    case RoleDb.Teacher:
                        roles.Add("Teacher");
                        break;

                    case RoleDb.Deanery:
                        roles.Add("Deanery");
                        break;

                    default:
                        break;
                }
            }

            var userAdmin = await _context.Admins.FindAsync(user.Id);

            if (userAdmin != null)
                roles.Add("Admin");

            return roles;
        }
    }
}
