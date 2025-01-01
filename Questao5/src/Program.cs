using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ailos.Api.Middleware;
using Ailos.Application.Common.Behaviors;
using Ailos.Application.Common.Validation;
using Ailos.Domain.Stores.Command;
using Ailos.Domain.Stores.Query;
using Ailos.Infrastructure.Configuration;
using Ailos.Infrastructure.Sqlite;
using Ailos.Infrastructure.Stores.CommandStore;
using Ailos.Infrastructure.Stores.QueryStore;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Serilog;

// Configuração inicial do Serilog para capturar erros de startup
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação...");

    var builder = WebApplication.CreateBuilder(args);

    // Configuração dos serviços
    ConfigureServices(builder);

    var app = builder.Build();

    // Configurações da aplicação
    ConfigureApplication(app);

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação terminou inesperadamente");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Métodos de extensão locais para organização do código

static void ConfigureServices(WebApplicationBuilder builder)
{
    ConfigureApi(builder);

    builder.Logging.ClearProviders();

    // Usando configuração simplificada do Serilog, usando as configurações do appsettings.json
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
    );

    // Configuração do Controller
    builder
        .Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; //Investigando nulo, removerei depois
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
        });

    // Configuração do CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowSpecificOrigins",
            policy =>
            {
                var origins =
                    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
                policy
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition");
            }
        );
    });

    // Configuração do banco de dados
    ConfigureDatabase(builder.Services, builder.Configuration);

    // MediatR
    ConfigureMediatR(builder);

    // Configurações adicionais
    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

    // Configuração do Swagger
    ConfigureSwagger(builder.Services);

    // Configuração dos serviços de domínio
    ConfigureDomainServices(builder.Services);
}

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "Ailos Bank API",
                Version = "v1",
                Description = "API para gerenciamento de contas correntes e suas movimentações",
                Contact = new OpenApiContact
                {
                    Name = "Ailos",
                    Email = "contato@ailos.coop.br",
                    Url = new Uri("https://www.ailos.coop.br"),
                },
            }
        );
    });
}

static void ConfigureDomainServices(IServiceCollection services)
{
    // Command Stores
    services.AddScoped<IMovimentoCommandStore, MovimentoCommandStore>();
    services.AddScoped<IControleIdempotenciaCommandStore, ControleIdempotenciaCommandStore>();

    // Query Stores
    services.AddScoped<IContaCorrenteQueryStore, ContaCorrenteQueryStore>();
    services.AddScoped<IMovimentoQueryStore, MovimentoQueryStore>();
    services.AddScoped<IControleIdempotenciaQueryStore, ControleIdempotenciaQueryStore>();
}

static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    // Registra o IDatabaseBootstrap
    services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
    // Utiliza a extensão para configurar o banco de dados
    services.AddDatabaseConfiguration(configuration);
}

static void ConfigureApplication(WebApplication app)
{
    var apiSettings = new ApiSettings();
    app.Configuration.GetSection("ApiSettings").Bind(apiSettings);

    // Configuração do Serilog para request logging
    // app.UseSerilogRequestLogging(options =>
    // {
    //     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    //     {
    //         diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
    //         diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    //         diagnosticContext.Set(
    //             "UserAgent",
    //             httpContext.Request.Headers["User-Agent"].ToString()
    //         );
    //     };
    // });

    // Obtém informações do ambiente
    var environment = app.Environment;
    var baseUrl = $"http://localhost:{apiSettings?.Port}";
    var baseUrlHttps = $"https://localhost:{apiSettings?.HttpsPort}";

    // Configura o Swagger ajustando a rota conforme configuração
    if (environment.IsDevelopment() && apiSettings.SwaggerEnabled == true)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ailos Bank API v1");
            c.RoutePrefix = apiSettings.SwaggerRoute;
        });
    }

    // Middleware e configurações da aplicação
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseCors("AllowSpecificOrigins");
    app.UseRouting();
    app.MapControllers();

    InitializeDatabase(app);

    //
    Log.Information("Iniciando Ailos Bank API...");
    Log.Information("Ambiente: {Environment}", environment.EnvironmentName);
    Log.Information("URL HTTP: {Url}", baseUrl);

    if (apiSettings.UseHttps == true)
    {
        Log.Information("URL HTTPS: {Url}", baseUrlHttps);
    }

    if (apiSettings.SwaggerEnabled == true)
    {
        var swaggerUrl = string.IsNullOrEmpty(apiSettings.SwaggerRoute)
            ? baseUrl
            : $"{baseUrl}/{apiSettings.SwaggerRoute}";

        Log.Information("Documentação Swagger: {Url}", swaggerUrl);
    }
}

static void ConfigureMediatR(WebApplicationBuilder builder)
{
    builder.Services.AddMediatR(cfg =>
    {
        // Registra os handlers do assembly
        cfg.RegisterServicesFromAssemblyContaining<Program>();
        // Em caso de multiplos projetos: registrar múltiplos assemblies:
        // cfg.RegisterServicesFromAssemblies(
        //     typeof(Program).Assembly,
        //     typeof(CreateMovimentoCommand).Assembly,
        //     typeof(GetSaldoContaCorrenteQuery).Assembly
        // );

        // Registra os behaviors
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
    });
}

static void InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var databaseBootstrap = scope.ServiceProvider.GetRequiredService<IDatabaseBootstrap>();
    databaseBootstrap.Setup();
}

static void ConfigureApi(WebApplicationBuilder builder)
{
    var apiSettings = GetApiSettings(builder);

    // Criamos uma instância de WebHostBuilder personalizada
    builder
        .WebHost.ConfigureKestrel(
            (context, options) =>
            {
                // Limpa todas as configurações anteriores
                options.ConfigureEndpointDefaults(opt => opt.Protocols = HttpProtocols.Http1);

                if (apiSettings.UseHttps)
                {
                    // Configuração HTTPS
                    options.Listen(
                        IPAddress.Any,
                        apiSettings.HttpsPort,
                        listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            listenOptions.UseHttps();
                        }
                    );

                    // Log da configuração HTTPS
                    Log.Information(
                        "Configurado endpoint HTTPS na porta {Port}",
                        apiSettings.HttpsPort
                    );
                }
                else
                {
                    // Configuração HTTP
                    options.Listen(
                        IPAddress.Any,
                        apiSettings.Port,
                        listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;
                        }
                    );

                    // Log da configuração HTTP
                    Log.Information("Configurado endpoint HTTP na porta {Port}", apiSettings.Port);
                }
            }
        )
        // Isso limpa qualquer configuração prévia de URLs
        .UseUrls()
        // Desabilita a configuração automática do servidor
        .UseDefaultServiceProvider(
            (context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            }
        );

    // Configuração da API
    builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true
    );
}
static ApiSettings GetApiSettings(WebApplicationBuilder builder)
{
    var apiSettings = new ApiSettings();
    builder.Configuration.GetSection("ApiSettings").Bind(apiSettings);
    return apiSettings;
}
