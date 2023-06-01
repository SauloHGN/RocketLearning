using Google.Apis.Drive.v3;
using Google.Apis.Auth;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RocketLearning.Models;
using System.Linq.Expressions;
using System.Diagnostics;

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
        public IActionResult UploadVideo(string titulo)
        {
            var videoFile = Request.Form.Files["upload-file"]; 
            var thumbnail = Request.Form.Files["upload-file-Thumb"];
            

            if (videoFile == null || thumbnail == null || titulo == null)
            {
                // Lógica de tratamento para arquivo inválido
                return BadRequest();
            }       


            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = videoFile.FileName,
                //propriedades do arquivo conforme necessário
            };

            FilesResource.CreateMediaUpload request;
            try
            {
                using (var stream = videoFile.OpenReadStream())
                {
                    request = _driveService.Files.Create(fileMetadata, stream, videoFile.ContentType);
                    request.Fields = "id";
                    request.Upload();                
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return BadRequest();
            }

            if (request.ResponseBody == null || string.IsNullOrEmpty(request.ResponseBody.Id))
            {
                return BadRequest(); // Tratamento para falha de upload
            }

            var uploadedFileId = request.ResponseBody?.Id;

            if (string.IsNullOrEmpty(uploadedFileId))
            {         
                return BadRequest();// Tratamento para Falha de upload
            }

            var thumbnailBytes = ConvertFileToByteArray(thumbnail);// converte a thumb para bytes

            var video = new Video
            {
                Titulo = titulo,
                Thumbnail = Convert.ToBase64String(thumbnailBytes),// converte a thumb para string
                UploadDate = DateTime.Now,
                GoogleDriveFileId = uploadedFileId,
                // Salvar caracteristicas do vídeo conforme necessário
            };

            _context.Videos.Add(video);
            _context.SaveChanges();

            // Lógica de retorno, Sucesso vídeo e a adição ao banco de dados

            return Ok();
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
    

