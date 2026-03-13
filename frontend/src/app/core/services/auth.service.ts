import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StorageService } from './storage.service';
import { AuthResult, User } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private storage: StorageService) {
    const user = this.storage.getUser();
    if (user) this.currentUserSubject.next(user);
  }

  login(username: string, password: string): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(tap(result => {
        if (result.success && result.token) {
          this.storage.setToken(result.token);
          if (result.refreshToken) this.storage.setRefreshToken(result.refreshToken);
          if (result.user) {
            this.storage.setUser(result.user);
            this.currentUserSubject.next(result.user);
          }
        }
      }));
  }

  logout(): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/logout`, {})
      .pipe(tap(() => {
        this.storage.clear();
        this.currentUserSubject.next(null);
      }));
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): boolean {
    return this.storage.isLoggedIn();
  }

  getUserRole(): string {
    return this.getCurrentUser()?.roleName || '';
  }

  hasRole(...roles: string[]): boolean {
    return roles.includes(this.getUserRole());
  }
}
