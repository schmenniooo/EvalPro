# EvalPro

The professional grading tool for IHK exams.

EvalPro is a WPF desktop application for managing and grading oral exams administered by the German Chamber of Commerce and Industry (IHK). It handles audit committees, examinees, and structured grading across four categories:

- **Project Documentation** — Evaluation of the written documentation
- **Project Presentation** — Evaluation of the oral presentation
- **Technical Conversation** — Evaluation of the technical discussion
- **Supplementary Examination** — Additional oral examination

## Installation

1. Go to the [Releases](../../releases) page
2. Download `App.zip` from the latest release
3. Extract the ZIP to a folder of your choice
4. Run `EvalPro.UI.exe`

> **Note:** Windows is required to run EvalPro.

## Development

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Windows (required for the WPF frontend)

### Build

```bash
# Clone the repository
git clone https://github.com/<your-org>/evalpro.git
cd evalpro

# Build the solution
dotnet build EvalPro.sln

# Run the application (Windows only)
dotnet run --project EvalPro.UI/EvalPro.UI.csproj
```

### Running Tests

```bash
# Run all tests
dotnet test EvalPro.Service.Test/EvalPro.Service.Test.csproj

# Run a single test
dotnet test EvalPro.Service.Test/EvalPro.Service.Test.csproj --filter "FullyQualifiedName~TestMethodName"
```

### CI/CD

GitHub Actions pipeline:
1. **Build & Test** — Runs automatically on pushes and pull requests
2. **Release** — Creates a ZIP artifact and GitHub Release on push to `main`

## License

School project — All rights reserved.