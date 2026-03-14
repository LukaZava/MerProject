namespace MERPROJ.DTO
{
    public class GetQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? Search { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsActive { get; set; }

        public string SortBy { get; set; } = "lastName";
        public string SortDirection { get; set; } = "asc";
    }
}
