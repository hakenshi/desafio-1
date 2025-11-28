"use server";

import { DashboardModel } from "../models/dashboard.model";
import { apiClient } from "./api-client.service";
import { cacheTag, cacheLife } from "next/cache";

export abstract class DashboardService {
  private static CACHE_TAGS = {
    dashboard: "dashboard",
  };

  static async getData(): Promise<DashboardModel.Dashboard> {
    "use cache";
    cacheTag(DashboardService.CACHE_TAGS.dashboard);
    cacheLife("seconds");

    const response = await apiClient.get<DashboardModel.Dashboard>("/dashboard");
    return DashboardModel.DashboardSchema.parse(response);
  }
}
