using Ailos.Domain.Stores.Command;
using Ailos.Infrastructure.Sqlite;

namespace Ailos.Infrastructure.Stores.CommandStore
{
    public class ContaCorrenteCommandStore : IContaCorrenteCommandStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public ContaCorrenteCommandStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
    }
}
