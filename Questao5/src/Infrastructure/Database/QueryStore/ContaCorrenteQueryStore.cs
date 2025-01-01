using Ailos.Domain.Entities;
using Ailos.Domain.Stores.Query;
using Ailos.Infrastructure.Sqlite;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Ailos.Infrastructure.Stores.QueryStore
{
    public class ContaCorrenteQueryStore : IContaCorrenteQueryStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public ContaCorrenteQueryStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<ContaCorrente> GetByIdAsync(string id)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                SELECT 
                    idcontacorrente as Id,
                    numero as Numero,
                    nome as Nome,
                    ativo as Ativo 
                FROM contacorrente 
                WHERE idcontacorrente = @id";

            var contaCorrente = await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                sql,
                new { id }
            );
            return contaCorrente;
        }

        public async Task<ContaCorrente> GetByNumeroAsync(int numero)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                SELECT 
                    idcontacorrente as Id,
                    numero as Numero,
                    nome as Nome,
                    ativo as Ativo 
                FROM contacorrente 
                WHERE numero = @numero";

            var contaCorrente = await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                sql,
                new { numero }
            );
            return contaCorrente;
        }
    }
}
