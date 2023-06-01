using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RocketLearning.Models
{
    [Table("videos")]
    public class Video
    {
        [Key]
        public int? IdVideo { get; set; }

        [Column("titulo")]
        public string? Titulo { get; set; }

        [Column("thumbnail")]
        public string? Thumbnail { get; set; }

        [Column("uploadDate")]
        public DateTime? UploadDate { get; set; }

        [Column("fileId")]
        public string? GoogleDriveFileId { get; set; }

    }
}
