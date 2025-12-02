# ADR-002: CQRS com MediatR

## Status
Aceito

## Contexto
Precisamos de um padrão para organizar operações de leitura e escrita que:
- Permita otimizações específicas para cada tipo de operação
- Facilite a implementação de cross-cutting concerns
- Mantenha o código desacoplado

## Decisão
Implementamos CQRS (Command Query Responsibility Segregation) usando MediatR:

- **Commands**: Operações de escrita (Create, Update, Delete)
- **Queries**: Operações de leitura (Get, List, Search)
- **Handlers**: Processam commands e queries
- **Behaviors**: Pipeline behaviors para validação, caching, logging

## Implementação

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<ProductDto>;

// Handler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // lógica de criação
    }
}

// Behavior (cross-cutting)
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // validação automática antes de cada handler
}
```

## Consequências

### Positivas
- Queries podem ser cacheadas independentemente
- Validação automática via pipeline
- Fácil adicionar logging, métricas, etc.
- Handlers pequenos e focados

### Negativas
- Mais arquivos (command + handler para cada operação)
- Indireção pode dificultar debugging inicial
