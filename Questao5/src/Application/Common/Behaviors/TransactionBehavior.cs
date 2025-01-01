using Ailos.Ailos.Application.Common.Attributes;
using Ailos.Application.Common.Interfaces;
using MediatR;

namespace Ailos.Application.Common.Behaviors;

// Incluido para mostrar o conceito. TODO: POC: Implementar um cenário real de uso
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    // No caso de se usar Entity Framework, você poderia injetar o contexto de banco de dados
    // E injetaria seu contexto de banco de dados ou gerenciador de transações
    // private readonly SeuDbContext _dbContext;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger
    // YourDbContext dbContext
    )
    {
        _logger = logger;
        // _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        // Verifica se o request precisa de transação
        if (!IsTransactional(request))
        {
            return await next();
        }

        _logger.LogInformation("Iniciando transação para {RequestName}", typeof(TRequest).Name);

        try
        {
            // Aqui você iniciaria sua transação
            // using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var response = await next();

            // await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Transação concluída com sucesso para {RequestName}",
                typeof(TRequest).Name
            );

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro durante transação para {RequestName}. Realizando rollback.",
                typeof(TRequest).Name
            );

            // await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsTransactional(TRequest request)
    {
        // Verifica se o request é decorado com um atributo de transação
        // ou se é um comando (vs. query)
        return request.GetType().GetCustomAttributes(typeof(TransactionalAttribute), false).Any()
            || request is ICommand<TResponse>;
    }
}
