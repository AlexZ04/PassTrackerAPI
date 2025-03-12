namespace PassTrackerAPI.DTO
{
    public class UsersPagedListDTO
    {
        public List<UserShortDTO> Requests { get; set; }
        public PageInfoDTO Pagination { get; set; }
    }
}
