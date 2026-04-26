
<h1 align="center">
  <br>
  HttpApiDemo — <code>minimal-api</code> branch
  <br>
</h1>

<h4 align="center">Minimal API endpoints alongside traditional controllers, with Swashbuckle + Swagger UI</h4>

<div align="center">

[![](https://img.shields.io/github/actions/workflow/status/dennisdoomen/HttpApiDemo/build.yml?branch=minimal-api)](https://github.com/dennisdoomen/HttpApiDemo/actions?query=branch%3Aminimal-api)
[![](https://img.shields.io/github/last-commit/dennisdoomen/HttpApiDemo)](https://github.com/dennisdoomen/HttpApiDemo)
[![open issues](https://img.shields.io/github/issues/dennisdoomen/HttpApiDemo)](https://github.com/dennisdoomen/HttpApiDemo/issues)
![Static Badge](https://img.shields.io/badge/net10.0-dummy?label=dotnet&color=%235027d5)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://makeapullrequest.com)

</div>

> [!NOTE]
> This is a feature branch. For full documentation, project background, API endpoint reference, and the list of all branches, see the **[README on the default branch](https://github.com/dennisdoomen/HttpApiDemo/blob/use-scalar/README.md)**.

## About this branch

**`minimal-api`** demonstrates how to mix ASP.NET Core [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) endpoints with traditional MVC controllers in the same project, while keeping [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) + Swagger UI as the OpenAPI toolchain.

## Building

Requirements:

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
* Visual Studio, JetBrains Rider, or VS Code with the C# DevKit

```
build.ps1
```

Or with the [Nuke global tool](https://nuke.build/docs/getting-started/installation/):

```
nuke
```

Use ``--help`` to see all available targets, or ``--plan`` to visualise the dependency graph.

To accept updated API surface snapshots after an intentional public API change, run:

```
AcceptApiChanges.ps1
```

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
