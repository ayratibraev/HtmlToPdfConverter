namespace Converter.Application.Services.Interfaces;

public interface IPdfConverter
{
    public Task<string> Convert(string filePath);
}