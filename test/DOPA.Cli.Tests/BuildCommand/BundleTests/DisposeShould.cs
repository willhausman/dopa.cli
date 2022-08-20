using FluentAssertions;
using Xunit;

namespace DOPA.Cli.Tests.BuildCommand.BundleTests;

public class DisposeShould
{
    [Fact]
    public void DeleteTheCreatedFile()
    {
        var filePath = Path.GetTempFileName();
        var bundle = new Bundle(filePath);

        File.Exists(filePath).Should().BeTrue();

        bundle.Dispose();

        File.Exists(filePath).Should().BeFalse();
    }
}
