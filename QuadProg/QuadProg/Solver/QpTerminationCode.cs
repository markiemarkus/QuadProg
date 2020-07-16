namespace QuadProg.Solver
{
    public enum QpTerminationCode
    {
        InProgress = 0,
        Success,
        MaxIterationsExceeded,
        Infeasible,
        Unknown
    }
}
