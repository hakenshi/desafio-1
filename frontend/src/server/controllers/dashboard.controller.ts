"use server";

import { DashboardModel } from "../models/dashboard.model";
import { DashboardService } from "../services/dashboard.service";

// For server components - token passed explicitly
export async function getDashboardData(token: string): Promise<DashboardModel.Dashboard> {
  const service = new DashboardService(token);
  return await service.getData();
}

export async function getAuditLogs(token: string, count = 10): Promise<DashboardModel.AuditLog[]> {
  const service = new DashboardService(token);
  return await service.getAuditLogs(count);
}

export async function getRecentProducts(token: string, count = 10): Promise<DashboardModel.RecentProduct[]> {
  const service = new DashboardService(token);
  return await service.getRecentProducts(count);
}
