namespace DOPA.Cli;

public class OpaTestsFailedException : Exception
{
    public OpaTestsFailedException(string details)
        : base(details)
    {
    }
}
