using Ailos.Application.Commands.CreateMovimento;
using Ailos.Application.Common.Models;
using Ailos.Application.Common.Validation;
using Ailos.Domain.Entities;
using Ailos.Domain.Enumerators;
using Ailos.Domain.Exceptions;
using Ailos.Domain.Stores.Command;
using Ailos.Domain.Stores.Query;
using MediatR;

public class CreateMovimentoHandler
    : IRequestHandler<CreateMovimentoCommand, CreateMovimentoResponse>
{
    private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;
    private readonly IMovimentoCommandStore _movimentoCommandStore;
    private readonly IControleIdempotenciaQueryStore _controleIdempotenciaQueryStore;
    private readonly IControleIdempotenciaCommandStore _controleIdempotenciaCommandStore;

    public CreateMovimentoHandler(
        IContaCorrenteQueryStore contaCorrenteQueryStore,
        IMovimentoCommandStore movimentoCommandStore,
        IControleIdempotenciaQueryStore controleIdempotenciaQueryStore,
        IControleIdempotenciaCommandStore controleIdempotenciaCommandStore
    )
    {
        _contaCorrenteQueryStore = contaCorrenteQueryStore;
        _movimentoCommandStore = movimentoCommandStore;
        _controleIdempotenciaQueryStore = controleIdempotenciaQueryStore;
        _controleIdempotenciaCommandStore = controleIdempotenciaCommandStore;
    }

    public async Task<CreateMovimentoResponse> Handle(
        CreateMovimentoCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Verificar se operação já foi processada
        var resultadoIdempotencia = await VerificarIdempotencia(command.IdempotencyKey);
        if (resultadoIdempotencia != null)
        {
            return new CreateMovimentoResponse { Id = resultadoIdempotencia };
        }

        // 2. Validar regras de negócio (complementares ao CreateMovimentoValidator)
        var conta = await ValidarRegrasNegocio(command);

        // 3. Criar e persistir o movimento
        var movimentoId = await CriarMovimento(command, conta.Id);

        // 4. Registrar controle de idempotência
        await RegistrarIdempotencia(command, movimentoId);

        return new CreateMovimentoResponse { Id = movimentoId };
    }

    private async Task<string> VerificarIdempotencia(string chave)
    {
        // Mantém o comportamento original de verificação de idempotência
        var controle = await _controleIdempotenciaQueryStore.GetByChaveAsync(chave);
        return controle?.Resultado;
    }

    private async Task<ContaCorrente> ValidarRegrasNegocio(CreateMovimentoCommand command)
    {
        // Aqui focamos apenas nas regras de negócio que não são cobertas pelo validator
        var conta = await _contaCorrenteQueryStore.GetByIdAsync(command.IdContaCorrente);

        // Validações específicas da conta
        if (conta == null)
        {
            throw new BusinessException(
                ValidationMessages.Conta.ContaNaoEncontrada,
                ErrorTypesEnum.InvalidAccount
            );
        }

        if (!conta.Ativo)
        {
            throw new BusinessException(
                ValidationMessages.Conta.ContaInativa,
                ErrorTypesEnum.InactiveAccount
            );
        }

        return conta;
    }

    private async Task<string> CriarMovimento(CreateMovimentoCommand command, string idConta)
    {
        var movimento = new Movimento
        {
            Id = Guid.NewGuid().ToString(),
            IdContaCorrente = idConta,
            DataMovimento = DateTime.Now,
            TipoMovimento = command.TipoMovimento,
            Valor = command.Valor,
        };

        return await _movimentoCommandStore.CreateAsync(movimento);
    }

    private async Task RegistrarIdempotencia(CreateMovimentoCommand command, string movimentoId)
    {
        var controleIdempotencia = new ControleIdempotencia
        {
            Chave = command.IdempotencyKey,
            Requisicao = System.Text.Json.JsonSerializer.Serialize(command),
            Resultado = movimentoId,
        };

        await _controleIdempotenciaCommandStore.SaveAsync(controleIdempotencia);
    }
}
