namespace DPWH_HRIS.Domain.Enums;

public enum EmploymentType
{
    Permanent = 1,
    Contractual = 2,
    Casual = 3,
    JobOrder = 4
}

public enum OfficeType
{
    CentralOffice = 1,
    RegionalOffice = 2,
    DistrictEngineeringOffice = 3
}

public enum LeaveType
{
    Vacation = 1,
    Sick = 2,
    Maternity = 3,
    Paternity = 4,
    SpecialPrivilege = 5,
    SoloParent = 6,
    Study = 7,
    VAWC = 8,
    Rehabilitation = 9,
    SpecialLeaveForWomen = 10,
    Calamity = 11,
    Adoption = 12,
    Monetization = 13
}

public enum ApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Disapproved = 3,
    Cancelled = 4,
    Withdrawn = 5
}

public enum PerformanceReviewType
{
    IPCR = 1,
    DPCR = 2,
    OPCR = 3
}

public enum SeparationType
{
    Resignation = 1,
    Retirement = 2,
    Termination = 3,
    EndOfContract = 4,
    TransferToOtherAgency = 5,
    Death = 6
}

public enum NotificationType
{
    Info = 1,
    Warning = 2,
    Success = 3,
    Error = 4,
    Approval = 5,
    LeaveStatus = 6,
    System = 7
}

public enum ContributionType
{
    GSIS = 1,
    PhilHealth = 2,
    PagIBIG = 3
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Others = 3
}

public enum CivilStatus
{
    Single = 1,
    Married = 2,
    Widowed = 3,
    Separated = 4,
    Others = 5
}

public enum BloodType
{
    APositive = 1,
    ANegative = 2,
    BPositive = 3,
    BNegative = 4,
    ABPositive = 5,
    ABNegative = 6,
    OPositive = 7,
    ONegative = 8
}

public enum EmploymentStatus
{
    Active = 1,
    Inactive = 2,
    OnLeave = 3,
    Suspended = 4,
    Resigned = 5,
    Retired = 6,
    Terminated = 7
}
