using Microsoft.AspNetCore.Html;
using SixLabors.ImageSharp.Formats;
using static System.Net.Mime.MediaTypeNames;

namespace RocketLearning.Models
{
    public static class ImageHelper
    {
        public static IHtmlContent GetImageFromBase64(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                // Retornar uma imagem padrão ou uma mensagem de erro, caso a string base64 esteja vazia
                return new HtmlString("<img src='/images/default-image.jpg' alt='Imagem Padrão' />");
            }

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);
                string mimeType = GetMimeType(base64Image);
                string base64DataUrl = $"data:{mimeType};base64,{base64Image}";

                return new HtmlString($"<img src='{base64DataUrl}' alt='Imagem' />");
            }
            catch (Exception ex)
            {
                // Tratar qualquer exceção que possa ocorrer durante a conversão da string base64 em imagem
                // Você pode retornar uma imagem padrão ou uma mensagem de erro, caso a conversão falhe
                return new HtmlString("<img src='~/Resources/icons8-user-default-96-cor2.png' alt='Erro ao carregar a imagem' />");
            }
        }


        public static string GetMimeType(string base64Image)
        {
            // Extrair o mimeType da string base64
            // A lógica para extrair o mimeType depende da estrutura da string base64
            // Neste exemplo, estou assumindo que o mimeType está contido na própria string base64

            // Exemplo de lógica para extrair o mimeType de uma string base64 PNG:
            if (base64Image.StartsWith("data:image/png"))
            {
                return "image/png";
            }

            // Exemplo de lógica para extrair o mimeType de uma string base64 JPEG:
            if (base64Image.StartsWith("data:image/jpeg") || base64Image.StartsWith("data:image/jpg"))
            {
                return "image/jpeg";
            }

            // Exemplo de lógica para extrair o mimeType de uma string base64 SVG:
            if (base64Image.StartsWith("data:image/svg+xml"))
            {
                return "image/svg+xml";
            }

            // Caso nenhum mimeType seja identificado, você pode retornar um valor padrão, como "image/jpeg" ou "image/png"
            return "image/jpeg";
        }


    }
}
