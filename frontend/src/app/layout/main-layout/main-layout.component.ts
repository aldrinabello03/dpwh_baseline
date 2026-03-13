import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, SidebarComponent, TopbarComponent, FooterComponent],
  template: `
    <div class="layout">
      <app-sidebar></app-sidebar>
      <div class="layout__content">
        <app-topbar></app-topbar>
        <main class="layout__main">
          <router-outlet></router-outlet>
        </main>
        <app-footer></app-footer>
      </div>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
      background: #F5F5F5;
    }
    .layout__content {
      margin-left: 260px;
      flex: 1;
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      transition: margin-left 0.3s ease;
    }
    .layout__main {
      flex: 1;
      padding: 1.5rem;
      overflow-y: auto;
    }
    @media (max-width: 768px) {
      .layout__content { margin-left: 64px; }
    }
  `]
})
export class MainLayoutComponent {}
