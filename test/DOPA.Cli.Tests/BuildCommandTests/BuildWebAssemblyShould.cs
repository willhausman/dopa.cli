using Xunit;
using FluentAssertions;

namespace DOPA.Cli.Tests.BuildCommandTests;

public class BuildWebAssemblyShould
{
    [Fact]
    public async Task ReturnSimpleModule()
    {
        using var bundle = await Opa.Build
            .WebAssembly()
            .Files("policies/example.rego")
            .Entrypoints("example/hello")
            .ExecuteAsync();
        using var stream = bundle.ExtractWebAssemblyModule();
        using var module = WasmModule.FromStream("example", stream);
        using var policy = module.CreatePolicy();
        policy.SetData(new { world = "hello" });

        var result = policy.Evaluate<bool>(new { message = "hello" });
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnModuleWithCapabilities()
    {
        using var bundle = await Opa.Build
            .WebAssembly()
            .Files("policies/builtins.rego")
            .Entrypoints("builtins/firstValue")
            .Capabilities("policies/builtins.capabilities.json")
            .ExecuteAsync();
        using var stream = bundle.ExtractWebAssemblyModule();
        using var module = WasmModule.FromStream("builtins", stream);
        using var policy = module.CreatePolicy();
        policy.AddBuiltin("custom.builtin0", () => 1);

        var result = policy.Evaluate<int>();
        result.Should().Be(1);
    }

    [Fact]
    public void PropagateExceptions()
    {
        Action act = () => Opa.Build.WebAssembly().Files("policies/example.rego").Execute();

        act.Should().Throw<OpaCliException>().WithMessage("*wasm compilation requires at least one entrypoint*");
    }
}
