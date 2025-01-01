using Ailos.Domain.Entities;
using Ailos.Domain.Stores.Command;
using Ailos.Infrastructure.Sqlite;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Ailos.Infrastructure.Stores.CommandStore
{
    public class MovimentoCommandStore : IMovimentoCommandStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public MovimentoCommandStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<string> CreateAsync(Movimento movimento)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                INSERT INTO movimento (
                    idmovimento,
                    idcontacorrente,
                    datamovimento,
                    tipomovimento,
                    valor
                ) VALUES (
                    @Id,
                    @IdContaCorrente,
                    @DataMovimento,
                    @TipoMovimento,
                    @Valor
                )";

            await connection.ExecuteAsync(
                sql,
                new
                {
                    movimento.Id,
                    movimento.IdContaCorrente,
                    DataMovimento = movimento.DataMovimento.ToString("dd/MM/yyyy"),
                    TipoMovimento = movimento.TipoMovimento.ToStringValue(),
                    movimento.Valor,
                }
            );

            return movimento.Id;
        }
    }
}
