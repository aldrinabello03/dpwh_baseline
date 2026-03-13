import { Injectable } from '@angular/core';

const TOKEN_KEY = 'dpwh_token';
const REFRESH_KEY = 'dpwh_refresh';
const USER_KEY = 'dpwh_user';

@Injectable({ providedIn: 'root' })
export class StorageService {
  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  setRefreshToken(token: string): void {
    localStorage.setItem(REFRESH_KEY, token);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_KEY);
  }

  setUser(user: any): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  getUser(): any {
    const u = localStorage.getItem(USER_KEY);
    return u ? JSON.parse(u) : null;
  }

  clear(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(USER_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
