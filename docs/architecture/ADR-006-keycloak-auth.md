# ADR-006: Keycloak para Autenticação

## Status
Aceito

## Contexto
Precisamos de uma solução de autenticação que:
- Suporte OAuth2/OpenID Connect
- Permita gestão de usuários e roles
- Seja self-hosted (sem dependência de serviços externos)
- Tenha UI de administração

## Decisão
Adotamos Keycloak como Identity Provider.

## Configuração

### Realm: hypesoft
- Client: `hypesoft-frontend` (public, para SPA)
- Client: `hypesoft-api` (confidential, para backend)

### Roles
| Role | Permissões |
|------|------------|
| admin | Tudo + gestão de usuários |
| manager | CRUD produtos e categorias |
| user | Visualização apenas |

### Fluxo de Autenticação
```
1. Usuário acessa /login
2. Frontend redireciona para Keycloak
3. Keycloak autentica e retorna JWT
4. Frontend armazena token em cookie HttpOnly
5. Requisições à API incluem Bearer token
6. API valida token com Keycloak
```

### JWT Claims
```json
{
  "sub": "user-uuid",
  "preferred_username": "admin",
  "email": "admin@hypesoft.com",
  "realm_access": {
    "roles": ["admin", "default-roles-hypesoft"]
  }
}
```

## Integração Backend
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://keycloak:8080/realms/hypesoft";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });
```

## Consequências

### Positivas
- Solução enterprise-grade
- UI de admin completa
- Suporte a SSO, MFA, social login
- Self-hosted (controle total)

### Negativas
- Consumo de recursos (Java)
- Configuração inicial complexa
- Overhead para projetos pequenos
