import { z } from "zod";

export namespace DashboardModel {
  // Dashboard Schema
  export const DashboardSchema = z.object({
    totalProducts: z.number(),
    totalStockValue: z.number(),
    lowStockCount: z.number(),
    productsByCategory: z.record(z.string(), z.number()),
  });

  // Types
  export type Dashboard = z.infer<typeof DashboardSchema>;
}
