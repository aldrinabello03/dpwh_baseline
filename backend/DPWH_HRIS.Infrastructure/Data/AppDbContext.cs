using Microsoft.EntityFrameworkCore;
using DPWH_HRIS.Domain.Entities;

namespace DPWH_HRIS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Core
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Office> Offices => Set<Office>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();

    // Payroll
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    public DbSet<PayrollDeduction> PayrollDeductions => Set<PayrollDeduction>();
    public DbSet<GovernmentContribution> GovernmentContributions => Set<GovernmentContribution>();
    public DbSet<TaxSchedule> TaxSchedules => Set<TaxSchedule>();
    public DbSet<LoanRecord> LoanRecords => Set<LoanRecord>();

    // Time & Leave
    public DbSet<DailyTimeRecord> DailyTimeRecords => Set<DailyTimeRecord>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<ShiftSchedule> ShiftSchedules => Set<ShiftSchedule>();

    // Recruitment
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<OnboardingTask> OnboardingTasks => Set<OnboardingTask>();

    // Performance
    public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
    public DbSet<PerformanceCommitment> PerformanceCommitments => Set<PerformanceCommitment>();
    public DbSet<CoachingJournal> CoachingJournals => Set<CoachingJournal>();
    public DbSet<IndividualDevelopmentPlan> IndividualDevelopmentPlans => Set<IndividualDevelopmentPlan>();

    // Learning & Development
    public DbSet<TrainingCourse> TrainingCourses => Set<TrainingCourse>();
    public DbSet<TrainingParticipant> TrainingParticipants => Set<TrainingParticipant>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();
    public DbSet<ScholarshipApplication> ScholarshipApplications => Set<ScholarshipApplication>();

    // Offboarding
    public DbSet<EmployeeSeparation> EmployeeSeparations => Set<EmployeeSeparation>();
    public DbSet<ExitInterview> ExitInterviews => Set<ExitInterview>();
    public DbSet<ClearanceForm> ClearanceForms => Set<ClearanceForm>();

    // System
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<WorkflowTransaction> WorkflowTransactions => Set<WorkflowTransaction>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();
    public DbSet<DataLibrary> DataLibraries => Set<DataLibrary>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Memorandum> Memorandums => Set<Memorandum>();
    public DbSet<DownloadableForm> DownloadableForms => Set<DownloadableForm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global soft-delete filter
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);

        // Self-referencing Office (hierarchy)
        modelBuilder.Entity<Office>()
            .HasOne(o => o.ParentOffice)
            .WithMany(o => o.ChildOffices)
            .HasForeignKey(o => o.ParentOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        // User -> Employee (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithOne(e => e.User)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // LeaveApplication -> Approver (restrict to avoid cycles)
        modelBuilder.Entity<LeaveApplication>()
            .HasOne(l => l.Approver)
            .WithMany()
            .HasForeignKey(l => l.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // CoachingJournal -> Supervisor
        modelBuilder.Entity<CoachingJournal>()
            .HasOne(c => c.Supervisor)
            .WithMany()
            .HasForeignKey(c => c.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        // PerformanceReview -> Reviewer
        modelBuilder.Entity<PerformanceReview>()
            .HasOne(p => p.Reviewer)
            .WithMany()
            .HasForeignKey(p => p.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        // WorkflowTransaction -> Requester/CurrentApprover
        modelBuilder.Entity<WorkflowTransaction>()
            .HasOne(w => w.Requester)
            .WithMany()
            .HasForeignKey(w => w.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkflowTransaction>()
            .HasOne(w => w.CurrentApprover)
            .WithMany()
            .HasForeignKey(w => w.CurrentApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeNumber).IsUnique();
        modelBuilder.Entity<AuditTrail>().HasIndex(a => a.Timestamp);
        modelBuilder.Entity<Notification>().HasIndex(n => new { n.UserId, n.IsRead });
    }
}
