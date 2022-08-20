using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace DOPA.Cli.Tests.BuildCommand.BundleTests;

public class ExtractFileShould
{
    [Fact]
    public async Task ExtractThePolicyWasm()
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
    public async Task ThrowWhenThereIsNoPolicyWasm()
    {
        using var bundle = await Opa.Build
            .Files("policies/example.rego")
            .Entrypoints("example/hello")
            .ExecuteAsync();
        Action act = () => bundle.ExtractWebAssemblyModule();
        
        act.Should().Throw<ArgumentException>().WithMessage("*policy.wasm*");
    }

    [Fact]
    public async Task ExtractTheDataFile()
    {
        using var bundle = await Opa.Build
            .Files("policies/example.rego")
            .Entrypoints("example/hello")
            .ExecuteAsync();
        using var stream = bundle.ExtractData();

        using var reader = new StreamReader( stream );
        var data = await reader.ReadToEndAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(data);

        // the data json on this policy is just {}. Avoiding string abnormalities by converting to an empty dictionary
        var expected = new Dictionary<string, string>();

        result.Should().BeEquivalentTo(expected);
    }
}
