using Microsoft.AspNetCore.Mvc;
using RocketLearning.Models;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace RocketLearning.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly DataContext _context;

        public UsuarioController(DataContext context)
        {
            _context = context;
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
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
            if (usuario != null)
            {
                Debug.WriteLine("VERIFICAÇÃO 01 PASSOU");
                bool senhaCorreta = VerificarSenha(senha, usuario.Senha);
                
                if (senhaCorreta)
                {
                    // Senha correta, redirecionar para a Pagina Inicia
                    return RedirectToAction("PaginaInicial", "Home");
                }
            }
                      
            // Usuário inválido, exibir mensagem de erro ou redirecionar para a tela de login novamente
             ViewBag.MensagemErro = "Email ou senha inválidos.";
             return RedirectToAction("Index", "Home");     
        }

        private bool VerificarSenha(string senha, string senhaCriptografada)
        {
            // Criptografa a senha, para comparar com a criptografia do banco de dados
            string senhaCriptografadaUsuario = CriptografarSenha(senha);
            
            // Comparar as senhas criptografadas
            return senhaCriptografada.Equals(senhaCriptografadaUsuario);
        }
    }
}
