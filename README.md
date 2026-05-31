LeagueStack
LeagueStack is a multi-tenanted platform designed to manage and display social sports competition results. The engine is architected to allow complete visual branding and custom competition logic to vary dynamically per tenant, providing a tailored experience for every user group.

Key Features
Multi-Tenancy: Supports isolated data and configurations for multiple sports leagues and clients on a single application instance.

Extensible Plugin Architecture: Decouples core logic from tenant-specific behavior. Custom rules and themes are loaded dynamically based on the active tenant context.

Unified Results Engine: A centralized system for tracking matches, team standings, and league statistics across diverse sports types.

Technical Stack
.NET 8.0

ASP.NET Core

Entity Framework Core

SQL Server

Getting Started
Prerequisites
.NET SDK 8.0+

SQL Server

Setup
Clone the repository:
git clone https://github.com/Sqelson/LeagueStack.git

Configuration:
Update your connection strings in appsettings.json to point to your local SQL Server instance.

Running the application:
Navigate to the web directory and start the service:
cd LeagueStack.Web
dotnet run

Architectural Overview
The core of LeagueStack is built upon a modular plugin system that follows the Open-Closed Principle. Rather than hardcoding league rules or branding elements, the application resolves tenant-specific implementations at runtime via dependency injection.

This design allows for rapid scaling—adding a new league or a new set of sports competition rules requires only the creation of a new plugin, leaving the core platform codebase clean and maintainable.
