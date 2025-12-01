"use server";

import { cookies } from "next/headers";
import { DashboardModel } from "../models/dashboard.model";
import { DashboardService } from "../services/dashboard.service";

async function getAuthToken(): Promise<string | undefined> {
  const cookieStore = await cookies();
  return cookieStore.get("authToken")?.value;
}

export async function getDashboardData(): Promise<DashboardModel.Dashboard> {
  const token = await getAuthToken();
  const service = new DashboardService(token);
  return await service.getData();
}
