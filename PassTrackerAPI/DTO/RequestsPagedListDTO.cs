namespace PassTrackerAPI.DTO
{
    public class RequestsPagedListDTO
    {
        public List<RequestShortDTO> Requests { get; set; }
        public PageInfoDTO Pagination { get; set; }
    }
}
