import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./pages/auth/login/login.component').then(m => m.LoginComponent) },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'personnel-info/pds', loadComponent: () => import('./pages/placeholder-pages').then(m => m.PdsFormComponent) },
      { path: 'personnel-info/directory', loadComponent: () => import('./pages/placeholder-pages').then(m => m.EmployeeDirectoryComponent) },
      { path: 'personnel-info/org-chart', loadComponent: () => import('./pages/placeholder-pages').then(m => m.OrgChartComponent) },
      { path: 'payroll/processing', loadComponent: () => import('./pages/placeholder-pages').then(m => m.PayrollProcessingComponent) },
      { path: 'payroll/deductions', loadComponent: () => import('./pages/placeholder-pages').then(m => m.DeductionsComponent) },
      { path: 'payroll/contributions', loadComponent: () => import('./pages/placeholder-pages').then(m => m.GovContributionsComponent) },
      { path: 'attendance/dtr', loadComponent: () => import('./pages/placeholder-pages').then(m => m.DtrComponent) },
      { path: 'attendance/leave-application', loadComponent: () => import('./pages/placeholder-pages').then(m => m.LeaveApplicationComponent) },
      { path: 'attendance/leave-balance', loadComponent: () => import('./pages/placeholder-pages').then(m => m.LeaveBalanceComponent) },
      { path: 'attendance/shift-scheduling', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ShiftSchedulingComponent) },
      { path: 'self-service/my-profile', loadComponent: () => import('./pages/placeholder-pages').then(m => m.MyProfileComponent) },
      { path: 'self-service/my-payslip', loadComponent: () => import('./pages/placeholder-pages').then(m => m.MyPayslipComponent) },
      { path: 'self-service/my-leave', loadComponent: () => import('./pages/placeholder-pages').then(m => m.MyLeaveComponent) },
      { path: 'self-service/my-performance', loadComponent: () => import('./pages/placeholder-pages').then(m => m.MyPerformanceComponent) },
      { path: 'self-service/service-record', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ServiceRecordComponent) },
      { path: 'self-service/request-documents', loadComponent: () => import('./pages/placeholder-pages').then(m => m.RequestDocumentsComponent) },
      { path: 'recruitment/job-postings', loadComponent: () => import('./pages/placeholder-pages').then(m => m.JobPostingsComponent) },
      { path: 'recruitment/applicants', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ApplicantTrackerComponent) },
      { path: 'recruitment/onboarding', loadComponent: () => import('./pages/placeholder-pages').then(m => m.OnboardingComponent) },
      { path: 'recruitment/plantilla', loadComponent: () => import('./pages/placeholder-pages').then(m => m.PlantillaComponent) },
      { path: 'performance/ipcr', loadComponent: () => import('./pages/placeholder-pages').then(m => m.IpcrComponent) },
      { path: 'performance/dpcr', loadComponent: () => import('./pages/placeholder-pages').then(m => m.DpcrComponent) },
      { path: 'performance/opcr', loadComponent: () => import('./pages/placeholder-pages').then(m => m.OpcrComponent) },
      { path: 'performance/coaching-journal', loadComponent: () => import('./pages/placeholder-pages').then(m => m.CoachingJournalComponent) },
      { path: 'performance/succession-planning', loadComponent: () => import('./pages/placeholder-pages').then(m => m.SuccessionPlanningComponent) },
      { path: 'learning-dev/training-calendar', loadComponent: () => import('./pages/placeholder-pages').then(m => m.TrainingCalendarComponent) },
      { path: 'learning-dev/training-courses', loadComponent: () => import('./pages/placeholder-pages').then(m => m.TrainingCoursesComponent) },
      { path: 'learning-dev/scholarships', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ScholarshipsComponent) },
      { path: 'learning-dev/e-learning', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ELearningComponent) },
      { path: 'learning-dev/reports', loadComponent: () => import('./pages/placeholder-pages').then(m => m.TrainingReportsComponent) },
      { path: 'offboarding/separation', loadComponent: () => import('./pages/placeholder-pages').then(m => m.SeparationComponent) },
      { path: 'offboarding/exit-interview', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ExitInterviewComponent) },
      { path: 'offboarding/clearance', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ClearanceComponent) },
      { path: 'reports', loadComponent: () => import('./pages/placeholder-pages').then(m => m.ReportsComponent) },
      { path: 'admin/users', loadComponent: () => import('./pages/placeholder-pages').then(m => m.UserManagementComponent) },
      { path: 'admin/data-libraries', loadComponent: () => import('./pages/placeholder-pages').then(m => m.DataLibrariesComponent) },
      { path: 'admin/audit-trail', loadComponent: () => import('./pages/placeholder-pages').then(m => m.AuditTrailComponent) },
      { path: 'admin/monitoring', loadComponent: () => import('./pages/placeholder-pages').then(m => m.SystemMonitoringComponent) }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
