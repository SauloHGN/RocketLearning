using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using System.Diagnostics;

namespace RocketLearning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();//retorna tela login
        }

        public IActionResult Cadastrar()
        {
            return PartialView("Cadastrar");// retorna tela cadastro
        }

        public IActionResult PaginaInicial()
        {
            return PartialView("PaginaInicial");//retorna tela da paginal inicial
        }

        public IActionResult Perfil()
        {         
            return PartialView("Perfil");//retorna tela da paginal de perfil       
        }

        public IActionResult RecuperarSenha()
        {
            return PartialView("RecuperarSenha");//retorna tela de Recuperar Senha      
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}