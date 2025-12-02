# ADR-005: Next.js App Router para Frontend

## Status
Aceito

## Contexto
Precisamos de um framework frontend que ofereça:
- Server-side rendering para SEO e performance
- Type safety com TypeScript
- Boa DX (Developer Experience)
- Integração fácil com APIs REST

## Decisão
Escolhemos Next.js 15 com App Router.

## Justificativa

### Por que Next.js?
- SSR/SSG out of the box
- Server Components reduzem bundle size
- Server Actions para mutations type-safe
- Excelente ecossistema (Vercel, shadcn/ui)

### Por que App Router (não Pages)?
- Server Components por padrão
- Layouts aninhados
- Streaming e Suspense nativos
- Melhor colocação de código (loading.tsx, error.tsx)

## Arquitetura Frontend

```
src/
├── app/                    # Rotas e páginas
│   ├── (auth)/            # Grupo de rotas públicas
│   └── (dashboard)/       # Grupo de rotas protegidas
├── components/            # Componentes React
│   ├── ui/               # Primitivos (shadcn/ui)
│   └── forms/            # Formulários específicos
├── server/               # Código server-side
│   ├── controllers/      # Server Actions
│   ├── services/         # API clients
│   └── models/           # Zod schemas
└── lib/                  # Utilitários
```

### Server Actions
```typescript
// server/controllers/product.controller.ts
"use server"

export async function createProduct(data: CreateProductDto) {
  const token = await getValidAuthToken();
  const service = new ProductService(token);
  return service.create(data);
}
```

### Validação com Zod
```typescript
// server/models/product.model.ts
export const CreateProductSchema = z.object({
  name: z.string().min(1),
  price: z.number().positive(),
  categoryId: z.string().uuid(),
});
```

## Consequências

### Positivas
- Performance excelente com Server Components
- Type safety end-to-end
- Menos código client-side
- SEO friendly

### Negativas
- Curva de aprendizado do App Router
- Algumas bibliotecas ainda não suportam Server Components
- Debugging pode ser mais complexo (server vs client)
