namespace ApiPiagetDois.Models
{
    //public class Base
    //{
    //    public Guid Id { get; set; }

    //}
        public abstract class Base
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAlteracao { get; set; }
    }
}

