using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AnuncianteId { get; set; }

        [ForeignKey("AnuncianteId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "É obrigaátorio informar o bairro.")]
        public string? Bairro { get; set; }

        [Required(ErrorMessage = "É obrigaátorio informar a cidade.")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "É obrigatório dar um título ao produto.")]
        [StringLength(150)]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "É obrigatório definir o preço do produto.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "É obrigaátorio informar a condição.")]
        public CondicaoProduto? Condicao { get; set; }

        public string? Imagem { get; set; }

        public bool AnuncioAtivo { get; set; } = true;

        public DateTime CriacaoProduto { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "É obrigaátorio informar o status.")]
        public StatusProduto? StatusProduto { get; set; } = Models.StatusProduto.Disponivel;


        public ICollection<ReservaProduto>? ReservasProduto { get; set; }

    }

    public enum CondicaoProduto
    {
        Novo,
        Seminovo,
        Usado,
        Defeituoso
    }

    public enum StatusProduto
    {
        Disponivel,
        Vendido,
        Excluido
    }
}

