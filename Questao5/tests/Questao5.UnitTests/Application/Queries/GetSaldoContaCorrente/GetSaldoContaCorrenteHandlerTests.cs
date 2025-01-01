using Ailos.Application.Common.Models;
using Ailos.Application.Queries.GetSaldoContaCorrente;
using Ailos.Domain.Entities;
using Ailos.Domain.Enumerators;
using Ailos.Domain.Exceptions;
using Ailos.Domain.Stores.Query;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Questao5.Tests.Application.Queries.GetSaldoContaCorrente;

/// <summary>
/// Testes unitários para o GetSaldoContaCorrenteHandler
/// Responsável por validar:
/// - Consulta de saldo com sucesso
/// - Validações de conta (existência e estado)
/// - Cálculo correto do saldo
/// - Formato da resposta
/// </summary>
public class GetSaldoContaCorrenteHandlerTests
{
    // Mocks dos repositórios e serviços necessários
    private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;
    private readonly IMovimentoQueryStore _movimentoQueryStore;
    private readonly ILogger<GetSaldoContaCorrenteHandler> _logger;
    private readonly GetSaldoContaCorrenteHandler _handler;

    public GetSaldoContaCorrenteHandlerTests()
    {
        // Inicializa os mocks
        _contaCorrenteQueryStore = Substitute.For<IContaCorrenteQueryStore>();
        _movimentoQueryStore = Substitute.For<IMovimentoQueryStore>();
        _logger = Substitute.For<ILogger<GetSaldoContaCorrenteHandler>>();

        // Inicializa o handler com os mocks
        _handler = new GetSaldoContaCorrenteHandler(
            _contaCorrenteQueryStore,
            _movimentoQueryStore,
            _logger
        );
    }

    [Fact]
    public async Task Handle_ComContaValidaESemMovimentos_RetornaSaldoZero()
    {
        // Arrange
        var query = CriarQueryValida();
        var conta = CriarContaCorrenteValida();
        var movimentos = Array.Empty<Movimento>();

        ConfigurarMocksParaConsultaSucesso(conta, movimentos);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        VerificarRespostaSaldo(resultado, conta, 0m);
    }

    [Fact]
    public async Task Handle_ComMovimentosVariados_CalculaSaldoCorretamente()
    {
        // Arrange
        var query = CriarQueryValida();
        var conta = CriarContaCorrenteValida();
        var movimentos = new[]
        {
            CriarMovimento(TipoMovimento.Credito, 1000m), // +1000
            CriarMovimento(TipoMovimento.Debito, 300m), // -300
            CriarMovimento(TipoMovimento.Credito, 500m), // +500
            CriarMovimento(TipoMovimento.Debito, 100m), // -100
        };
        var saldoEsperado = 1100m; // 1000 - 300 + 500 - 100 = 1100

        ConfigurarMocksParaConsultaSucesso(conta, movimentos);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        VerificarRespostaSaldo(resultado, conta, saldoEsperado);
    }

    [Fact]
    public async Task Handle_ComContaInexistente_LancaExcecaoNegocio()
    {
        // Arrange
        var query = CriarQueryValida();
        _contaCorrenteQueryStore.GetByIdAsync(query.IdContaCorrente).ReturnsNull();

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(query, CancellationToken.None)
        );

        excecao.Type.Should().Be(ErrorTypesEnum.InvalidAccount);
    }

    [Fact]
    public async Task Handle_ComContaInativa_LancaExcecaoNegocio()
    {
        // Arrange
        var query = CriarQueryValida();
        var contaInativa = CriarContaCorrenteValida(ativo: false);
        _contaCorrenteQueryStore.GetByIdAsync(query.IdContaCorrente).Returns(contaInativa);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(query, CancellationToken.None)
        );

        excecao.Type.Should().Be(ErrorTypesEnum.InactiveAccount);
    }

    [Fact]
    public async Task Handle_ComApenasCreditos_CalculaSaldoPositivo()
    {
        // Arrange
        var query = CriarQueryValida();
        var conta = CriarContaCorrenteValida();
        var movimentos = new[]
        {
            CriarMovimento(TipoMovimento.Credito, 1000m),
            CriarMovimento(TipoMovimento.Credito, 500m),
        };
        var saldoEsperado = 1500m;

        ConfigurarMocksParaConsultaSucesso(conta, movimentos);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        VerificarRespostaSaldo(resultado, conta, saldoEsperado);
    }

    [Fact]
    public async Task Handle_ComApenasDebitos_CalculaSaldoNegativo()
    {
        // Arrange
        var query = CriarQueryValida();
        var conta = CriarContaCorrenteValida();
        var movimentos = new[]
        {
            CriarMovimento(TipoMovimento.Debito, 1000m),
            CriarMovimento(TipoMovimento.Debito, 500m),
        };
        var saldoEsperado = -1500m;

        ConfigurarMocksParaConsultaSucesso(conta, movimentos);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        VerificarRespostaSaldo(resultado, conta, saldoEsperado);
    }

    #region Métodos Auxiliares

    private GetSaldoContaCorrenteQuery CriarQueryValida()
    {
        return new GetSaldoContaCorrenteQuery { IdContaCorrente = Guid.NewGuid().ToString() };
    }

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

    private Movimento CriarMovimento(TipoMovimento tipo, decimal valor)
    {
        return new Movimento
        {
            Id = Guid.NewGuid().ToString(),
            IdContaCorrente = Guid.NewGuid().ToString(),
            DataMovimento = DateTime.Now,
            TipoMovimento = tipo,
            Valor = valor,
        };
    }

    private void ConfigurarMocksParaConsultaSucesso(
        ContaCorrente conta,
        IEnumerable<Movimento> movimentos
    )
    {
        _contaCorrenteQueryStore.GetByIdAsync(Arg.Any<string>()).Returns(conta);
        _movimentoQueryStore.GetByContaCorrenteIdAsync(Arg.Any<string>()).Returns(movimentos);
    }

    private void VerificarRespostaSaldo(
        GetSaldoContaCorrenteResponse resposta,
        ContaCorrente conta,
        decimal saldoEsperado
    )
    {
        resposta.Should().NotBeNull();
        resposta.NumeroConta.Should().Be(conta.Numero);
        resposta.NomeTitular.Should().Be(conta.Nome);
        resposta.Saldo.Should().Be(saldoEsperado);
        resposta.DataConsulta.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    #endregion
}
