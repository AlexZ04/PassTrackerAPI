
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Migrations;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class RequestServiceImpl : IRequestService
    {
        private readonly DataContext _context;

        public RequestServiceImpl(DataContext context)
        {
            _context = context;
        }



        public async Task CreateRequest(RequestCreateDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var userProfile = await _context.Users.Include(u => u.Roles)
             .FirstOrDefaultAsync(u => u.Id == new Guid(userId));
            if (!userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Student)) 
            {
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.USER_IS_NOT_STUDENT);
            }
            RequestDB newRequest = new RequestDB
            {
                Id = Guid.NewGuid(),
                User = userProfile,
                StartDate = request.StartDate,
                FinishDate = request.FinishDate,
                TypeRequest = request.TypeRequest,
                StatusRequest = StatusRequestDB.Pending,
                Comment = null,
                Photo = request.Photo
            };

            _context.Requests.Add(newRequest);

            await _context.SaveChangesAsync();

        }


        public async Task ChangeRequest(Guid requestId, RequestChangeDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.Include(el => el.User).FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            if (req.User.Id != new Guid(userId)) { throw new UnauthorizedAccessException(); }

            req.StartDate = request.StartDate;
            req.FinishDate = request.FinishDate;
            req.TypeRequest = request.TypeRequest;
            req.Photo = request.Photo;
            await _context.SaveChangesAsync();


        }
        public async Task DeleteRequest(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.Include(el => el.User).FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            if (req.User.Id != new Guid(userId)) { throw new UnauthorizedAccessException(); }

            if (req.StatusRequest != StatusRequestDB.Accepted)
            {
                _context.Requests.Remove(req);
                await _context.SaveChangesAsync();

            }
            throw new CredentialsException(ErrorTitles.REQUEST_ERROR, ErrorMessages.CANNOT_DELETE_ACCEPTED_REQUEST);

        }

        public async Task<RequestDTO> GetRequestInfo(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.Include(el => el.User).FirstOrDefaultAsync(el => el.Id == requestId);
            var userProfile = await _context.Users.Include(u => u.Roles)
             .FirstOrDefaultAsync(u => u.Id == new Guid(userId));
            bool isUserAmin = await _context.Admins.FirstOrDefaultAsync(el => el.Id == new Guid(userId)) != null ? true : false;

            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
 
            if(userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Deanery) ||
                userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Teacher ) || isUserAmin || req.User.Id == new Guid(userId)) 
            {
                var request = new RequestDTO
                {
                    Id = req.Id,
                    UserName = req.User.SecondName + " " + req.User.SecondName + " " + req.User.MiddleName,
                    StartDate = req.StartDate,
                    FinishDate = req.FinishDate,
                    TypeRequest = req.TypeRequest,
                    StatusRequest = req.StatusRequest,
                    Comment = req.Comment,
                    Photo = req.Photo,
                    Group = req.User.Group
                };

                return request;
            }
            throw new UnauthorizedAccessException(); 


        }



        public async Task<RequestsPagedListModel> GetAllRequests(StatusRequestDB? StatusRequestSort, 
            DateTime? StartDate, DateTime? FinishDate, int? Group, string? Name, int page, int size)
        {
            var requests = await _context.Requests.Include(el => el.User)
            .Select(o => new RequestShortDTO
            {
                Id = o.Id,
                UserName = o.User.SecondName + " " + o.User.SecondName + " " + o.User.MiddleName,
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest,
                Group = o.User.Group

            }).ToListAsync();

            if (StatusRequestSort != null)
            {
                requests = requests.Where(el => el.StatusRequest == StatusRequestSort).ToList();
            }

            if (StartDate != null)
            {
                requests = requests.Where(el => el.StartDate >= StartDate).ToList();
            }
            if (FinishDate != null)
            {
                requests = requests.Where(el => el.FinishDate <= FinishDate).ToList();
            }
            if (Group != null)
            {
                requests = requests.Where(el => el.Group == Group).ToList();
            }
            if(Name != null)
            {
                requests = requests.Where(el => el.UserName.ToUpper().Contains(Name.ToUpper())).ToList();
            }
            var paged = requests.Skip((page - 1) * size).Take(size).ToList();
            RequestsPagedListModel response = new RequestsPagedListModel
            {
                Requests = paged,
                Pagination = new PageInfoModel
                {
                    size = size,
                    count = (int)Math.Ceiling((decimal)requests.Count() / size),
                    current = page
                }

            };

            return (response);
        }





        public async Task<RequestsPagedListModel> GetAllUserRequests(ClaimsPrincipal user, int page, int size)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new UnauthorizedAccessException();
            var userRequests = await _context.Requests.Include(el => el.User)
            .Where(r => r.User.Id == new Guid(userId))
            .Select(o => new RequestShortDTO
            {
                Id = o.Id,
                UserName = o.User.SecondName + " " + o.User.SecondName + " " + o.User.MiddleName,
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest,
                Group = o.User.Group

            }).ToListAsync();
            
            var paged = userRequests.Skip((page - 1) * size).Take(size).ToList();
            RequestsPagedListModel response = new RequestsPagedListModel
            {
                Requests = paged,
                Pagination = new PageInfoModel
                {
                    size = size,
                    count = (int)Math.Ceiling((decimal)userRequests.Count() / size),
                    current = page
                }

            };

            return (response);
        }

 
    }
}
