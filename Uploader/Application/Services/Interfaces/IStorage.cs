namespace Uploader.Application.Services.Interfaces;

public interface IStorage
{
    public void Upload(string filePath);
    public string? Download();
}