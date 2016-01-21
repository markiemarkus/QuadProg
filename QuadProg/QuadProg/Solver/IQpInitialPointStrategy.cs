using MathNet.Numerics.LinearAlgebra;

namespace QuadProg.Solver
{
    public interface IQpInitialPointStrategy
    {
        Variables GenerateInitialPoint(QpProblem problem, Vector<double> x);

        NewtonSystem InitialNewtonEquations { get; }

        Residuals InitialResiduals { get; }
    }
}