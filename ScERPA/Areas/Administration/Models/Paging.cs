namespace ScERPA.Areas.Administration.Models
{
    public class Paging
    {
        public int TotalItems { get; set; } = 1;
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
