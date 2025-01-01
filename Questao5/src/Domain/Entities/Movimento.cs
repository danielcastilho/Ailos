using Ailos.Domain.Enumerators;

namespace Ailos.Domain.Entities
{
    public class Movimento
    {
        public string Id { get; set; }
        public string IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
