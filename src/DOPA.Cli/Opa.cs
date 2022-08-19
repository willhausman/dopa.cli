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

    public static BuildCommand Build => new BuildCommand();

    internal static Process Execute(IEnumerable<string> arguments)
    {
        var args = string.Join(' ', arguments);
        var p = StartProcess(args);
        p.WaitForExit();

        if (p.ExitCode != OK)
        {
            var details = p.StandardOutput.ReadToEnd();
            throw new OpaCliException(opaPath, args, details);
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
            throw new OpaCliException(opaPath, args, details);
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
                UseShellExecute = false,
            },
        };

        p.Start();

        return p;
    }
}
