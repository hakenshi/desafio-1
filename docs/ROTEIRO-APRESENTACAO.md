# Roteiro Técnico — HypeSoft — 10 Minutos

## 0:00 – 0:20 | Abertura

"Prazer, me chamo Felipe e irei apresentar a minha resolução do desafio técnico que consiste em um sistema de gestão de produtos. O foco é mostrar a arquitetura, as decisões técnicas e como cada componente se integra para entregar uma aplicação consistente, escalável e de fácil manutenção."

---

## 1) 0:20 – 2:00 | Infraestrutura

**Abrir `docker-compose.yaml`**

"Toda a solução roda em containers Docker. Com um único comando `docker-compose up`, o ambiente completo sobe: banco de dados, cache, autenticação, backend e frontend."

### Nginx
"O Nginx atua como reverse proxy na porta 80. Ele roteia `/api` para o backend .NET e todo o resto para o frontend Next.js. Também adiciona headers de segurança e controla o fluxo de requisições."

### MongoDB
"Escolhi MongoDB porque o modelo de documentos se encaixa bem com produtos que podem ter atributos variáveis. Não preciso de migrations, o schema evolui naturalmente, e a performance em leitura é excelente para o dashboard."

### Redis
"Redis funciona como cache distribuído. Queries verificam o Redis antes de ir ao MongoDB. Quando um produto é criado ou atualizado, as chaves relacionadas são invalidadas automaticamente. Isso reduz a latência para milissegundos."

### Keycloak
"Keycloak gerencia toda a autenticação via OAuth2/OpenID Connect. O backend não precisa implementar login, refresh token ou gestão de usuários. Ele apenas valida o JWT que vem nas requisições."

---

## 2) 2:00 – 4:10 | Backend — Clean Architecture

**Abrir pasta `backend/src`**

"O backend segue Clean Architecture com quatro projetos separados. Isso garante que cada camada tenha uma responsabilidade clara e possa ser testada isoladamente."

### 2.1 HypeSoft.Domain

**Abrir `Product.cs` e `Category.cs`**

"No Domain ficam as entidades Product e Category. Elas têm construtor privado e métodos de fábrica como `Product.Create()`. Isso garante que nunca exista um produto com dados inválidos."

"O método `IsLowStock()` retorna true quando o estoque está abaixo de 10 unidades. Essa regra de negócio fica no domínio, não espalhada pelo código."

### 2.2 HypeSoft.Application — CQRS

**Abrir `CreateProductCommandHandler.cs` e `GetAllProductsQueryHandler.cs`**

"A camada Application implementa CQRS com MediatR. Commands como `CreateProductCommand` alteram dados e invalidam cache. Queries como `GetAllProductsQuery` apenas leem e podem ser cacheadas."

"Os Behaviors do MediatR adicionam funcionalidades transversais: `ValidationBehavior` valida com FluentValidation, `CachingBehavior` cacheia queries automaticamente, e `CacheInvalidationBehavior` limpa o cache após commands."

### 2.3 HypeSoft.Infrastructure

**Abrir `ProductRepository.cs` e `KeycloakService.cs`**

"Infrastructure conecta o sistema ao mundo externo. `ProductRepository` implementa a interface do Domain usando MongoDB. `KeycloakService` integra com o Keycloak para autenticação. `RedisCacheService` gerencia o cache."

"Se eu precisar trocar MongoDB por PostgreSQL, só altero esta camada. O resto do sistema continua funcionando."

### 2.4 HypeSoft.API

**Abrir `ProductsController.cs`**

"Os Controllers são finos. Recebem a requisição, enviam para o MediatR e retornam o resultado. Toda a lógica está nos Handlers."

"A autorização usa policies baseadas nas roles do JWT: admin tem acesso total, manager pode criar e editar, user apenas visualiza."

---

## 3) 4:10 – 5:20 | Autenticação e Autorização

**Abrir Keycloak Admin Console ou `realm-export.json`**

"O Keycloak está configurado com o realm `hypesoft` e três roles: admin, manager e user. Já vem com usuários pré-configurados para teste."

