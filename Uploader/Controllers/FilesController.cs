using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Uploader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
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
            
            var redis = ConnectionMultiplexer.Connect("localhost");
            var db = redis.GetDatabase();

            var value = System.IO.File.ReadAllBytes(filePath);
            db.StringSet("html_file", value);

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
