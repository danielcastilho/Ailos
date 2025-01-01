namespace Ailos.Application.Common.Models
{
    public class ErrorResponse
    {
        /// <summary>
        /// Tipo do erro para identificação
        /// </summary>
        /// <example>VALIDATION_ERROR, INVALID_ACCOUNT, INACTIVE_ACCOUNT</example>
        public ErrorTypesEnum Type { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro
        /// </summary>
        /// <example>O valor deve ser maior que zero</example>
        public string Message { get; set; }
    }
}
