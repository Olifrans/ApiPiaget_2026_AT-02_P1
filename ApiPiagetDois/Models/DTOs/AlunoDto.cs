using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models.DTOs
{
    public class AlunoDto
    {
        public Guid Id { get; set; }
        public string? AlunoNome { get; set; }
        public string? AlunoTelefone { get; set; }
        public string? AlunoEmail { get; set; }
        public string? AlunoMatricula { get; set; }
        public Guid EscolaId { get; set; }
        public string? EscolaNome { get; set; }
        public int TotalProfessores { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class CreateAlunoDto
    {
        [Required]
        [StringLength(100)]
        public string AlunoNome { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string AlunoTelefone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string AlunoEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AlunoMatricula { get; set; } = string.Empty;

        [Required]
        public Guid EscolaId { get; set; }
    }

    public class UpdateAlunoDto
    {
        [StringLength(100)]
        public string? AlunoNome { get; set; }

        [Phone]
        public string? AlunoTelefone { get; set; }

        [EmailAddress]
        public string? AlunoEmail { get; set; }

        [StringLength(50)]
        public string? AlunoMatricula { get; set; }

        public Guid? EscolaId { get; set; }
    }
}
