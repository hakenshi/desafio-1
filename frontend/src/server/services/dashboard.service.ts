import { DashboardModel } from "../models/dashboard.model";
import { BaseService } from "./base.service";

export class DashboardService extends BaseService {
  async getData(): Promise<DashboardModel.Dashboard> {
    const response = await this.client.get<DashboardModel.Dashboard>("/dashboard", undefined, this.token);
    return DashboardModel.DashboardSchema.parse(response);
  }
}
