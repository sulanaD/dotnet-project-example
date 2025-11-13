# dotnet-project-example

A demonstration .NET web application project showcasing modern ASP.NET Core development practices, build automation, and testing strategies. This repository serves as an example for .NET project structure, CI/CD pipelines, and release management.

## 🚀 Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web application framework
- **C#** - Primary programming language
- **Bootstrap 5** - CSS framework for responsive design
- **jQuery** - JavaScript library for DOM manipulation
- **jQuery Validation** - Client-side form validation
- **MSTest** - Unit testing framework
- **Visual Studio** - IDE support via solution file

## 📋 Prerequisites

Before running this project, ensure you have:

- **.NET 9.0 SDK** or later installed
  - Download from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
  - Or use the included `dotnet-install.sh` script: `./dotnet-install.sh --channel 9.0`
- **Git** for version control
- **Visual Studio 2022**, **Visual Studio Code**, or any compatible IDE (optional)

## 🏃‍♂️ Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/danhellem/dotnet-project-example.git
cd dotnet-project-example
```

### 2. Install .NET 9.0 (if needed)
```bash
# Use the included installation script
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0

# Add to PATH (Linux/macOS)
export PATH="$HOME/.dotnet:$PATH"
```

### 3. Restore Dependencies
```bash
dotnet restore
```

### 4. Build the Project
```bash
dotnet build
```

### 5. Run the Web Application
```bash
# Navigate to the web app directory
cd example-web-app

# Run the application
dotnet run

# Or run from solution root
dotnet run --project example-web-app
```

The application will start and be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

## 🧪 Running Tests

Execute the unit tests using:

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests from specific project
dotnet test example-web-app-tests
```

## 📁 Project Structure

```
dotnet-project-example/
├── example-web-app/              # Main web application
│   ├── Controllers/              # MVC Controllers
│   ├── Models/                   # Data models and view models
│   ├── Views/                    # Razor views and layouts
│   ├── wwwroot/                  # Static files (CSS, JS, images)
│   ├── Program.cs                # Application entry point
│   └── example-web-app.csproj    # Project file
├── example-web-app-tests/        # Unit tests
│   ├── ExampleTests.cs           # Test cases
│   └── example-web-app-tests.csproj
├── example-web-app.sln           # Visual Studio solution file
├── dotnet-install.sh             # .NET SDK installation script
└── README.md                     # This file
```

## 🛠️ Development

### Local Development
1. Open the solution in Visual Studio or your preferred IDE
2. Set `example-web-app` as the startup project
3. Press F5 or use `dotnet run` to start debugging

### Adding New Features
1. Create new controllers in `example-web-app/Controllers/`
2. Add corresponding views in `example-web-app/Views/`
3. Add unit tests in `example-web-app-tests/`
4. Update models in `example-web-app/Models/` as needed

### Building for Production
```bash
# Build in Release mode
dotnet build --configuration Release

# Publish for deployment
dotnet publish --configuration Release --output ./publish
```

## 🔄 CI/CD Pipeline

This project includes a comprehensive CI/CD pipeline using **GitHub Actions** with the following features:

### 🎯 Pipeline Overview

**Main CI Pipeline** (`.github/workflows/ci.yml`):
- ✅ **Multi-target build** - Supports .NET 8.0
- 🧪 **Automated testing** - Runs all unit tests with MSTest
- 📊 **Code coverage** - Generates coverage reports with ReportGenerator
- 🔒 **Security scanning** - Checks for vulnerable dependencies
- 📈 **Quality gates** - Ensures all checks pass before success

**PR Validation** (`.github/workflows/pr-validation.yml`):
- 🎨 **Code formatting** - Validates consistent code style
- 📊 **Coverage threshold** - Enforces minimum 70% code coverage
- 🔒 **Security validation** - Blocks PRs with security vulnerabilities
- 💬 **Automated comments** - Posts validation results on PRs

### 🚀 Triggers

The CI pipeline runs on:
- **Push** to `main` or `develop` branches
- **Pull requests** targeting `main` or `develop` branches

### 📊 Pipeline Jobs

1. **Build & Test**
   - Restores NuGet packages
   - Builds in Release configuration
   - Runs unit tests with coverage collection
   - Uploads coverage reports to Codecov
   - Publishes test results

2. **Security Scan**
   - Scans for vulnerable dependencies
   - Fails pipeline if vulnerabilities found
   - Uploads security scan artifacts

3. **Quality Gate**
   - Ensures all previous jobs succeeded
   - Provides final pass/fail status

### 🛡️ Quality Standards

- **Minimum code coverage**: 70%
- **Security**: Zero vulnerable dependencies allowed
- **Code formatting**: Consistent style enforced
- **All tests must pass**: No failing tests allowed

### 📈 Monitoring & Reporting

- **Test results**: Published to GitHub with detailed reports
- **Coverage reports**: Available as downloadable artifacts
- **Security scans**: Results uploaded for review
- **PR feedback**: Automated comments with validation status

### 🔧 Local Quality Checks

Before pushing code, run these commands locally:

```bash
# Format code
dotnet format

# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Check for security vulnerabilities
dotnet list package --vulnerable --include-transitive

# Build in release mode
dotnet build --configuration Release
```

## 📝 License

This project is for demonstration purposes. Check individual component licenses in the `wwwroot/lib/` directories for third-party dependencies.

## 🤝 Contributing

This is an example project for testing and demonstration. Feel free to fork and experiment with different configurations and features.
