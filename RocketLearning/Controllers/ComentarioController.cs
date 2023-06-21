using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MySql.Data.MySqlClient;
using RocketLearning.Models;

namespace RocketLearning.Controllers
{
    public class ComentarioController: Controller
    {

        private readonly DataContext _context;

        public ComentarioController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/Comentario/GetAll")]
        public IActionResult GetAll()
        {
            var comentarios = _context.Comentarios.ToList();
            return PartialView("_CommentList", comentarios);
        }

        [HttpPost]
        [Route("/Comentario/Create")]
        public IActionResult Create(Comentario comentario, string idVideo)
        {

            //lógica para criar um novo comentário no banco de dados
            int autorID = UsuarioController.IdUserAtual;
            string videoID = idVideo;
            var usuario = _context.Set<Usuarios>().FirstOrDefault(u => u.Id == autorID);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            string autorNome = usuario.Nome;      
            string text = Request.Form["textComentario"];               
            DateTime dataAtual = DateTime.Now;
            string data = dataAtual.ToString("dd/MM/yy");

            if(string.IsNullOrEmpty(text))
            {            
                return Content("<script>window.location.reload();</script>", "text/html");
            }
            else if(text.Length >= 250)
            {
                return Content("<script>window.location.reload();</script>", "text/html");
            }

            comentario.AutorID = autorID;
            comentario.VideoID = videoID;
            comentario.AutorNome = autorNome;
            comentario.Text = text;
            comentario.Data = data;

            _context.Comentarios.Add(comentario);
            _context.SaveChanges();

            var comentarios = _context.Comentarios.ToList();
            return Json(new
            {
                comentario.AutorNome,
                comentario.Data,
                comentario.Text
            });
        }

        [HttpPost]
        public IActionResult Edit(Comentario comentario)
        {
            // Adicione a lógica para editar um comentário existente no banco de dados
            _context.Comentarios.Update(comentario);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Adicione a lógica para excluir um comentário pelo ID do banco de dados
            var comentario = _context.Comentarios.Find(id);
            if (comentario != null)
            {
                try
                {
                    _context.Entry(comentario).Reload(); // Recarrega a entidade do banco de dados
                    _context.Comentarios.Remove(comentario);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException ex)
                {             
                    Console.WriteLine("Erro de concorrência: " + ex.Message);
                    return Json(new { success = false, error = "Erro de concorrência" });
                }
            }

            return Json(new { success = false });
        }
    }
}
