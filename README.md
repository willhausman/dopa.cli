# DOPA.Cli

[![Build status](https://github.com/willhausman/dopa.cli/actions/workflows/build.yml/badge.svg "Build status")](https://github.com/willhausman/dopa.cli/actions/workflows/build.yml)
[![Latest version](https://img.shields.io/nuget/v/DOPA.Cli)](https://www.nuget.org/packages/DOPA.Cli)

A dotnet wrapper on the [OPA](https://www.openpolicyagent.org) binaries designed primarily for [DOPA](https://github.com/willhausman/dopa) to create WebAssembly bundles on the fly.

## Usage

`opa` commands are made available off the static `Opa` class. Args are added with a fluent syntax, and then run with `Execute()` or `ExecuteAsync()`.

This document refers to the test files found in [test/DOPA.Cli.Tests/policies](test/DOPA.Cli.Tests/policies).

### Implemented commands

DOPA.Cli is not a complete replacement for running `opa`, but it can help simplfy some automation. If you have a need for another arg, or command, PRs are welcome. Priority will be given to features needed by [DOPA](https://github.com/willhausman/dopa).

Currently, the following commands are implemented with a subset of the possible args.

#### Build

The [build](https://www.openpolicyagent.org/docs/latest/cli/#opa-build) command is supported with optional [capabilities](https://www.openpolicyagent.org/docs/latest/cli/#capabilities), and the ability to build a WebAssembly module.

The `Build` command returns a disposable `Bundle`. A `Bundle` contains the path to the generated opa bundle, and can extract contents from the bundle. The underlying file will be deleted when the `Bundle` is disposed.

```csharp
// Default
using Bundle bundle = Opa.Build.Files("policies/example.rego").Execute();
using Stream dataJson = bundle.ExtractData();

// WebAssembly
using Bundle bundle = await Opa.Build
            .WebAssembly()
            .Files("policies/builtins.rego")
            .Entrypoints("builtins/firstValue")
            .Capabilities("policies/builtins.capabilities.json")
            .ExecuteAsync();
using Stream wasm = bundle.ExtractWebAssemblyModule();
```

#### Test

The [test](https://www.openpolicyagent.org/docs/latest/cli/#opa-test) command is supported with files. `OpaTestsFailedException` is thrown if any tests fail. The exception message contains the details about which test failed.

```csharp
var results = Opa.Test.Files("policies/api.rego").Execute();
results.Trim().Should().BeEquivalentTo("PASS: 4/4");
```

## Versioning

DOPA.Cli uses a `Major.Minor.Patch.Revision` versioning scheme, where `Major.Minor.Patch` match the corresponding `Major.Minor.Patch` of the `opa` binary used to run commands. e.g. `DOPA.Cli@0.43.0.12` equates to revision 12 of `opa@0.43.0`.

## Contributing

Pull requests and issues are appreciated and encouraged.

## License

DOPA is licensed under the [MIT License](./LICENSE).
