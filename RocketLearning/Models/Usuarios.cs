using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RocketLearning.Models
{
    [Table("usuarios")]
    public class Usuarios
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("telefone")]
        public string Telefone { get; set; }

        [Column("senha")]
        public string Senha { get; set; }

        [Column("codigo")]
        public string? Codigo { get; set; }

        [Column("foto")]
        public string? Foto { get; set; }
    }
}