using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models.DTOs
{
    public class EscolaDto
    {
        public Guid Id { get; set; }
        public string? EscolaNome { get; set; }
        public string? EscolaTelefone { get; set; }
        public string? EscolaEmail { get; set; }
        public string? EscolaEndereco { get; set; }
        public int TotalProfessores { get; set; }
        public int TotalAlunos { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class CreateEscolaDto
    {
        [Required]
        [StringLength(100)]
        public string EscolaNome { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string EscolaTelefone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EscolaEmail { get; set; } = string.Empty;

        [StringLength(500)]
        public string? EscolaEndereco { get; set; }
    }

    public class UpdateEscolaDto
    {
        [StringLength(100)]
        public string? EscolaNome { get; set; }

        [Phone]
        public string? EscolaTelefone { get; set; }

        [EmailAddress]
        public string? EscolaEmail { get; set; }

        [StringLength(500)]
        public string? EscolaEndereco { get; set; }
    }
}
