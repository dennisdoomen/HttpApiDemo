# HttpApiDemo – Agent Guide

## What this project is

HttpApiDemo is a reference ASP.NET Core Web API on **.NET 10** that demonstrates different approaches to OpenAPI documentation, API versioning, and endpoint organisation. The domain is a simple NuGet-style package registry; it is intentionally straightforward so the focus stays on infrastructure patterns.

**This branch (`use-scalar`)** uses the .NET 10 built-in OpenAPI middleware (`Microsoft.AspNetCore.OpenApi`) together with [Scalar](https://scalar.com/) for the interactive documentation UI. No Swashbuckle is involved.

## Repository layout

```
HttpApiDemo/                    Main web API project (net10.0, SDK Web)
  Infrastructure/
    SwaggerGenerationExtensions.cs  OpenAPI + Scalar wiring
  PackageController.cs            REST endpoints (v1 deprecated, v2 current)
  PackageRepository.cs            In-memory package store
  Program.cs                      Startup / DI composition root

HttpApiDemo.Specs/              Integration tests (xUnit, FluentAssertions)
  HttpApiDemoSpecs.cs             HTTP-level specs via WebApplicationFactory

HttpApiDemo.ApiVerificationTests/  Public API surface snapshot tests
  ApiApproval.cs                  Uses PublicApiGenerator + Verify

Build/
  Build.cs                        Nuke build definition
  Configuration.cs                Build configuration enum

.github/workflows/build.yml       CI pipeline (GitHub Actions, Windows runner)
Directory.Build.props             Shared MSBuild props and Roslyn analyzers
AcceptApiChanges.ps1 / .sh        Helper to accept updated API snapshots
```

## Branches at a glance

| Branch | OpenAPI generation | UI |
|---|---|---|
| `use-scalar` *(this)* | .NET 10 built-in (`Microsoft.AspNetCore.OpenApi`) | Scalar |
| `use-swashbuckle` | Swashbuckle | Swagger UI |
| `use-redoc` | Swashbuckle | ReDoc (one page per version group) |
| `minimal-api` | Swashbuckle | Swagger UI + Minimal API endpoints alongside controllers |

## Tech stack

| Concern | Library / Tool |
|---|---|
| Framework | ASP.NET Core 10 |
| OpenAPI generation | `Microsoft.AspNetCore.OpenApi` 10.x |
| OpenAPI UI | `Scalar.AspNetCore` |
| API versioning | `Asp.Versioning.Mvc.ApiExplorer` |
| Unit / integration tests | xUnit + FluentAssertions + `Microsoft.AspNetCore.Mvc.Testing` |
| API surface snapshots | `PublicApiGenerator` + `Verify.Xunit` + `Verify.DiffPlex` |
| Code coverage | Coverlet (cobertura) + ReportGenerator (lcov + HTML) |
| Build automation | Nuke |
| Versioning | GitVersion (Semantic Versioning) |
| Static analysis | StyleCop, Roslynator, CSharpGuidelinesAnalyzer, Meziantou.Analyzer |
| OpenTelemetry | `OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Instrumentation.AspNetCore`, `OpenTelemetry.Instrumentation.Http`, `OpenTelemetry.Instrumentation.Runtime`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`, `OpenTelemetry.Exporter.Console` |

## Key conventions

* **API versioning**: URL path segment (`/api/v{version}/`) is primary; query string (`?api-version=`) is supported as a fallback. Default version is `2.0`. Unrecognised versions return `404`.
* **Endpoint groups**: Each endpoint declares a `GroupName` of `public`, `internal`, or `private`. Each group is exposed as a separate OpenAPI document at `/api-docs/open-api-{groupName}-v{version}.json`.
* **Scalar route**: The interactive UI is served at `/scalar/{groupName}` for each discovered API version description.
* **Problem Details**: All error responses use RFC 9457 problem details. The exception handler middleware is enabled globally.
* **Enums**: Always serialised as their string name (configured via `JsonStringEnumConverter`).
* **Warnings as errors**: All projects build with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`. Roslyn analysers run only on the `net10.0` TFM to keep incremental build times low.

## Build targets (Nuke)

Run `build.ps1 --plan` or `nuke --plan` to see the full dependency graph. Key targets:

| Target | What it does |
|---|---|
| `Restore` | `dotnet restore` (no cache) |
| `Compile` | `dotnet build`, stamps assembly and file versions from GitVersion |
| `RunInspectCode` | JetBrains `inspectcode` → `Artifacts/CodeIssues.sarif` |
| `RunTests` | Runs `HttpApiDemo.Specs` with code coverage (cobertura format) |
| `ApiChecks` | Runs `HttpApiDemo.ApiVerificationTests` (snapshot verification) |
| `ScanPackages` | PackageGuard license/vulnerability scan |
| `GenerateCodeCoverageReport` | Produces lcov + HTML report under `Artifacts/TestResults/reports/` |
| `Pack` | `dotnet pack` → `Artifacts/` |
| `Push` | Publishes to NuGet (only on git tags) |
| `Default` | `Pack` + `Push` |

## How to run locally

```powershell
dotnet run --project HttpApiDemo
```

Then open `https://localhost:{port}/scalar/v1` (or whichever port is in `launchSettings.json`) for the interactive docs.

## Accepting API surface changes

After an intentional public API change, regenerate the snapshot:

```powershell
./AcceptApiChanges.ps1
```

This runs the verification tests with `VERIFY_AUTOACCEPT=1` which overwrites the `.verified.txt` files. Commit the updated snapshots alongside the code change.

## What to watch out for when making changes

* **OpenTelemetry wiring** lives in `HttpApiDemo/Telemetry/OpenTelemetryConfiguration.cs`. It registers tracing (ASP.NET Core + HTTP client instrumentation), metrics (+ .NET runtime metrics), and OpenTelemetry logging. Set `OpenTelemetry:Endpoint` in configuration to enable the OTLP exporter; the console exporter is active automatically in `Development`.
* **Health check** is a simple liveness check (`/health`). To add custom checks, extend `ServiceExtensions.AddHealthChecking` in `HealthChecking/ServiceExtensions.cs`.
* **OpenAPI wiring** lives entirely in `HttpApiDemo/Infrastructure/SwaggerGenerationExtensions.cs`. The `AddOpenApi` extension registers one OpenAPI document per API version group; the `UseOpenApiUi` extension mounts both the JSON endpoints and Scalar.
* **Adding a new endpoint group** requires adding a new `builder.Services.AddOpenApi(groupName, …)` call and a corresponding `options.AddDocument(…)` call in the Scalar configuration.
* **Version deprecation** must be reflected both in the `[ApiVersion("x.y", Deprecated = true)]` attribute on the controller and in the Scalar `operation.Deprecated` transformer (already wired up generically).
* **Analyzer conditions** in `Directory.Build.props` gate all Roslyn analysers on `'$(TargetFramework)' == 'net10.0'` — keep this in sync if the TFM changes.
