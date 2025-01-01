using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Ailos.Domain.Attributes;

namespace Ailos.Domain.Enumerators
{
    public enum TipoMovimento
    {
        [JsonStringEnumMemberName("C")]
        [StringValue("C")]
        [EnumMember(Value = "C")]
        Credito,

        [JsonStringEnumMemberName("D")]
        [StringValue("D")]
        [EnumMember(Value = "D")]
        Debito,
    }
}
