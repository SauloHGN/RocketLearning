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
                
                return new HtmlString("<img src='~/Resources/icons8-user-default-96-cor2.png' alt='Erro ao carregar a imagem' />");
            }
        }


        public static string GetMimeType(string base64Image)
        {
           
            if (base64Image.StartsWith("data:image/png"))
            {
                return "image/png";
            }

            
            if (base64Image.StartsWith("data:image/jpeg") || base64Image.StartsWith("data:image/jpg"))
            {
                return "image/jpeg";
            }

            
            if (base64Image.StartsWith("data:image/svg+xml"))
            {
                return "image/svg+xml";
            }

            
            return "image/jpeg";
        }


    }
}
