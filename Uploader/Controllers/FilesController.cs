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

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var filePath = Path.GetTempFileName();
            if (file.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }

            await _mediator.Publish(new HtmlUploadedNotification(filePath));

            return Ok(new { name = Path.GetFileName(filePath), file.Length });
        }
        
        // Just for check in docker 
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);            
            }
            memoryStream.Position = 0;

            return File(memoryStream, "text/html", Path.GetFileName(filePath));
        }
    }
}
