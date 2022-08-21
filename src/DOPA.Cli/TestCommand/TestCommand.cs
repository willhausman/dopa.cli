namespace DOPA.Cli;

public sealed record TestCommand
{
    private List<string> Arguments { get; init; }

    internal TestCommand()
    {
        Arguments = new() { "test" };
    }

    public TestCommand Files(params string[] filePaths) => this with { Arguments = Arguments.Union(filePaths).ToList() };

    public string Execute()
    {
        try
        {
            using var p = Opa.Execute(Arguments);
            return p.StandardOutput.ReadToEnd();
        }
        catch (OpaCliException e)
        {
            if (e.StandardOutput.Contains("FAIL:", StringComparison.Ordinal))
            {
                throw new OpaTestsFailedException(e.StandardOutput);
            }
            throw;
        }
    }

    public Task<string> ExecuteAsync()
    {
        try
        {
            using var p = Opa.Execute(Arguments);
            return p.StandardOutput.ReadToEndAsync();
        }
        catch (OpaCliException e)
        {
            if (e.StandardOutput.Contains("FAIL:"))
            {
                throw new OpaTestsFailedException(e.StandardOutput);
            }
            throw;
        }
    }
}
