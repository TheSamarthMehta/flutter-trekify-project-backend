using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrekifyBackend.Models
{
    [Table("Treks")]
    public class Trek
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SerialNumber { get; set; }

        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TrekName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TrekType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DifficultyLevel { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Season { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Distance { get; set; } = string.Empty;

        [MaxLength(100)]
        public string MaxAltitude { get; set; } = string.Empty;

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string TrekDescription { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Image { get; set; } = string.Empty;
    }
}
