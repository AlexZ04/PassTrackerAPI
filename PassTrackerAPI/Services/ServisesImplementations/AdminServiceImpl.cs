﻿
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Repositories;

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

        public async Task GiveUserRole(Guid id, RoleControlDTO role)
        {
            var user = await _userRepository.GetUserById(id);

            CheckRole(role);

            var roleFromDb = role == RoleControlDTO.Teacher ? RoleDb.Teacher : RoleDb.Deanery;

            if (user.Roles.Select(u => u.Role).Contains(roleFromDb))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_IS_ALREADY_HAVE_THIS_ROLE);

            var newUserRole = BuildUserRoleDb(user, roleFromDb);

            user.Roles.Add(newUserRole);
            _context.UserRoles.Add(newUserRole);

            await _context.SaveChangesAsync();
        }

        public async Task TakeUserRole(Guid id, RoleControlDTO role)
        {
            var user = await _userRepository.GetUserById(id);

            CheckRole(role);

            var roleFromDb = role == RoleControlDTO.Teacher ? RoleDb.Teacher : RoleDb.Deanery;

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
