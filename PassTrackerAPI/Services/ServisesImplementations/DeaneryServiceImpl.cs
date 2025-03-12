using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Repositories;
using System.Drawing;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class DeaneryServiceImpl: IDeaneryService
    {
        private readonly DataContext _context;

        public DeaneryServiceImpl(DataContext context)
        {
            _context = context;
        }

        public async Task AcceptRequest(Guid requestId)
        {
            var req = await _context.Requests.FirstOrDefaultAsync(el => el.Id == requestId);
            if (req == null) throw new NotFoundException(ErrorTitles.KEY_NOT_FOUND, ErrorMessages.NOT_EXISTING_REQUEST);

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

        public async Task<byte[]> DownloadRequest(bool havePhoto)
        {
            var table = await _context.Requests
                .Include(o => o.User)
            .Select(o => new RequestExcelDTO
            {
                    
                UserName = o.User.SecondName + " " + o.User.SecondName + " " + o.User.MiddleName,
                StartDate = o.StartDate,
                FinishDate = o.FinishDate,
                TypeRequest = o.TypeRequest,
                StatusRequest = o.StatusRequest,
                Group = o.User.Group,
                Photo = havePhoto  ? o.Photo : null,
                Comment = o.Comment

            }).ToListAsync();
            
            byte[] excelData;
            using (var package = new ExcelPackage())
            {
                
                var worksheet = package.Workbook.Worksheets.Add("Requests");

              
                worksheet.Cells[1, 1].Value = "UserName";
                worksheet.Cells[1, 2].Value = "Start Date";
                worksheet.Cells[1, 3].Value = "Finish Date";
                worksheet.Cells[1, 4].Value = "Type Request";
                worksheet.Cells[1, 5].Value = "Status Request";
                worksheet.Cells[1, 6].Value = "Comment";
                //worksheet.Cells[1, 7].Value = "Photo";

                for (int i = 0; i < table.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = table[i].UserName;
                    worksheet.Cells[i + 2, 2].Value = table[i].StartDate.ToString();
                    worksheet.Cells[i + 2, 3].Value = table[i].FinishDate.ToString();
                    worksheet.Cells[i + 2, 4].Value = table[i].TypeRequest;
                    worksheet.Cells[i + 2, 5].Value = table[i].StatusRequest;
                    worksheet.Cells[i + 2, 6].Value = table[i].Comment;
                    //worksheet.Cells[i + 2, 7].Value = table[i].Photo;
                }

                worksheet.Cells.AutoFitColumns();

                excelData = package.GetAsByteArray();
            }

            return excelData;
        }
    }
}
