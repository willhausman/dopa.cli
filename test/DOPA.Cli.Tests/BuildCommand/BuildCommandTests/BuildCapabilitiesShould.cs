using Xunit;
using FluentAssertions;

namespace DOPA.Cli.Tests.BuildCommand.BuildCommandTests;

public class BuildCapabilitiesShould
{
    [Fact]
    public async Task BuildAUsableBundleWithCustomFunctions()
    {
        using var bundle = await Opa.Build
            .WebAssembly()
            .Files("policies/builtins.rego")
            .Entrypoints("builtins")
            .Capabilities("policies/builtins.capabilities.json")
            .ExecuteAsync();
        using var stream = bundle.ExtractWebAssemblyModule();

        stream.Length.Should().NotBe(0);
    }

    [Fact]
    public async Task ThrowWhenCapabilitiesAreMissing()
    {
        Func<Task> act = () => Opa.Build.Files("policies/builtins.rego").ExecuteAsync();

        await act.Should().ThrowAsync<OpaCliException>().WithMessage("*undefined function custom.builtin0*");
    }
}
