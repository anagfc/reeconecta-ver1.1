using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Contatos")]
    public class Contato
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar a mensagem!")]
        [Display(Name = "Mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        [Display(Name = "Data de envio")]
        public DateTime DataEnvio { get; set; } = DateTime.Now;
    }
}
