using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório informar seu documento.")]
        public string Documento { get; set; }

        [Required(ErrorMessage = "É obrigatório informar seu nome.")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Tipo de Perfil")]
        public Tipo TipoPerfil { get; set; }

        [Required(ErrorMessage = "É obrigatório informar seu CEP.")]
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "É obrigatório informar seu endereço.")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "É obrigatório informar ao menos um telefone.")]
        [Display(Name = "Telefone nº1")]
        public string Telefone01 { get; set; }

        [Display(Name = "Telefone nº2")]
        public string Telefone02 { get; set; }

        [Required(ErrorMessage = "É obrigatório informar seu email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É obrigatório inserir uma senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Display(Name = "Situação")]
        public bool ContaAtiva { get; set; }

        [Display(Name = "Criação da Conta")]
        public DataType CriacaoConta { get; set; }

        public ICollection<Produto> Produtos { get; set; }

    }
    public enum Tipo
    {
        Administrador,
        User
    }
}
