import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import { ChartWidgetComponent } from '../../shared/components/chart-widget/chart-widget.component';
import { environment } from '../../../environments/environment';
import { DashboardSummary, Announcement, Memorandum, DownloadableForm } from '../../core/models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, StatCardComponent, ChartWidgetComponent],
  template: `
    <div class="dashboard">
      <!-- Page title -->
      <div class="page-header">
        <h2 class="page-title">Dashboard</h2>
        <span class="page-date">{{ today | date:'EEEE, MMMM d, yyyy' }}</span>
      </div>

      <!-- Stat Cards -->
      <div class="stat-cards-grid">
        <app-stat-card
          label="Permanent"
          [count]="summary?.permanentCount || 0"
          icon="pi pi-id-card"
          bgColor="#FFF9E6"
          iconColor="#F39C12">
        </app-stat-card>
        <app-stat-card
          label="Contractual"
          [count]="summary?.contractualCount || 0"
          icon="pi pi-briefcase"
          bgColor="#E8F5E9"
          iconColor="#2ECC71">
        </app-stat-card>
        <app-stat-card
          label="Casual"
          [count]="summary?.casualCount || 0"
          icon="pi pi-users"
          bgColor="#E3F2FD"
          iconColor="#3498DB">
        </app-stat-card>
        <app-stat-card
          label="Job Order"
          [count]="summary?.jobOrderCount || 0"
          icon="pi pi-clipboard"
          bgColor="#FFF3E0"
          iconColor="#E85D26">
        </app-stat-card>
      </div>

      <!-- Main content grid -->
      <div class="dashboard-grid">
        <!-- Left column: Announcements + Chart -->
        <div class="dashboard-col">
          <!-- Announcements -->
          <div class="card">
            <div class="card__header">
              <h3><i class="pi pi-megaphone"></i> Announcements</h3>
              <a class="see-all" href="#">See All</a>
            </div>
            <div class="card__body">
              <div class="announcement" *ngFor="let ann of announcements">
                <div class="announcement__icon">
                  <i class="pi pi-bell"></i>
                </div>
                <div class="announcement__content">
                  <div class="announcement__title">{{ ann.title }}</div>
                  <div class="announcement__text">{{ ann.content }}</div>
                  <div class="announcement__meta">
                    Posted by {{ ann.postedBy || 'Admin' }}
                    <span *ngIf="ann.startDate"> · {{ ann.startDate | date:'MMM d, yyyy' }}</span>
                  </div>
                </div>
              </div>
              <div class="empty-state" *ngIf="!announcements.length">
                <i class="pi pi-inbox"></i>
                <p>No announcements at this time.</p>
              </div>
            </div>
          </div>

          <!-- Employment Type Chart -->
          <div class="card" *ngIf="summary">
            <div class="card__header">
              <h3><i class="pi pi-chart-pie"></i> Employment Distribution</h3>
            </div>
            <div class="card__body">
              <app-chart-widget
                type="donut"
                [series]="employmentChartSeries"
                [labels]="employmentChartLabels"
                [height]="260">
              </app-chart-widget>
            </div>
          </div>
        </div>

        <!-- Right column: Memos + Forms -->
        <div class="dashboard-col">
          <!-- Memorandums -->
          <div class="card">
            <div class="card__header">
              <h3><i class="pi pi-file"></i> Memorandums</h3>
              <a class="see-all" href="#">See All</a>
            </div>
            <div class="card__body">
              <div class="memo-item" *ngFor="let memo of memorandums">
                <div class="memo-item__ref">{{ memo.referenceNumber || 'DPWH-MEMO' }}</div>
                <div class="memo-item__title">{{ memo.title }}</div>
                <div class="memo-item__date">{{ memo.dateIssued | date:'MMM d, yyyy' }}</div>
                <a *ngIf="memo.filePath" [href]="memo.filePath" class="memo-item__download">
                  <i class="pi pi-download"></i> Download
                </a>
              </div>
              <div class="empty-state" *ngIf="!memorandums.length">
                <i class="pi pi-inbox"></i>
                <p>No memorandums found.</p>
              </div>
            </div>
          </div>

          <!-- Downloadable Forms -->
          <div class="card">
            <div class="card__header">
              <h3><i class="pi pi-download"></i> Downloadable Forms</h3>
            </div>
            <div class="card__body">
              <div class="form-item" *ngFor="let form of downloadableForms">
                <div class="form-item__info">
                  <i class="pi pi-file-pdf form-item__icon"></i>
                  <div>
                    <div class="form-item__name">{{ form.formName }}</div>
                    <div class="form-item__meta">{{ form.category }} · {{ form.version || 'Latest' }}</div>
                  </div>
                </div>
                <a [href]="form.filePath" class="form-item__download-btn">
                  <i class="pi pi-download"></i>
                </a>
              </div>
              <div class="empty-state" *ngIf="!downloadableForms.length">
                <i class="pi pi-inbox"></i>
                <p>No forms available.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-title { font-size: 1.5rem; font-weight: 700; color: #1B2A4A; margin: 0; }
    .page-date { color: #888; font-size: 0.875rem; }
    .stat-cards-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
    @media (max-width: 1200px) { .stat-cards-grid { grid-template-columns: repeat(2, 1fr); } }
    @media (max-width: 600px) { .stat-cards-grid { grid-template-columns: 1fr; } }
    .dashboard-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
    @media (max-width: 960px) { .dashboard-grid { grid-template-columns: 1fr; } }
    .dashboard-col { display: flex; flex-direction: column; gap: 1.5rem; }
    .card { background: white; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.06); overflow: hidden; }
    .card__header {
      display: flex; justify-content: space-between; align-items: center;
      padding: 1rem 1.25rem; border-bottom: 1px solid #f0f0f0;
    }
    .card__header h3 { margin: 0; font-size: 0.95rem; font-weight: 600; color: #1B2A4A; display: flex; align-items: center; gap: 0.5rem; }
    .card__header h3 i { color: #E85D26; }
    .see-all { color: #E85D26; font-size: 0.8rem; text-decoration: none; }
    .card__body { padding: 1rem 1.25rem; }
    .announcement { display: flex; gap: 1rem; padding: 0.75rem 0; border-bottom: 1px solid #f5f5f5; }
    .announcement:last-child { border-bottom: none; }
    .announcement__icon { width: 36px; height: 36px; background: #FFF3E0; border-radius: 50%; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
    .announcement__icon i { color: #E85D26; }
    .announcement__title { font-weight: 600; font-size: 0.875rem; color: #1B2A4A; }
    .announcement__text { font-size: 0.8rem; color: #666; margin-top: 0.25rem; line-height: 1.4; }
    .announcement__meta { font-size: 0.75rem; color: #aaa; margin-top: 0.25rem; }
    .memo-item { padding: 0.75rem 0; border-bottom: 1px solid #f5f5f5; }
    .memo-item:last-child { border-bottom: none; }
    .memo-item__ref { font-size: 0.7rem; color: #E85D26; font-weight: 600; text-transform: uppercase; }
    .memo-item__title { font-size: 0.875rem; font-weight: 600; color: #1B2A4A; margin: 0.25rem 0; }
    .memo-item__date { font-size: 0.75rem; color: #888; }
    .memo-item__download { font-size: 0.75rem; color: #3498DB; text-decoration: none; margin-top: 0.25rem; display: inline-flex; align-items: center; gap: 0.25rem; }
    .form-item { display: flex; justify-content: space-between; align-items: center; padding: 0.75rem 0; border-bottom: 1px solid #f5f5f5; }
    .form-item:last-child { border-bottom: none; }
    .form-item__info { display: flex; align-items: center; gap: 0.75rem; }
    .form-item__icon { color: #dc3545; font-size: 1.5rem; }
    .form-item__name { font-size: 0.875rem; font-weight: 600; color: #1B2A4A; }
    .form-item__meta { font-size: 0.75rem; color: #888; }
    .form-item__download-btn { width: 32px; height: 32px; background: #E85D26; color: white; border-radius: 6px; display: flex; align-items: center; justify-content: center; text-decoration: none; }
    .empty-state { text-align: center; padding: 2rem; color: #aaa; }
    .empty-state i { font-size: 2rem; margin-bottom: 0.5rem; }
    .empty-state p { font-size: 0.875rem; margin: 0; }
  `]
})
export class DashboardComponent implements OnInit {
  today = new Date();
  summary: DashboardSummary | null = null;
  announcements: Announcement[] = [];
  memorandums: Memorandum[] = [];
  downloadableForms: DownloadableForm[] = [];

