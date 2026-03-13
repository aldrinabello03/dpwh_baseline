# DPWH Human Resource Information System (HRIS)

**Department of Public Works and Highways — Philippines**

A production-ready baseline codebase for the DPWH HRIS — a full-stack web application supporting **20,004 users nationwide** across Central Office (CO), Regional Offices (RO), and District Engineering Offices (DEO).

---

## Architecture Overview

```
dpwh_baseline/
├── backend/                      → .NET 9 Clean Architecture API
│   ├── DPWH_HRIS.API/            → Controllers, Middleware, Program.cs
│   ├── DPWH_HRIS.Application/    → Services, DTOs, Interfaces
│   ├── DPWH_HRIS.Domain/         → Entities, Enums, Base Classes
│   ├── DPWH_HRIS.Infrastructure/ → EF Core, Repositories, Services
│   └── DPWH_HRIS.Shared/         → Constants, Helpers, Extensions
└── frontend/                     → Angular 17+ with PrimeNG
    └── src/app/
        ├── core/                 → Guards, Interceptors, Services, Models
        ├── shared/               → Reusable Components
        ├── layout/               → Sidebar, Topbar, Footer, MainLayout
        └── pages/                → All Module Pages
```

---

## Tech Stack

| Layer | Technology | License |
|-------|-----------|---------|
| Frontend | Angular 17+ (Standalone) | MIT |
| UI Library | PrimeNG 17 | MIT |
| Charts | ApexCharts / ng-apexcharts | MIT |
| Backend | .NET 9 Web API (C#) | MIT |
| ORM | Entity Framework Core 9 | MIT |
| Database | SQL Server 2019+ | Commercial |
| PDF Export | QuestPDF | MIT |
| Excel Export | ClosedXML | MIT |
| Auth | JWT + AD integration scaffold | MIT |
| Logging | Serilog | Apache 2.0 |

---

## Prerequisites

**Backend:**
- .NET 9 SDK — https://dotnet.microsoft.com/download/dotnet/9.0
- SQL Server 2019+ (or SQL Server Express)
- dotnet-ef: `dotnet tool install --global dotnet-ef`

**Frontend:**
- Node.js 18+ — https://nodejs.org
- Angular CLI 17: `npm install -g @angular/cli@17`

---

## Setup Instructions

### Backend

```bash
cd dpwh_baseline/backend

# Update connection string in DPWH_HRIS.API/appsettings.json
# "DefaultConnection": "Server=YOUR_SERVER;Database=DPWH_HRIS;..."

# Run migrations
dotnet ef database update --project DPWH_HRIS.Infrastructure --startup-project DPWH_HRIS.API

# Start API
cd DPWH_HRIS.API
dotnet run
# API runs at: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Frontend

```bash
cd dpwh_baseline/frontend
npm install
ng serve
# App runs at: http://localhost:4200
```

---

## Default Admin Credentials

| Field | Value |
|-------|-------|
| Username | `admin` |
| Email | `admin@dpwh.gov.ph` |
| Password | `Admin@DPWH2026!` |

> **Change the default password immediately in production!**

---

## Module Descriptions

| Module | Status | Description |
|--------|--------|-------------|
| Dashboard | Functional | Stats, announcements, memorandums, forms |
| Personnel Information | Scaffolded | PDS, Employee Directory, Org Chart |
| Payroll Management | Scaffolded | Payroll processing, deductions, contributions |
| Time, Attendance & Leave | Scaffolded | DTR, Leave applications, balances |
| Self-Service Portal | Scaffolded | Profile, payslip, leave, performance |
| Recruitment & Placement | Scaffolded | Job postings, applicants, onboarding, plantilla |
| Performance Management | Scaffolded | IPCR, DPCR, OPCR, coaching journals |
| Learning & Development | Scaffolded | Training, scholarships, e-learning |
| Offboarding & Separation | Scaffolded | Separation, exit interviews, clearance |
| Reports | Scaffolded | SSRS/Crystal Reports integration scaffold |
| Administration | Scaffolded | User management, data libraries, audit trail |

---

## Pre-seeded Data

- 17 Regional Offices (NCR, CAR, Regions I–XIII, BARMM)
- 34 Sample District Engineering Offices (2 per region)
- 12 Central Office bureaus/services
- 6 System roles (Admin, HR Admin, HR Staff, Regional HR, Dept Head, Employee)
- Salary Grades 1–33 with SSL rates
- Standard data libraries (Civil Status, Blood Types, Education Levels, Eligibility Types)

---

## Notes for the Solutions Architect

### Active Directory Integration
`IActiveDirectoryService` is scaffolded. Implement `ActiveDirectoryService` using
`System.DirectoryServices.AccountManagement`. Configure LDAP in `appsettings.json`.

### SSRS Reports
`IReportService` is scaffolded. Connect using `Microsoft.Reporting.NETCore`.
Configure SSRS server URL in `appsettings.json` under `"SSRS"` section.

### Email (Outlook/Exchange)
`IEmailService` is scaffolded. Implement using `MailKit`.
Configure SMTP settings in `appsettings.json` under `"Email"` section.

### Deployment (IIS)
```bash
# Backend
dotnet publish DPWH_HRIS.API -c Release -o ./publish

# Frontend
ng build --configuration production
# Deploy dist/frontend to IIS wwwroot
```

### Database Migrations
```bash
dotnet ef database update --project DPWH_HRIS.Infrastructure --startup-project DPWH_HRIS.API
```

---

## Security Notes

- JWT tokens expire after 8 hours (configurable in appsettings.json)
- Refresh tokens expire after 7 days
- Passwords hashed with BCrypt
- Role-based authorization on all sensitive endpoints
- Audit trail on all write operations
- Soft-delete pattern to preserve audit history
- EF Core parameterized queries (SQL injection protection)

---

*DPWH HRIS Baseline v1.0.0 | March 2026*
*For support: hras@dpwh.gov.ph*
