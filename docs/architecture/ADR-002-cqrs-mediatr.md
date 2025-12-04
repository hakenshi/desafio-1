# ADR-002: CQRS com MediatR

## Status
Aceito

## Contexto
Necessidade de organizar operações de leitura e escrita de forma desacoplada, permitindo otimizações específicas e cross-cutting concerns.

## Decisão
Implementamos CQRS usando MediatR com:

- **Commands**: CreateProductCommand, UpdateProductCommand, DeleteProductCommand
- **Queries**: GetAllProductsQuery, GetProductByIdQuery, SearchProductsQuery
- **Handlers**: Um handler por command/query
- **Behaviors**: ValidationBehavior, CachingBehavior, CacheInvalidationBehavior

## Estrutura de Arquivos
```
Application/
├── Products/
│   ├── Commands/
│   │   ├── CreateProductCommand.cs
│   │   └── CreateProductCommandHandler.cs
│   └── Queries/
│       ├── GetAllProductsQuery.cs
│       └── GetAllProductsQueryHandler.cs
└── Behaviors/
    ├── ValidationBehavior.cs
    ├── CachingBehavior.cs
    └── CacheInvalidationBehavior.cs
```

## Consequências

### Positivas
- Queries cacheadas automaticamente via CachingBehavior
- Validação automática via FluentValidation
- Handlers pequenos e testáveis
- Fácil adicionar logging e métricas

### Negativas
- Mais arquivos por operação
- Indireção adicional no fluxo
