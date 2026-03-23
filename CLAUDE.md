# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

EvalPro is a German-language WPF desktop application for grading oral IHK (Industrie- und Handelskammer) exams. It manages audit committees, examinees, and their ratings across four categories: project documentation, project presentation, tech conversation, and supplementary examination.

## Build & Test Commands

```bash
# Build entire solution
dotnet build EvalPro.sln

# Build individual projects
dotnet build EvalPro.Service/EvalPro.Service.csproj
dotnet build EvalPro.UI/EvalPro.UI.csproj

# Run all tests
dotnet test EvalPro.Service.Test/EvalPro.Service.Test.csproj

# Run a single test by name
dotnet test EvalPro.Service.Test/EvalPro.Service.Test.csproj --filter "FullyQualifiedName~TestMethodName"

# Run tests in a specific class
dotnet test EvalPro.Service.Test/EvalPro.Service.Test.csproj --filter "FullyQualifiedName~ClassName"
```

## Architecture

Three-project .NET 10 solution:

- **EvalPro.Service** — Backend library (namespace `EvalProService`). Contains domain model, business logic, and JSON-file persistence.
- **EvalPro.Service.Test** — xUnit test project (namespace `EvalProServiceTest`).
- **EvalPro.UI** — WPF frontend (namespace `EvalProUI`, targets `net10.0-windows`). Cannot be built/run on macOS.

### Backend Layering

`IEvalProServiceApi` (single API interface) → `EvalProService` (service implementation with thread-safe data store using `Lock`) → `config.json` (flat-file persistence)

- **Entities** inherit from `BaseEntity` (Id, CreatedAt, UpdatedAt). Two entity types: `AuditCommittee` and `Examinee`.
- **Ratings** inherit from `BaseRating` (PointsPerCriteria, CommentsPerCriteria, FinalComment). Four types: `ProjectDocumentation`, `ProjectPresentation`, `TechConversation`, `SupplementaryExamination`.
- Relationships use direct object references. `AuditCommittee.Examinee` holds an examinee; `Examinee` has four nullable rating properties.
- `AutoDataSaver` persists to `config.json` every 500ms via timer and does a final save on dispose. Save errors are surfaced via `OnSaveError` event. Takes an `Action` delegate for the save operation.
- All data operations are guarded by a `Lock` for thread safety within `EvalProService`.

### UI Pattern

WPF with XAML views in `src/views/` and code-behind controllers in `src/controllers/`. The UI project references no backend project — integration is pending.

## Key Conventions

- Logging uses Serilog with structured logging (console + rolling file `evalpro.log`).
- Tests use xUnit with `Microsoft.Extensions.Logging.Abstractions` (NullLogger) for the logger dependency.
- The service class is the sole entry point for all business operations.
