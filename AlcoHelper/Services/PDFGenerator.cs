using AlcoHelper.Models;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using System;
using System.IO;

public class PDFGenerator
{
    public byte[] GenerateAlcoholPDF(Alcohol alcohol)
    {
        using (var memoryStream = new MemoryStream())
        {
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");

            document.Add(new Paragraph(new Text($"Nazwa: {alcohol.Name}").SetFont(boldFont).SetFontSize(18)));
            document.Add(new Paragraph($"Opis: {alcohol.Description}").SetFontSize(12));
            document.Add(new Paragraph($"Procent alkoholu: {alcohol.AlcoholPercentage}%").SetFontSize(12));
            document.Add(new Paragraph($"Kraj: {alcohol.Country}").SetFontSize(12));

            if (!string.IsNullOrEmpty(alcohol.ImageUrl))
            {
                try
                {
                    byte[] imageBytes;

                    if (alcohol.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        imageBytes = httpClient.GetByteArrayAsync(alcohol.ImageUrl).Result;
                    }
                    else
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", alcohol.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        imageBytes = File.ReadAllBytes(path);
                    }

                    var imageData = ImageDataFactory.Create(imageBytes);
                    var img = new Image(imageData);

                    // Formatowanie obrazu: rozmiar, marginesy, ramka, wyśrodkowanie
                    img.SetWidth(200)
                       .SetHeight(200)
                       .SetMarginTop(10)
                       .SetMarginBottom(10)
                       .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                       .SetBorder((new SolidBorder(ColorConstants.BLACK, 1))
);

                    document.Add(img);
                }
                catch (Exception)
                {
                    document.Add(new Paragraph("Brak zdjęcia."));
                }
            }

            document.Close();

            return memoryStream.ToArray();
        }
    }
}
