import { z } from "zod";

export namespace DashboardModel {
  // Dashboard Schema
  export const DashboardSchema = z.object({
    totalProducts: z.number(),
    totalStockValue: z.number(),
    lowStockCount: z.number(),
    productsByCategory: z.record(z.string(), z.number()),
  });

  // Audit Log Schema
  export const AuditLogSchema = z.object({
    id: z.string(),
    userId: z.string(),
    username: z.string(),
    action: z.string(),
    entityType: z.string(),
    entityId: z.string(),
    entityName: z.string().nullable().optional(),
    details: z.string().nullable().optional(),
    createdAt: z.string().datetime(),
  });

  // Recent Product Schema
  export const RecentProductSchema = z.object({
    id: z.string(),
    name: z.string(),
    price: z.number(),
    categoryName: z.string(),
    stockQuantity: z.number(),
    createdAt: z.string().datetime(),
  });

  // Types
  export type Dashboard = z.infer<typeof DashboardSchema>;
  export type AuditLog = z.infer<typeof AuditLogSchema>;
  export type RecentProduct = z.infer<typeof RecentProductSchema>;
}
