using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Functions;
using PassTrackerAPI.Migrations;
using PassTrackerAPI.Repositories;
using PassTrackerAPI.Repositories.RepositoriesImplementations;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class RequestServiceImpl : IRequestService
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;

        public RequestServiceImpl(DataContext context, IUserRepository userRepository, IRequestRepository requestRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        public async Task CreateRequest(RequestCreateDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var userProfile = await _userRepository.GetUserById(new Guid(userId));

            if (userProfile == null || !userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Student)) 
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
                Photo = request.Photo,
                InDeanery = request.InDeanery
            };

            _context.Requests.Add(newRequest);

            await _context.SaveChangesAsync();

        }

        public async Task ChangeRequest(Guid requestId, RequestChangeDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var req = await _requestRepository.GetRequestById(requestId);

            if (req.User.Id != new Guid(userId))
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION,
                ErrorMessages.THIS_REQUEST_IS_NOT_BELONGS_TO_USER);

            req.StartDate = request.StartDate;
            req.FinishDate = request.FinishDate;
            req.TypeRequest = request.TypeRequest;
            req.Photo = request.Photo;
            req.InDeanery = request.InDeanery;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRequest(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var req = await _requestRepository.GetRequestById(requestId);

            if (req.User.Id != new Guid(userId)) 
                throw new InvalidActionException(ErrorTitles.INVALID_ACTION,
                ErrorMessages.THIS_REQUEST_IS_NOT_BELONGS_TO_USER);

            if (req.StatusRequest != StatusRequestDB.Accepted)
            {
                _context.Requests.Remove(req);
                await _context.SaveChangesAsync();
                return;
            }

            throw new InvalidActionException(ErrorTitles.REQUEST_ERROR, ErrorMessages.CANNOT_DELETE_ACCEPTED_REQUEST);
        }

        public async Task<RequestDTO> GetRequestInfo(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var req = await _requestRepository.GetRequestById(requestId);

            var userProfile = await _userRepository.GetUserById(new Guid(userId));

            bool isUserAdmin = await _context.Admins.FirstOrDefaultAsync(el => el.Id == new Guid(userId)) != null ? true : false;
 
            if(userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Deanery) ||
                userProfile.Roles.Select(u => u.Role).Contains(RoleDb.Teacher ) || isUserAdmin || req.User.Id == new Guid(userId)) 
            {
                var request = new RequestDTO
                {
                    Id = req.Id,
                    UserName = ConcatName.ConcatNameFunc(req.User.SecondName, req.User.FirstName, req.User.MiddleName),
                    StartDate = req.StartDate,
                    FinishDate = req.FinishDate,
                    TypeRequest = req.TypeRequest,
                    StatusRequest = req.StatusRequest,
                    Comment = req.Comment,
                    Photo = req.Photo,
                    Group = req.User.Group,
                    InDeanery = req.InDeanery
                };

                return request;
            }

            throw new InvalidActionException(ErrorTitles.INVALID_ACTION, ErrorMessages.YOU_CANT_GET_THIS_REQUEST_INFO); 
        }

        public async Task<RequestsPagedListDTO> GetAllRequests(StatusRequestDB? StatusRequestSort, 
            DateTime? StartDate, DateTime? FinishDate, int? Group, string? Name, int page, int size)
        {
            var requests = await _context.Requests.Include(el => el.User)
            .Select(o => new RequestShortDTO
            {
                Id = o.Id,
                UserName = ConcatName.ConcatNameFunc(o.User.SecondName, o.User.FirstName, o.User.MiddleName),
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest,
                Group = o.User.Group

            }).ToListAsync();

            if (StatusRequestSort != null)
                requests = requests.Where(el => el.StatusRequest == StatusRequestSort).ToList();

            if (StartDate != null)
                requests = requests.Where(el => el.StartDate >= StartDate).ToList();

            if (FinishDate != null)
                requests = requests.Where(el => el.FinishDate <= FinishDate).ToList();

            if (Group != null)
                requests = requests.Where(el => el.Group == Group).ToList();

            if(Name != null)
                requests = requests.Where(el => el.UserName.ToUpper().Contains(Name.ToUpper())).ToList();

            var paged = requests.Skip((page - 1) * size).Take(size).ToList();
            RequestsPagedListDTO response = new RequestsPagedListDTO
            {
                Requests = paged,
                Pagination = new PageInfoDTO
                {
                    size = size,
                    count = (int)Math.Ceiling((decimal)requests.Count() / size),
                    current = page
                }

            };

            return response;
        }

        public async Task<RequestsPagedListDTO> GetAllUserRequests(ClaimsPrincipal user, int page, int size)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            return await GetAllUsersRequestById(new Guid(userId), page, size);
        }

        public async Task<RequestsPagedListDTO> GetAllUsersRequestById(Guid id, int page, int size)
        {
            var userRequests = await _context.Requests
                .Include(el => el.User)
                .Where(r => r.User.Id == id)
                .Select(o => new RequestShortDTO
                {
                    Id = o.Id,
                    UserName = ConcatName.ConcatNameFunc(o.User.SecondName, o.User.FirstName, o.User.MiddleName),
                    StartDate = o.StartDate,
                    FinishDate = o.FinishDate,
                    TypeRequest = o.TypeRequest,
                    StatusRequest = o.StatusRequest,
                    Group = o.User.Group

                }).ToListAsync();

            var paged = userRequests.Skip((page - 1) * size).Take(size).ToList();

            RequestsPagedListDTO response = new RequestsPagedListDTO
            {
                Requests = paged,
                Pagination = new PageInfoDTO
                {
                    size = size,
                    count = (int)Math.Ceiling((decimal)userRequests.Count() / size),
                    current = page
                }

            };

            return response;
        }

    }
}
