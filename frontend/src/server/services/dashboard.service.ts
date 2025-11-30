import { DashboardModel } from "../models/dashboard.model";
import { apiClient } from "./api-client.service";
import { cacheTag, cacheLife } from "next/cache";

export abstract class DashboardService {
  static async getData(): Promise<DashboardModel.Dashboard> {
    const response = await apiClient.get<DashboardModel.Dashboard>("/dashboard");
    return DashboardModel.DashboardSchema.parse(response);
  }
}
