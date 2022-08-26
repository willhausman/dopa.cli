using FluentAssertions;
using Xunit;

namespace DOPA.Cli.Tests.TestCommand.TestCommandTests;

public class TestShould : IDisposable
{
    private readonly string testRegoPath;

    public TestShould()
    {
        testRegoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
    }

    [Fact]
    public void RunAnyTestsInFile()
    {
        var results = Opa.Test.Files("policies/api.rego").Execute();

        results.Trim().Should().BeEquivalentTo("PASS: 4/4");
    }

    [Fact]
    public void RunTestsAcrossFiles()
    {
        CreateTestRego(@"
package test
default variable = true
test_variable {
    variable
}");
        var results = Opa.Test.Files("policies/api.rego", testRegoPath).Execute();

        results.Trim().Should().BeEquivalentTo("PASS: 5/5");
    }

    [Fact]
    public async Task ThrowOriginalErrorIfNotTestingRelated()
    {
        Func<Task> act = () => Opa.Test.Files("policies/").ExecuteAsync();

        await act.Should().ThrowAsync<OpaCliException>().WithMessage("*conflicting rule for data path*");
    }

    [Fact]
    public async Task ThrowTestException()
    {
        CreateTestRego(@"
package test
default variable = false
test_variable {
    variable
}");
        Func<Task> act = () => Opa.Test.Files(testRegoPath).ExecuteAsync();

        await act.Should().ThrowAsync<OpaTestsFailedException>().WithMessage("*test.test_variable: FAIL*");
    }

    public void Dispose() => File.Delete(testRegoPath);

    private void CreateTestRego(string rego)
    {
        using var stream = File.OpenWrite(testRegoPath);
        using var writer = new StreamWriter(stream);
        writer.Write(rego);
    }
}
