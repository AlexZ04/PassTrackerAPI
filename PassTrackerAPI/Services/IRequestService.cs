using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using System.Security.Claims;

namespace PassTrackerAPI.Services
{
    public interface IRequestService
    {
        public Task CreateRequest(RequestCreateDTO request, ClaimsPrincipal user);
        public Task ChangeRequest(Guid requestId, RequestChangeDTO request, ClaimsPrincipal user);
        public Task DeleteRequest(Guid requestId, ClaimsPrincipal user);
        public Task<RequestDTO> GetRequestInfo(Guid requestId, ClaimsPrincipal user);
        public Task<RequestsPagedListModel> GetAllRequests(StatusRequestDB? StatusRequestSort,
            DateTime? StartDate, DateTime? FinishDate, int? Group, string? Name, int page , int size);
        
        public Task<RequestsPagedListModel> GetAllUserRequests(ClaimsPrincipal user, int page, int size);
        public Task<RequestsPagedListModel> GetAllUsersRequestById(Guid id, int page, int size);
    }
}
