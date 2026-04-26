
<h1 align="center">
  <br>
  HttpApiDemo
  <br>
</h1>


<h4 align="center">A reference implementation for building .NET HTTP APIs with OpenAPI documentation</h4>


<div align="center">

[![](https://img.shields.io/github/actions/workflow/status/dennisdoomen/HttpApiDemo/build.yml?branch=use-scalar)](https://github.com/dennisdoomen/HttpApiDemo/actions?query=branch%3Ause-scalar)
[![](https://img.shields.io/github/last-commit/dennisdoomen/HttpApiDemo)](https://github.com/dennisdoomen/HttpApiDemo)
[![GitHub contributors](https://img.shields.io/github/contributors/dennisdoomen/HttpApiDemo)](https://github.com/dennisdoomen/HttpApiDemo/graphs/contributors)
[![open issues](https://img.shields.io/github/issues/dennisdoomen/HttpApiDemo)](https://github.com/dennisdoomen/HttpApiDemo/issues)
![Static Badge](https://img.shields.io/badge/net10.0-dummy?label=dotnet&color=%235027d5)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://makeapullrequest.com)

<a href="#about">About</a> •
<a href="#branches">Branches</a> •
<a href="#api-endpoints">API Endpoints</a> •
<a href="#building">Building</a> •
<a href="#contributing">Contributing</a> •
<a href="#credits">Credits</a> •
<a href="#license">License</a>

</div>


## About

HttpApiDemo is a reference .NET 10 ASP.NET Core Web API that demonstrates different approaches to hosting OpenAPI documentation alongside a versioned HTTP API. The domain is a simple NuGet-style package registry that exposes typical CRUD, pagination, async-upload, and statistics endpoints — kept intentionally simple so the focus stays on the infrastructure patterns.

**This branch (`use-scalar`)** uses the .NET 10 built-in OpenAPI middleware (`Microsoft.AspNetCore.OpenApi`) together with [Scalar](https://scalar.com/) as the interactive API documentation UI. No Swashbuckle required.

Patterns demonstrated across the project:

* API versioning (URL path + query string) via `Asp.Versioning`, including a deprecated v1 alongside a current v2
* Endpoint grouping (`public`, `internal`, `private`) surfaced as separate OpenAPI documents
* Problem Details for error responses (`RFC 9457`)
* Health check endpoint (`/health`) wired to Application Insights
* CORS configuration
* Application Insights integration (non-development environments only)
* Enum serialization to string names
* API surface contract verification via snapshot tests (`PublicApiGenerator` + `Verify`)
* Code coverage collection with Coverlet and HTML/lcov report generation


## Branches

Each branch explores a different OpenAPI toolchain or API style, using the same domain and project structure:

| Branch | OpenAPI generation | UI | Notes |
|---|---|---|---|
| **`use-scalar`** *(this branch)* | .NET 10 built-in (`Microsoft.AspNetCore.OpenApi`) | [Scalar](https://scalar.com/) | No Swashbuckle; native .NET 10 OpenAPI pipeline |
| `use-swashbuckle` | [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | Swagger UI | Security definitions (Bearer + Basic), XML doc comments, custom operation filters |
| `use-redoc` | [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | [ReDoc](https://github.com/Redocly/redoc) | A separate ReDoc page per API version/group |
| `minimal-api` | [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | Swagger UI | Minimal API endpoints alongside traditional controllers |


## API Endpoints

The package registry API (base path `/api/v{version}/packages`) exposes:

| Method | Route | Version | Group | Description |
|---|---|---|---|---|
| `GET` | `/` | v2 | public | List packages (paginated via `$skip`/`$take`) |
| `GET` | `/{packageId}` | v2 | internal | Get package details with full version metadata |
| `GET` | `/{packageId}` | v1 *(deprecated)* | internal | Get package details (limited version summary) |
| `GET` | `/{packageId}/statistics` | v2 | private | Get download statistics |
| `PUT` | `/{packageId}` | v2 | private | Create or replace a package (idempotent) |
| `PATCH` | `/{packageId}` | v2 | private | Partially update a package |
| `DELETE` | `/{packageId}` | v2 | private | Delete a package (idempotent) |
| `POST` | `/` | v2 | private | Upload package data (async, returns `202 Accepted`) |
| `GET` | `/status/{pendingId}` | v2 | private | Poll the status of a pending upload |

Additional endpoints:

| Route | Description |
|---|---|
| `/health` | Health check (integrates with Application Insights) |
| `/scalar/{groupName}` | Scalar interactive API documentation UI |
| `/api-docs/open-api-{documentName}.json` | Raw OpenAPI JSON per document |


## Building

Requirements:

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
* Visual Studio, JetBrains Rider, or VS Code with the C# DevKit

Build, test, and package using the Nuke-based build script:

```
build.ps1
```

Or, if you have the [Nuke global tool](https://nuke.build/docs/getting-started/installation/) installed:

```
nuke
```

Use `--help` to see all available targets, or `--plan` to visualise the dependency graph before running.

To accept updated API surface snapshots after an intentional public API change, run:

```
AcceptApiChanges.ps1
```


## Contributing

Your contributions are always welcome! Please have a look at the [contribution guidelines](CONTRIBUTING.md) first.

<a href="https://github.com/dennisdoomen/HttpApiDemo/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=dennisdoomen/HttpApiDemo" alt="contrib.rocks image" />
</a>

(Made with [contrib.rocks](https://contrib.rocks))


## Credits

This project wouldn't be possible without the following tools and packages:

* [Scalar](https://scalar.com/) — Beautiful, modern API documentation UI
* [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) — API versioning for ASP.NET Core
* [Nuke](https://nuke.build/) — Smart automation for DevOps teams by [Matthias Koch](https://github.com/matkoch)
* [xUnit](https://xunit.net/) — Community-focused unit testing tool for .NET by [Brad Wilson](https://github.com/bradwilson)
* [Coverlet](https://github.com/coverlet-coverage/coverlet) — Cross-platform code coverage for .NET
* [PublicApiGenerator](https://github.com/PublicApiGenerator/PublicApiGenerator) — Generate a text representation of a public API surface
* [Verify](https://github.com/VerifyTests/Verify) — Snapshot testing by [Simon Cropp](https://github.com/SimonCropp)
* [FluentAssertions](https://fluentassertions.com/) — Natural language assertion library for .NET
* [GitVersion](https://gitversion.net/) — Semantic versioning from git history
* [ReportGenerator](https://reportgenerator.io/) — Coverage report generation by [Daniel Palme](https://github.com/danielpalme)
* [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) — StyleCop rules for .NET
* [Roslynator](https://github.com/dotnet/roslynator) — Roslyn-based code analysis by [Josef Pihrt](https://github.com/josefpihrt)
* [CSharpCodingGuidelines](https://github.com/bkoelman/CSharpGuidelinesAnalyzer) — Roslyn analyzers by [Bart Koelman](https://github.com/bkoelman)
* [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Framework) — Additional Roslyn analyzers by [Gérald Barré](https://github.com/meziantou)
* [PackageGuard](https://github.com/your-username/packageguard) — Open-source license and vulnerability scanning


## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
