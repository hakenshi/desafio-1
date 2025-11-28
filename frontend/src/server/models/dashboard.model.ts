import { z } from "zod";
import { ProductModel } from "./product.model";

export namespace DashboardModel {
  // Dashboard Schema
  export const DashboardSchema = z.object({
    totalProducts: z.number(),
    totalStockValue: z.number(),
    lowStockProducts: z.array(ProductModel.ProductSchema),
    productsByCategory: z.record(z.string(), z.number()),
  });

  // Types
  export type Dashboard = z.infer<typeof DashboardSchema>;
}
