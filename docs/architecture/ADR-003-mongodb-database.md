# ADR-003: MongoDB como Banco de Dados

## Status
Aceito

## Contexto
Necessidade de um banco de dados flexível para produtos com atributos variáveis, queries dinâmicas e boa performance em leituras.

## Decisão
MongoDB com driver oficial .NET, containerizado via Docker.

## Coleções
- **products**: Produtos com SKU, nome, descrição, preço, categoryId, stockQuantity
- **categories**: Categorias com nome e descrição
- **auditLogs**: Logs de auditoria das operações

## Índices Implementados
- Products: índice em CategoryId, StockQuantity, CreatedAt
- Categories: índice único em Name
- AuditLogs: índice em CreatedAt, EntityType

## Consequências

### Positivas
- Schema flexível para evolução do modelo
- Performance em leituras com índices apropriados
- Fácil containerização e replicação
- Aggregation pipeline para dashboard

### Negativas
- Sem transações ACID multi-documento
- Necessidade de design cuidadoso para consistência
