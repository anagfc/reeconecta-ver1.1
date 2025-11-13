using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Pontos")]
    public class PontoDeColeta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome do ponto!")]
        [Display(Name = "Nome do ponto")]
        public string NomePonto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o tipo de ponto!")]
        [Display(Name = "Tipo de ponto")]
        public TipoPonto Tipo { get; set; } 

        [Required(ErrorMessage = "Obrigatório informar a descrição!")]
        [Display(Name = "Descrição")]
        public string? DescricaoPonto { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o CEP!")]
        [Display(Name = "CEP")]
        public string? CepPonto {  get; set; }

        [Required(ErrorMessage = "Obrigatório informar a data de funcionamento!")]
        [Display(Name = "Data de funcionamento")]
        public DayOfWeek FuncionamentoPonto { get; set; }

        [Required(ErrorMessage = "É obrigatório informar o horário de funcionamento!")]
        [Display(Name = "Horário de funcionamento!")]
        public TimeOnly HorarioPonto { get; set; }

        [Required(ErrorMessage = "É obrigatório informar ao menos um telefone.")]
        [Display(Name = "Telefone nº 1")]
        public string TelefoneP01 { get; set; } = string.Empty;

        [Display(Name = "Telefone nº 1 possui WhatsApp.")]
        public bool WppTelP1 { get; set; } = false;

        [Display(Name = "Telefone nº 2")]
        public string TelefoneP02 { get; set; } = string.Empty;

        [Display(Name = "Telefone nº 2 possui WhatsApp.")]
        public bool WppTelP2 { get; set; } = false;

        public string? Imagem { get; set; }

    }

    public enum TipoPonto
    {
        Compra,
        Descarte
    }
}
