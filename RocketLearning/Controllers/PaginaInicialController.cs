using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace RocketLearning.Controllers
{
    public class PaginaInicialController : Controller
    {
        private readonly string apiKey = "AIzaSyC41K2PO6rXl-MsqOlvEtTDGKLFqgM_5Y0";

        public static string acao = null;

        public static List<VideoViewModel> listaVideos = new List<VideoViewModel>();

        public static List<VideoViewModel> listaVideosRecentes = new List<VideoViewModel>();

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
            videos.Clear();

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

            acao = "Todos";
            return videos;
        }

        public async Task<IActionResult> ExibirTodos()
        {
            var videos = await TesteVideo();

            return RedirectToAction("TodosVideos", "Home", videos);
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


        [Route("PaginaInicial/MaisVistos")]
        public async Task<IActionResult> MaisVistos()
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
            videos.Clear();
            listaVideos.Clear();
            listaVideosRecentes.Clear();

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

            videos = videos.OrderByDescending(v => Convert.ToInt32(v.Views)).ToList();
            acao = "MaisVistos";
            exibirMaisVistos();
            listaVideos = videos;
            return RedirectToAction("MaisVistos", "Home", videos);
        }    
        
        public IActionResult exibirMaisVistos()
        {
            return Json(new { success = true});
        }


        [Route("PaginaInicial/MaisRecentes")]
        public async Task<IActionResult> MaisRecentes()
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
            videos.Clear();
            listaVideos.Clear();
            listaVideosRecentes.Clear();

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

            videos = videos.OrderByDescending(v => DateTime.ParseExact(v.DataPublicacao, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();
            acao = "MaisRecentes";
            listaVideosRecentes = videos;
            return RedirectToAction("MaisRecentes", "Home", videos);
        }
    }
}
