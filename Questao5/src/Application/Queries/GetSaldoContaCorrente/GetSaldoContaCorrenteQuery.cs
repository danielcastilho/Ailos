using Ailos.Application.Common.Interfaces;
using MediatR;

namespace Ailos.Application.Queries.GetSaldoContaCorrente
{
    public class GetSaldoContaCorrenteQuery : IQuery<GetSaldoContaCorrenteResponse>
    {
        public string IdContaCorrente { get; set; }
    }
}
