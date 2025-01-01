using Ailos.Domain.Entities;

namespace Ailos.Domain.Stores.Query
{
    public interface IControleIdempotenciaQueryStore
    {
        Task<ControleIdempotencia> GetByChaveAsync(string chave);
    }
}
