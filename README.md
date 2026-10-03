# StudySphere
StudySphere is online learning platform project. We are building it using C# and .net concepts with real life problem solving functionality.

## Authentication setup

The application uses ASP.NET Core Identity for account registration, sign-in,
sign-out, and role-based authorization. Apply the database migrations before
running the application:

```powershell
cd StudySphere\StudySphere
dotnet ef database update
dotnet run
```

Students and instructors can register from the sign-in page. Administrator
registration is intentionally disabled. To provision an initial administrator,
set `BootstrapAdmin__Email` and `BootstrapAdmin__Password` in the environment
or a secrets provider before starting the application. The account is created
with the `Admin` role on startup; no administrator credentials are stored in
the repository.
