using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using Ailos.Domain.Entities;
using Ailos.Domain.Enumerators;
using Ailos.Domain.Stores.Query;
using Ailos.Infrastructure.Sqlite;
using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Ailos.Infrastructure.Stores.QueryStore
{
    public class MovimentoQueryStore : IMovimentoQueryStore
    {
        private readonly DatabaseConfig _databaseConfig;

        public MovimentoQueryStore(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<IEnumerable<Movimento>> GetByContaCorrenteIdAsync(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            const string sql =
                @"
                SELECT 
                    idmovimento as Id,
                    idcontacorrente as IdContaCorrente,
                    datamovimento as DataMovimento,
                    tipomovimento as TipoMovimento,
                    valor as Valor
                FROM movimento 
                WHERE idcontacorrente = @idContaCorrente";

            var movimentos = await connection.QueryAsync<MovimentoDTO>(
                sql,
                new { idContaCorrente }
            );

            return movimentos.Select(dto => new Movimento
            {
                Id = dto.Id,
                IdContaCorrente = dto.IdContaCorrente,
                DataMovimento = DateTime.ParseExact(
                    dto.DataMovimento,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture
                ),
                TipoMovimento = dto.TipoMovimento switch
                {
                    "C" => TipoMovimento.Credito,
                    "D" => TipoMovimento.Debito,
                    _ => throw new InvalidEnumArgumentException(
                        $"Tipo de movimento inválido: {dto.TipoMovimento}"
                    ),
                },
                Valor = dto.Valor,
            });
        }

        private class MovimentoDTO
        {
            public string Id { get; set; }
            public string IdContaCorrente { get; set; }
            public string DataMovimento { get; set; }
            public string TipoMovimento { get; set; }
            public decimal Valor { get; set; }
        }
    }
}
