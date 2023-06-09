using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using System.Diagnostics;

namespace RocketLearning.Controllers
{
    public class PaginaInicialController : Controller
    {
        private readonly string apiKey = "AIzaSyC41K2PO6rXl-MsqOlvEtTDGKLFqgM_5Y0";

        public IActionResult ExibirVideo(string videoId)
        {
            // Aqui você pode implementar a lógica para buscar as informações do vídeo com base no ID (se necessário) e passá-las para a view

            ViewBag.VideoId = videoId; // Exemplo: passando o ID do vídeo para a view através da ViewBag

            return View("~/Views/Home/ExibirVideo.cshtml");
        }

        public async Task<IActionResult> DadosVideo()
        {
            var videos = await TesteVideo();
            return View(videos);
        }

        public async Task<List<VideoViewModel>> TesteVideo()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "Rocket Learning"
            });

            var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
            playlistItemsListRequest.PlaylistId = "PLYC1mJnsCyOBClheD4cSsuNt59imeMVDI";
            playlistItemsListRequest.MaxResults = 5;

            var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

            var videos = new List<VideoViewModel>();

            foreach (var playlistItem in playlistItemsListResponse.Items)
            {
                var video = new VideoViewModel
                {
                    VideoId = playlistItem.Snippet.ResourceId.VideoId,
                    Title = playlistItem.Snippet.Title,
                    Description = playlistItem.Snippet.Description,
                    ThumbnailUrl = playlistItem.Snippet.Thumbnails.Default__.Url
                };

                videos.Add(video);
            }

            foreach(var teste in videos)
            {
                Debug.WriteLine("Video "+teste);
            }

            return videos;
        }
    }
}
