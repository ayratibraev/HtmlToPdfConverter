using CommonUtils.Services.Interfaces;

namespace CommonUtils.Services;

public sealed class FileSystem : IFileSystem 
{
    private string GetTempFileName()
    {
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            "HtmlToPdfConverter");
        if (Directory.Exists(tempPath) == false)
        {
            Directory.CreateDirectory(tempPath);
        }
        
        return Path.Combine(tempPath, Path.GetFileName(Path.GetTempFileName()));
    }
    public string GetTempFileNameHtml()
    {
        var path = GetTempFileName();
        return Path.ChangeExtension(path, "html");
    }
    
    public string GetTempFileNamePdf()
    {
        var path = GetTempFileName();
        return Path.ChangeExtension(path, "pdf");
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }
}