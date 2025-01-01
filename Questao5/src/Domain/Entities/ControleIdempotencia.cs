namespace Ailos.Domain.Entities
{
    public class ControleIdempotencia
    {
        public string Chave { get; set; }
        public string Requisicao { get; set; }
        public string Resultado { get; set; }
    }
}