  employmentChartSeries: number[] = [];
  employmentChartLabels: string[] = ['Permanent', 'Contractual', 'Casual', 'Job Order'];

  // Mock data for demo purposes
  private mockSummary: DashboardSummary = {
    permanentCount: 12450,
    contractualCount: 3200,
    casualCount: 2800,
    jobOrderCount: 1554,
    totalEmployees: 20004
  };

  private mockAnnouncements: Announcement[] = [
    { id: '1', title: 'Welcome to DPWH HRIS', content: 'The new Human Resource Information System is now live. Please log in with your assigned credentials.', postedBy: 'Admin', startDate: new Date().toISOString() },
    { id: '2', title: 'System Maintenance Notice', content: 'Scheduled maintenance every Sunday from 12:00 AM to 4:00 AM. Please save your work before this period.', postedBy: 'IT Admin', startDate: new Date().toISOString() }
  ];

  private mockMemos: Memorandum[] = [
    { id: '1', title: 'Circular on Leave Administration', referenceNumber: 'DPWH-HRAS-MC-2026-001', dateIssued: new Date(Date.now() - 30 * 86400000).toISOString() },
    { id: '2', title: 'Updated Performance Evaluation Guidelines', referenceNumber: 'DPWH-HRAS-MC-2026-002', dateIssued: new Date(Date.now() - 15 * 86400000).toISOString() }
  ];

