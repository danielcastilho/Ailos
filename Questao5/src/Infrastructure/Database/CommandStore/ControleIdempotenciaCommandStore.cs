using Dapper;
using Ailos.Domain.Entities;
using Ailos.Domain.Stores.Command;
using Microsoft.Data.Sqlite;
using Ailos.Infrastructure.Sqlite;

namespace Ailos.Infrastructure.Stores.CommandStore
{
    public class ControleIdempotenciaCommandStore : IControleIdempotenciaCommandStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public ControleIdempotenciaCommandStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task SaveAsync(ControleIdempotencia controle)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                INSERT INTO idempotencia (
                    chave_idempotencia,
                    requisicao,
                    resultado
                ) VALUES (
                    @Chave,
                    @Requisicao,
                    @Resultado
                )";

            await connection.ExecuteAsync(sql, controle);
        }
    }
}
