using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models.DTOs
{
    public class ProfessorDto
    {
        public Guid Id { get; set; }
        public string? ProfessorNome { get; set; }
        public string? ProfessorTelefone { get; set; }
        public string? ProfessorEmail { get; set; }
        public string? ProfessorDisciplina { get; set; }
        public Guid EscolaId { get; set; }
        public string? EscolaNome { get; set; }
        public int TotalAlunosLecionados { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class CreateProfessorDto
    {
        [Required]
        [StringLength(100)]
        public string ProfessorNome { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string ProfessorTelefone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string ProfessorEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ProfessorDisciplina { get; set; } = string.Empty;

        [Required]
        public Guid EscolaId { get; set; }
    }

    public class UpdateProfessorDto
    {
        [StringLength(100)]
        public string? ProfessorNome { get; set; }

        [Phone]
        public string? ProfessorTelefone { get; set; }

        [EmailAddress]
        public string? ProfessorEmail { get; set; }

        [StringLength(100)]
        public string? ProfessorDisciplina { get; set; }

        public Guid? EscolaId { get; set; }
    }
}
