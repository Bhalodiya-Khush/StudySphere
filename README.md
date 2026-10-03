# StudySphere
StudySphere is online learning platform project. We are building it using C# and .net concepts with real life problem solving functionality.

## Run locally (Windows)

### Prerequisites

- .NET 10 SDK.
- SQL Server LocalDB (included with Visual Studio's data storage components) or a
  SQL Server instance whose connection string you can configure.
- The `dotnet-ef` command-line tool, version 10:

  ```powershell
  dotnet tool install --global dotnet-ef --version 10.0.12
  ```

  If already installed, update it with
  `dotnet tool update --global dotnet-ef --version 10.0.12`.

### 1. Configure and create the administrator

The default connection string in `StudySphere\StudySphere\appsettings.json`
targets LocalDB and creates/uses a database named `StudySphereDb`. Change the
`ConnectionStrings:DefaultConnection` value if using another SQL Server.

Admin registration is disabled. Set up a private .NET user-secrets store from
the application directory and choose your own administrator email and password:

```powershell
cd StudySphere\StudySphere
dotnet user-secrets init
dotnet user-secrets set "BootstrapAdmin:Email" "admin@example.com"
dotnet user-secrets set "BootstrapAdmin:Password" "<choose-a-strong-password>"
```

Replace the example values before running. Do not put real credentials in source
files or commit them. The application provisions this account with the `Admin`
role at startup.

### 2. Restore, migrate, and run

From `StudySphere\StudySphere`:

```powershell
dotnet restore
dotnet ef database update
dotnet build
dotnet run
```

`dotnet ef database update` applies all included migrations to the configured
database. Do not run it against a shared or production database without first
reviewing the migrations and backing up the data.

Open the HTTPS URL printed by `dotnet run`. Sign in with the admin credentials
configured above, then approve instructor accounts and course submissions from
the admin dashboard.

### 3. Try the learner and instructor workflows

- Register a student and enroll in an approved course to unlock its materials,
  assignments, quizzes, and live lectures.
- Register an instructor; an admin must activate the account before its first
  sign-in. Once active, the instructor can submit courses for admin approval,
  then manage course content and activities.
- Live video, microphone, and screen sharing require browser media permissions.
  Test using the HTTPS local URL.

Uploaded course files and assignment submissions are stored under the
application's `App_Data` directory; back up that directory along with the
database if retaining user uploads.
