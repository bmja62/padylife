namespace Application.Rates.DTOs
{
    public class IsRateExistDTO
    {
        public bool IsRated { get; internal set; }
        public bool? CanGiveRate { get; set; }
    }
}
