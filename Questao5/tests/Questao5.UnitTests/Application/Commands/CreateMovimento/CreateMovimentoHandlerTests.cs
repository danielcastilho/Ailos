using Ailos.Application.Commands.CreateMovimento;
using Ailos.Domain.Entities;
using Ailos.Domain.Enumerators;
using Ailos.Domain.Exceptions;
using Ailos.Domain.Stores.Command;
using Ailos.Domain.Stores.Query;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Questao5.Tests.Application.Commands.CreateMovimento;

/// <summary>
/// Testes unitários para o CreateMovimentoHandler
/// Responsável por validar o comportamento do handler em diferentes cenários:
/// - Verificação de idempotência
/// - Criação bem-sucedida de movimento
/// - Validações de regras de negócio
/// </summary>
public class CreateMovimentoHandlerTests
{
    // Mocks dos repositórios necessários
    private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;
    private readonly IMovimentoCommandStore _movimentoCommandStore;
    private readonly IControleIdempotenciaQueryStore _controleIdempotenciaQueryStore;
    private readonly IControleIdempotenciaCommandStore _controleIdempotenciaCommandStore;
    private readonly CreateMovimentoHandler _handler;

    public CreateMovimentoHandlerTests()
    {
        // Inicializa os mocks usando NSubstitute
        _contaCorrenteQueryStore = Substitute.For<IContaCorrenteQueryStore>();
        _movimentoCommandStore = Substitute.For<IMovimentoCommandStore>();
        _controleIdempotenciaQueryStore = Substitute.For<IControleIdempotenciaQueryStore>();
        _controleIdempotenciaCommandStore = Substitute.For<IControleIdempotenciaCommandStore>();

        // Inicializa o handler com os mocks
        _handler = new CreateMovimentoHandler(
            _contaCorrenteQueryStore,
            _movimentoCommandStore,
            _controleIdempotenciaQueryStore,
            _controleIdempotenciaCommandStore
        );
    }

    [Fact]
    public async Task Handle_QuandoOperacaoJaProcessada_RetornaIdMovimentoExistente()
    {
        // Arrange
        var command = CriarCommandValido();
        var idMovimentoExistente = Guid.NewGuid().ToString();
        var controleIdempotencia = new ControleIdempotencia
        {
            Chave = command.IdempotencyKey,
            Resultado = idMovimentoExistente,
        };

        // Configura o mock para retornar um controle de idempotência existente
        _controleIdempotenciaQueryStore
            .GetByChaveAsync(command.IdempotencyKey)
            .Returns(controleIdempotencia);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Id.Should().Be(idMovimentoExistente);
        // Verifica que não foi criado um novo movimento
        await _movimentoCommandStore.DidNotReceive().CreateAsync(Arg.Any<Movimento>());
    }

    [Fact]
    public async Task Handle_ComCommandValido_CriaMovimentoComSucesso()
    {
        // Arrange
        var command = CriarCommandValido();
        var contaCorrente = CriarContaCorrenteValida();
        var idMovimentoEsperado = Guid.NewGuid().ToString();

        ConfigurarMocksParaCriacaoSucesso(command, contaCorrente, idMovimentoEsperado);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Id.Should().Be(idMovimentoEsperado);
        await VerificarCriacaoMovimento(command, contaCorrente.Id);
        await VerificarRegistroIdempotencia(command, idMovimentoEsperado);
    }

    [Fact]
    public async Task Handle_ComContaInvalida_LancaExcecaoNegocio()
    {
        // Arrange
        var command = CriarCommandValido();
        // Configura o mock para retornar null, simulando conta não encontrada
        _contaCorrenteQueryStore.GetByIdAsync(command.IdContaCorrente).ReturnsNull();

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        excecao.Type.Should().Be(ErrorTypesEnum.InvalidAccount);
    }

    [Fact]
    public async Task Handle_ComContaInativa_LancaExcecaoNegocio()
    {
        // Arrange
        var command = CriarCommandValido();
        var contaInativa = CriarContaCorrenteValida(ativo: false);
        _contaCorrenteQueryStore.GetByIdAsync(command.IdContaCorrente).Returns(contaInativa);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        excecao.Type.Should().Be(ErrorTypesEnum.InactiveAccount);
    }

    [Fact]
    public async Task Handle_ComValorDebitoAlto_LancaExcecaoNegocio()
    {
        // Arrange
        var command = CriarCommandValido(TipoMovimento.Debito, 1000001m); // Valor > 1.000.000
        var conta = CriarContaCorrenteValida();
        _contaCorrenteQueryStore.GetByIdAsync(command.IdContaCorrente).Returns(conta);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        excecao.Type.Should().Be(ErrorTypesEnum.InvalidValue);
    }

    #region Métodos Auxiliares

    /// <summary>
    /// Cria um comando válido para testes
    /// </summary>
    private CreateMovimentoCommand CriarCommandValido(
        TipoMovimento tipo = TipoMovimento.Credito,
        decimal valor = 100m
    )
    {
        return new CreateMovimentoCommand
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            IdempotencyKey = Guid.NewGuid().ToString(),
            TipoMovimento = tipo,
            Valor = valor,
        };
    }

    /// <summary>
    /// Cria uma conta corrente válida para testes
    /// </summary>
    private ContaCorrente CriarContaCorrenteValida(bool ativo = true)
    {
        return new ContaCorrente
        {
            Id = Guid.NewGuid().ToString(),
            Numero = 123,
            Nome = "Conta Teste",
            Ativo = ativo,
        };
    }

    /// <summary>
    /// Configura os mocks para um cenário de criação bem-sucedida
    /// </summary>
    private void ConfigurarMocksParaCriacaoSucesso(
        CreateMovimentoCommand command,
        ContaCorrente conta,
        string idMovimentoEsperado
    )
    {
        _controleIdempotenciaQueryStore.GetByChaveAsync(command.IdempotencyKey).ReturnsNull();
        _contaCorrenteQueryStore.GetByIdAsync(command.IdContaCorrente).Returns(conta);
        _movimentoCommandStore.CreateAsync(Arg.Any<Movimento>()).Returns(idMovimentoEsperado);
    }

    /// <summary>
    /// Verifica se o movimento foi criado corretamente
    /// </summary>
    private async Task VerificarCriacaoMovimento(CreateMovimentoCommand command, string idConta)
    {
        await _movimentoCommandStore
            .Received(1)
            .CreateAsync(
                Arg.Is<Movimento>(m =>
                    m.IdContaCorrente == idConta
                    && m.TipoMovimento == command.TipoMovimento
                    && m.Valor == command.Valor
                )
            );
    }

    /// <summary>
    /// Verifica se o controle de idempotência foi registrado corretamente
    /// </summary>
    private async Task VerificarRegistroIdempotencia(
        CreateMovimentoCommand command,
        string idMovimento
    )
    {
        await _controleIdempotenciaCommandStore
            .Received(1)
            .SaveAsync(
                Arg.Is<ControleIdempotencia>(c =>
                    c.Chave == command.IdempotencyKey && c.Resultado == idMovimento
                )
            );
    }

    #endregion
}
