import { Injectable } from "@angular/core";
import { keycloak } from "../services/keycloak.service";

@Injectable({ providedIn: 'root' })
export class AuthService {

  async getToken(): Promise<string | null> {
    if (!keycloak.authenticated) return null;

    await keycloak.updateToken(30);
    return keycloak.token ?? null;
  }

  isLoggedIn(): boolean {
    return keycloak.authenticated ?? false;
  }

  login(): void {
    keycloak.login();
  }

  logout(): void {
    keycloak.logout();
  }
}
