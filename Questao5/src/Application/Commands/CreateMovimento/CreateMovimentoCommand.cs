using Ailos.Application.Common.Interfaces;
using Ailos.Domain.Enumerators;

namespace Ailos.Application.Commands.CreateMovimento
{
    public class CreateMovimentoCommand : ICommand<CreateMovimentoResponse>
    {
        public string IdempotencyKey { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
    }
}
