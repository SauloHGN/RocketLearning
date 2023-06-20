using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using System.Diagnostics;

namespace RocketLearning.Controllers
{
    public class PerfilController : Controller
    {
        private readonly DataContext _context;
        public static string? fotoUser { get; set; }
        public static string? tipoImagem { get; set; }

        public PerfilController(DataContext context)
        {
            _context = context;
        }

        public static Usuarios ObterUsuario(DataContext context, int idUsuario)
        {
            var usuario = context.Set<Usuarios>().FirstOrDefault(u => u.Id == idUsuario);
            if (usuario != null)
            {
                return usuario;
            }
            
            throw new Exception("Usuário não encontrado");
        }

        public IActionResult Perfil()
        {

            int? idUsuario = UsuarioController.IdUserAtual;
            Debug.WriteLine("ViewBag2 = "+idUsuario);
            if (idUsuario != null)
            {
                Usuarios usuario = PerfilController.ObterUsuario(_context, idUsuario.Value);

                TempData["Nome"] = usuario.Nome;
                TempData["Email"] = usuario.Email;
                TempData["Telefone"] = usuario.Telefone;

                return RedirectToAction("Perfil", "Home");
            }
            return RedirectToAction("Error", "Home");
        }

        public IActionResult Editar()
        {
            string nome = Request.Form["nome"];
            string email = Request.Form["email"];
            string telefone = Request.Form["telefone"];

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == UsuarioController.IdUserAtual);

            if (usuario != null)
            {
                if (_context.Usuarios.Any(u => u.Id != usuario.Id && u.Nome == nome))
                {
                    ModelState.AddModelError("nome", "O nome já está sendo usado por outro usuário.");
                    return View(); 
                }

                // Verificar se o e-mail já está em uso
                if (_context.Usuarios.Any(u => u.Id != usuario.Id && u.Email == email))
                {
                    ModelState.AddModelError("email", "O e-mail já está sendo usado por outro usuário.");
                    return View(); 
                }

                // Atualize os dados do perfil
                usuario.Nome = nome;
                usuario.Email = email;
                usuario.Telefone = telefone;

                // Salve as alterações no banco de dados
                _context.SaveChanges();

                // Redifinir o valor dos TempDatas para serem recarregados
                TempData["Nome"] = nome;
                TempData["Email"] = email;
                TempData["Telefone"] = telefone;

                return RedirectToAction("Perfil", "Home");          
            }
            return RedirectToAction("Error", "Home");           
        }

        [HttpPost]
        public IActionResult UploadFoto(IFormFile file)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == UsuarioController.IdUserAtual);
            // Verifica se um arquivo foi enviado
            if (file != null && file.Length > 0)
            {
                if (IsImageFile(file))
                {
                    byte[] imageData;

                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        imageData = memoryStream.ToArray();
                        string fotoBase64 = Convert.ToBase64String(imageData);
                        fotoUser = fotoBase64;
                        usuario.Foto = fotoBase64;

                        _context.SaveChanges();
                    }

                    TempData["Nome"] = usuario.Nome;
                    TempData["Email"] = usuario.Email;
                    TempData["Telefone"] = usuario.Telefone;

                    return RedirectToAction("Perfil", "Home");
                }
                else
                {

                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Error", "Home");
        }

        private bool IsImageFile(IFormFile file)
        {
            if (file.ContentType.Contains("image/"))
            {
                string[] validExtensions = { ".png", ".jpg", ".jpeg", ".svg" };
                string fileExtension = Path.GetExtension(file.FileName);
                TempData["TipoImagem"] = fileExtension;
                tipoImagem = fileExtension;
                return validExtensions.Contains(fileExtension.ToLower());
            }
            return false;
        }

        public static byte[] ConvertToPng(byte[] imageBytes)
        {
            using (var image = Image.Load(imageBytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, new PngEncoder());
                    return outputStream.ToArray();
                }
            }
        }

        public static Image ConvertByteArrayToImage(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                var image = Image.Load(ms);
                return image;
            }
        }


    }
}
