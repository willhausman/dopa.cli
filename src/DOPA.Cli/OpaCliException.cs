namespace DOPA.Cli;

public class OpaCliException : InvalidOperationException
{
    public OpaCliException(string executableFilePath, string arguments, string details, string errorDetails)
        : base($"Failed to run opa executable\nExecutable: {executableFilePath}\nArgs: {arguments}\nstdout: {details}\nstderr: {errorDetails}")
    {
        ExecutableFilePath = executableFilePath;
        Arguments = arguments;
        StandardOutput = details;
        StandardError = errorDetails;
    }

    public string ExecutableFilePath { get; }

    public string Arguments { get; }

    public string StandardOutput { get; }

    public string StandardError { get; }
}
