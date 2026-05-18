You are a senior ASP.NET Core architect and full-stack engineer.

I need a complete production-ready ASP.NET Core 8 Razor Pages MCQ Examination System using MongoDB and Clean Architecture.

The project must be generated from requirement markdown files such as:
- Features.md
- AdminHMD.md
- Flow.md
- Architecture.md

Read and follow all requirements from those markdown files before generating code.

====================================================
PROJECT REQUIREMENTS
====================================================

Technology Stack:
- ASP.NET Core 10 Razor Pages
- MongoDB
- Clean Architecture
- Repository Pattern
- Dependency Injection
- Bootstrap 5
- ASP.NET Core Identity
- AutoMapper
- FluentValidation
- MediatR (optional)
- LINQ
- Async/Await

====================================================
ARCHITECTURE
====================================================

Use proper Clean Architecture:

src/
 ├── WebUI
 ├── Application
 ├── Domain
 ├── Infrastructure
 ├── Persistence
 ├── Shared
 └── Tests

====================================================
LAYER RESPONSIBILITIES
====================================================

1. Domain Layer
- Entities
- Enums
- Interfaces
- BaseEntity
- Common Models

2. Application Layer
- DTOs
- Interfaces
- Services
- Validation
- Business Logic
- CQRS Structure
- Response Models

3. Infrastructure Layer
- Authentication
- External Services
- Email Service
- File Service

4. Persistence Layer
- MongoDB Context
- Repository Implementations
- Seed Data
- Database Configurations

5. WebUI Layer
- Razor Pages
- ViewModels
- Bootstrap UI
- JavaScript
- Authentication Pages
- Admin Dashboard

====================================================
MODULES
====================================================

ADMIN MODULE:
- Login
- Dashboard
- Manage Exams
- Manage Questions
- Manage Categories
- Manage Users
- View Results
- Analytics
- Search
- Pagination
- Export Result
- Settings

STUDENT MODULE:
- Register/Login
- Profile
- Exam List
- Start Exam
- MCQ Navigation
- Timer
- Submit Exam
- Result View
- History

====================================================
DATABASE DESIGN
====================================================

MongoDB Collections:
- Users
- Roles
- Exams
- Questions
- Categories
- StudentAnswers
- Results
- Settings

====================================================
QUESTION STRUCTURE
====================================================

Each question must support:
- Multiple Choice
- 4 Options
- Correct Answer
- Difficulty Level
- Marks
- Negative Marks
- Explanation
- Category

====================================================
FEATURES
====================================================

Core Features:
- Authentication & Authorization
- Role-based Access
- JWT-ready Structure
- Repository Pattern
- Generic Repository
- Unit of Work
- DTO Mapping
- Validation
- Exception Middleware
- Toast Notifications
- Logging
- Audit Trail
- Search & Filter
- Pagination
- Responsive Layout

Exam Features:
- Countdown Timer
- Auto Submit
- Random Questions
- Result Calculation
- Pass/Fail Logic
- Negative Marking
- Leaderboard
- Exam Analytics

====================================================
UI REQUIREMENTS
====================================================

Use:
- Bootstrap 5
- Responsive Admin Dashboard
- Sidebar Navigation
- Cards
- Charts
- Tables
- Modal Forms
- SweetAlert2
- Toast Notifications

Admin Dashboard Design:
- Modern ERP Style
- Clean Sidebar
- Top Navbar
- Statistics Cards
- Exam Analytics
- Recent Activities

====================================================
CODE QUALITY
====================================================

Requirements:
- SOLID Principles
- Clean Code
- Async Everywhere
- Reusable Components
- Proper Naming Conventions
- XML Comments
- Error Handling
- Logging
- Service Registration Extensions

====================================================
DELIVERABLES
====================================================

Generate:
1. Full Solution Structure
2. All Projects
3. Full Source Code
4. MongoDB Configuration
5. Program.cs Setup
6. Dependency Injection Setup
7. Authentication Setup
8. Role Seeder
9. Sample Data Seeder
10. Razor Pages UI
11. Admin Dashboard
12. CRUD Operations
13. Repository Layer
14. MongoDB Generic Repository
15. DTOs and Mapping
16. Validation Logic
17. Timer JavaScript
18. Result Calculation Logic
19. Responsive Design
20. README.md

====================================================
ADDITIONAL REQUIREMENTS
====================================================

- Use MongoDB.Driver
- Use IConfiguration for settings
- Store MongoDB settings in appsettings.json
- Use strongly typed settings
- Add Environment-based configuration
- Add middleware pipeline properly
- Add authorization policies
- Add repository interfaces
- Add seed admin account

====================================================
OUTPUT FORMAT
====================================================

Generate project step-by-step:

STEP 1:
Create Solution Structure

STEP 2:
Create Domain Layer

STEP 3:
Create Application Layer

STEP 4:
Create Persistence Layer

STEP 5:
Create Infrastructure Layer

STEP 6:
Create WebUI Layer

STEP 7:
Configure MongoDB

STEP 8:
Configure Authentication

STEP 9:
Build Admin Dashboard

STEP 10:
Build Exam Engine

STEP 11:
Build Result Engine

STEP 12:
Final Optimization

Provide complete code for every file.

Do not skip implementation details.

Use production-ready enterprise architecture.