# ADR-004: Redis para Cache Distribuído

## Status
Aceito

## Contexto
Necessidade de cache distribuído para reduzir carga no MongoDB e melhorar latência das queries mais frequentes.

## Decisão
Redis com StackExchange.Redis, integrado via MediatR Behaviors.

## Estratégia de Cache

### Padrão de Chaves
- `HypeSoft:GetAllProductsQuery:{hash}` - Lista de produtos
- `HypeSoft:GetProductByIdQuery:{id}` - Produto individual
- `HypeSoft:GetAllCategoriesQuery:{hash}` - Lista de categorias
- `HypeSoft:GetDashboardQuery:{hash}` - Dados do dashboard

### TTL por Tipo
- Dashboard: 1 minuto
- Listas paginadas: 2 minutos
- Itens individuais: 5 minutos

### Invalidação
- CacheInvalidationBehavior remove prefixos relacionados após Commands
- Criação/atualização de produto invalida queries de produtos e dashboard
- Criação/atualização de categoria invalida queries de categorias

## Consequências

### Positivas
- Latência < 10ms para dados cacheados
- Redução de carga no MongoDB
- Invalidação automática via Behaviors
- Funciona com múltiplas instâncias da API

### Negativas
- Eventual consistency temporária
- Infraestrutura adicional
