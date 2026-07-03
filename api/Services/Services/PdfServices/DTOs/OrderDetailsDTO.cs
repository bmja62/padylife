namespace Services.Services.PdfServices.DTOs
{
    public class PDFOrderDetailsDTO
    {
        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public PDFUserInfoDTO UserInfo { get; set; }
        public List<PDFOrderItemDTO> Items { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Address { get; set; }
    }
    public class PDFUserInfoDTO
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class PDFOrderItemDTO
    {
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
