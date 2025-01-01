# API de Movimentação Bancária

Este projeto implementa uma API REST para gerenciamento de movimentações bancárias, oferecendo endpoints para realizar movimentações e consultar saldos de contas correntes. O projeto foi desenvolvido seguindo as melhores práticas de arquitetura, design patterns e princípios de desenvolvimento de software.

## Objetivo

O sistema permite duas operações principais:
1. Movimentação de conta corrente (crédito ou débito)
2. Consulta de saldo da conta corrente

## Regras de Negócio

### Movimentação de Conta
- Apenas contas cadastradas podem receber movimentação (INVALID_ACCOUNT)
- Apenas contas ativas podem receber movimentação (INACTIVE_ACCOUNT)
- Valores devem ser positivos (INVALID_VALUE)
- Apenas tipos "débito" ou "crédito" são aceitos (INVALID_TYPE)
- Operações são idempotentes através de chave de idempotência

### Consulta de Saldo
- Apenas contas cadastradas podem ser consultadas
- Apenas contas ativas podem ser consultadas
- Saldo é calculado pela fórmula: SOMA_DOS_CREDITOS - SOMA_DOS_DEBITOS
- Contas sem movimentação retornam saldo zero

## Arquitetura e Padrões

### CQRS (Command Query Responsibility Segregation)
Implementamos uma clara separação entre comandos (modificação de estado) e consultas (leitura de dados):
- Commands: CreateMovimentoCommand para operações de movimentação
- Queries: GetSaldoContaCorrenteQuery para consultas de saldo

### Mediator Pattern
Utilizamos o MediatR para implementar o padrão mediator, reduzindo o acoplamento entre componentes:
- Handlers específicos para cada comando/consulta
- Pipeline behaviors para validação e logging
- Encapsulamento da lógica de negócio em handlers dedicados

### Idempotência
Implementamos um mecanismo robusto de idempotência para garantir consistência em caso de falhas:
- Chave de idempotência única por requisição
- Armazenamento de estado em tabela dedicada
- Verificação automática de operações duplicadas

### SOLID Principles
- Single Responsibility: Cada classe tem uma única responsabilidade
- Open/Closed: Extensibilidade através de interfaces e handlers
- Liskov Substitution: Uso correto de herança e abstrações
- Interface Segregation: Interfaces pequenas e focadas
- Dependency Inversion: Injeção de dependências e inversão de controle

### Clean Code
- Nomes descritivos e significativos
- Funções pequenas e focadas
- Comentários explicativos quando necessário
- Tratamento adequado de erros
- Código autoexplicativo

### DRY (Don't Repeat Yourself)
- Reutilização de código através de classes base
- Compartilhamento de validações comuns
- Centralização de lógicas repetitivas

## Tecnologias Utilizadas

### Banco de Dados
- SQLite: Banco de dados leve e embutido
- Dapper: Micro ORM para acesso a dados
- Migrations automáticas na inicialização

### Validação e Documentação
- FluentValidation: Validações declarativas e extensíveis
- Swagger/OpenAPI: Documentação interativa da API em `/docs`
- Exemplos de requisição/resposta na documentação

### Testes
- xUnit: Framework de testes
- NSubstitute: Framework de mocking
- FluentAssertions: Assertions expressivas e legíveis

### Logging e Monitoramento
- Serilog: Logging estruturado
- Logs em arquivo e console
- Rastreamento de operações

## Testes Unitários

### Cobertura de Testes
- Handlers de comandos e consultas
- Validadores
- Regras de negócio
- Casos de erro

### Padrões de Teste
- Arrange-Act-Assert
- Given-When-Then
- Testes isolados e independentes
- Mocks para dependências externas

### Exemplos de Testes
```csharp
[Fact]
public async Task Handle_ComContaValidaESemMovimentos_RetornaSaldoZero()
{
    // Arrange
    var query = CriarQueryValida();
    var conta = CriarContaCorrenteValida();
    var movimentos = Array.Empty<Movimento>();

    ConfigurarMocksParaConsultaSucesso(conta, movimentos);

    // Act
    var resultado = await _handler.Handle(query, CancellationToken.None);

    // Assert
    VerificarRespostaSaldo(resultado, conta, 0m);
}
```

## Como Executar

1. Clone o repositório
```bash
git clone [url-do-repositorio]
cd [nome-do-projeto]
```

2. Restaure as dependências
```bash
dotnet restore
```

3. Execute a aplicação
```bash
dotnet run --project src/Questao5.csproj
```

A aplicação iniciará e:
- As migrations serão executadas automaticamente na inicialização
- A documentação da API estará disponível em `http://localhost:5000/docs`
- Os logs serão gravados em `src/logs/`

## Ideias para próximos passos

- Implementar autenticação e autorização
- Adicionar cache distribuído
- Expandir cobertura de testes
- Aprimorar observability:
  - Distributed tracing (ex: Grafana Tempo, Jaeger, Zipkin)
  - Métricas de performance e negócio (ex: Prometheus, Grafana)
  - Agregação e análise centralizada de logs (ex: Elasticsearch, Loki)
  - Dashboards de monitoramento e alertas
- Adicionar health checks:
  - Monitoramento em tempo real da saúde da aplicação e suas dependências (banco de dados, cache, etc.)
  - Facilita diagnóstico rápido de problemas
  - Integração com orquestradores (ex: Kubernetes) para auto-healing
  - Auxilia em decisões de roteamento de tráfego e load balancing

## Conclusão

Este projeto demonstra a implementação de uma API REST seguindo as melhores práticas e padrões da indústria. A combinação de CQRS, Mediator Pattern, e princípios SOLID resulta em um código limpo, testável e manutenível. O uso de tecnologias modernas como FluentValidation e Dapper proporciona uma base sólida para o desenvolvimento de funcionalidades futuras.