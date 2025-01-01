using Ailos.Domain.Entities;
using Ailos.Domain.Stores.Query;
using Ailos.Infrastructure.Sqlite;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Ailos.Infrastructure.Stores.QueryStore
{
    public class ControleIdempotenciaQueryStore : IControleIdempotenciaQueryStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public ControleIdempotenciaQueryStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<ControleIdempotencia> GetByChaveAsync(string chave)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                SELECT 
                    chave_idempotencia as Chave,
                    requisicao as Requisicao,
                    resultado as Resultado
                FROM idempotencia 
                WHERE chave_idempotencia = @chave";

            var controle = await connection.QueryFirstOrDefaultAsync<ControleIdempotencia>(
                sql,
                new { chave }
            );
            return controle;
        }
    }
}
