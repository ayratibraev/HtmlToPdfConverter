namespace CommonUtils.Services.Interfaces;

public interface IStorage
{
    public void UploadHtml(string filePath);
    public string? DownloadHtml();
    
    public void UploadPdf(string filePath);
    public string? DownloadPdf();
}