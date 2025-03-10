namespace PassTrackerAPI.DTO
{
    public class UsersPagedListModel
    {
        public List<UserShortDTO> Requests { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
