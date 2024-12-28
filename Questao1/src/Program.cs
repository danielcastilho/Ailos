using System;
using System.Globalization;

namespace Questao1
{
    class Program
    {
        static void Main(string[] args)
        {
            IBankingService bankingService = new BankingService();

            ContaBancaria conta = CadastrarConta(bankingService);
            Deposito(conta);
            Saque(conta);

            /* Output expected:
            Exemplo 1:

            Entre o número da conta: 5447
            Entre o titular da conta: Milton Gonçalves
            Haverá depósito inicial(s / n) ? s
            Entre o valor de depósito inicial: 350.00

            Dados da conta:5447
            Conta 5447, Titular: Milton Gonçalves, Saldo: $ 350.00

            Entre um valor para depósito: 200
            Dados da conta atualizados:
            Conta 5447, Titular: Milton Gonçalves, Saldo: $ 550.00

            Entre um valor para saque: 199
            Dados da conta atualizados:
            Conta 5447, Titular: Milton Gonçalves, Saldo: $ 347.50

            Exemplo 2:
            Entre o número da conta: 5139
            Entre o titular da conta: Elza Soares
            Haverá depósito inicial(s / n) ? n

            Dados da conta:
            Conta 5139, Titular: Elza Soares, Saldo: $ 0.00

            Entre um valor para depósito: 300.00
            Dados da conta atualizados:
            Conta 5139, Titular: Elza Soares, Saldo: $ 300.00

            Entre um valor para saque: 298.00
            Dados da conta atualizados:
            Conta 5139, Titular: Elza Soares, Saldo: $ -1.50
            */
        }

        public static void DadosAtualizados(ContaBancaria conta)
        {
            Console.WriteLine("\nDados da conta atualizados:");
            Console.WriteLine(conta);
            Console.WriteLine();
        }

        public static ContaBancaria CadastrarConta(IBankingService bankingService)
        {
            ContaBancaria result = null;
            Console.Write("Entre o número da conta: ");
            int numero = int.Parse(Console.ReadLine());
            Console.Write("Entre o titular da conta: ");
            string titular = Console.ReadLine();
            Console.Write("Haverá depósito inicial (s/n)? ");
            char resp = char.Parse(Console.ReadLine());
            if (resp == 's' || resp == 'S')
            {
                Console.Write("Entre o valor de depósito inicial: ");
                double depositoInicial = double.Parse(
                    Console.ReadLine(),
                    CultureInfo.InvariantCulture
                );
                result = new ContaBancaria(bankingService, numero, titular, depositoInicial);
            }
            else
            {
                result = new ContaBancaria(bankingService, numero, titular);
            }
            DadosAtualizados(result);
            return result;
        }

        public static void Deposito(ContaBancaria conta)
        {
            Console.Write("Entre um valor para depósito: ");
            double quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            conta.Deposito(quantia);
            DadosAtualizados(conta);
        }

        public static void Saque(ContaBancaria conta)
        {
            Console.Write("Entre um valor para saque: ");
            double quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            conta.Saque(quantia);
            DadosAtualizados(conta);
        }
    }
}
