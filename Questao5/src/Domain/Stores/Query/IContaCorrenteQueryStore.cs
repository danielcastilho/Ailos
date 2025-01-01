using Ailos.Domain.Entities;

namespace Ailos.Domain.Stores.Query
{
    public interface IContaCorrenteQueryStore
    {
        Task<ContaCorrente> GetByIdAsync(string id);
        Task<ContaCorrente> GetByNumeroAsync(int numero);
    }
}
