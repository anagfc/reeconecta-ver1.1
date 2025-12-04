using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Avaliacoes")]
    public class Avaliacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PontoId { get; set; }

        [ForeignKey("PontoId")]
        public Ponto? Ponto { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [Range(0, 5, ErrorMessage = "A nota deve estar entre 0 e 5.")]
        [Display(Name = "Nota")]
        public decimal Nota { get; set; }

        [StringLength(500)]
        [Display(Name = "Comentário")]
        public string? Comentario { get; set; }

        [Display(Name = "Data da Avaliação")]
        public DateTime DataAvaliacao { get; set; } = DateTime.Now;
    }
}