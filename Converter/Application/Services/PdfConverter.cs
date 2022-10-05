using Converter.Application.Services.Interfaces;
using PuppeteerSharp;

namespace Converter.Application.Services;

public sealed class PdfConverter : IPdfConverter
{
    public async Task<string> Convert(string filePath)
    {
        await new BrowserFetcher().DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();
        page.DefaultTimeout = 5000;
        await page.GoToAsync("file://" + filePath, WaitUntilNavigation.Networkidle2);
        var pdfPath = Path.ChangeExtension(filePath, "pdf");
        page.PdfAsync(pdfPath).GetAwaiter().GetResult();
        return pdfPath;
    }
}