# ADR-004: Redis para Cache Distribuído

## Status
Aceito

## Contexto
Precisamos de uma estratégia de caching que:
- Funcione em ambiente distribuído (múltiplas instâncias)
- Permita invalidação granular
- Tenha baixa latência
- Seja fácil de monitorar

## Decisão
Implementamos cache distribuído com Redis usando:
- `IDistributedCache` para operações básicas
- `IConnectionMultiplexer` para invalidação por prefixo
- MediatR Behaviors para caching automático de queries

## Estratégia de Cache

### Cache Keys
```
HypeSoft:products:list:{page}:{pageSize}:{categoryId}
HypeSoft:products:detail:{id}
HypeSoft:categories:list:{page}:{pageSize}
HypeSoft:dashboard:stats
```

### Invalidação
- **Por prefixo**: Ao criar/atualizar produto, invalida `HypeSoft:products:*`
- **Por TTL**: Cache expira automaticamente após 5 minutos
- **Manual**: Endpoints de admin podem forçar invalidação

### Behaviors
```csharp
// Caching automático de queries
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    // Verifica cache antes de executar handler
    // Armazena resultado após execução
}

// Invalidação automática após commands
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorCommand
{
    // Invalida prefixos relacionados após command
}
```

## Consequências

### Positivas
- Redução significativa de carga no MongoDB
- Latência < 10ms para dados cacheados
- Invalidação inteligente por prefixo
- Funciona com múltiplas instâncias da API

### Negativas
- Complexidade adicional de infraestrutura
- Possível inconsistência temporária (eventual consistency)
- Necessidade de monitorar uso de memória do Redis
