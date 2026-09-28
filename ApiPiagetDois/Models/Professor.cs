using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models
{
    //public class Professor : Base
    //{
    //    [Required(ErrorMessage ="Informe o nome")]
    //    public string? ProfessorNome { get; set; }

    //    [Required]
    //    public string? ProfessorTelefone { get; set; }

    //    [Required]
    //    [EmailAddress(ErrorMessage = "Formato Email Invalido")]
    //    public string? ProfessorEmail { get; set; }

    //    [Required]
    //    public string? ProfessorDisciplina { get; set; }

    //}


    public class Professor : Base
    {
        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string? ProfessorNome { get; set; }

        [Required(ErrorMessage = "Informe o telefone")]
        [Phone(ErrorMessage = "Telefone inválido")]
        public string? ProfessorTelefone { get; set; }

        [Required(ErrorMessage = "Informe o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? ProfessorEmail { get; set; }

        [Required(ErrorMessage = "Informe a disciplina")]
        [StringLength(100, ErrorMessage = "A disciplina deve ter no máximo 100 caracteres")]
        public string? ProfessorDisciplina { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "Informe a escola")]
        public Guid EscolaId { get; set; }

        // Navigation Properties
        public virtual Escola? Escola { get; set; }
        public virtual ICollection<Aluno>? AlunosLecionados { get; set; }
    }

}
