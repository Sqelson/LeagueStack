# LeagueStack

A highly scalable, multi-tenanted platform for managing and displaying social sports competition results. The engine is architected to allow complete visual branding and custom competition logic to vary dynamically per tenant, providing a tailored experience for every user group.

## Key Features

- Advanced Multi-Tenancy: Supports isolated data and configurations for multiple sports leagues/clients on a single application instance.
- Extensible Plugin API: Decouples core logic from tenant-specific behavior, allowing custom components, rules, and themes to be loaded dynamically based on the active tenant.
- Automated Data Provisioning: Utilizes self-initializing LocalDB instances that automatically build schema and seed sample data on first launch.
- Unified Results Engine: A centralized system for tracking matches, team standings, and league statistics across diverse sports types.

## Technical Stack & Patterns

- .NET 8.0+ / ASP.NET Core
- Plugin Architecture / Strategy Pattern: Tenant variations are handled via discrete plugins rather than conditional logic in the core controller layer.
- Entity Framework Core: Powering the multi-tenant data isolation layer.
- LocalDB / SQL Server: Relational storage utilizing standard T-SQL syntax for high performance and deep query capability.

---

## Installation & Setup

### Prerequisites
- Visual Studio (Community Edition or higher)
- SQL Server LocalDB (Included with Visual Studio's data workload)

### Step-by-Step Setup

1. Clone the Repository:
```bash
git clone https://github.com/Sqelson/LeagueStack.git
cd LeagueStack
```

2. Database Initialization:
The application automatically handles database creation and seeding on startup using LocalDB. No manual SQL script execution is required. You can use SSMS or any T-SQL-compatible GUI to view and query the generated databases.

3. Running the Application via Visual Studio:

- 1. Open the .sln file in Visual Studio.
- 2. Set LeagueStack.Web as the Startup Project.
- 3. Select the http launch profile from the run dropdown menu.
- 4. Press F5 or click Start.

---

## Architecture Highlight: Tenant Plugin API

The backbone of LeagueStack is its modular design. Rather than hardcoding league rules or branding elements, the application identifies the incoming tenant and resolves the appropriate implementation at runtime.

The platform is designed for rapid expansion; new competition styles and custom scoring logic can be deployed instantly, ensuring the core service remains stable and fully isolated for every league.