"O fluxo funciona assim: o frontend envia email e senha para `/api/auth/login`. O backend autentica com o Keycloak e retorna o JWT. O token fica em cookie HttpOnly, protegido contra XSS."

"O backend valida o JWT usando a chave pública do Keycloak via JWKS. Não precisa chamar o Keycloak a cada requisição."

---

## 4) 5:20 – 6:40 | Cache, Auditoria e Testes

### Cache com Redis

**Abrir `CachingBehavior.cs`**

"Queries passam pelo `CachingBehavior`. Ele gera uma chave baseada no tipo da query e seus parâmetros, verifica o Redis, e só vai ao MongoDB se não encontrar."

"Quando um produto é criado ou atualizado, o `CacheInvalidationBehavior` remove todas as chaves que começam com `GetAllProductsQuery:` e `GetDashboardQuery:`. Isso mantém os dados consistentes."

### Auditoria

"Cada operação de criação, atualização e exclusão registra um log de auditoria com usuário, entidade, ação e timestamp. Isso permite rastrear quem alterou o quê e quando."

### Testes

**Abrir pasta `HypeSoft.UnitTests`**

"Os testes unitários cobrem Domain e Application com mocks. Testes de entidade validam regras como `IsLowStock`. Testes de handlers verificam o fluxo completo com repositórios mockados."

"No frontend, testes E2E com Playwright validam os fluxos principais: login, CRUD de produtos e categorias."

---

## 5) 6:40 – 9:00 | Frontend — Next.js 15

**Abrir pasta `frontend/src`**

"O frontend usa Next.js 15 com App Router. Server Components reduzem o JavaScript enviado ao cliente e melhoram o tempo de carregamento."

### Arquitetura em Camadas

**Abrir pasta `frontend/src/server`**

"Organizei o frontend em três camadas dentro da pasta `server`, seguindo um padrão similar ao backend:"

- **Models**: Schemas Zod que definem tipos e validação (ProductModel, CategoryModel)
- **Services**: Classes que encapsulam chamadas HTTP à API (ProductService, CategoryService)
- **Controllers**: Server Actions que orquestram Services e gerenciam tokens

### Vantagem Principal: Separação de Responsabilidades

**Abrir `product.controller.ts` e `product.service.ts`**

"A principal vantagem dessa organização é a separação clara entre orquestração e comunicação HTTP."

"O **Controller** é a Server Action que o componente chama. Ele busca o token, instancia o Service, chama o método e faz `revalidatePath`. Não sabe nada de HTTP."

"O **Service** encapsula toda a comunicação com a API. Valida dados com Zod, monta URLs, faz requests. Não sabe nada de tokens ou cache do Next.js."

"Isso permite testar Services isoladamente, reutilizar lógica HTTP, e manter Controllers enxutos. Se a API mudar, só altero o Service."

### Barrel Export

**Abrir `controllers/index.ts`**

"Todos os controllers são exportados via barrel em um objeto `actions`. Nos componentes, importo `actions.product.createProduct()`. Isso dá autocomplete, organização e facilita refatoração."

### Componentes

**Abrir `data-table.tsx` e `dashboard-content.tsx`**

"Uso shadcn/ui para componentes como tabelas, formulários e gráficos. O DataTable é genérico e reutilizado em produtos, categorias e usuários."

### Segurança

"O frontend nunca expõe tokens. Server Actions rodam no servidor, onde o token está em cookie HttpOnly. Client Components chamam Server Actions, nunca a API diretamente."

---

## 6) 9:00 – 10:00 | Encerramento

"Resumindo as decisões técnicas do HypeSoft:

- **Clean Architecture** isola regras de negócio e facilita testes
- **CQRS com MediatR** separa leitura e escrita com behaviors reutilizáveis
- **MongoDB** oferece flexibilidade de schema e performance em leitura
- **Redis** reduz latência com cache automático e invalidação inteligente
- **Keycloak** entrega autenticação enterprise-grade sem código customizado
- **Next.js 15** otimiza renderização com Server Components
- **Docker Compose** garante ambiente reproduzível com um comando

O resultado é uma aplicação modular, testável e pronta para evoluir. Cada componente pode ser substituído ou escalado independentemente."
