"use server";

import { DashboardModel } from "../models/dashboard.model";
import { DashboardService } from "../services/dashboard.service";
import { cacheTag, cacheLife } from "next/cache";

const CACHE_TAGS = {
  dashboard: "dashboard",
};

export async function getDashboardData(): Promise<DashboardModel.Dashboard> {
  "use cache";
  cacheTag(CACHE_TAGS.dashboard);
  cacheLife("seconds");
  
  return await DashboardService.getData();
}
