namespace Ailos.Application.Common.Behaviors;

public static class LoggingBehaviorExtensions
{
    public static string SanitizeForLog(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Lista de campos sensíveis que devem ser mascarados
        var sensitiveFields = new[]
        {
            "password",
            "senha",
            "secret",
            "token",
            "authorization",
            "api-key",
            "apikey",
            "creditcard",
            "cartao",
        };

        // Substitui valores de campos sensíveis
        foreach (var field in sensitiveFields)
        {
            var pattern = $@"""{field}"":\s*""[^""]+""";
            input = System.Text.RegularExpressions.Regex.Replace(
                input,
                pattern,
                $@"""{field}"": ""***""",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        return input;
    }
}
