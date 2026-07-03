namespace Services.Services.MedalServices.DTOs
{
    public class GetMetalDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool IsLocked { get; set; }
    }
}
