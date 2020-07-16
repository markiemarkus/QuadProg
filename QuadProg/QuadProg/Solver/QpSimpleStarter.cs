namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;

    public class QpSimpleStarter : IQpInitialPointStrategy
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
            Vector<double> z = Vector<double>.Build.Dense(problem.A.ColumnCount, 1);
            Vector<double> s = Vector<double>.Build.Dense(problem.A.ColumnCount, 1);

            Variables initialPoint = new Variables(x, z, s);

            this.startingResiduals = new Residuals(problem, initialPoint);
            this.startingEquations = new NewtonSystem(problem, initialPoint);

            return initialPoint;
        }

        #endregion
    }
}
