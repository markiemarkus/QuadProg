namespace QuadProg.Solver
{
    public interface IQpPreSolver
    {
        QpProgressReport PreSolve();
    }
}