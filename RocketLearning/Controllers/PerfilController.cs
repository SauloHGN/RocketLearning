using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RocketLearning.Models;
using System.Diagnostics;

namespace RocketLearning.Controllers
{
    public class PerfilController : Controller
    {
        private readonly DataContext _context;

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

                ViewBag.Nome = usuario.Nome;
                ViewBag.Email = usuario.Email;
                ViewBag.Telefone = usuario.Telefone;

                return RedirectToAction("Perfil", "Home");
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
