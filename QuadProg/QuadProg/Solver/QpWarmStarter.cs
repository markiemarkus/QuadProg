using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace QuadProg.Solver
{
    public class QpWarmStarter : IQpInitialPointStrategy
    {
        #region Fields

        private NewtonSystem startingEquations;

        private Residuals startingResiduals;

        #endregion

        #region Properties

        public NewtonSystem InitialNewtonEquations
        {
            get { return this.startingEquations; }
        }

        public Residuals InitialResiduals
        {
            get { return this.startingResiduals; }
        }

        #endregion

        #region Methods

        public Variables GenerateInitialPoint(QpProblem problem, Vector<double> x)
        {
            var initialPoint = this.BuildVariables(problem, x);

            this.startingResiduals = new Residuals(problem, initialPoint);
            this.startingEquations = new NewtonSystem(problem, initialPoint);

            ISolver<double> choleskyFactor = this.startingEquations.InitialCholeskyFactor;
            Variables step = this.startingEquations.ComputeStep(initialPoint, this.startingResiduals, choleskyFactor);

            initialPoint.UpdateMultipliersWithoutViolation(step);

            this.startingResiduals.Update(initialPoint);

            return initialPoint;
        }

        private Variables BuildVariables(QpProblem problem, Vector<double> x)
        {
            double sqrtDataNorm = System.Math.Sqrt(problem.InfinityNorm());

            Vector<double> z = Vector<double>.Build.Dense(problem.A.ColumnCount, sqrtDataNorm);
            Vector<double> s = Vector<double>.Build.Dense(problem.A.ColumnCount, sqrtDataNorm);

            return new Variables(x, z, s);
        }

        #endregion
    }
}
