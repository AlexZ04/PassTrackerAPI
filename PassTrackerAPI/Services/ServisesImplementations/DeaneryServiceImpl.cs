using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Migrations;
using PassTrackerAPI.Repositories;
using System.Drawing;
using System.Security.Claims;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class DeaneryServiceImpl: IDeaneryService
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        public DeaneryServiceImpl(DataContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task AcceptRequest(Guid requestId)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) throw new NotFoundException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST);
            req.Comment = null;
            req.StatusRequest = StatusRequestDB.Accepted;
            await _context.SaveChangesAsync();
        }

        public async Task DeclineRequest(Guid requestId, CommentToDeclinedRequestDTO Comment)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) throw new NotFoundException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST);

            req.StatusRequest = StatusRequestDB.Declined;
            req.Comment = Comment.Comment;
            await _context.SaveChangesAsync();
        }

        public async Task ProlongEducationRequest(Guid requestId, FinishDateDTO Date)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) throw new NotFoundException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST);
            
            if(req.TypeRequest != TypeRequestDB.EducationalActivity )
                throw new NotFoundException(ErrorTitles.REQUEST_ERROR, ErrorMessages.NOT_EXISTING_REQUEST);
            if (req.StatusRequest != StatusRequestDB.Accepted)
                throw new NotFoundException(ErrorTitles.REQUEST_ERROR, ErrorMessages.YOU_CAN_PROLONG_ONLY_ACCEPTED_REQUEST);
            if (req.FinishDate < Date.FinishDate)
                throw new NotFoundException(ErrorTitles.REQUEST_ERROR, ErrorMessages.YOU_CANT_SHORT_REQUEST);
            
            req.FinishDate = Date.FinishDate;
            await _context.SaveChangesAsync();
        }


        public async Task<byte[]> DownloadRequest(StatusRequestDB? StatusRequestSort, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();
            var userProfile = await _userRepository.GetUserById(new Guid(userId));

            bool isUserAdmin = await _context.Admins.FirstOrDefaultAsync(el => el.Id == new Guid(userId)) != null ? true : false;
            bool isUserTeacger = false;
            bool isUserDeanery = false;
            var userRoles =  userProfile.Roles.Select(u => u.Role).ToList();
            
            if (userRoles.Contains(RoleDb.Teacher)) { isUserTeacger = true; }
            if (userRoles.Contains(RoleDb.Deanery)) { isUserDeanery = true; }



            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var table = await _context.Requests
                .Include(o => o.User)
            .Select(o => new RequestExcelDTO
            {
                    
                UserName = o.User.SecondName + " " + o.User.FirstName + " " + o.User.MiddleName,
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest,
                Group = o.User.Group,
                Comment = o.Comment

            }).ToListAsync();

            if (StatusRequestSort != null)
                table = table.Where(el => el.StatusRequest == StatusRequestSort).ToList();

            byte[] excelData;
            using (var package = new ExcelPackage())
            {
                
                var worksheet = package.Workbook.Worksheets.Add("Requests");
                worksheet.Cells[1, 1].Value = "UserName";
                worksheet.Cells[1, 2].Value = "Start Date";
                worksheet.Cells[1, 3].Value = "Finish Date";
                worksheet.Cells[1, 4].Value = "Group";
                if (isUserAdmin)
                {
                    worksheet.Cells[1, 5].Value = "Type Request";
                    worksheet.Cells[1, 6].Value = "Status Request";
                    worksheet.Cells[1, 7].Value = "Comment";
                    
                    for (int i = 0; i < table.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = table[i].UserName;
                        worksheet.Cells[i + 2, 2].Value = table[i].StartDate.ToString();
                        worksheet.Cells[i + 2, 3].Value = table[i].FinishDate.ToString();
                        worksheet.Cells[i + 2, 4].Value = table[i].Group;
                        worksheet.Cells[i + 2, 5].Value = table[i].TypeRequest;
                        worksheet.Cells[i + 2, 6].Value = table[i].StatusRequest;
                        worksheet.Cells[i + 2, 7].Value = table[i].Comment;
                    }
                }
                else if (isUserDeanery)
                {
                    worksheet.Cells[1, 5].Value = "Type Request";
                    for (int i = 0; i < table.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = table[i].UserName;
                        worksheet.Cells[i + 2, 2].Value = table[i].StartDate.ToString();
                        worksheet.Cells[i + 2, 3].Value = table[i].FinishDate.ToString();
                        worksheet.Cells[i + 2, 4].Value = table[i].Group;
                        worksheet.Cells[i + 2, 5].Value = table[i].TypeRequest;

                    }
                }
                else if(isUserTeacger)
                {
                    for (int i = 0; i < table.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = table[i].UserName;
                        worksheet.Cells[i + 2, 2].Value = table[i].StartDate.ToString();
                        worksheet.Cells[i + 2, 3].Value = table[i].FinishDate.ToString();
                        worksheet.Cells[i + 2, 4].Value = table[i].Group;


                    }
                }



                worksheet.Cells.AutoFitColumns();

                excelData = package.GetAsByteArray();
            }

            return excelData;
        }
    }
}
