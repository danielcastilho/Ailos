using Ailos.Domain.Attributes;
using Ailos.Domain.Enumerators;

namespace Ailos.Domain.Extensions;

public static class TipoMovimentoExtensions
{
    private static StringValueAttribute GetTipoMovimentoAttribute(TipoMovimento tipoMovimento)
    {
        var type = tipoMovimento.GetType();
        var memberInfo = type.GetMember(tipoMovimento.ToString());
        return memberInfo[0]
                .GetCustomAttributes(typeof(StringValueAttribute), false)
                .FirstOrDefault() as StringValueAttribute;
    }

    public static string ToStringValue(this TipoMovimento tipoMovimento)
    {
        var attribute = GetTipoMovimentoAttribute(tipoMovimento);
        return attribute?.Value;
    }
}
