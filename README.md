# HypeSoft - Sistema de Gestão de Produtos

Sistema completo de gestão de produtos desenvolvido como desafio técnico, demonstrando arquitetura moderna, boas práticas e tecnologias de ponta.

![Dashboard Preview](https://img.shields.io/badge/Status-Completo-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Next.js](https://img.shields.io/badge/Next.js-15-black)
![License](https://img.shields.io/badge/License-MIT-blue)

## Funcionalidades Implementadas

### Gestão de Produtos
-  CRUD completo de produtos
-  Campos: nome, descrição, preço, categoria, quantidade em estoque
-  Validação de dados obrigatórios
-  Busca por nome do produto
-  Filtro por categoria
-  Paginação eficiente

### Sistema de Categorias
-  CRUD de categorias
-  Associação de produtos a categorias
-  Filtro de produtos por categoria

### Controle de Estoque
-  Controle de quantidade em estoque
-  Atualização manual de estoque
-  Indicador visual de estoque baixo (< 10 unidades)

### Dashboard
-  Total de produtos cadastrados
-  Valor total do estoque
-  Lista de produtos com estoque baixo
-  Gráfico de barras - produtos por categoria
-  Gráfico de pizza - distribuição de categorias
-  Tabela de atividades recentes (audit logs)
-  Tabela de produtos recentes

### Autenticação & Autorização
-  Integração com Keycloak (OAuth2/OpenID Connect)
-  Login/Logout integrado
-  Proteção de rotas no frontend
-  Autorização baseada em roles (Admin, Manager, User)
-  JWT Token validation

### Gestão de Usuários (Admin)
-  Listagem de usuários do Keycloak
-  Criação de novos usuários
-  Edição de usuários existentes
-  Filtro por role
-  Busca por username

## Arquitetura

### Backend - Clean Architecture + DDD + CQRS

```
backend/src/
├── HypeSoft.API/              # Camada de Apresentação
│   ├── Controllers/           # REST Controllers
│   ├── Middlewares/           # Security, Validation, CorrelationId
│   └── Extensions/            # Database Seeder
├── HypeSoft.Application/      # Camada de Aplicação
│   ├── Commands/              # CQRS Commands
│   ├── Queries/               # CQRS Queries
│   ├── Behaviors/             # MediatR Behaviors (Caching, Validation)
│   └── Validators/            # FluentValidation
├── HypeSoft.Domain/           # Camada de Domínio
│   ├── Entities/              # Product, Category, AuditLog
│   ├── Repositories/          # Repository Interfaces
│   └── ValueObjects/          # Money, SKU
└── HypeSoft.Infrastructure/   # Camada de Infraestrutura
    ├── Data/                  # MongoDB Context, Indexes
    ├── Repositories/          # Repository Implementations
    ├── Caching/               # Redis Cache Service
    └── Services/              # Keycloak, Audit
```

### Frontend - Next.js App Router

```
frontend/src/
├── app/                       # App Router Pages
│   ├── (auth)/               # Login page
│   └── (dashboard)/          # Protected pages
├── components/               # React Components
│   ├── ui/                   # shadcn/ui components
│   ├── forms/                # Form components
│   └── dashboard/            # Dashboard components
├── server/                   # Server-side code
│   ├── controllers/          # Server Actions
│   ├── services/             # API Services
│   └── models/               # Zod Schemas
└── tests/                    # Tests
    ├── e2e/                  # Playwright E2E
    └── services/             # Unit tests
```

## Stack Tecnológica

### Backend
| Tecnologia | Uso |
|------------|-----|
| .NET 9 | Framework principal |
| MongoDB | Banco de dados |
| Redis | Cache distribuído |
| MediatR | CQRS pattern |
| FluentValidation | Validação |
| AutoMapper | Mapeamento |
| Serilog | Logging estruturado |
| xUnit + FluentAssertions | Testes |

### Frontend
| Tecnologia | Uso |
|------------|-----|
| Next.js 16 | Framework React |
| TypeScript | Type safety |
| Tailwind CSS | Estilização |
| shadcn/ui | Componentes UI |
| Recharts | Gráficos |
| Zod | Validação de schemas |
| React Hook Form | Formulários |
| Playwright | Testes E2E |
| Vitest | Testes unitários |

### Infraestrutura
| Tecnologia | Uso |
|------------|-----|
| Docker Compose | Orquestração |
| Keycloak | Autenticação |
| Nginx | Reverse proxy |
| MongoDB Express | Admin DB |

## Pré-requisitos

- Docker Desktop 4.0+
- Node.js 18+ (para desenvolvimento)
- .NET 9 SDK (para desenvolvimento)
- Git

## Como Executar

### Com Docker (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/hakenshi/desafio-1.git
cd desafio-1

# Copie as variáveis de ambiente
cp .env.example .env

# Execute toda a aplicação
docker-compose up -d

# Aguarde ~30 segundos para os serviços iniciarem
docker-compose ps
```

### URLs de Acesso

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| Frontend | http://localhost:3000 | admin@hypesoft.com / admin123 |
| API Swagger | http://localhost:5000/swagger | - |
| Keycloak | http://localhost:8080 | admin / admin123 |
| MongoDB Express | http://localhost:8081 | admin / admin123 |

### Desenvolvimento Local

```bash
# Frontend
cd frontend
bun install  # ou npm install
bun dev      # ou npm run dev

# Backend
cd backend/src
dotnet restore
dotnet run --project HypeSoft.API
```

## Testes

### Backend
```bash
cd backend/src

# Testes unitários
dotnet test

# Com coverage
dotnet test --collect:"XPlat Code Coverage"

# Testes de mutação
dotnet stryker
```

### Frontend
```bash
cd frontend

# Testes unitários
bun test

# Testes E2E (requer app rodando)
bun test:e2e
```

## Segurança Implementada

-  Rate Limiting (100 req/min geral, 30 req/min POST)
-  Security Headers (CSP, X-Frame-Options, HSTS, etc.)
-  JWT Token validation com Keycloak
-  CORS configurado
-  Validação em múltiplas camadas
-  Sanitização de inputs
-  Correlation ID para rastreamento

## Performance

- Cache Redis com invalidação inteligente
- Paginação server-side
- Índices MongoDB otimizados
- Server-side rendering (Next.js)
- Suspense + Streaming
- Response < 500ms

## Responsividade

- Layout adaptável (mobile/tablet/desktop)
- Sidebar colapsável em mobile
- Tabelas com scroll horizontal
- Gráficos responsivos
- Touch-friendly

## UX/UI

-  Design moderno baseado no ShopSense
-  Dark/Light mode
-  Loading states com skeletons
-  Toast notifications
-  Error boundary com retry
-  Validação em tempo real

## Estrutura de Pastas

```
.
├── backend/
│   └── src/
│       ├── HypeSoft.API/
│       ├── HypeSoft.Application/
│       ├── HypeSoft.Domain/
│       ├── HypeSoft.Infrastructure/
│       ├── HypeSoft.UnitTests/
│       └── HypeSoft.IntegrationTests/
├── frontend/
│   └── src/
│       ├── app/
│       ├── components/
│       ├── server/
│       └── tests/
├── shared/
│   ├── keycloak/          # Realm config
│   └── nginx/             # Reverse proxy config
├── docker-compose.yaml
└── README.md
```

## Padrões de Commit

Seguindo [Conventional Commits](https://conventionalcommits.org/):

```
feat(scope): nova funcionalidade
fix(scope): correção de bug
docs(scope): documentação
refactor(scope): refatoração
test(scope): testes
perf(scope): performance
chore(scope): manutenção
```

## Decisões Arquiteturais

### Por que MongoDB?
- Flexibilidade de schema para produtos com atributos variados
- Performance em leituras com índices compostos
- Integração nativa com .NET via EF Core

### Por que Redis para Cache?
- Cache distribuído para escalabilidade horizontal
- Invalidação por prefixo para grupos de queries
- TTL configurável por tipo de dado

### Por que CQRS?
- Separação clara entre leitura e escrita
- Facilita caching de queries
- Behaviors reutilizáveis (validation, caching)

### Por que Next.js App Router?
- Server Components para melhor performance
- Server Actions para mutations type-safe
- Streaming e Suspense nativos

## Autor

Desenvolvido como parte do desafio técnico HypeSoft.

## 📄 Licença

Este projeto está sob a licença MIT.
