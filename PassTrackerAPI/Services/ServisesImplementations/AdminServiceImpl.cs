
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Repositories;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class AdminServiceImpl : IAdminService
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;

        public AdminServiceImpl(
            DataContext context,
            IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task GiveUserRole(Guid id, RoleControlDTO role, ClaimsPrincipal Execuser)
        {
            var user = await _userRepository.GetUserById(id);

            //CheckRole(role);

            //var roleFromDb = role == RoleControlDTO.Teacher ? RoleDb.Teacher : RoleDb.Deanery;
            var roleFromDb = RoleDb.Teacher;
            if (role == RoleControlDTO.Teacher) {  roleFromDb = RoleDb.Teacher; }

            if (role == RoleControlDTO.Student) { roleFromDb = RoleDb.Student; }
            
            if (role == RoleControlDTO.Deanery) { roleFromDb = RoleDb.Deanery; }

            var execuserId = Execuser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (execuserId == null)
                throw new UnauthorizedAccessException();

            bool isExecuserAdmin = await _context.Admins.FirstOrDefaultAsync(el => el.Id == new Guid(execuserId)) != null ? true : false;


            if (user.Roles.Select(u => u.Role).Contains(roleFromDb))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_IS_ALREADY_HAVE_THIS_ROLE);


            if (user.Roles.Select(u => u.Role).Contains(RoleDb.New))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_IS_NOT_CONFIRMED);

            if (roleFromDb == RoleDb.Deanery && !isExecuserAdmin)
            {
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.ONLY_FOR_ADMINS);
            }

            var newUserRole = BuildUserRoleDb(user, roleFromDb);

            user.Roles.Add(newUserRole);
            _context.UserRoles.Add(newUserRole);

            await _context.SaveChangesAsync();
        }

        public async Task TakeUserRole(Guid id, RoleControlDTO role, ClaimsPrincipal Execuser)
        {
            var user = await _userRepository.GetUserById(id);

            //CheckRole(role);
            var roleFromDb = RoleDb.Teacher;
            if (role == RoleControlDTO.Teacher) { roleFromDb = RoleDb.Teacher; }

            if (role == RoleControlDTO.Student) { roleFromDb = RoleDb.Student; }

            if (role == RoleControlDTO.Deanery) { roleFromDb = RoleDb.Deanery; }

            var execuserId = Execuser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (execuserId == null)
                throw new UnauthorizedAccessException();

            bool isExecuserAdmin = await _context.Admins.FirstOrDefaultAsync(el => el.Id == new Guid(execuserId)) != null ? true : false;

            if (roleFromDb == RoleDb.Deanery && !isExecuserAdmin)
            {
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.ONLY_FOR_ADMINS);
            }

            if (!user.Roles.Select(u => u.Role).Contains(roleFromDb))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_IS_NOT_HAVING_THIS_ROLE);

            var userRole = user.Roles.FirstOrDefault(u => u.Role == roleFromDb);
            _context.UserRoles.Remove(userRole);
            user.Roles.Remove(userRole);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _userRepository.GetUserById(id);

            _context.Users.Remove(user);

            foreach (var role in user.Roles)
                _context.UserRoles.Remove(role);

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmUser(Guid id, RoleControlDTO role)
        {
            var user = await _userRepository.GetUserById(id);

            if (!user.Roles.Select(u => u.Role).Contains(RoleDb.New))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_NOT_NEW);
            var roleFromDb = RoleDb.Teacher;
            if (role == RoleControlDTO.Teacher) { roleFromDb = RoleDb.Teacher; }

            if (role == RoleControlDTO.Student) { roleFromDb = RoleDb.Student; }

            if (role == RoleControlDTO.Deanery) { roleFromDb = RoleDb.Deanery; }

            var newUserRole = new UserRoleDb
            {
                Id = Guid.NewGuid(),
                Role = roleFromDb,
                User = user
            };

            var userActualRole = await _context.UserRoles
                .FirstOrDefaultAsync(u => u.User.Equals(user) && u.Role == RoleDb.New);

            if (userActualRole == null)
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_NOT_NEW);

            _context.UserRoles.Remove(userActualRole);
            user.Roles.Remove(userActualRole);

            user.Roles.Add(newUserRole);
            _context.UserRoles.Add(newUserRole);

            await _context.SaveChangesAsync();
        }

        private UserRoleDb BuildUserRoleDb(UserDb user, RoleDb role)
        {
            UserRoleDb newUserRole = new UserRoleDb
            {
                Id = Guid.NewGuid(),
                User = user,
                Role = role
            };

            return newUserRole;
        }

        private void CheckRole(RoleControlDTO role)
        {
            if (role != RoleControlDTO.Teacher && role != RoleControlDTO.Deanery)
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.INCORRECT_ROLE);
        }
        
    }
}
