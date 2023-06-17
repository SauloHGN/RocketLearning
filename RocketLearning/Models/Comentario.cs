using System.ComponentModel.DataAnnotations.Schema;

namespace RocketLearning.Models
{
    [Table("comentarios")]
    public class Comentario
    {
        [Column("comentarioID")]
        public int ComentarioID { get; set; }

        [Column("autorID")]
        public int AutorID { get; set; }

        [Column("videoID")]
        public string? VideoID { get; set; }

        [Column("autorNome")]
        public string AutorNome { get; set; }

        [Column("texto")]
        public string? Text { get; set; }

        [Column("data")]
        public string? Data { get; set;}
    }
}
