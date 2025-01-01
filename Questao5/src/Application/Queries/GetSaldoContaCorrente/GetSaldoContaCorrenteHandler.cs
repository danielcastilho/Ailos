using Ailos.Application.Common.Models;
using Ailos.Application.Common.Validation;
using Ailos.Domain.Entities;
using Ailos.Domain.Enumerators;
using Ailos.Domain.Exceptions;
using Ailos.Domain.Stores.Query;
using MediatR;

namespace Ailos.Application.Queries.GetSaldoContaCorrente;

public class GetSaldoContaCorrenteHandler
    : IRequestHandler<GetSaldoContaCorrenteQuery, GetSaldoContaCorrenteResponse>
{
    private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;
    private readonly IMovimentoQueryStore _movimentoQueryStore;
    private readonly ILogger<GetSaldoContaCorrenteHandler> _logger;

    public GetSaldoContaCorrenteHandler(
        IContaCorrenteQueryStore contaCorrenteQueryStore,
        IMovimentoQueryStore movimentoQueryStore,
        ILogger<GetSaldoContaCorrenteHandler> logger
    )
    {
        _contaCorrenteQueryStore = contaCorrenteQueryStore;
        _movimentoQueryStore = movimentoQueryStore;
        _logger = logger;
    }

    public async Task<GetSaldoContaCorrenteResponse> Handle(
        GetSaldoContaCorrenteQuery query,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Iniciando consulta de saldo para conta {ContaId}",
            query.IdContaCorrente
        );

        // 1. Obter e validar conta corrente
        var conta = await ValidarContaCorrente(query.IdContaCorrente);

        // 2. Calcular saldo da conta
        var saldo = await CalcularSaldoConta(query.IdContaCorrente);

        // 3. Criar e retornar resposta
        var response = CriarRespostaSaldo(conta, saldo);

        _logger.LogInformation(
            "Saldo calculado com sucesso. Conta: {ContaId}, Saldo: {Saldo}",
            query.IdContaCorrente,
            saldo
        );

        return response;
    }

    private async Task<ContaCorrente> ValidarContaCorrente(string idContaCorrente)
    {
        var conta = await _contaCorrenteQueryStore.GetByIdAsync(idContaCorrente);

        if (conta == null)
        {
            _logger.LogWarning("Conta não encontrada: {ContaId}", idContaCorrente);
            throw new BusinessException(
                ValidationMessages.Conta.ContaNaoEncontrada,
                ErrorTypesEnum.InvalidAccount
            );
        }

        if (!conta.Ativo)
        {
            _logger.LogWarning(
                "Tentativa de consulta em conta inativa: {ContaId}",
                idContaCorrente
            );
            throw new BusinessException(
                ValidationMessages.Conta.ContaInativa,
                ErrorTypesEnum.InactiveAccount
            );
        }

        return conta;
    }

    private async Task<decimal> CalcularSaldoConta(string idContaCorrente)
    {
        var movimentos = await _movimentoQueryStore.GetByContaCorrenteIdAsync(idContaCorrente);

        _logger.LogInformation(
            "Processando {QtdMovimentos} movimentos para conta {ContaId}",
            movimentos.Count(),
            idContaCorrente
        );

        return movimentos.Aggregate(
            0m,
            (saldo, movimento) =>
                movimento.TipoMovimento == TipoMovimento.Credito
                    ? saldo + movimento.Valor
                    : saldo - movimento.Valor
        );
    }

    private GetSaldoContaCorrenteResponse CriarRespostaSaldo(ContaCorrente conta, decimal saldo)
    {
        return new GetSaldoContaCorrenteResponse
        {
            NumeroConta = conta.Numero,
            NomeTitular = conta.Nome,
            DataConsulta = DateTime.Now,
            Saldo = saldo,
        };
    }
}
