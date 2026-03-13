using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DPWH_HRIS.Domain.Entities;
using DPWH_HRIS.Domain.Enums;
using BCrypt.Net;

namespace DPWH_HRIS.Infrastructure.Data;

/// <summary>
/// Seeds initial data required to run the DPWH HRIS system.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();
            await SeedRolesAsync(context);
            await SeedRegionsAndOfficesAsync(context);
            await SeedAdminUserAsync(context);
            await SeedPositionsAsync(context);
            await SeedDataLibrariesAsync(context);
            await SeedSampleContentAsync(context);
            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Id = Guid.NewGuid(), Name = "System Administrator", Description = "Full system access", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "HR Administrator", Description = "Full HR module access", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "HR Staff", Description = "HR staff access", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Regional HR Officer", Description = "Regional HR access", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Department Head/Supervisor", Description = "Department head access", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Employee", Description = "Employee self-service access", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "System Administrator");

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@dpwh.gov.ph",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@DPWH2026!"),
            RoleId = adminRole.Id,
            IsActive = true,
            CreatedBy = "System",
            CreatedDate = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRegionsAndOfficesAsync(AppDbContext context)
    {
        if (await context.Regions.AnyAsync()) return;

        // Seed Regions
        var regions = new List<Region>
        {
            new() { Id = Guid.NewGuid(), Name = "National Capital Region", Code = "NCR", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cordillera Administrative Region", Code = "CAR", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Ilocos Region", Code = "I", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cagayan Valley", Code = "II", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Central Luzon", Code = "III", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "CALABARZON", Code = "IV-A", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "MIMAROPA", Code = "IV-B", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Bicol Region", Code = "V", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Western Visayas", Code = "VI", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Central Visayas", Code = "VII", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Eastern Visayas", Code = "VIII", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Zamboanga Peninsula", Code = "IX", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Northern Mindanao", Code = "X", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Davao Region", Code = "XI", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "SOCCSKSARGEN", Code = "XII", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "CARAGA", Code = "XIII", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Bangsamoro Autonomous Region in Muslim Mindanao", Code = "BARMM", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        };

        await context.Regions.AddRangeAsync(regions);
        await context.SaveChangesAsync();

        // Seed Central Office
        var coOffice = new Office
        {
            Id = Guid.NewGuid(),
            Name = "DPWH Central Office",
            Code = "CO",
            OfficeType = OfficeType.CentralOffice,
            CreatedBy = "System",
            CreatedDate = DateTime.UtcNow
        };
        await context.Offices.AddAsync(coOffice);

        // CO Sub-offices
        var coSubOffices = new[]
        {
            "Office of the Secretary", "Office of the Undersecretary for Operations",
            "Office of the Undersecretary for Technical Services",
            "Human Resource and Administrative Service (HRAS)",
            "Financial and Management Service", "Legal Service", "Planning Service",
            "Bureau of Construction", "Bureau of Design", "Bureau of Maintenance",
            "Bureau of Quality and Safety", "Bureau of Research and Standards"
        };

        foreach (var sub in coSubOffices)
        {
            await context.Offices.AddAsync(new Office
            {
                Id = Guid.NewGuid(),
                Name = sub,
                Code = sub.Substring(0, Math.Min(10, sub.Length)).Replace(" ", "").ToUpper(),
                OfficeType = OfficeType.CentralOffice,
                ParentOfficeId = coOffice.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            });
        }

        // Seed Regional Offices with sample DEOs
        foreach (var region in regions)
        {
            var roOffice = new Office
            {
                Id = Guid.NewGuid(),
                Name = $"DPWH Regional Office {region.Code}",
                Code = $"RO-{region.Code}",
                OfficeType = OfficeType.RegionalOffice,
                RegionId = region.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };
            await context.Offices.AddAsync(roOffice);
            await context.SaveChangesAsync();

            // Add 2 sample DEOs per region
            await context.Offices.AddRangeAsync(
                new Office
                {
                    Id = Guid.NewGuid(),
                    Name = $"{region.Name} 1st District Engineering Office",
                    Code = $"DEO-{region.Code}-1",
                    OfficeType = OfficeType.DistrictEngineeringOffice,
                    RegionId = region.Id,
                    ParentOfficeId = roOffice.Id,
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow
                },
                new Office
                {
                    Id = Guid.NewGuid(),
                    Name = $"{region.Name} 2nd District Engineering Office",
                    Code = $"DEO-{region.Code}-2",
                    OfficeType = OfficeType.DistrictEngineeringOffice,
                    RegionId = region.Id,
                    ParentOfficeId = roOffice.Id,
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow
                }
            );
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPositionsAsync(AppDbContext context)
    {
        if (await context.Positions.AnyAsync()) return;

        var positions = new List<Position>
        {
            new() { Title = "Secretary", SalaryGrade = 33, StepIncrement = 1, PlantillaItemNo = "CO-001", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Undersecretary", SalaryGrade = 31, StepIncrement = 1, PlantillaItemNo = "CO-002", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Assistant Secretary", SalaryGrade = 29, StepIncrement = 1, PlantillaItemNo = "CO-003", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Director IV", SalaryGrade = 28, StepIncrement = 1, PlantillaItemNo = "CO-004", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Director III", SalaryGrade = 27, StepIncrement = 1, PlantillaItemNo = "CO-005", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Director II", SalaryGrade = 26, StepIncrement = 1, PlantillaItemNo = "CO-006", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Engineer V", SalaryGrade = 24, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Engineer IV", SalaryGrade = 22, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Engineer III", SalaryGrade = 18, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Engineer II", SalaryGrade = 16, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Engineer I", SalaryGrade = 12, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Officer V", SalaryGrade = 18, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Officer IV", SalaryGrade = 15, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Officer III", SalaryGrade = 14, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Officer II", SalaryGrade = 11, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Aide VI", SalaryGrade = 6, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Administrative Aide IV", SalaryGrade = 4, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Clerk III", SalaryGrade = 9, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Clerk II", SalaryGrade = 8, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Title = "Laborer I", SalaryGrade = 1, StepIncrement = 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        };

        await context.Positions.AddRangeAsync(positions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDataLibrariesAsync(AppDbContext context)
    {
        if (await context.DataLibraries.AnyAsync()) return;

        var libraries = new List<DataLibrary>();

        // Salary Grades 1-33 with basic pay rates (Step 1)
        var salaryGradeRates = new decimal[] {
            12517, 13181, 13879, 14614, 15388, 16403, 17492, 18668, 19933, 21307,
            22801, 24430, 26112, 27967, 29966, 32053, 34314, 36771, 39399, 42219,
            45289, 48620, 52342, 56399, 61478, 68411, 75890, 84140, 93320, 103390,
            114553, 126972, 140771
        };

        for (int sg = 1; sg <= 33; sg++)
        {
            libraries.Add(new DataLibrary
            {
                Category = "SalaryGrade",
                Code = $"SG-{sg:D2}",
                Value = $"Salary Grade {sg}",
                Description = $"Basic Pay (Step 1): ₱{salaryGradeRates[sg - 1]:N2}",
                SortOrder = sg,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            });
        }

        // Civil Status
        string[] civilStatuses = ["Single", "Married", "Widowed", "Separated", "Others"];
        for (int i = 0; i < civilStatuses.Length; i++)
        {
            libraries.Add(new DataLibrary { Category = "CivilStatus", Code = civilStatuses[i].ToUpper(), Value = civilStatuses[i], SortOrder = i + 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow });
        }

        // Blood Types
        string[] bloodTypes = ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];
        for (int i = 0; i < bloodTypes.Length; i++)
        {
            libraries.Add(new DataLibrary { Category = "BloodType", Code = bloodTypes[i].Replace("+", "POS").Replace("-", "NEG"), Value = bloodTypes[i], SortOrder = i + 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow });
        }

        // Education Levels
        string[] educLevels = ["Elementary Graduate", "High School Graduate", "Vocational/Technical Course", "College Level", "College Graduate", "With Master's Degree", "With Doctorate Degree"];
        for (int i = 0; i < educLevels.Length; i++)
        {
            libraries.Add(new DataLibrary { Category = "EducationLevel", Code = $"EDU-{i + 1}", Value = educLevels[i], SortOrder = i + 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow });
        }

        // Eligibility Types
        string[] eligibilityTypes = [
            "Career Service Professional", "Career Service Sub-Professional",
            "RA 1080 (Licensure Examination)", "PD 907 (Honor Graduate)",
            "MC 11, s. 1996 (Scientific and Technological Specialist)",
            "MC 10, s. 2013 (Special Category)"
        ];
        for (int i = 0; i < eligibilityTypes.Length; i++)
        {
            libraries.Add(new DataLibrary { Category = "EligibilityType", Code = $"ELIG-{i + 1}", Value = eligibilityTypes[i], SortOrder = i + 1, CreatedBy = "System", CreatedDate = DateTime.UtcNow });
        }

        await context.DataLibraries.AddRangeAsync(libraries);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSampleContentAsync(AppDbContext context)
    {
        if (await context.Announcements.AnyAsync()) return;

        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null) return;

        // Sample Announcements
        await context.Announcements.AddRangeAsync(
            new Announcement
            {
                Title = "Welcome to DPWH HRIS",
                Content = "The new Human Resource Information System is now live. Please log in with your assigned credentials.",
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                PostedBy = adminUser.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            },
            new Announcement
            {
                Title = "System Maintenance Notice",
                Content = "Scheduled maintenance every Sunday from 12:00 AM to 4:00 AM. Please save your work before this period.",
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(3),
                PostedBy = adminUser.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            }
        );

        // Sample Memorandums
        await context.Memorandums.AddRangeAsync(
            new Memorandum
            {
                Title = "Circular on Leave Administration",
                ReferenceNumber = "DPWH-HRAS-MC-2026-001",
                Content = "All employees are reminded to file their leave applications at least 5 working days in advance, except for emergency leaves.",
                DateIssued = DateTime.UtcNow.AddDays(-30),
                PostedBy = adminUser.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            },
            new Memorandum
            {
                Title = "Updated Performance Evaluation Guidelines",
                ReferenceNumber = "DPWH-HRAS-MC-2026-002",
                Content = "Effective immediately, all IPCR submissions must be done through the new HRIS portal. Manual submissions will no longer be accepted.",
                DateIssued = DateTime.UtcNow.AddDays(-15),
                PostedBy = adminUser.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            }
        );

        // Sample Downloadable Forms
        await context.DownloadableForms.AddRangeAsync(
            new DownloadableForm { FormName = "CSC Form 212 (Personal Data Sheet)", Description = "Official CSC Personal Data Sheet form", Category = "HR Forms", FilePath = "/forms/CSC-Form-212.pdf", Version = "Revised 2017", IsActive = true, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new DownloadableForm { FormName = "Application for Leave (CSC Form 6)", Description = "Application for leave form", Category = "Leave Forms", FilePath = "/forms/CSC-Form-6.pdf", Version = "Revised 2020", IsActive = true, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new DownloadableForm { FormName = "IPCR Form", Description = "Individual Performance Commitment and Review Form", Category = "Performance Forms", FilePath = "/forms/IPCR-Form.xlsx", Version = "2026", IsActive = true, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new DownloadableForm { FormName = "Service Record Form", Description = "Request for Service Record", Category = "HR Forms", FilePath = "/forms/Service-Record-Request.pdf", Version = "2024", IsActive = true, CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();
    }
}
