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

        public IActionResult Sobre()
        {
            return PartialView("Sobre");
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

        public IActionResult Upload()
        {
            return PartialView("Upload");//retorna tela de Recuperar Senha      
        }

        public IActionResult TesteVideo()
        {
            return PartialView("TesteVideo");//retorna tela de Teste Video      
        }

        public IActionResult Busca()
        {
            return PartialView("Busca");//retorna tela de busca de video      
        }

        public IActionResult MaisVistos()
        {
            var controller = new PaginaInicialController();
            var videos = controller.MaisVistos().Result;

            return RedirectToAction("PaginaInicial", videos);
        }

        public IActionResult MaisRecentes()
        {
            var controller = new PaginaInicialController();
            var videos = controller.MaisRecentes().Result;

            return RedirectToAction("PaginaInicial", videos);
        }

        public IActionResult TodosVideos()
        {
            var controller = new PaginaInicialController();
            var videos = controller.TesteVideo().Result;

            return RedirectToAction("PaginaInicial", videos);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Sair()
        {
            Response.Redirect("/");

            // Desabilitar o cache da página
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Expires"] = "0";
            Response.Headers["Pragma"] = "no-cache";


            return PartialView("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}