using System.Text.Json.Serialization;
using Ailos.Domain.Enumerators;

namespace Ailos.Application.Commands.CreateMovimento
{
    /// <summary>
    /// Classe usada para representar o request de criação de movimento.
    /// </summary>
    public class CreateMovimentoRequest
    {
        [JsonPropertyName("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("tipoMovimento")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoMovimento TipoMovimento { get; set; }
    }
}
