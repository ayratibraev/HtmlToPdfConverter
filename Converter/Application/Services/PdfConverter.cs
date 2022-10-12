using CommonUtils.Services.Interfaces;
using Converter.Application.Services.Interfaces;
using PuppeteerSharp;

namespace Converter.Application.Services;

public sealed class PdfConverter : IPdfConverter
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<PdfConverter> _logger;

    public PdfConverter(IFileSystem fileSystem, ILogger<PdfConverter> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    public async Task<string> Convert(string filePath)
    {
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = true,
            Args = new [] {
                "--disable-gpu",
                "--disable-dev-shm-usage",
                "--disable-setuid-sandbox",
                "--no-sandbox"}
        });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync("file://" + filePath, WaitUntilNavigation.Networkidle2);
        
        var pdfPath = Path.ChangeExtension(filePath, "pdf");
        await page.PdfAsync(pdfPath);
        _fileSystem.DeleteFile(filePath);
        return pdfPath;
    }
}