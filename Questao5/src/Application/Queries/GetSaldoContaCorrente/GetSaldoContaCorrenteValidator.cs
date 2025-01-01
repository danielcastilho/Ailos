using Ailos.Application.Common.Validation;
using Ailos.Domain.Stores.Query;
using FluentValidation;

namespace Ailos.Application.Queries.GetSaldoContaCorrente
{
    public class GetSaldoContaCorrenteValidator : BaseValidator<GetSaldoContaCorrenteQuery>
    {
        private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;

        public GetSaldoContaCorrenteValidator(IContaCorrenteQueryStore contaCorrenteQueryStore)
        {
            _contaCorrenteQueryStore = contaCorrenteQueryStore;

            // Utilizando os métodos herdados do BaseValidator
            ValidateGuid(x => x.IdContaCorrente, "ID da conta corrente");
        }
    }
}
