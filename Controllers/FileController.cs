using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using NanoidDotNet;
using System;

namespace Assignment2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload"), Authorize]
        public IActionResult Upload(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    // Check if the uploaded file is a valid JPEG image
                    if (file.ContentType == "image/jpeg" || file.ContentType == "image/jpg")
                    {
                        var id = Nanoid.Generate(size: 10);
                        var filename = id + ".jpg"; // Always save as .jpg
                        var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads", filename);

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        return Ok(new { filename });
                    }
                    else
                    {
                        // Return a response indicating that only JPEG images are allowed
                        return BadRequest("Only JPEG images are allowed.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions, e.g., file system errors
                    return StatusCode(500, $"Internal Server Error: {ex.Message}");
                }
            }

            // Handle other cases, like no file provided
            return BadRequest("No file provided.");
        }
    }
}
