using AlcoHelper.Models;
using Org.BouncyCastle.Crypto;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
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

            // Wczytanie czcionki (pogrubionej) z pliku .ttf lub .otf
            var boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");

            // Dodawanie pogrubionego tekstu
            document.Add(new Paragraph(new Text($"Nazwa: {alcohol.Name}").SetFont(boldFont).SetFontSize(18)));

            // Dodajemy inne informacje o alkoholu
            document.Add(new Paragraph($"Opis: {alcohol.Description}").SetFontSize(12));
            document.Add(new Paragraph($"Procent alkoholu: {alcohol.AlcoholPercentage}%").SetFontSize(12));
            document.Add(new Paragraph($"Kraj: {alcohol.Country}").SetFontSize(12));

            // Dodajemy zdjęcie, jeśli istnieje
            if (!string.IsNullOrEmpty(alcohol.ImageUrl))
            {
                try
                {
                    var image = ImageDataFactory.Create(alcohol.ImageUrl);
                    var img = new Image(image);
                    document.Add(img.SetWidth(200).SetHeight(200)); // Ustalamy rozmiar obrazu
                }
                catch (Exception)
                {
                    document.Add(new Paragraph("Brak zdjęcia."));
                }
            }

            // Zakończenie dokumentu
            document.Close();

            // Zwracamy dane PDF w postaci bajtów
            return memoryStream.ToArray();
        }
    }
}
