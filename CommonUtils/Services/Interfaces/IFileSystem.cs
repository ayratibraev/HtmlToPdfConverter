namespace CommonUtils.Services.Interfaces;

public interface IFileSystem
{
    public string GetTempFileNameHtml();
    public string GetTempFileNamePdf();
    void DeleteFile(string filePath);
}