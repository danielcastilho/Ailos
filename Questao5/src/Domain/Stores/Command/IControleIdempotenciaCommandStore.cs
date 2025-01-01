using Ailos.Domain.Entities;

namespace Ailos.Domain.Stores.Command
{
    public interface IControleIdempotenciaCommandStore
    {
        Task SaveAsync(ControleIdempotencia controle);
    }
}
