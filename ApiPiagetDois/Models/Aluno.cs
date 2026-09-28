using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models
{

    //public class Aluno : Base
    //{
    //    [Required (ErrorMessage ="Informe o nome")]
    //    public string? AlunoNome { get; set; }

    //    [Required]
    //    public string? AlunoTelefone { get; set; }

    //    [Required]
    //    [EmailAddress(ErrorMessage = "Formato Email Invalido")]
    //    public string? AlunoEmail { get; set; }


    //    [Required]
    //    public string? AlunoMatricula { get; set; }
    //}



    /// <summary>
    /// Classe aluno
    /// </summary>
    public class Aluno : Base
    {
        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string? AlunoNome { get; set; }

        [Required(ErrorMessage = "Informe o telefone")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? AlunoTelefone { get; set; }

        [Required(ErrorMessage = "Informe o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? AlunoEmail { get; set; }

        [Required(ErrorMessage = "Informe a matrícula")]
        [StringLength(50, ErrorMessage = "A matrícula deve ter no máximo 50 caracteres")]
        public string? AlunoMatricula { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "Informe a escola")]
        public Guid EscolaId { get; set; }

        // Navigation Properties
        public virtual Escola? Escola { get; set; }
        public virtual ICollection<Professor>? Professores { get; set; }
    }
}
