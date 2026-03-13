import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route?: string;
  children?: NavItem[];
  expanded?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="sidebar" [class.collapsed]="collapsed">
      <!-- Logo -->
      <div class="sidebar__logo">
        <img src="assets/images/dpwh-logo-icon.svg" alt="DPWH" class="sidebar__logo-icon" />
        <span class="sidebar__logo-text" *ngIf="!collapsed">DPWH HRIS</span>
      </div>

      <!-- Navigation -->
      <ul class="sidebar__nav">
        <ng-container *ngFor="let item of navItems">
          <!-- Menu item without children -->
          <li class="sidebar__item" *ngIf="!item.children" routerLinkActive="active">
            <a [routerLink]="item.route" class="sidebar__link">
              <i [class]="item.icon"></i>
              <span *ngIf="!collapsed">{{ item.label }}</span>
            </a>
          </li>

          <!-- Menu item with children (collapsible) -->
          <li class="sidebar__group" *ngIf="item.children">
            <div class="sidebar__group-header" (click)="toggleGroup(item)">
              <i [class]="item.icon"></i>
              <span *ngIf="!collapsed">{{ item.label }}</span>
              <i class="pi sidebar__chevron" [class.pi-chevron-down]="item.expanded" [class.pi-chevron-right]="!item.expanded" *ngIf="!collapsed"></i>
            </div>
            <ul class="sidebar__children" *ngIf="item.expanded && !collapsed">
              <li *ngFor="let child of item.children" routerLinkActive="active">
                <a [routerLink]="child.route" class="sidebar__child-link">
                  <span>{{ child.label }}</span>
                </a>
              </li>
            </ul>
          </li>
        </ng-container>
      </ul>

      <!-- Collapse toggle -->
      <div class="sidebar__collapse-btn" (click)="toggleCollapse()">
        <i class="pi" [class.pi-chevron-left]="!collapsed" [class.pi-chevron-right]="collapsed"></i>
      </div>
    </nav>
  `,
  styles: [`
    .sidebar {
      width: 260px;
      min-height: 100vh;
      background: #1B2A4A;
      color: white;
      display: flex;
      flex-direction: column;
      transition: width 0.3s ease;
      position: fixed;
      top: 0;
      left: 0;
      z-index: 1000;
      overflow-y: auto;
      overflow-x: hidden;
    }
    .sidebar.collapsed { width: 64px; }
    .sidebar__logo {
      display: flex;
      align-items: center;
      padding: 1rem;
      border-bottom: 1px solid rgba(255,255,255,0.1);
      gap: 0.75rem;
      min-height: 64px;
    }
    .sidebar__logo-icon { width: 36px; height: 36px; flex-shrink: 0; }
    .sidebar__logo-text { font-size: 1.1rem; font-weight: 700; white-space: nowrap; color: white; }
    .sidebar__nav { list-style: none; padding: 0.5rem 0; margin: 0; flex: 1; }
    .sidebar__item, .sidebar__group { margin: 2px 0; }
    .sidebar__link, .sidebar__group-header {
      display: flex;
      align-items: center;
      padding: 0.6rem 1rem;
      color: rgba(255,255,255,0.8);
      text-decoration: none;
      cursor: pointer;
      border-radius: 6px;
      margin: 0 8px;
      gap: 0.75rem;
      transition: background 0.2s, color 0.2s;
      font-size: 0.875rem;
    }
    .sidebar__link:hover, .sidebar__group-header:hover { background: rgba(255,255,255,0.1); color: white; }
    .sidebar__item.active .sidebar__link { background: #E85D26; color: white; }
    .sidebar__link i, .sidebar__group-header i { font-size: 1rem; width: 20px; flex-shrink: 0; }
    .sidebar__chevron { margin-left: auto; font-size: 0.75rem; }
    .sidebar__children { list-style: none; padding: 0; margin: 0; }
    .sidebar__child-link {
      display: block;
      padding: 0.5rem 1rem 0.5rem 3rem;
      color: rgba(255,255,255,0.7);
      text-decoration: none;
      font-size: 0.8rem;
      transition: color 0.2s;
      border-radius: 4px;
      margin: 0 8px;
    }
    .sidebar__child-link:hover { color: white; background: rgba(255,255,255,0.08); }
    .sidebar__children li.active .sidebar__child-link { color: #E85D26; font-weight: 600; }
    .sidebar__collapse-btn {
      padding: 1rem;
      border-top: 1px solid rgba(255,255,255,0.1);
      cursor: pointer;
      display: flex;
      justify-content: flex-end;
      color: rgba(255,255,255,0.6);
    }
    .sidebar__collapse-btn:hover { color: white; }
  `]
})
export class SidebarComponent {
  collapsed = false;

  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'pi pi-home', route: '/dashboard' },
    {
      label: 'Personnel Information', icon: 'pi pi-id-card', expanded: false,
      children: [
        { label: 'Personal Data Sheet', icon: 'pi pi-file', route: '/personnel-info/pds' },
        { label: 'Employee Directory', icon: 'pi pi-users', route: '/personnel-info/directory' },
        { label: 'Organization Chart', icon: 'pi pi-sitemap', route: '/personnel-info/org-chart' }
      ]
    },
    {
      label: 'Payroll Management', icon: 'pi pi-dollar', expanded: false,
      children: [
        { label: 'Payroll Processing', icon: 'pi pi-calculator', route: '/payroll/processing' },
        { label: 'Deductions & Loans', icon: 'pi pi-minus-circle', route: '/payroll/deductions' },
        { label: 'Government Contributions', icon: 'pi pi-building', route: '/payroll/contributions' }
      ]
    },
    {
      label: 'Time, Attendance & Leave', icon: 'pi pi-clock', expanded: false,
      children: [
        { label: 'Daily Time Record', icon: 'pi pi-calendar', route: '/attendance/dtr' },
        { label: 'Leave Applications', icon: 'pi pi-file-edit', route: '/attendance/leave-application' },
        { label: 'Leave Balances', icon: 'pi pi-chart-bar', route: '/attendance/leave-balance' },
        { label: 'Shift Scheduling', icon: 'pi pi-calendar-plus', route: '/attendance/shift-scheduling' }
      ]
    },
    {
      label: 'Self-Service Portal', icon: 'pi pi-user', expanded: false,
      children: [
        { label: 'My Profile', icon: 'pi pi-user-edit', route: '/self-service/my-profile' },
        { label: 'My Payslip', icon: 'pi pi-money-bill', route: '/self-service/my-payslip' },
        { label: 'My Leave', icon: 'pi pi-calendar', route: '/self-service/my-leave' },
        { label: 'My Performance', icon: 'pi pi-chart-line', route: '/self-service/my-performance' },
        { label: 'Service Record', icon: 'pi pi-file', route: '/self-service/service-record' },
        { label: 'Request Documents', icon: 'pi pi-envelope', route: '/self-service/request-documents' }
      ]
    },
    {
      label: 'Recruitment & Placement', icon: 'pi pi-briefcase', expanded: false,
      children: [
        { label: 'Job Postings', icon: 'pi pi-megaphone', route: '/recruitment/job-postings' },
        { label: 'Applicant Tracker', icon: 'pi pi-user-plus', route: '/recruitment/applicants' },
        { label: 'Onboarding', icon: 'pi pi-check-circle', route: '/recruitment/onboarding' },
        { label: 'Plantilla Management', icon: 'pi pi-list', route: '/recruitment/plantilla' }
      ]
    },
    {
      label: 'Performance Management', icon: 'pi pi-chart-line', expanded: false,
      children: [
        { label: 'IPCR', icon: 'pi pi-star', route: '/performance/ipcr' },
        { label: 'DPCR', icon: 'pi pi-star', route: '/performance/dpcr' },
        { label: 'OPCR', icon: 'pi pi-star', route: '/performance/opcr' },
        { label: 'Coaching Journal', icon: 'pi pi-book', route: '/performance/coaching-journal' },
        { label: 'Succession Planning', icon: 'pi pi-sitemap', route: '/performance/succession-planning' }
      ]
    },
    {
      label: 'Learning & Development', icon: 'pi pi-graduation-cap', expanded: false,
      children: [
        { label: 'Training Calendar', icon: 'pi pi-calendar', route: '/learning-dev/training-calendar' },
        { label: 'Training Courses', icon: 'pi pi-book', route: '/learning-dev/training-courses' },
        { label: 'Scholarships', icon: 'pi pi-academic-cap', route: '/learning-dev/scholarships' },
        { label: 'E-Learning', icon: 'pi pi-desktop', route: '/learning-dev/e-learning' },
        { label: 'Training Reports', icon: 'pi pi-chart-bar', route: '/learning-dev/reports' }
      ]
    },
    {
      label: 'Offboarding & Separation', icon: 'pi pi-sign-out', expanded: false,
      children: [
        { label: 'Employee Separation', icon: 'pi pi-times-circle', route: '/offboarding/separation' },
        { label: 'Exit Interview', icon: 'pi pi-comments', route: '/offboarding/exit-interview' },
        { label: 'Clearance', icon: 'pi pi-verified', route: '/offboarding/clearance' }
      ]
    },
    { label: 'Reports', icon: 'pi pi-file-pdf', route: '/reports' },
    {
      label: 'Administration', icon: 'pi pi-cog', expanded: false,
      children: [
        { label: 'User Management', icon: 'pi pi-users', route: '/admin/users' },
        { label: 'Data Libraries', icon: 'pi pi-database', route: '/admin/data-libraries' },
        { label: 'Audit Trail', icon: 'pi pi-history', route: '/admin/audit-trail' },
        { label: 'System Monitoring', icon: 'pi pi-server', route: '/admin/monitoring' }
      ]
    }
  ];

  toggleGroup(item: NavItem): void {
    item.expanded = !item.expanded;
  }

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
  }
}
