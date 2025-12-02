import { apiClient } from "./api-client.service";

export abstract class BaseService {
  protected token?: string;

  protected constructor(token?: string) {
    this.token = token;
  }

  protected get client() {
    return apiClient;
  }

  protected setToken(token?: string) {
    this.token = token;
  }
}
