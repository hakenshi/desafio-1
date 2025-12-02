# ADR-003: MongoDB como Banco de Dados Principal

## Status
Aceito

## Contexto
Precisamos escolher um banco de dados que suporte:
- Produtos com atributos variáveis
- Queries flexíveis com filtros dinâmicos
- Boa performance em leituras
- Escalabilidade horizontal

## Decisão
Escolhemos MongoDB com Entity Framework Core provider.

## Justificativa

### Por que NoSQL?
- Produtos podem ter atributos diferentes por categoria
- Schema flexível facilita evolução
- Documentos aninhados reduzem JOINs

### Por que MongoDB especificamente?
- Suporte maduro no ecossistema .NET
- Índices compostos para queries complexas
- Aggregation pipeline para relatórios
- Fácil containerização

### Índices Criados
```javascript
// Produtos
{ "CategoryId": 1, "Name": 1 }  // Filtro por categoria + busca
{ "StockQuantity": 1 }          // Produtos com estoque baixo
{ "CreatedAt": -1 }             // Ordenação por data

// Categorias
{ "Name": 1 }                   // Busca por nome (unique)

// Audit Logs
{ "CreatedAt": -1 }             // Logs recentes
{ "EntityType": 1, "EntityId": 1 } // Histórico de entidade
```

## Consequências

### Positivas
- Flexibilidade de schema
- Performance excelente em leituras
- Fácil de escalar horizontalmente
- Boa integração com Docker

### Negativas
- Sem transações ACID multi-documento (mitigado com design cuidadoso)
- Menos familiar para devs acostumados com SQL
- Aggregations complexas podem ser verbosas
