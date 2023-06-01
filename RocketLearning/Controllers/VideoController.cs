using Google.Apis.Drive.v3;
using Google.Apis.Auth;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RocketLearning.Models;
using System.Net.Http;
using System.Linq.Expressions;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;

namespace RocketLearning.Controllers
{
    public class VideoController : Controller
    {
        private readonly DriveService _driveService;
        private readonly DataContext _context;

        public VideoController(DataContext context, DriveService driveService)
        {
            _context = context;
            _driveService = driveService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo()
        {
            var videoFile = Request.Form.Files["upload-file"];

            if (videoFile != null && videoFile.Length > 0)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = videoFile.FileName,
                    MimeType = videoFile.ContentType
                };

                var request = _driveService.Files.Create(fileMetadata, videoFile.OpenReadStream(), fileMetadata.MimeType);
                request.Fields = "id";

                try
                {
                    var mediaUpload = request.UploadAsync();

                    await mediaUpload;

                    var file = Task.FromResult(request.ResponseBody);
                    

                    if (file != null)
                    {
                        return Ok("Vídeo enviado com sucesso. ID do arquivo: " + file.Id);
                    }
                    else
                    {
                        return BadRequest("Ocorreu um erro ao fazer o upload do vídeo para o Google Drive.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Nenhum vídeo encontrado para fazer upload.");
        }

        private byte[] ConvertFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }  
}
    

