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
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace RocketLearning.Controllers
{
    public class VideoController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public VideoController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/Upload/Video")]
        [Obsolete]
        public async Task<IActionResult> UploadVideo()
        {
            var videoFile = Request.Form.Files.GetFile("upload-file");
            var thumbnailFile = Request.Form.Files.GetFile("upload-file-Thumb");
            var title = Request.Form["titulo"];


            UserCredential credential;
            using (var stream = new FileStream("client_secret_271082792456-a95aivs2vdmq634iebb8klg54f1717fq.apps.googleusercontent.com.json",
                FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Rocket Learning")
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Rocket Learning"
            });

            // Crie um objeto Video com as informações do vídeo
            var video = new Google.Apis.YouTube.v3.Data.Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = title;
            video.Snippet.Description = "Descrição do vídeo";
            video.Snippet.Tags = new[] { "tag1", "tag2", "tag3" };

            // Upload do thumbnail
            using (var thumbnailStream = thumbnailFile.OpenReadStream())
            {
                var thumbnailInsertRequest = youtubeService.Thumbnails.Set(video.Id, thumbnailStream, "image/jpeg");
                await thumbnailInsertRequest.UploadAsync();
            }

            try
            {
                // Upload do vídeo
                using (var videoStream = videoFile.OpenReadStream())
                {
                    var videoInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", videoStream, "video/*");

                    // Manipuladores de eventos para acompanhar o progresso do upload                   
                    videoInsertRequest.ResponseReceived += VideoInsertRequest_ResponseReceived;
                    videoInsertRequest.ProgressChanged += VideoInsertRequest_ProgressChanged;

                    var uploadResponse = await videoInsertRequest.UploadAsync();

                    if (uploadResponse.Status == UploadStatus.Failed)
                    {
                        // Upload falhou
                        Debug.WriteLine("Falha ao fazer upload do vídeo");
                        Debug.WriteLine("Status de falha: " + uploadResponse.Exception.Message);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu uma exceção durante o upload do vídeo: " + ex.Message);
            }

            return RedirectToAction("PaginaInicial", "Home");
        }

        private void VideoInsertRequest_ProgressChanged(IUploadProgress progress)
        {

            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Debug.WriteLine($"Progresso do upload: {progress.BytesSent} bytes enviados.");
                    break;
                case UploadStatus.Completed:
                    Debug.WriteLine("Upload completo!");
                    break;
                case UploadStatus.Failed:
                    Debug.WriteLine("Falha no upload!");             
                    break;
            }

        }

        // Manipulador para o upload
        private void VideoInsertRequest_ResponseReceived(Google.Apis.YouTube.v3.Data.Video video)
        {
            Console.WriteLine("Upload em andamento: " + video.Status.UploadStatus);
            Debug.WriteLine("Video enviado com sucesso!");
            Debug.WriteLine("ID do vídeo: " + video.Id);
            Debug.WriteLine("Título do vídeo: " + video.Snippet.Title);
            //informações sobre o vídeo
        }
    }  
}
    

