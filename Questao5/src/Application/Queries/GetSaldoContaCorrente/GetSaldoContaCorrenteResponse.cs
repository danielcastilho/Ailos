namespace Ailos.Application.Queries.GetSaldoContaCorrente
{
    public class GetSaldoContaCorrenteResponse
    {
        public int NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; }
        public decimal Saldo { get; set; }
    }
}
