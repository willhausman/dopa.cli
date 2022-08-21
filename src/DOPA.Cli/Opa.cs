using System.Diagnostics;
namespace DOPA.Cli;

public static class Opa
{
    private const int OK = 0;
    private static readonly string opaPath;

    static Opa()
    {
        opaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "opa");
    }

    public static BuildCommand Build => new();

    public static TestCommand Test => new();

    internal static Process Execute(IEnumerable<string> arguments)
    {
        var args = string.Join(' ', arguments);
        var p = StartProcess(args);
        p.WaitForExit();

        if (p.ExitCode != OK)
        {
            var details = p.StandardOutput.ReadToEnd();
            var errorDetails = p.StandardError.ReadToEnd();
            p.Dispose();
            throw new OpaCliException(opaPath, args, details, errorDetails);
        }

        return p;
    }

    internal static async Task<Process> ExecuteAsync(IEnumerable<string> arguments)
    {
        var args = string.Join(' ', arguments);
        var p = StartProcess(args);
        await p.WaitForExitAsync();

        if (p.ExitCode != OK)
        {
            var details = await p.StandardOutput.ReadToEndAsync();
            var errorDetails = await p.StandardError.ReadToEndAsync();
            p.Dispose();
            throw new OpaCliException(opaPath, args, details, errorDetails);
        }

        return p;
    }

    private static Process StartProcess(string args)
    {
        var p = new Process
        {
            StartInfo = new(opaPath)
            {
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            },
        };

        p.Start();

        return p;
    }
}
