using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("ReservaProduto")]
    public class ReservaProduto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public DateTime DataReserva { get; set; } = DateTime.Now;

        public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    }

    public enum StatusReserva
    {
        Pendente,
        Confirmada,
        Cancelada
    }
}
