using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Configuration;

namespace RocketLearning.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }      

        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; } // ACESSAR OS DADOS DA TABELA
        public DbSet<Video> Videos { get; set; }// Acessar a Tebala de Videos
    }
}
