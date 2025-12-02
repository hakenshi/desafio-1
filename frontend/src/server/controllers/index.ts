import * as authController from "./auth.controller";
import * as categoryController from "./category.controller";
import * as productController from "./product.controller";
import * as dashboardController from "./dashboard.controller";
import * as tokenController from "./token.controller";

export const actions = {
  auth: authController,
  category: categoryController,
  product: productController,
  dashboard: dashboardController,
  token: tokenController,
};

// Export individual actions for convenience
export * from "./auth.controller";
export * from "./category.controller";
export * from "./product.controller";
export * from "./dashboard.controller";
export * from "./token.controller";
