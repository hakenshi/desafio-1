# ADR-006: Keycloak para Autenticação

## Status
Aceito

## Contexto
Necessidade de autenticação OAuth2/OIDC com gestão de usuários e roles, self-hosted.

## Decisão
Keycloak como Identity Provider, containerizado via Docker.

## Configuração

### Realm: hypesoft
- Client: hypesoft-api (confidential)

### Roles
| Role | Permissões |
|------|------------|
| admin | CRUD completo + gestão de usuários |
| manager | CRUD produtos e categorias |
| user | Visualização apenas |

### Usuários Pré-configurados
- admin@hypesoft.com (admin)
- manager@hypesoft.com (manager)
- user@hypesoft.com (user)
- Senha padrão: admin123

## Fluxo de Autenticação
1. Frontend envia credenciais para /api/auth/login
2. Backend autentica com Keycloak e retorna JWT
3. Token armazenado em cookie HttpOnly
4. Requisições incluem Bearer token
5. API valida token com chave pública do Keycloak

## JWT Claims Utilizados
- sub: ID do usuário
- preferred_username: Nome de usuário
- email: Email
- realm_access.roles: Roles do usuário

## Consequências

### Positivas
- Solução enterprise-grade
- Gestão completa de usuários via Admin Console
- Suporte a SSO e MFA
- Self-hosted com controle total

### Negativas
- Consumo de recursos (JVM)
- Configuração inicial complexa
