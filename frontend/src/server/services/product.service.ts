import { ProductModel } from "../models/product.model";
import { BaseService } from "./base.service";
import { z } from "zod";

export class ProductService extends BaseService {
  private static instance: ProductService | null = null;

  private constructor(token?: string) {
    super(token);
  }

  static initialize(token?: string): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService(token);
    } else {
      ProductService.instance.setToken(token);
    }
    return ProductService.instance;
  }

  static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }
  async getAll(query?: ProductModel.GetAllProductsQuery): Promise<ProductModel.PaginatedProducts> {
    const validatedQuery = ProductModel.GetAllProductsQuerySchema.parse(query || {});
    const params = new URLSearchParams({
      page: validatedQuery.page.toString(),
      pageSize: validatedQuery.pageSize.toString(),
    });

    if (validatedQuery.categoryId) {
      params.append("categoryId", validatedQuery.categoryId);
    }

    const response = await this.client.get<ProductModel.PaginatedProducts>(
      `/products?${params.toString()}`, 
      undefined, 
      this.token
    );
    return ProductModel.PaginatedProductsSchema.parse(response);
  }

  async getById(id: string): Promise<ProductModel.Product> {
    const response = await this.client.get<ProductModel.Product>(
      `/products/${id}`, 
      undefined, 
      this.token
    );
    return ProductModel.ProductSchema.parse(response);
  }

  async search(query: ProductModel.SearchProductsQuery): Promise<ProductModel.Product[]> {
    const validatedQuery = ProductModel.SearchProductsQuerySchema.parse(query);
    const params = new URLSearchParams();

    if (validatedQuery.searchTerm) {
      params.append("searchTerm", validatedQuery.searchTerm);
    }
    if (validatedQuery.categoryId) {
      params.append("categoryId", validatedQuery.categoryId);
    }
    params.append("page", validatedQuery.page.toString());
    params.append("pageSize", validatedQuery.pageSize.toString());

    const response = await this.client.get<ProductModel.Product[]>(
      `/products/search?${params.toString()}`, 
      undefined, 
      this.token
    );
    return z.array(ProductModel.ProductSchema).parse(response);
  }

  async getLowStock(): Promise<ProductModel.Product[]> {
    const response = await this.client.get<ProductModel.Product[]>(
      "/products/low-stock", 
      undefined, 
      this.token
    );
    return z.array(ProductModel.ProductSchema).parse(response);
  }

  async create(data: ProductModel.CreateProductDto): Promise<ProductModel.Product> {
    const validatedData = ProductModel.CreateProductSchema.parse(data);
    const response = await this.client.post<ProductModel.Product>(
      "/products", 
      validatedData, 
      undefined, 
      this.token
    );
    return ProductModel.ProductSchema.parse(response);
  }

  async update(id: string, data: ProductModel.UpdateProductDto): Promise<ProductModel.Product> {
    const validatedData = ProductModel.UpdateProductSchema.parse(data);
    const response = await this.client.put<ProductModel.Product>(
      `/products/${id}`, 
      validatedData, 
      undefined, 
      this.token
    );
    return ProductModel.ProductSchema.parse(response);
  }

  async delete(id: string): Promise<void> {
    await this.client.delete<void>(`/products/${id}`, undefined, this.token);
  }
}
