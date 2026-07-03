using Common;
using Common.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Services.Services.PdfServices.DTOs;
using System.Text;

namespace Services.Services.PdfServices
{
    public interface IPdfService
    {
        Task<ServiceResult<PdfFileResult>> GenerateInvoicePdf(PDFOrderDetailsDTO orderDetails);
    }
    public class PdfService : IPdfService, IScopedDependency
    {

        public async Task<ServiceResult<PdfFileResult>> GenerateInvoicePdf(PDFOrderDetailsDTO orderDetails)
        {
            try
            {
                using var ms = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4, 40, 40, 20, 40);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // بارگذاری فونت فارسی
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Vazirmatn-Regular.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font titleFont = new Font(baseFont, 14, Font.BOLD, BaseColor.BLACK);
                Font font = new Font(baseFont, 11, Font.NORMAL, BaseColor.BLACK);
                Font boldFont = new Font(baseFont, 11, Font.BOLD, BaseColor.BLACK);

                // تاریخ شمسی
                var persianDate = new System.Globalization.PersianCalendar();
                var createdAt = orderDetails.OrderDate;
                string persianOrderDate = $"{persianDate.GetYear(createdAt):0000}/{persianDate.GetMonth(createdAt):00}/{persianDate.GetDayOfMonth(createdAt):00}";

                // عنوان اصلی
                var titleTable = new PdfPTable(1)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                titleTable.AddCell(new PdfPCell(new Phrase("صورت حساب الکترونیک فروش", titleFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f
                });

                // سلول سمت چپ: لوگو
                var logoCell = new PdfPCell { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logo", "logo.png");
                if (File.Exists(logoPath))
                {
                    var logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(70f, 70f);
                    logo.Alignment = Element.ALIGN_LEFT;
                    logoCell.AddElement(logo);
                }
                titleTable.AddCell(logoCell);

                document.Add(titleTable);

                // جدول اصلی اطلاعات (فروشنده و خریدار کنار هم)
                PdfPTable mainInfoTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    SpacingBefore = 10f,
                    SpacingAfter = 15f
                };
                mainInfoTable.SetWidths(new float[] { 1f, 1f });

                // اطلاعات فروشنده
                var sellerData = new
                {
                    sellerName = "پادی لایف",
                    sellerPhoneNumber = "\u200E" + "03142510361",
                    sellerAddress = "استان : اصفهان - شهرستان : نجف آباد - بخش : مرکزی - شهر : نجف آباد - محله : معرفت - خیابان بصیرت - بن بست رز - پلاک : 25.0 - طبقه : 1",
                };

                PdfPTable sellerTable = new PdfPTable(1)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                // اضافه کردن کاراکتر LRM (Left-to-Right Mark) قبل از تاریخ
                string dateText = "\u200E" + $"{persianOrderDate}";

                sellerTable.AddCell(new PdfPCell(new Phrase("فروشنده", boldFont)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase(sellerData.sellerName, font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase($"شماره فاکتور: {orderDetails.OrderId}", font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase($"تلفن: {sellerData.sellerPhoneNumber}", font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase($"آدرس: {sellerData.sellerAddress}", font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase($"وضعیت: {orderDetails.Status}", font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase("تاریخ", font)) { Border = Rectangle.NO_BORDER });
                sellerTable.AddCell(new PdfPCell(new Phrase(dateText, font)) { Border = Rectangle.NO_BORDER });

                // اطلاعات خریدار
                PdfPTable customerTable = new PdfPTable(1)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };

                customerTable.AddCell(new PdfPCell(new Phrase("خریدار", boldFont)) { Border = Rectangle.NO_BORDER });
                customerTable.AddCell(new PdfPCell(new Phrase($"نام: {orderDetails.UserInfo.FullName}", font)) { Border = Rectangle.NO_BORDER });
                customerTable.AddCell(new PdfPCell(new Phrase($"تلفن: {orderDetails.UserInfo.PhoneNumber}", font)) { Border = Rectangle.NO_BORDER });
                customerTable.AddCell(new PdfPCell(new Phrase($"آدرس: {orderDetails.Address}", font)) { Border = Rectangle.NO_BORDER });

                // اضافه کردن جداول فروشنده و خریدار به جدول اصلی
                mainInfoTable.AddCell(new PdfPCell(sellerTable) { Border = Rectangle.NO_BORDER, Padding = 5 });
                mainInfoTable.AddCell(new PdfPCell(customerTable) { Border = Rectangle.NO_BORDER, Padding = 5 });




                document.Add(mainInfoTable);

                // جدول محصولات
                PdfPTable productsTable = new PdfPTable(5)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    SpacingBefore = 10f
                };
                // تنظیم عرض ستون‌ها: ردیف | شرح کالا | تعداد | قیمت واحد | قیمت کل
                productsTable.SetWidths(new float[] {
                    3f,
                    2f,
                    1f,
                    4f,
                    1f
                });
                BaseColor headerBg = new BaseColor(240, 240, 240);
                BaseColor borderColor = new BaseColor(200, 200, 200);

                // هدر جدول
                string[] headers = { "ردیف", "شرح کالا", "تعداد", "قیمت واحد (ریال)", "قیمت کل (ریال)" };
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, boldFont))
                    {
                        BackgroundColor = headerBg,
                        BorderColor = borderColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    productsTable.AddCell(cell);
                }

                // محتوای جدول
                int index = 1;
                foreach (var item in orderDetails.Items)
                {
                    AddProductRow(productsTable, index++.ToString(), item.ProductName,
                                 item.Quantity.ToString(), item.UnitPrice.ToString("N0"),
                                 item.TotalPrice.ToString("N0"), font, borderColor);
                }

                // جمع کل
                PdfPCell totalLabel = new PdfPCell(new Phrase("جمع کل:", boldFont))
                {
                    Colspan = 4,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = new BaseColor(245, 245, 245),
                    BorderColor = borderColor,
                    Padding = 8
                };

                PdfPCell totalValue = new PdfPCell(new Phrase(orderDetails.TotalPrice.ToString("N0") + " ریال", boldFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(245, 245, 245),
                    BorderColor = borderColor,
                    Padding = 8
                };

                productsTable.AddCell(totalLabel);
                productsTable.AddCell(totalValue);

                document.Add(productsTable);

                // پانویس
                var footerTable = new PdfPTable(1)
                {
                    WidthPercentage = 100,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    SpacingBefore = 20f
                };

                // روش پرداخت
                var paymentCell = new PdfPCell(new Phrase($"وضعیت پرداخت: {orderDetails.PaymentStatus} - مبلغ: {orderDetails.TotalPrice.ToString("N0")} ریال", font))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingBottom = 10f
                };
                footerTable.AddCell(paymentCell);

                var farsiTotalPrice = new PdfPCell(new Phrase($"{((long)orderDetails.TotalPrice).Convert()} ريال", font))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingBottom = 10f
                };
                footerTable.AddCell(farsiTotalPrice);

                // مهر و امضا
                var signatureTable = new PdfPTable(2)
                {
                    WidthPercentage = 80,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                signatureTable.DefaultCell.Border = Rectangle.NO_BORDER;

                signatureTable.AddCell(new PdfPCell(new Phrase("مهر و امضای فروشنده", font))
                {
                    Border = Rectangle.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 30f
                });

                signatureTable.AddCell(new PdfPCell(new Phrase("مهر و امضای خریدار", font))
                {
                    Border = Rectangle.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 30f
                });

                footerTable.AddCell(new PdfPCell(signatureTable) { Border = Rectangle.NO_BORDER });

                document.Add(footerTable);
                document.Close();

                return ServiceResult.Ok(new PdfFileResult(ms.ToArray(), $"Invoice_{orderDetails.OrderId}.pdf"));
            }
            catch (Exception ex)
            {
                return ServiceResult.BadRequest<PdfFileResult>("خطا در تولید فایل PDF: " + ex.Message);
            }
        }

        // متد کمکی برای اضافه کردن سطر محصول
        private void AddProductRow(PdfPTable table, string index, string name, string quantity,
                                 string unitPrice, string totalPrice, Font font, BaseColor borderColor)
        {
            table.AddCell(new PdfPCell(new Phrase(index, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderColor = borderColor,
                Padding = 5
            });

            table.AddCell(new PdfPCell(new Phrase(name, font))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderColor = borderColor,
                Padding = 5
            });

            table.AddCell(new PdfPCell(new Phrase(quantity, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderColor = borderColor,
                Padding = 5
            });

            table.AddCell(new PdfPCell(new Phrase(unitPrice, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderColor = borderColor,
                Padding = 5
            });

            table.AddCell(new PdfPCell(new Phrase(totalPrice, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderColor = borderColor,
                Padding = 5
            });
        }


    }
}
