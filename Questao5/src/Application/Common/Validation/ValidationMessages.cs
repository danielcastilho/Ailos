namespace Ailos.Application.Common.Validation
{
    public static class ValidationMessages
    {
        public static string Required(string fieldName) => $"O campo {fieldName} é obrigatório";

        public static string InvalidGuid(string fieldName) =>
            $"O campo {fieldName} deve ser um GUID válido";

        public static string MaxLength(string fieldName, int maxLength) =>
            $"O campo {fieldName} deve ter no máximo {maxLength} caracteres";

        public static string Range(string fieldName, decimal min, decimal max) =>
            $"O campo {fieldName} deve estar entre {min} e {max}";

        public static class Conta
        {
            public const string ContaInexistenteOuInativa = "Conta não encontrada ou inativa";

            public const string ContaNaoEncontrada = "Conta não encontrada";

            public const string ContaInativa = "Conta está inativa";
        }

        public static class Movimento
        {
            public const string ValorMaiorQueZero = "O valor deve ser maior que zero";

            public const string ValorMaximoDebito =
                "Débitos não podem ser maiores que R$ 1.000.000,00";

            public const string TipoMovimentoInvalido =
                "Tipo de movimento deve ser Crédito (C) ou Débito (D)";
        }
    }
}
