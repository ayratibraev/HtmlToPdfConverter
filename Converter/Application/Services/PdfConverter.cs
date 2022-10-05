using Converter.Application.Services.Interfaces;
using PuppeteerSharp;

namespace Converter.Application.Services;

public sealed class PdfConverter : IPdfConverter
{
    public async Task<string> Convert(string filePath)
    {
        var bfOptions = new BrowserFetcherOptions();
        using var browserFetcher = new BrowserFetcher(bfOptions);
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync("file://" + filePath);
        var pdfPath = Path.ChangeExtension(filePath, "pdf");
        await page.PdfAsync(pdfPath);
        return pdfPath;
    }
}