import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-placeholder',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="placeholder-page">
      <div class="placeholder-card">
        <i [class]="'pi ' + icon + ' placeholder-icon'"></i>
        <h2>{{ title }}</h2>
        <p class="placeholder-desc">{{ description }}</p>
        <div class="placeholder-badge">
          <i class="pi pi-wrench"></i> Under Development
        </div>
        <p class="placeholder-note">
          This module is scaffolded and ready for implementation.
          The backend API endpoints are available in the <strong>DPWH_HRIS.API</strong> project.
        </p>
      </div>
    </div>
  `,
  styles: [`
    .placeholder-page {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 60vh;
    }
    .placeholder-card {
      background: white;
      border-radius: 16px;
      padding: 3rem;
      text-align: center;
      box-shadow: 0 4px 16px rgba(0,0,0,0.08);
      max-width: 480px;
      width: 100%;
    }
    .placeholder-icon {
      font-size: 4rem;
      color: #1B2A4A;
      margin-bottom: 1.5rem;
      display: block;
    }
    .placeholder-card h2 {
      font-size: 1.5rem;
      font-weight: 700;
      color: #1B2A4A;
      margin: 0 0 0.75rem;
    }
    .placeholder-desc {
      color: #666;
      margin-bottom: 1.5rem;
      line-height: 1.6;
    }
    .placeholder-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      background: #FFF3E0;
      color: #E85D26;
      padding: 0.4rem 1rem;
      border-radius: 20px;
      font-size: 0.875rem;
      font-weight: 600;
      margin-bottom: 1.5rem;
    }
    .placeholder-note {
      font-size: 0.8rem;
      color: #aaa;
      background: #f8f9fa;
      border-radius: 8px;
      padding: 0.75rem;
      line-height: 1.5;
    }
  `]
})
export class PlaceholderPageComponent {
  @Input() title = 'Module Page';
  @Input() description = 'This page is part of the DPWH HRIS baseline and will be implemented by the Solutions Architect.';
  @Input() icon = 'pi-file';
}

// Individual placeholder page components for each module

@Component({ selector: 'app-pds-form', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Personal Data Sheet (PDS)\'" [description]="\'CSC Form 212 — Employee Personal Data Sheet. Complete PDS in Civil Service Commission format.\'" [icon]="\'pi-id-card\'"></app-placeholder>' })
export class PdsFormComponent {}

@Component({ selector: 'app-employee-directory', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Employee Directory\'" [description]="\'Searchable directory of all DPWH employees nationwide. Filter by office, position, and employment type.\'" [icon]="\'pi-users\'"></app-placeholder>' })
export class EmployeeDirectoryComponent {}

@Component({ selector: 'app-org-chart', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Organization Chart\'" [description]="\'Visual hierarchy of DPWH organizational structure from Central Office to District Engineering Offices.\'" [icon]="\'pi-sitemap\'"></app-placeholder>' })
export class OrgChartComponent {}

@Component({ selector: 'app-payroll-processing', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Payroll Processing\'" [description]="\'Process payroll for all employee types. Compute basic pay, PERA, RATA, and all allowances per DBM guidelines.\'" [icon]="\'pi-calculator\'"></app-placeholder>' })
export class PayrollProcessingComponent {}

@Component({ selector: 'app-deductions', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Deductions & Loans\'" [description]="\'Manage employee deductions including GSIS, PhilHealth, Pag-IBIG, and loan amortizations.\'" [icon]="\'pi-minus-circle\'"></app-placeholder>' })
export class DeductionsComponent {}

@Component({ selector: 'app-gov-contributions', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Government Contributions\'" [description]="\'Track employee and employer shares for GSIS, PhilHealth, and Pag-IBIG contributions.\'" [icon]="\'pi-building\'"></app-placeholder>' })
export class GovContributionsComponent {}

@Component({ selector: 'app-dtr', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Daily Time Record (DTR)\'" [description]="\'Record and monitor employee attendance. Integrates with biometric devices for automated time-in/time-out logging.\'" [icon]="\'pi-clock\'"></app-placeholder>' })
export class DtrComponent {}

@Component({ selector: 'app-leave-application', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Leave Applications\'" [description]="\'File and manage leave applications. Supports all CSC leave types with multi-level approval workflow.\'" [icon]="\'pi-file-edit\'"></app-placeholder>' })
export class LeaveApplicationComponent {}

@Component({ selector: 'app-leave-balance', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Leave Balances\'" [description]="\'View earned and used leave credits for all leave types. Includes leave monetization computation.\'" [icon]="\'pi-chart-bar\'"></app-placeholder>' })
export class LeaveBalanceComponent {}

@Component({ selector: 'app-shift-scheduling', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Shift Scheduling\'" [description]="\'Manage employee work schedules and shift assignments.\'" [icon]="\'pi-calendar-plus\'"></app-placeholder>' })
export class ShiftSchedulingComponent {}

@Component({ selector: 'app-my-profile', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'My Profile\'" [description]="\'View and update your personal information, contact details, and profile photo.\'" [icon]="\'pi-user\'"></app-placeholder>' })
export class MyProfileComponent {}

@Component({ selector: 'app-my-payslip', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'My Payslip\'" [description]="\'View and download your payslips and salary history.\'" [icon]="\'pi-money-bill\'"></app-placeholder>' })
export class MyPayslipComponent {}

@Component({ selector: 'app-my-leave', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'My Leave\'" [description]="\'Apply for leave, view your leave balances, and track application status.\'" [icon]="\'pi-calendar\'"></app-placeholder>' })
export class MyLeaveComponent {}

@Component({ selector: 'app-my-performance', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'My Performance\'" [description]="\'View your IPCR ratings, performance commitments, and evaluation history.\'" [icon]="\'pi-chart-line\'"></app-placeholder>' })
export class MyPerformanceComponent {}

@Component({ selector: 'app-service-record', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Service Record\'" [description]="\'View your complete government service record including positions, salaries, and awards.\'" [icon]="\'pi-file\'"></app-placeholder>' })
export class ServiceRecordComponent {}

@Component({ selector: 'app-request-documents', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Request Documents\'" [description]="\'Request official HR documents such as certificate of employment, service record, and payslip.\'" [icon]="\'pi-envelope\'"></app-placeholder>' })
export class RequestDocumentsComponent {}

@Component({ selector: 'app-job-postings', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Job Postings\'" [description]="\'Manage DPWH job vacancies per Plantilla. Publish positions and track applicants through the recruitment process.\'" [icon]="\'pi-megaphone\'"></app-placeholder>' })
export class JobPostingsComponent {}

@Component({ selector: 'app-applicant-tracker', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Applicant Tracker\'" [description]="\'Track all applicants from initial application through interview, selection, and appointment.\'" [icon]="\'pi-user-plus\'"></app-placeholder>' })
export class ApplicantTrackerComponent {}

@Component({ selector: 'app-onboarding', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Onboarding\'" [description]="\'Manage new employee onboarding tasks and checklists for smooth integration.\'" [icon]="\'pi-check-circle\'"></app-placeholder>' })
export class OnboardingComponent {}

@Component({ selector: 'app-plantilla', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Plantilla Management\'" [description]="\'Manage the DPWH Plantilla of Positions. Track filled, vacant, and proposed positions.\'" [icon]="\'pi-list\'"></app-placeholder>' })
export class PlantillaComponent {}

@Component({ selector: 'app-ipcr', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Individual Performance Commitment and Review (IPCR)\'" [description]="\'Create and manage IPCR forms. Track performance commitments and ratings per CSC SPMS guidelines.\'" [icon]="\'pi-star\'"></app-placeholder>' })
export class IpcrComponent {}

@Component({ selector: 'app-dpcr', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Division Performance Commitment and Review (DPCR)\'" [description]="\'Manage division-level performance commitments and consolidated ratings.\'" [icon]="\'pi-star\'"></app-placeholder>' })
export class DpcrComponent {}

@Component({ selector: 'app-opcr', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Office Performance Commitment and Review (OPCR)\'" [description]="\'Office-level performance management aligned with DPWH strategic objectives.\'" [icon]="\'pi-star\'"></app-placeholder>' })
export class OpcrComponent {}

@Component({ selector: 'app-coaching-journal', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Coaching Journal\'" [description]="\'Document coaching sessions between supervisors and employees. Track action items and development progress.\'" [icon]="\'pi-book\'"></app-placeholder>' })
export class CoachingJournalComponent {}

@Component({ selector: 'app-succession-planning', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Succession Planning\'" [description]="\'Identify and develop potential successors for key positions in DPWH.\'" [icon]="\'pi-sitemap\'"></app-placeholder>' })
export class SuccessionPlanningComponent {}

@Component({ selector: 'app-training-calendar', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Training Calendar\'" [description]="\'View and manage the annual DPWH training and development calendar.\'" [icon]="\'pi-calendar\'"></app-placeholder>' })
export class TrainingCalendarComponent {}

@Component({ selector: 'app-training-courses', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Training Courses\'" [description]="\'Manage training courses, participants, and completion records.\'" [icon]="\'pi-book\'"></app-placeholder>' })
export class TrainingCoursesComponent {}

@Component({ selector: 'app-scholarships', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Scholarships\'" [description]="\'Manage DPWH scholarship programs and applicant tracking.\'" [icon]="\'pi-graduation-cap\'"></app-placeholder>' })
export class ScholarshipsComponent {}

@Component({ selector: 'app-elearning', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'E-Learning\'" [description]="\'Access online learning modules and track completion of mandatory trainings.\'" [icon]="\'pi-desktop\'"></app-placeholder>' })
export class ELearningComponent {}

@Component({ selector: 'app-training-reports', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Training Reports\'" [description]="\'Generate reports on training activities, participation rates, and learning outcomes.\'" [icon]="\'pi-chart-bar\'"></app-placeholder>' })
export class TrainingReportsComponent {}

@Component({ selector: 'app-separation', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Employee Separation\'" [description]="\'Process employee separations including resignation, retirement, and end of contract.\'" [icon]="\'pi-times-circle\'"></app-placeholder>' })
export class SeparationComponent {}

@Component({ selector: 'app-exit-interview', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Exit Interview\'" [description]="\'Conduct and record exit interviews for separating employees.\'" [icon]="\'pi-comments\'"></app-placeholder>' })
export class ExitInterviewComponent {}

@Component({ selector: 'app-clearance', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Clearance Form\'" [description]="\'Process clearance requirements across all DPWH departments for separating employees.\'" [icon]="\'pi-verified\'"></app-placeholder>' })
export class ClearanceComponent {}

@Component({ selector: 'app-reports', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Reports\'" [description]="\'Generate and download HR reports. Integrates with SSRS/Crystal Reports for standardized government reports.\'" [icon]="\'pi-file-pdf\'"></app-placeholder>' })
export class ReportsComponent {}

@Component({ selector: 'app-user-management', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'User Management\'" [description]="\'Manage system users, assign roles, and configure access permissions.\'" [icon]="\'pi-users\'"></app-placeholder>' })
export class UserManagementComponent {}

@Component({ selector: 'app-data-libraries', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Data Libraries\'" [description]="\'Manage standard HR data libraries: salary grades, leave types, eligibilities, and other reference data.\'" [icon]="\'pi-database\'"></app-placeholder>' })
export class DataLibrariesComponent {}

@Component({ selector: 'app-audit-trail', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'Audit Trail\'" [description]="\'View all system transactions with user, action, timestamp, and IP address tracking.\'" [icon]="\'pi-history\'"></app-placeholder>' })
export class AuditTrailComponent {}

@Component({ selector: 'app-system-monitoring', standalone: true, imports: [PlaceholderPageComponent], template: '<app-placeholder [title]="\'System Monitoring\'" [description]="\'Monitor system performance, active sessions, and error logs.\'" [icon]="\'pi-server\'"></app-placeholder>' })
export class SystemMonitoringComponent {}
