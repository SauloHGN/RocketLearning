using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public IActionResult Create(Comentario comentario)
        {
            // Adicione a lógica para criar um novo comentário no banco de dados
            _context.Comentarios.Add(comentario);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
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
                _context.Comentarios.Remove(comentario);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
