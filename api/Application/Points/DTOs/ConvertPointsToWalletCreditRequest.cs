namespace Application.Points.DTOs
{
    public class ConvertPointsToWalletCreditRequest
    {
        public long UserId { get; set; }
        public int PointsToConvert { get; set; }
        public string Description { get; set; } = "تبدیل امتیاز به اعتبار";
    }

}
