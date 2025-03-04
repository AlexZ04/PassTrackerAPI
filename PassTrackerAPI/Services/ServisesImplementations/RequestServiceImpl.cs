
using Microsoft.AspNetCore.Mvc;
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

 

        public async Task<string> CreateRequest(RequestCreateDTO request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var userProfile = await _context.Users
             .FirstOrDefaultAsync(u => u.Id == new Guid(userId));

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

            return("Request has been created");
        }


        public async Task<string> ChangeRequest(Guid requestId, RequestChangeDTO request, ClaimsPrincipal user)
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

            return ("Request has been changed");
        }
        public async Task<string> DeleteRequest(Guid requestId, ClaimsPrincipal user)
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
                return ("Request has been deleted");
            }
            throw new CredentialsException(ErrorTitles.REQUEST_ERROR, ErrorMessages.CANNOT_DELETE_ACCEPTED_REQUEST);

        }

        public async Task<RequestDTO> GetRequestInfo(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var req = await _context.Requests.Include(el => el.User).FirstOrDefaultAsync(el => el.Id == requestId);
            
            if (req == null) { throw new CredentialsException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST); }
            //НУЖНА ПРОВЕРКА НА РОЛЬ 
            if (req.User.Id != new Guid(userId)) { throw new UnauthorizedAccessException(); }

            var request = new RequestDTO
            {
                Id = req.Id,
                UserName = req.User.SecondName + " " + req.User.SecondName + " " + req.User.MiddleName,
                StartDate = req.StartDate,
                FinishDate = req.FinishDate,
                TypeRequest = req.TypeRequest,
                StatusRequest =  req.StatusRequest,
                Comment = req.Comment,
                Photo = req.Photo
            };

            return request;
        }

        public async Task<List<RequestShortDTO>> GetAllRequests(StatusRequestDB? StatusRequestSort)
        {
            var requests = await _context.Requests.Include(el => el.User)
            .Select(o => new RequestShortDTO
            {
                Id = o.Id,
                UserName = o.User.SecondName + " " + o.User.SecondName + " " + o.User.MiddleName,
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest

            }).ToListAsync();
            if(StatusRequestSort != null)
            {
                requests = requests.Where(el => el.StatusRequest == StatusRequestSort).ToList();
            }
            return (requests);
        }

        public async Task<List<RequestShortDTO>> GetAllUserRequests(ClaimsPrincipal user)
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
                StatusRequest = o.StatusRequest

            }).ToListAsync();
            return (userRequests);
        }
    }
}
