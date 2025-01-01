using Ailos.Domain.Entities;

namespace Ailos.Domain.Stores.Command
{
    public interface IMovimentoCommandStore
    {
        Task<string> CreateAsync(Movimento movimento);
    }
}
