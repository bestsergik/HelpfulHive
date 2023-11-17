using HelpfulHive.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HelpfulHive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageUploadController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("api/images/upload")]
        public async Task<IActionResult> UploadImage([FromBody] ImageUploadModel model)
        {
            try
            {
                var imageUrl = await _imageService.SaveImageAsync(model.Image, "content_images");
                return Ok(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("034983048750394850129835-1283-59213-5  ОШИБОЧКА");
                return StatusCode(500, ex.Message);
            }
        }

        public class ImageUploadModel
        {
            public string Image { get; set; }
        }
    }

}