  private mockForms: DownloadableForm[] = [
    { id: '1', formName: 'CSC Form 212 (PDS)', description: 'Personal Data Sheet', category: 'HR Forms', filePath: '#', version: 'Revised 2017' },
    { id: '2', formName: 'Application for Leave', description: 'CSC Form 6', category: 'Leave Forms', filePath: '#', version: '2020' },
    { id: '3', formName: 'IPCR Form', description: 'Performance Commitment', category: 'Performance', filePath: '#', version: '2026' },
    { id: '4', formName: 'Service Record Form', description: 'Document Request', category: 'HR Forms', filePath: '#', version: '2024' }
  ];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    // Try to load from API, fall back to mock data
    this.http.get<any>(`${environment.apiUrl}/dashboard/summary`).subscribe({
      next: (res) => {
        this.summary = res.data;
        this.updateChart();
      },
      error: () => {
        this.summary = this.mockSummary;
        this.updateChart();
      }
    });

    this.http.get<any>(`${environment.apiUrl}/dashboard/announcements`).subscribe({
      next: (res) => this.announcements = res.data || [],
      error: () => this.announcements = this.mockAnnouncements
    });

    this.http.get<any>(`${environment.apiUrl}/dashboard/memorandums`).subscribe({
      next: (res) => this.memorandums = res.data || [],
      error: () => this.memorandums = this.mockMemos
    });

    this.http.get<any>(`${environment.apiUrl}/dashboard/forms`).subscribe({
      next: (res) => this.downloadableForms = res.data || [],
      error: () => this.downloadableForms = this.mockForms
    });
  }

  updateChart(): void {
    if (this.summary) {
      this.employmentChartSeries = [
        this.summary.permanentCount,
        this.summary.contractualCount,
        this.summary.casualCount,
        this.summary.jobOrderCount
      ];
    }
  }
}
