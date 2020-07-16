namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;

    public interface IQpInitialPointStrategy
    {
        Variables GenerateInitialPoint(QpProblem problem, Vector<double> x);

        NewtonSystem InitialNewtonEquations { get; }

        Residuals InitialResiduals { get; }
    }
}
