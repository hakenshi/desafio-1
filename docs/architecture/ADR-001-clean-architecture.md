# ADR-001: Clean Architecture

## Status
Aceito

## Contexto
O projeto HypeSoft necessita de uma arquitetura que permita separação de responsabilidades, testabilidade e independência de frameworks externos.

## Decisão
Adotamos Clean Architecture com quatro camadas:

1. **HypeSoft.Domain** - Entidades (Product, Category), interfaces de repositórios
2. **HypeSoft.Application** - Commands, Queries, Handlers, DTOs, Validators
3. **HypeSoft.Infrastructure** - Repositórios MongoDB, serviços Redis e Keycloak
4. **HypeSoft.API** - Controllers REST, middlewares, configuração DI

## Consequências

### Positivas
- Código testável com injeção de dependências
- Facilidade para substituir tecnologias
- Regras de negócio isoladas na camada Domain
- Separação clara entre leitura e escrita

### Negativas
- Maior quantidade de arquivos e projetos
- Curva de aprendizado inicial

## Alternativas Consideradas
- MVC tradicional: descartado por misturar responsabilidades
- Vertical Slice: considerado, mas Clean Architecture é mais familiar
