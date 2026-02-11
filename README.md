# UsalClinic.NET-FYP

![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=.net&logoColor=white) ![Status](https://img.shields.io/badge/build-passing-brightgreen) ![License](https://img.shields.io/badge/license-MIT-yellow.svg)

A clinic management system built with ASP.NET Core — Final Year Project. This repo includes a Web UI, a REST API, domain and application layers, EF Core data access, and unit tests.

Highlights

- Clean layered architecture: `UsalClinic.Core`, `UsalClinic.Application`, `UsalClinic.Infrastructure`, `UsalClinic.Web`, `UsalClinic.Api`.
- Authentication & Identity integration
- EF Core migrations & seeding
- Unit tests for core services

Repository structure

- `UsalClinic.Web` — Razor Pages / MVC frontend
- `UsalClinic.Api` — REST API
- `UsalClinic.Core` — domain entities
- `UsalClinic.Application` — DTOs & business logic
- `UsalClinic.Infrastructure` — EF Core `ApplicationDbContext`, repositories, migrations
- `UsalClinic.Tests` — unit tests

Prerequisites

- .NET SDK 7.0+ — check with `dotnet --version`
- A database (SQL Server or other EF Core provider)
- `dotnet-ef` CLI for migrations: `dotnet tool install --global dotnet-ef`

Quick start

1. Clone the repo and open the solution folder.
2. Set your DB connection string in `UsalClinic.Api/appsettings.json` or `UsalClinic.Web/appsettings.json` (key: `ConnectionStrings:DefaultConnection`).
3. Apply EF Core migrations to create the database:

   ```sh
   # from repo root
   dotnet ef database update --project UsalClinic.Infrastructure --startup-project UsalClinic.Api
   ```

4. Build & run:

   ```sh
   dotnet build
   dotnet run --project UsalClinic.Api   # run API
   dotnet run --project UsalClinic.Web   # run Web UI
   ```

5. Run tests:

   ```sh
   dotnet test UsalClinic.Tests
   ```

Configuration

- Use `appsettings.Development.json` for development overrides.
- Keep secrets out of source — use `dotnet user-secrets` or environment variables.

Development notes

- EF Core migrations: `UsalClinic.Infrastructure/Data/Migrations`
- To add a migration:

  ```sh
  dotnet ef migrations add <Name> --project UsalClinic.Infrastructure --startup-project UsalClinic.Api
  dotnet ef database update --project UsalClinic.Infrastructure --startup-project UsalClinic.Api
  ```

- Keep business logic in `UsalClinic.Application`, data access in `UsalClinic.Infrastructure`.

Troubleshooting

- Ensure the startup project exists and is runnable when using `dotnet ef`.
- If migrations fail, verify the connection string and DB server reachability.

Contributing

Contributions welcome — open an issue or PR with details.

License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

