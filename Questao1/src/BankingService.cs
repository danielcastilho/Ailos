namespace Questao1
{
    // Não é o escopo deste projeto de avaliação, mas criei este serviço para fornecer funçoes externas à classe ContaBancaria,
    // como a geração de número de conta e a taxa de saque.
    // Em um cenario um pouco mais realista, este serviço poderia consumir uma api externa do banco
    // para gerar o número da conta, obter a taxa de saque ou outras funções.
    // Por isso, a interface IBankingService foi criada, para que o serviço possa ser facilmente substituido e
    // utilizado via injeção de dependência, ou instanciado diretamente.
    public class BankingService : IBankingService
    {
        private int contadorNumeroConta = 1;
        private const double taxaSaque = 3.5;

        public int GerarNumeroConta()
        {
            return contadorNumeroConta++;
        }

        public double TaxaSaque()
        {
            return taxaSaque;
        }
    }
}
