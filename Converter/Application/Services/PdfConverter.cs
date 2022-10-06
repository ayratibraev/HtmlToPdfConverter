using CommonUtils.Services.Interfaces;
using Converter.Application.Services.Interfaces;
using PuppeteerSharp;

namespace Converter.Application.Services;

public sealed class PdfConverter : IPdfConverter
{
    private readonly IFileSystem _fileSystem;

    public PdfConverter(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

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
        await page.PdfAsync(pdfPath);
        _fileSystem.DeleteFile(filePath);
        return pdfPath;
    }
}