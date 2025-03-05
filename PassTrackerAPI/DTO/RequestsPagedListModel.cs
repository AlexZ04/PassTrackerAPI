namespace PassTrackerAPI.DTO
{
    public class RequestsPagedListModel
    {
        public List<RequestShortDTO> Requests { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
