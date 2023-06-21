using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Diagnostics;

namespace RocketLearning.Models
{
    public static class YoutubeService
    {

        public static void AddYouTubeService(this IServiceCollection services, IConfiguration configuration)
        {
            var youtubeApiKey = configuration["YouTube:ApiKey"];

            services.AddSingleton<YouTubeService>(provider =>
            {
                return new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = youtubeApiKey,
                    ApplicationName = "Rocket Learning"
                });
            });

            // Criação do serviço do YouTube
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = youtubeApiKey
            });

            try
            {
                // Exemplo de chamada para listar vídeos de um canal
                var playlistItemsRequest = youtubeService.PlaylistItems.List("snippet");
                playlistItemsRequest.PlaylistId = "PLYC1mJnsCyOBClheD4cSsuNt59imeMVDI";
                playlistItemsRequest.MaxResults = 10;

                var playlistItemsResponse = playlistItemsRequest.Execute();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ocorreu um erro ao chamar a API do YouTube: " + ex.Message);
            }

        }
    }
}
