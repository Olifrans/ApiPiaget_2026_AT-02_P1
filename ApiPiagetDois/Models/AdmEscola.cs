using System.ComponentModel.DataAnnotations;

namespace ApiPiagetDois.Models
{
    //public class AdmEscola : Base
    //{
    //    public string? AdmNomeFuncionario { get; set; }
    //    public string? AdmFuncaoFuncionario { get; set; }
    //    public string? AdmMatriculaAluno { get; set; }
    //    public DateTime AdmDataCadastroFuncionario { get; set; }
    //    public DateTime AdmDataCadastroAluno { get; set; }
    //}


    public class AdmEscola : Base
    {
        [Required(ErrorMessage = "Informe o nome do funcionário")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string? AdmNomeFuncionario { get; set; }

        [Required(ErrorMessage = "Informe a função")]
        [StringLength(100, ErrorMessage = "A função deve ter no máximo 100 caracteres")]
        public string? AdmFuncaoFuncionario { get; set; }

        public string? AdmMatriculaAluno { get; set; }

        public DateTime AdmDataCadastroFuncionario { get; set; } = DateTime.UtcNow;
        public DateTime AdmDataCadastroAluno { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [Required(ErrorMessage = "Informe a escola")]
        public Guid EscolaId { get; set; }

        // Navigation Property
        public virtual Escola? Escola { get; set; }
    }

}
