using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RocketLearning.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;


namespace RocketLearning.Controllers
{
    public class UsuarioController : Controller
    {
        public static int? IdUserAtual { get; set; }

        private readonly DataContext _context;

        public UsuarioController(DataContext context)
        {
            _context = context;
        }

        public Usuarios ObterUsuario(int idUsuario)
        {
            var usuario = _context.Set<Usuarios>().FirstOrDefault(u => u.Id == idUsuario);
            if (usuario == null)
            {
                string nome = usuario.Nome;
                string email = usuario.Email;
                string telefone = usuario.Telefone;

                return usuario;
            }
            throw new Exception("Usuário não encontrado");
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuarios usuario)
        {
            // obter valores do formulário
            string nome = Request.Form["nome"];
            string email = Request.Form["email"];
            string telefone = Request.Form["telefone"];
            string senha = Request.Form["password"];

            // Verificar se já existe um usuário com o mesmo e-mail e telefone           
            bool emailJaExiste = _context.Usuarios.Any(u => u.Email == email);
            bool telefoneJaExiste = _context.Usuarios.Any(u => u.Telefone == telefone);
            Debug.WriteLine("emailJaExiste: " + emailJaExiste);
            Debug.WriteLine("telefoneJaExiste: " + telefoneJaExiste);
            if (emailJaExiste || telefoneJaExiste)
            {
                // 
                return RedirectToAction("Cadastrar", "Home");
            }

            // Criptografar a senha antes de salvar no banco de dados
            usuario.Senha = CriptografarSenha(senha);

            // Adicionar o usuário ao contexto e salvar as mudanças no banco de dados
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");          
        }

        private string CriptografarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                throw new ArgumentException("A senha não pode ser nula ou vazia", nameof(senha));
            }


            // lógica de criptografia da senha
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                //return senha;
                return builder.ToString();
            }
        }


        // INICIO LOGICA LOGIN
        [HttpPost]
        public IActionResult VerificarLogin()
        {          
            string email = Request.Form["email"];
            string senha = Request.Form["password"];
            Debug.WriteLine("Email: " + email);
            Debug.WriteLine("Senha: " + senha);

            // Consultar o banco de dados para encontrar um usuário com o email e senha fornecidos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email != null && u.Email == email);

            if (usuario != null)
            {              

                bool senhaCorreta = VerificarSenha(senha, usuario.Senha);
                
                if (senhaCorreta)
                {
                    int idUsuario = usuario.Id;
                    UsuarioController.IdUserAtual = usuario.Id;
                    //ObterIdUsuario(idUsuario);                 
                    // Senha correta, redirecionar para a Pagina Inicia
                    return RedirectToAction("PaginaInicial", "Home");
                }
            }
                      
            // Usuário inválido, exibir mensagem de erro ou redirecionar para a tela de login novamente
             ViewBag.MensagemErro = "Email ou senha inválidos.";
             return RedirectToAction("Index", "Home");     
        }

        /*public  int ObterIdUsuario(int idUsuario)
        {
            int IdUserAtual = idUsuario;
            // Lógica para obter o ID do usuário
            Debug.WriteLine("ID de retorno = " + idUsuario);
            return IdUserAtual; // Substitua pelo valor correto
        }*/

        private bool VerificarSenha(string senha, string senhaCriptografada)
        {
            // Criptografa a senha, para comparar com a criptografia do banco de dados
            string senhaCriptografadaUsuario = CriptografarSenha(senha);

            // Comparar as senhas criptografadas
            return StringComparer.OrdinalIgnoreCase.Equals(senhaCriptografada, senhaCriptografadaUsuario);
        }
        // FIM LOGICA LOGIN
        public IActionResult EsqueciMinhaSenha()
        {
            string email = Request.Form["email"];
            ViewData["Email"] = email;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email != null && u.Email == email);

            if (usuario != null)
            {
                string codigoRedefinicao = GerarCodigoRedefinicao();
                usuario.Codigo = codigoRedefinicao;
                _context.SaveChanges();

                EnviarEmailRedefinicaoSenha(email, codigoRedefinicao);

                ViewData["EmailContainerDisplay"] = "none";
                ViewData["CodigoContainerDisplay"] = "block";
                ViewData["RedefinirContainerDisplay"] = "none";

                return View("~/Views/Home/RecuperarSenha.cshtml");
            }

            return View("~/Views/Home/RecuperarSenha.cshtml");
        }

        private string GerarCodigoRedefinicao()
        {
            Random random = new Random();
            int codigo = random.Next(1000, 10000);
            return codigo.ToString();
        }

        private void EnviarEmailRedefinicaoSenha(string emailDestinatario, string codigoRedefinicao)
        {
            string remetente = "SuporteRocketLearning@gmail.com";
            string senhaRemetente = "xkmqzpgvkfvtfvzp";
            string assunto = "Redefinição de senha";
            string corpo = $"Seu código de redefinição de senha é: {codigoRedefinicao}";

            MailMessage mensagem = new MailMessage(remetente, emailDestinatario, assunto, corpo);
            SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587);
            clienteSmtp.Credentials = new NetworkCredential(remetente, senhaRemetente);
            clienteSmtp.EnableSsl = true;
            clienteSmtp.Send(mensagem);
        }

        public IActionResult VerificarCodigoRedefinicao(string email, string codigoDigitado)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email != null && u.Email == email);

            if (usuario != null && string.Equals(usuario.Codigo, codigoDigitado))
            {
                usuario.Codigo = null;
                ViewData["EmailContainerDisplay"] = "none";
                ViewData["CodigoContainerDisplay"] = "none";
                ViewData["RedefinirContainerDisplay"] = "block";
                ViewData["Email"] = email;
                return View("~/Views/Home/RecuperarSenha.cshtml");
            }
            else
            {
                ViewData["EmailContainerDisplay"] = "none";
                ViewData["CodigoContainerDisplay"] = "block";
                ViewData["RedefinirContainerDisplay"] = "none";
                ViewData["Email"] = email;
                return View("~/Views/Home/RecuperarSenha.cshtml");
            }
        }
        public IActionResult AtualizarSenhaUsuario(string email, string novaSenha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email != null && u.Email == email);

            if (usuario != null)
            {
                string senhaCriptografada = CriptografarSenha(novaSenha);
                usuario.Senha = senhaCriptografada;
                _context.SaveChanges();

                return View("~/Views/Home/Index.cshtml");
            }
            else
            {
                ViewData["EmailContainerDisplay"] = "none";
                ViewData["CodigoContainerDisplay"] = "none";
                ViewData["RedefinirContainerDisplay"] = "block";

                return View("~/Views/Home/RecuperarSenha.cshtml");
            }
        }
    }
}
