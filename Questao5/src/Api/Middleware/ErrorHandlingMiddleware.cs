using Ailos.Application.Common.Models;
using Ailos.Domain.Exceptions;
using FluentValidation;

namespace Ailos.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger
        )
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a execução da requisição");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.Type = ErrorTypesEnum.ValidationError;
                    errorResponse.Message = string.Join(
                        "; ",
                        validationEx.Errors.Select(x => x.ErrorMessage)
                    );
                    break;

                case BusinessException businessEx:
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.Type = businessEx.Type;
                    errorResponse.Message = businessEx.Message;
                    break;

                default:
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    errorResponse.Type = ErrorTypesEnum.InternalError;
                    errorResponse.Message = "Ocorreu um erro interno no servidor";
                    break;
            }

            await response.WriteAsJsonAsync(errorResponse);
        }
    }
}
