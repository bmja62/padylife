namespace Services.Services.PdfServices
{
    public class PdfFileResult
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }

        public PdfFileResult(byte[] content, string fileName)
        {
            Content = content;
            FileName = fileName;
        }
    }
}
