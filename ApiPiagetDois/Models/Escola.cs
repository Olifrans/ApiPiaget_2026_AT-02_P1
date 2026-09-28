using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models
{
    //public class Escola : Base
    //{
    //    [Required(ErrorMessage ="Informe o nome")]
    //    public string? EscolaNome { get; set; }

    //    [Required]
    //    public string? EscolaTelefone { get; set; }

    //    [Required]
    //    [EmailAddress(ErrorMessage = "Formato Email Invalido")]
    //    public string? EscolaEmail { get; set; }

    //}

    public class Escola : Base
    {
        [Required(ErrorMessage = "Informe o nome da escola")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string? EscolaNome { get; set; }

        [Required(ErrorMessage = "Informe o telefone")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? EscolaTelefone { get; set; }

        [Required(ErrorMessage = "Informe o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? EscolaEmail { get; set; }

        [StringLength(500)]
        public string? EscolaEndereco { get; set; }


        // Navigation Properties
        public virtual ICollection<Professor>? Professores { get; set; }
        public virtual ICollection<Aluno>? Alunos { get; set; }
        public virtual ICollection<AdmEscola>? AdmEscolas { get; set; }
    }



}



