using DPWH_HRIS.Domain.Common;
using DPWH_HRIS.Domain.Enums;

namespace DPWH_HRIS.Domain.Entities;

// ===================== CORE ENTITIES =====================

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public Guid? OfficeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
    public string? LastLoginIP { get; set; }
    public string? ADObjectGuid { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation
    public Role? Role { get; set; }
    public Office? Office { get; set; }
    public Employee? Employee { get; set; }
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PermissionsJson { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    // Navigation
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}

public class Region : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    // Navigation
    public ICollection<Office> Offices { get; set; } = new List<Office>();
}

public class Office : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public OfficeType OfficeType { get; set; }
    public Guid? RegionId { get; set; }
    public Guid? ParentOfficeId { get; set; }

    // Navigation
    public Region? Region { get; set; }
    public Office? ParentOffice { get; set; }
    public ICollection<Office> ChildOffices { get; set; } = new List<Office>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int SalaryGrade { get; set; }
    public int StepIncrement { get; set; }
    public string? PlantillaItemNo { get; set; }
    public Guid? OfficeId { get; set; }
    public bool IsFilled { get; set; } = false;
    public string? QualificationStandards { get; set; }

    // Navigation
    public Office? Office { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public CivilStatus CivilStatus { get; set; }
    public string? Citizenship { get; set; }
    public BloodType? BloodType { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? ContactNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? ResidentialAddress { get; set; }
    public string? PermanentAddress { get; set; }
    public string? GSISNumber { get; set; }
    public string? PagIBIGNumber { get; set; }
    public string? PhilHealthNumber { get; set; }
    public string? SSSNumber { get; set; }
    public string? TINNumber { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? PositionId { get; set; }
    public DateTime DateHired { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;
    public string? PhotoPath { get; set; }

    // Navigation
    public User? User { get; set; }
    public Office? Office { get; set; }
    public Position? Position { get; set; }
    public ICollection<DailyTimeRecord> DailyTimeRecords { get; set; } = new List<DailyTimeRecord>();
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
    public ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
    public ICollection<TrainingParticipant> TrainingParticipants { get; set; } = new List<TrainingParticipant>();
}

// ===================== PAYROLL ENTITIES =====================

public class PayrollRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string PayPeriod { get; set; } = string.Empty;
    public decimal BasicPay { get; set; }
    public decimal PERA { get; set; }
    public decimal RATA { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public string Status { get; set; } = "Draft";

    // Navigation
    public Employee? Employee { get; set; }
    public ICollection<PayrollDeduction> Deductions { get; set; } = new List<PayrollDeduction>();
}

public class PayrollDeduction : BaseEntity
{
    public Guid PayrollRecordId { get; set; }
    public string DeductionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    // Navigation
    public PayrollRecord? PayrollRecord { get; set; }
}

public class GovernmentContribution : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public ContributionType ContributionType { get; set; }
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public DateTime EffectiveDate { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

public class TaxSchedule : BaseEntity
{
    public decimal BracketMin { get; set; }
    public decimal BracketMax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal FixedTax { get; set; }
    public int EffectiveYear { get; set; }
}

public class LoanRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public decimal LoanAmount { get; set; }
    public decimal MonthlyAmortization { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

// ===================== TIME & LEAVE ENTITIES =====================

public class DailyTimeRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? TimeIn { get; set; }
    public TimeSpan? TimeOut { get; set; }
    public TimeSpan? LunchOut { get; set; }
    public TimeSpan? LunchIn { get; set; }
    public decimal OvertimeHours { get; set; }
    public int UndertimeMinutes { get; set; }
    public int TardinessMinutes { get; set; }
    public bool IsAbsent { get; set; }
    public string? Remarks { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

public class LeaveApplication : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public decimal TotalDays { get; set; }
    public string? Reason { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public Guid? ApproverId { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? SupportingDocumentPath { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
    public User? Approver { get; set; }
}

public class LeaveBalance : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal UsedCredits { get; set; }
    public decimal RemainingCredits { get; set; }
    public int Year { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

public class ShiftSchedule : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string ShiftName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

// ===================== RECRUITMENT ENTITIES =====================

public class JobPosting : BaseEntity
{
    public Guid? PositionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? QualificationRequirements { get; set; }
    public DateTime PostingDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public string Status { get; set; } = "Open";
    public bool IsPublished { get; set; } = false;

    // Navigation
    public Position? Position { get; set; }
    public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
}

public class Applicant : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string? ResumeFilePath { get; set; }
    public DateTime ApplicationDate { get; set; }
    public Guid JobPostingId { get; set; }
    public string Status { get; set; } = "Applied";
    public string? EvaluationNotes { get; set; }
    public DateTime? InterviewSchedule { get; set; }

    // Navigation
    public JobPosting? JobPosting { get; set; }
}

public class OnboardingTask : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedDate { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
}

// ===================== PERFORMANCE ENTITIES =====================

public class PerformanceReview : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string ReviewPeriod { get; set; } = string.Empty;
    public PerformanceReviewType ReviewType { get; set; }
    public decimal? NumericalRating { get; set; }
    public string? AdjectivalRating { get; set; }
    public Guid? ReviewerId { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime? DateSubmitted { get; set; }

    // Navigation
    public Employee? Employee { get; set; }
    public User? Reviewer { get; set; }
    public ICollection<PerformanceCommitment> Commitments { get; set; } = new List<PerformanceCommitment>();
}

public class PerformanceCommitment : BaseEntity
{
    public Guid PerformanceReviewId { get; set; }
    public string Output { get; set; } = string.Empty;
    public string? SuccessIndicator { get; set; }
    public string? ActualAccomplishment { get; set; }
    public decimal? Rating { get; set; }
    public string? Remarks { get; set; }

    // Navigation
    public PerformanceReview? PerformanceReview { get; set; }
}

public class CoachingJournal : BaseEntity
{
    public Guid SupervisorId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime CoachingDate { get; set; }
    public string? Notes { get; set; }
    public string? ActionItems { get; set; }

    // Navigation
    public User? Supervisor { get; set; }
    public Employee? Employee { get; set; }
}

public class IndividualDevelopmentPlan : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string CompetencyGap { get; set; } = string.Empty;
    public string? DevelopmentIntervention { get; set; }
    public DateTime? TargetDate { get; set; }
    public string Status { get; set; } = "Planned";

    // Navigation
    public Employee? Employee { get; set; }
}

// ===================== LEARNING & DEVELOPMENT ENTITIES =====================

public class TrainingCourse : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CourseType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Venue { get; set; }
    public int MaxParticipants { get; set; }
    public string Status { get; set; } = "Planned";
    public Guid? FacilitatorId { get; set; }

    // Navigation
    public User? Facilitator { get; set; }
    public ICollection<TrainingParticipant> Participants { get; set; } = new List<TrainingParticipant>();
}

public class TrainingParticipant : BaseEntity
{
    public Guid TrainingCourseId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = "Enrolled";
    public DateTime? CompletionDate { get; set; }
    public string? CertificatePath { get; set; }
    public int? Rating { get; set; }

    // Navigation
    public TrainingCourse? TrainingCourse { get; set; }
    public Employee? Employee { get; set; }
}

public class Scholarship : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SponsorName { get; set; }
    public string? Requirements { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public string Status { get; set; } = "Open";

    // Navigation
    public ICollection<ScholarshipApplication> Applications { get; set; } = new List<ScholarshipApplication>();
}

public class ScholarshipApplication : BaseEntity
{
    public Guid ScholarshipId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = "Applied";
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }

    // Navigation
    public Scholarship? Scholarship { get; set; }
    public Employee? Employee { get; set; }
}

// ===================== OFFBOARDING ENTITIES =====================

public class EmployeeSeparation : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public SeparationType SeparationType { get; set; }
    public DateTime SeparationDate { get; set; }
    public DateTime? LastWorkingDay { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Initiated";

    // Navigation
    public Employee? Employee { get; set; }
    public ExitInterview? ExitInterview { get; set; }
    public ICollection<ClearanceForm> ClearanceForms { get; set; } = new List<ClearanceForm>();
}

public class ExitInterview : BaseEntity
{
    public Guid EmployeeSeparationId { get; set; }
    public DateTime InterviewDate { get; set; }
    public string? InterviewerName { get; set; }
    public string? Feedback { get; set; }
    public string? Suggestions { get; set; }

    // Navigation
    public EmployeeSeparation? EmployeeSeparation { get; set; }
}

public class ClearanceForm : BaseEntity
{
    public Guid EmployeeSeparationId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ClearedBy { get; set; }
    public DateTime? ClearedDate { get; set; }
    public string? Remarks { get; set; }

    // Navigation
    public EmployeeSeparation? EmployeeSeparation { get; set; }
}

// ===================== SYSTEM ENTITIES =====================

public class AuditTrail : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public NotificationType NotificationType { get; set; }
    public string? RelatedEntityType { get; set; }
    public string? RelatedEntityId { get; set; }

    // Navigation
    public User? User { get; set; }
}

public class WorkflowTransaction : BaseEntity
{
    public string TransactionType { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public Guid? CurrentApproverId { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    // Navigation
    public User? Requester { get; set; }
    public User? CurrentApprover { get; set; }
}

public class FileAttachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Guid? UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? Uploader { get; set; }
}

public class DataLibrary : BaseEntity
{
    public string Category { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? PostedBy { get; set; }

    // Navigation
    public User? PostedByUser { get; set; }
}

public class Memorandum : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public DateTime DateIssued { get; set; }
    public Guid? PostedBy { get; set; }

    // Navigation
    public User? PostedByUser { get; set; }
}

public class DownloadableForm : BaseEntity
{
    public string FormName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Version { get; set; }
    public bool IsActive { get; set; } = true;
}
