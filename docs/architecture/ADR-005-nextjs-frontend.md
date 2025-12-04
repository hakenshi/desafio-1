# ADR-005: Next.js App Router

## Status
Aceito

## Contexto
Necessidade de framework frontend com SSR, TypeScript e boa integração com APIs REST.

## Decisão
Next.js 15 com App Router, shadcn/ui e Tailwind CSS.

## Estrutura
```
src/
├── app/                    # Rotas (login, dashboard, products, categories, users)
├── components/             # Componentes React e shadcn/ui
├── server/
│   ├── controllers/        # Server Actions
│   ├── services/           # API clients (ProductService, CategoryService)
│   └── models/             # Schemas Zod para validação
└── lib/                    # Utilitários
```

## Padrões Utilizados
- Server Actions para mutations
- Zod para validação de dados
- React Query pattern com Server Components
- Cookies HttpOnly para tokens

## Consequências

### Positivas
- Server Components reduzem bundle size
- Type safety com Zod schemas
- Validação client e server-side
- Integração nativa com React 19

### Negativas
- Curva de aprendizado do App Router
- Debugging mais complexo entre server/client
