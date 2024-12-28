using System.Globalization;

namespace Questao1
{
    public class ContaBancaria(
        IBankingService bankingService,
        int numero,
        string titular,
        double depositoInicial = 0.0
    )
    {
        private readonly int numero = numero;
        private string titular = titular;
        private double saldo = depositoInicial;
        private readonly IBankingService _bankingService = bankingService;

        public int Numero => numero;

        public string Titular
        {
            get => titular;
            set => titular = value;
        }
        public double Saldo
        {
            get => saldo;
        }

        public void Deposito(double valor)
        {
            saldo += valor;
        }

        public void Saque(double valor)
        {
            saldo -= valor + _bankingService.TaxaSaque();
        }

        public override string ToString()
        {
            return $"Conta {numero}, Titular: {titular}, Saldo: $ {saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
