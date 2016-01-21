namespace QuadProg.Solver
{
    public interface IQpAlgorithm
    {
        QpProgressReport Solve(QpProblem data);
    }
}
