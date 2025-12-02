# ADR-001: Adoção de Clean Architecture

## Status
Aceito

## Contexto
Precisamos de uma arquitetura que permita:
- Separação clara de responsabilidades
- Testabilidade independente de cada camada
- Facilidade de manutenção e evolução
- Independência de frameworks e bibliotecas externas

## Decisão
Adotamos Clean Architecture com as seguintes camadas:

1. **Domain** - Entidades, Value Objects, Interfaces de repositórios
2. **Application** - Use cases, Commands, Queries, DTOs
3. **Infrastructure** - Implementações de repositórios, serviços externos
4. **API** - Controllers, Middlewares, configurações

## Consequências

### Positivas
- Código altamente testável
- Fácil substituição de tecnologias (ex: trocar MongoDB por PostgreSQL)
- Regras de negócio isoladas e protegidas
- Facilita trabalho em equipe (cada dev pode focar em uma camada)

### Negativas
- Mais código boilerplate
- Curva de aprendizado para novos desenvolvedores
- Pode ser overkill para projetos muito simples

## Alternativas Consideradas
- **MVC tradicional**: Mais simples, mas mistura responsabilidades
- **Vertical Slice**: Boa alternativa, mas menos familiar para a equipe
