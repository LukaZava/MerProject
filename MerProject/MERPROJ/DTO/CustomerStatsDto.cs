namespace MERPROJ.DTO
{
    public class CustomerStatsDto
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public List<CityCountDto> TopCities { get; set; } = new();
    }
}
