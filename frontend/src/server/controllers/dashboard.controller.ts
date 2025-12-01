"use server";

import { cookies } from "next/headers";
import { DashboardModel } from "../models/dashboard.model";
import { DashboardService } from "../services/dashboard.service";

async function getAuthToken(): Promise<string | undefined> {
  const cookieStore = await cookies();
  return cookieStore.get("authToken")?.value;
}

async function getService(): Promise<DashboardService> {
  const token = await getAuthToken();
  return new DashboardService(token);
}

export async function getDashboardData(): Promise<DashboardModel.Dashboard> {
  const service = await getService();
  return await service.getData();
}

export async function getAuditLogs(count = 10): Promise<DashboardModel.AuditLog[]> {
  const service = await getService();
  return await service.getAuditLogs(count);
}

export async function getRecentProducts(count = 10): Promise<DashboardModel.RecentProduct[]> {
  const service = await getService();
  return await service.getRecentProducts(count);
}
