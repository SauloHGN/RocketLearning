using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RocketLearning.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using SixLabors.ImageSharp.Formats.Png;
using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp.Formats;
using System.Drawing;
using System.Drawing.Imaging;


namespace RocketLearning.Controllers
{
    public class UsuarioController : Controller
    {
        public static int IdUserAtual { get; set; }
        public static byte[]? fotoUserAtual { get; set; }

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
                string foto = usuario.Foto;

                return usuario;
            }
            throw new Exception("Usuário não encontrado");
        }

        public IActionResult Cadastrar()
        {
            //return View();
            return RedirectToAction("Cadastrar", "Home");
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuarios usuario)
        {
            TempData["erroMensagem"] = "false";
            // obter valores do formulário
            string nome = Request.Form["nome"];
            string email = Request.Form["email"];
            string telefone = Request.Form["telefone"];
            string senha = Request.Form["password"];

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(telefone) || string.IsNullOrEmpty(senha))
            {
                TempData["erroMensagem"] = true;
                return RedirectToAction("Cadastrar", "Home");
            }

            // Verificar se já existe um usuário com o mesmo e-mail e telefone           
            bool emailJaExiste = _context.Usuarios.Any(u => u.Email == email);
            bool telefoneJaExiste = _context.Usuarios.Any(u => u.Telefone == telefone);
            Debug.WriteLine("emailJaExiste: " + emailJaExiste);
            Debug.WriteLine("telefoneJaExiste: " + telefoneJaExiste);
            if (emailJaExiste || telefoneJaExiste)
            {
                TempData["erroMensagem"] = "true";
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

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                TempData["erroLogin"] = true;
                return RedirectToAction("Index", "Home");
            }

            // Consultar o banco de dados para encontrar um usuário com o email e senha fornecidos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email != null && u.Email == email);

            if (usuario != null)
            {              

                bool senhaCorreta = VerificarSenha(senha, usuario.Senha);
                
                if (senhaCorreta)
                {
                    int idUsuario = usuario.Id;
                    UsuarioController.IdUserAtual = usuario.Id;
                    TempData["idUserAtual"] = usuario.Id;
                    RecuperarFoto();
                    return RedirectToAction("PaginaInicial", "Home");
                }
            }

            // Usuário inválido, exibir mensagem de erro ou redirecionar para a tela de login novamente
            TempData["erroLogin"] = "true";
            return RedirectToAction("Index", "Home");     
        }

        public void RecuperarFoto()
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == UsuarioController.IdUserAtual);

            var perfilController = new PerfilController(_context);
        
            var base64Image = usuario.Foto;

            try
            {
                if (!string.IsNullOrEmpty(base64Image))
                {
                    PerfilController.fotoUser = base64Image;


                    byte[] imageBytes = Convert.FromBase64String(base64Image);

                    // Obtendo o tipo da imagem usando ImageFormat
                    ImageFormat imageFormat = ImageFormat.Jpeg; // Define um formato padrão
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                        {
                            imageFormat = image.RawFormat;
                        }
                    }

                    // Obtendo a extensão do formato da imagem
                    string imageExtension = ImageCodecInfo.GetImageEncoders()
                        .First(codec => codec.FormatID == imageFormat.Guid)
                        .FilenameExtension
                        .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .First()
                        .Trim('*')
                        .ToLower();

                    // Exibindo o tipo da imagem
                    Console.WriteLine("Tipo da imagem: " + imageExtension);
                    PerfilController.tipoImagem = imageExtension;
                }
            }
            catch
            {
                PerfilController.fotoUser = null;
                PerfilController.tipoImagem = null;
            }         
        }


        public  int ObterIdUsuario(int idUsuario)
        {
            int IdUserAtual = idUsuario;
            // Lógica para obter o ID do usuário
            Debug.WriteLine("ID de retorno = " + idUsuario);
            return IdUserAtual; // Substitua pelo valor correto
        }

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
            string corpo = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Código de Verificação</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            color: #333;
            padding: 20px;
            margin: 0;
            font-size: 16px;
            line-height: 1.5;
        }}
        .container {{
            background-color: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,.1);
        }}
        h1 {{
            font-size: 24px;
            margin-top: 0;
        }}
        p {{
            margin-bottom: 10px;
        }}
        .code {{
            display: inline-block;
            background-color: #eee;
            padding: 10px;
            border-radius: 5px;
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 20px;          
        }}
        .btn {{
            display: inline-block;
            background-color: #007bff;
            color: #fff;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            margin-bottom: 20px;
            transition: background-color .2s ease;
        }}
        .btn:hover {{
            background-color: #0069d9;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Código de Verificação</h1>
        <p>Olá, recebemos uma solicitação para redefinir sua senha. Use o código abaixo para prosseguir:</p>
        <div class='code'>{codigoRedefinicao}</div>
        <p>Copie o código acima e insira na página de redefinição de senha.</p>
        <p><span style='color: red;'><i>Se você não solicitou essa redefinição, pode ignorar este email.</i></span></p>
        <p>Obrigado,</p>
        <p>Equipe Rocket Learning</p>
    </div>
</body>
</html>";

            MailMessage mensagem = new MailMessage(remetente, emailDestinatario, assunto, corpo);
            mensagem.IsBodyHtml = true;
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
