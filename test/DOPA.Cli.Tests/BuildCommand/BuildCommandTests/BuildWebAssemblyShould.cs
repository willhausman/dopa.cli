using Xunit;
using FluentAssertions;

namespace DOPA.Cli.Tests.BuildCommand.BuildCommandTests;

public class BuildWebAssemblyShould
{
    [Fact]
    public async Task BuildAUsableBundle()
    {
        using var bundle = await Opa.Build
            .WebAssembly()
            .Files("policies/example.rego")
            .Entrypoints("example/hello")
            .ExecuteAsync();
        using var stream = bundle.ExtractWebAssemblyModule();

        stream.Length.Should().NotBe(0);
    }

    [Fact]
    public void RequireAnEntrypoint()
    {
        Action act = () => Opa.Build.WebAssembly().Files("policies/example.rego").Execute();

        act.Should().Throw<OpaCliException>().WithMessage("*wasm compilation requires at least one entrypoint*");
    }

    [Fact]
    public async Task RequireAFile()
    {
        Func<Task> act = () => Opa.Build
            .WebAssembly()
            .ExecuteAsync();
        
        await act.Should().ThrowAsync<OpaCliException>().WithMessage("*at least one path*");
    }
}
