# MCQ Examination System

ASP.NET Core 10 Razor Pages MCQ Examination System generated from the requirement markdown files in this workspace. The UI uses the local `adminHMD` Bootstrap 5 template assets for layouts, forms, tables, cards, auth pages, and dashboard styling.

## Stack

- ASP.NET Core 10 Razor Pages
- MongoDB with `MongoDB.Driver`
- Clean Architecture
- Repository Pattern and Unit of Work abstraction
- Cookie authentication with Identity password hashing
- Role-based authorization
- AutoMapper
- FluentValidation
- Bootstrap 5 and Bootstrap Icons from `adminHMD`

## Structure

```text
src/
  Domain/          Entities, enums, base entity
  Application/     DTOs, interfaces, services, validation, mapping
  Persistence/     MongoDB context, repositories, seed data
  Infrastructure/  Password hashing, email/file/date services
  Shared/          Role and policy constants
  WebUI/           Razor Pages, AdminHMD assets, auth, admin/student UI
tests/
  MCQSystem.Tests/
```

## Seed Accounts

MongoDB is seeded on startup.

| Role | Email | Password |
| --- | --- | --- |
| Admin | `admin@mcq.local` | `Admin@12345` |
| Student | `student@mcq.local` | `Student@12345` |

## Run

MongoDB must be running before you start the web app. The default connection is `mongodb://localhost:27017`.

### Start MongoDB (Windows)

**Option A — helper script (service, local mongod, or Docker):**

```powershell
.\scripts\Start-MongoDB.ps1
```

**Option B — install MongoDB Community Server**

1. Download: https://www.mongodb.com/try/download/community  
2. Install with “Install MongoDB as a Service” checked.  
3. Confirm port `27017` is listening, then run the app.

**Option C — Docker**

```powershell
docker compose up -d
```

**Option D — MongoDB Atlas (cloud)**

Set your connection string in `src/WebUI/appsettings.Development.json`:

```json
"MongoDb": {
  "ConnectionString": "mongodb+srv://<user>:<password>@<cluster>.mongodb.net/?retryWrites=true&w=majority",
  "DatabaseName": "MCQSystemDevDb"
}
```

### Run the app

```powershell
dotnet restore .\MCQSystem.slnx
dotnet build .\MCQSystem.slnx
dotnet run --project .\src\WebUI\MCQSystem.WebUI.csproj
```

If you see `actively refused` on port `27017`, MongoDB is not running — use one of the options above first.

The default MongoDB settings are in `src/WebUI/appsettings.json`.

## Implemented Modules

- Admin login and dashboard
- Manage exams
- Manage questions with four options, correct answer, difficulty, marks, negative marks, explanation, and category
- Manage categories
- Manage users
- View and export results as CSV
- Analytics view
- Settings page
- Student registration/login
- Student profile
- Exam list
- Start exam
- Countdown timer with auto-submit
- Result calculation with negative marking
- Result view and history
- MCQ question navigation (previous/next) during exams
- SweetAlert2 confirmations and toast notifications
- Admin dashboard charts and recent activity feed
- Pagination on admin list pages
- Forgot password flow
- Student leaderboard by exam
- Admin add-user form
