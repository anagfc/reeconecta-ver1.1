using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reeconecta.Models
{
    [Table("Usuarios")]
    public class Usuario 
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tipo de perfil")]
        public TipoPerfil TipodePerfil { get; set; } = TipoPerfil.PessoaFisica;

        [Required(ErrorMessage = "É obrigatório informar seu documento.")]
        public string Documento { get; set; } = string.Empty;

        //[Required(ErrorMessage = "É obrigatório informar seu nome.")]
        public string? Nome { get; set; }

        //[Required(ErrorMessage = "É obrigatório informar a razão social.")]
        public string? RazaoSocial { get; set; }

        //[Required(ErrorMessage = "É obrigatório informar o nome fantasia.")]
        public string? NomeFantasia { get; set; }

        //[Required(ErrorMessage = "É obrigatório informar o representante legal.")]
        public string? RepresentanteLegal { get; set; }

        //[Required(ErrorMessage = "É obrigatório informar o email do representante legal.")]
        [Display(Name = "Email do representante")]
        [DataType(DataType.EmailAddress)]
        public string? EmailRepresentante { get; set; }

        [Display(Name = "Tipo de Usuário")]
        public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.User;

        [Required(ErrorMessage = "É obrigatório informar seu CEP.")]
        [Display(Name = "CEP")]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "É obrigatório informar seu endereço.")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "É obrigatório informar ao menos um telefone.")]
        [Display(Name = "Telefone nº1")]
        public string Telefone01 { get; set; } = string.Empty;

        [Display(Name = "Telefone nº 1 possui WhatsApp.")]
        public bool WppTel1 { get; set; } = false;

        [Display(Name = "Telefone nº2")]
        public string? Telefone02 { get; set; }

        [Display(Name = "Telefone nº 2 possui WhatsApp.")]
        public bool WppTel2 { get; set; } = false;

        [Required(ErrorMessage = "É obrigatório informar seu email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "É obrigatório inserir uma senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Display(Name = "Situação")]
        public bool ContaAtiva { get; set; } = true;

        [DataType(DataType.DateTime)]
        [Display(Name = "Criação da Conta")]
        public DateTime CriacaoConta { get; set; } = DateTime.Now;

        public ICollection<Produto>? Produtos { get; set; } = new List<Produto>();

        public ICollection<ReservaProduto>? ReservasProduto { get; set; }
    }

    public enum TipoUsuario
    {
        Administrador,
        User
    }

    public enum TipoPerfil
    {
        [Display(Name = "Pessoa Física")]
        PessoaFisica,

        [Display(Name = "Pessoa Jurídica")]
        PessoaJuridica
    }
}
