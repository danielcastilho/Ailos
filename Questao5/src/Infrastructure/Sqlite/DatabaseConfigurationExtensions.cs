using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ailos.Infrastructure.Sqlite
{
    public static class DatabaseConfigurationExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Registra a configuração como singleton
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));

            // Também registra a classe diretamente para injeção
            var dbConfig = new DatabaseConfig();
            configuration.GetSection("DatabaseConfig").Bind(dbConfig);
            services.AddSingleton(dbConfig);

            return services;
        }
    }
}
