using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using System.Diagnostics;
using System.Xml;

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
            playlistItemsListRequest.MaxResults = 50;

            var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

            var videos = new List<VideoViewModel>();

            foreach (var playlistItem in playlistItemsListResponse.Items)
            {
                var videoId = playlistItem.Snippet.ResourceId.VideoId;

                var videoRequest = youtubeService.Videos.List("snippet,statistics,contentDetails");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                var videoInfo = videoResponse.Items.FirstOrDefault();

                var video = new VideoViewModel
                {
                    VideoId = playlistItem.Snippet.ResourceId.VideoId,
                    Title = playlistItem.Snippet.Title,
                    Description = playlistItem.Snippet.Description,
                    ThumbnailUrl = playlistItem.Snippet.Thumbnails.Default__.Url,
                    Views = videoInfo?.Statistics.ViewCount.ToString(),
                    DataPublicacao = videoInfo?.Snippet?.PublishedAt?.ToString("dd/MM/yyyy"),
                    Tempo = XmlConvert.ToTimeSpan(videoInfo?.ContentDetails?.Duration ?? "PT0S").ToString(@"mm\:ss")

                };

                videos.Add(video);
            }

            foreach(var teste in videos)
            {
                Debug.WriteLine("Video "+teste);
            }

            return videos;
        }

        [HttpPost]
        [Route("/PaginaInicial/Busca")]
        public async Task<IActionResult> BuscaPorTitulo()
        {
            string busca = Request.Form["textBusca"];

            var videos = await TesteVideo();

            var buscaResults = videos.Where(video => video.Title.Contains(busca)).ToList();

            return ExibirResultadosBusca(buscaResults);
        }

        public IActionResult ExibirResultadosBusca(List<VideoViewModel> buscaResults)
        {
            ViewBag.BuscaResults = buscaResults;
            return View("~/Views/Home/Busca.cshtml", buscaResults);
        }

    }
}
