using Ailos.Domain.Entities;

namespace Ailos.Domain.Stores.Query
{
    public interface IMovimentoQueryStore
    {
        Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(string idContaCorrente);
    }
}
