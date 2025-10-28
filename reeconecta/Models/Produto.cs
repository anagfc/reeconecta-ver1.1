using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório dar um nome ao produto.")]
        [Display(Name = "Produto")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório dar um valor ao produto.")]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "É obrigatório informar a condição do produto.")]
        [Display(Name = "Condição")]
        public CondicoesProduto Condicao { get; set; }

        [Display(Name = "Anunciante")]
        [Required(ErrorMessage = "É obrigatório informar o anunciante.")] // Posterioemente, deverá ser puxado automaticamente do usuário logado
        public int AnuncianteId { get; set; }

        [ForeignKey("AnuncianteId")]
        public Usuario Usuario { get; set; }

        public StatusAnuncio StatusAnuncio { get; set; }

        public DataType CriacaoAnuncio { get; set; }

        public bool AnuncioAtivo { get; set; }

    }

    public enum CondicoesProduto
    {
        Novo,
        Seminovo,
        Restaurado,
        Defeituoso,
        Estragado
    }

    public enum StatusAnuncio
    {
        Divulgando,
        Negociando,
        Vendido,
        Retirado
    }


}
