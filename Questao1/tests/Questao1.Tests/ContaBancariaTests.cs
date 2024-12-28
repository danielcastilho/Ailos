using Questao1;
using Xunit;

public class ContaBancariaTests
{
    private readonly IBankingService _bankingService;

    public ContaBancariaTests()
    {
        _bankingService = new BankingService();
    }

    [Fact]
    public void Test_CadastroConta()
    {
        var conta = new ContaBancaria(_bankingService, 10, "Beltrano de Tal", 1000);
        Assert.NotNull(conta);
        Assert.Equal(10, conta.Numero);
        Assert.Equal(1000, conta.Saldo);
    }

    [Fact]
    public void Test_Deposito()
    {
        var conta = new ContaBancaria(_bankingService, 10, "Beltrano de Tal", 1000);
        conta.Deposito(500);
        Assert.Equal(1500, conta.Saldo);
    }

    [Fact]
    public void Test_Saque()
    {
        var conta = new ContaBancaria(_bankingService, 10, "Beltrano de Tal", 1000);
        conta.Saque(300);
        Assert.Equal(696.5, conta.Saldo);
    }

    [Fact]
    public void Test_SaqueSemDepositoInicial()
    {
        var conta = new ContaBancaria(_bankingService, 10, "Beltrano de Tal");
        conta.Saque(300);
        Assert.Equal(-303.5, conta.Saldo);
    }
}
