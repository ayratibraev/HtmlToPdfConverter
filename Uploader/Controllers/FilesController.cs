using CommonUtils.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uploader.Application.Notifications;

namespace Uploader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IMediator mediator, IFileSystem fileSystem, ILogger<FilesController> logger)
        {
            _mediator = mediator;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                var filePath = _fileSystem.GetTempFileNameHtml();
                if (file.Length > 0)
                {
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
    _logger.LogInformation($"File uploaded: {filePath}");
                await _mediator.Publish(new HtmlUploadedNotification(filePath));

                return Ok("Файл загружен. Вам придет ссылка для загрузки конвертированного файла.");
            }
            catch (Exception exception)
            {
                // _logger.LogError(exception, "Loading failed. {0}", HttpContext.Request);
                return BadRequest("Произошла ошибка.");
            }
        }
        
        // Just for check in docker 
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            var filePath = 
                Path.Combine(
                    Path.GetDirectoryName(_fileSystem.GetTempFileNamePdf()),
                    fileName);
            return Ok(filePath);
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);            
            }
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf", Path.GetFileName(filePath));
        }
    }
}
