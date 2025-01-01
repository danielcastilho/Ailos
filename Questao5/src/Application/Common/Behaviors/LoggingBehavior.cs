using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // Início do processamento
        _logger.LogInformation(
            "Iniciando request {RequestName}. Dados: {Request}",
            typeof(TRequest).Name,
            SerializeRequest(request)
        );

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            // Log de sucesso
            _logger.LogInformation(
                "Request {RequestName} concluído com sucesso. Tempo de execução: {ElapsedMilliseconds}ms. Response: {Response}",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                SerializeResponse(response)
            );

            return response;
        }
        catch (Exception ex)
        {
            // Log de erro
            _logger.LogError(
                ex,
                "Erro ao processar request {RequestName}. Erro: {Error}",
                typeof(TRequest).Name,
                ex.Message
            );

            throw;
        }
    }

    private static string SerializeRequest(TRequest request)
    {
        try
        {
            // Configurações para serialização segura
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                MaxDepth = 5,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            return JsonSerializer.Serialize(request, options);
        }
        catch (Exception ex)
        {
            return $"Erro ao serializar request: {ex.Message}";
        }
    }

    private static string SerializeResponse(TResponse response)
    {
        try
        {
            if (response == null)
                return "null";

            // Se for um tipo primitivo ou string, retorna diretamente
            if (response.GetType().IsPrimitive || response is string)
                return response.ToString() ?? "null";

            // Configurações para serialização segura
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                MaxDepth = 5,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            return JsonSerializer.Serialize(response, options);
        }
        catch (Exception ex)
        {
            return $"Erro ao serializar response: {ex.Message}";
        }
    }
}
