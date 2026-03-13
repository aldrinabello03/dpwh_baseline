import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { BadgeModule } from 'primeng/badge';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/models';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule, MenuModule, BadgeModule],
  template: `
    <header class="topbar">
      <div class="topbar__left">
        <div class="topbar__greeting">
          <span class="topbar__hello">Hello, <strong>{{ user?.username || 'User' }}</strong>!</span>
          <span class="topbar__welcome">Welcome to DPWH Human Resource Information System</span>
        </div>
      </div>
      <div class="topbar__right">
        <!-- Notification bell -->
        <div class="topbar__bell" (click)="toggleNotifications()">
          <i class="pi pi-bell"></i>
          <span class="topbar__badge" *ngIf="unreadCount > 0">{{ unreadCount }}</span>
        </div>

        <!-- Profile -->
        <div class="topbar__profile" (click)="toggleProfileMenu()">
          <div class="topbar__avatar">
            <img *ngIf="user?.photoPath" [src]="user?.photoPath" alt="Profile"/>
            <i *ngIf="!user?.photoPath" class="pi pi-user"></i>
          </div>
          <div class="topbar__user-info">
            <span class="topbar__username">{{ user?.username }}</span>
            <span class="topbar__role">{{ user?.roleName }}</span>
          </div>
          <i class="pi pi-chevron-down topbar__arrow"></i>
        </div>

        <!-- Profile dropdown -->
        <div class="topbar__dropdown" *ngIf="showProfileMenu">
          <a class="topbar__dropdown-item" routerLink="/self-service/my-profile" (click)="showProfileMenu = false">
            <i class="pi pi-user"></i> My Profile
          </a>
          <a class="topbar__dropdown-item" routerLink="/self-service/my-leave" (click)="showProfileMenu = false">
            <i class="pi pi-calendar"></i> My Leave
          </a>
          <hr>
          <a class="topbar__dropdown-item topbar__dropdown-item--danger" (click)="logout()">
            <i class="pi pi-sign-out"></i> Logout
          </a>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .topbar {
      height: 64px;
      background: white;
      border-bottom: 1px solid #e9ecef;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 1.5rem;
      position: sticky;
      top: 0;
      z-index: 999;
      box-shadow: 0 1px 4px rgba(0,0,0,0.08);
    }
    .topbar__greeting { display: flex; flex-direction: column; }
    .topbar__hello { font-size: 1rem; color: #1B2A4A; }
    .topbar__welcome { font-size: 0.75rem; color: #888; }
    .topbar__right { display: flex; align-items: center; gap: 1rem; position: relative; }
    .topbar__bell {
      position: relative;
      cursor: pointer;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      background: #f5f5f5;
    }
    .topbar__bell:hover { background: #e9ecef; }
    .topbar__bell i { font-size: 1.1rem; color: #555; }
    .topbar__badge {
      position: absolute;
      top: 2px;
      right: 2px;
      background: #E85D26;
      color: white;
      font-size: 0.65rem;
      border-radius: 50%;
      width: 18px;
      height: 18px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
    }
    .topbar__profile {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      cursor: pointer;
      padding: 0.5rem;
      border-radius: 8px;
    }
    .topbar__profile:hover { background: #f5f5f5; }
    .topbar__avatar {
      width: 38px;
      height: 38px;
      border-radius: 50%;
      background: #1B2A4A;
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
    }
    .topbar__avatar img { width: 100%; height: 100%; object-fit: cover; }
    .topbar__avatar i { color: white; font-size: 1.1rem; }
    .topbar__user-info { display: flex; flex-direction: column; }
    .topbar__username { font-size: 0.875rem; font-weight: 600; color: #1B2A4A; }
    .topbar__role { font-size: 0.7rem; color: #888; }
    .topbar__arrow { font-size: 0.75rem; color: #888; }
    .topbar__dropdown {
      position: absolute;
      top: calc(100% + 0.5rem);
      right: 0;
      background: white;
      border: 1px solid #e9ecef;
      border-radius: 8px;
      box-shadow: 0 4px 16px rgba(0,0,0,0.12);
      min-width: 180px;
      z-index: 1001;
      padding: 0.5rem 0;
    }
    .topbar__dropdown-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.6rem 1rem;
      color: #333;
      text-decoration: none;
      font-size: 0.875rem;
      cursor: pointer;
    }
    .topbar__dropdown-item:hover { background: #f5f5f5; }
    .topbar__dropdown-item--danger { color: #dc3545; }
    .topbar__dropdown-item--danger:hover { background: #fff5f5; }
    hr { border: none; border-top: 1px solid #e9ecef; margin: 0.25rem 0; }
  `]
})
export class TopbarComponent implements OnInit {
  user: User | null = null;
  unreadCount = 0;
  showProfileMenu = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(u => this.user = u);
  }

  toggleNotifications(): void { /* Navigate to notifications */ }

  toggleProfileMenu(): void {
    this.showProfileMenu = !this.showProfileMenu;
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => { this.router.navigate(['/login']); }
    });
    this.showProfileMenu = false;
  }
}
