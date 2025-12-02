import { z } from "zod";
import { DashboardModel } from "../models/dashboard.model";
import { BaseService } from "./base.service";

export class DashboardService extends BaseService {
  async getData(): Promise<DashboardModel.Dashboard> {
    const response = await this.client.get<DashboardModel.Dashboard>("/dashboard", undefined, this.token);
    return DashboardModel.DashboardSchema.parse(response);
  }

  async getAuditLogs(count = 10): Promise<DashboardModel.AuditLog[]> {
    const response = await this.client.get<DashboardModel.AuditLog[]>(
      `/dashboard/audit-logs?count=${count}`,
      undefined,
      this.token
    );
    return z.array(DashboardModel.AuditLogSchema).parse(response);
  }

  async getRecentProducts(count = 10): Promise<DashboardModel.RecentProduct[]> {
    const response = await this.client.get<DashboardModel.RecentProduct[]>(
      `/dashboard/recent-products?count=${count}`,
      undefined,
      this.token
    );
    return z.array(DashboardModel.RecentProductSchema).parse(response);
  }
}
