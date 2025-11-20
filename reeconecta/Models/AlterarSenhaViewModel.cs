using System.ComponentModel.DataAnnotations;

namespace reeconecta.Models
{
    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação devem ser iguais.")]
        public string ConfirmacaoSenha { get; set; } = string.Empty;
    }
}