namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Factorization;

    public class NewtonSystem
    {
        #region Fields

        MatrixBuilder<double> matBuilder = Matrix<double>.Build;

        private readonly Matrix<double> Q;
        private readonly Matrix<double> A;

        private readonly ISolver<double> initialCholeskyFactor;

        #endregion

        #region Constructors

        public NewtonSystem(QpProblem data, Variables initialPoint)
        {
            this.Q = data.Q;
            this.A = data.A;

            this.initialCholeskyFactor = this.Factorize(initialPoint);
        }

        #endregion

        #region Properties

        public ISolver<double> InitialCholeskyFactor
        {
            get { return this.initialCholeskyFactor; }
        }

        #endregion

        #region Public Methods

        public ISolver<double> Factorize(Variables currentIterate, int iteration)
        {
            if (iteration == 0)
            {
                return this.initialCholeskyFactor; 
            }

            return this.Factorize(currentIterate);
        }

        public Variables ComputeStep(Variables currentIterate, Residuals residuals, ISolver<double> CholeskyFactor)
        {
            // Define shorter names
            Vector<double> z = currentIterate.z;
            Vector<double> s = currentIterate.s;
            Vector<double> rp = residuals.rp;
            Vector<double> rd = residuals.rd;
            Vector<double> rs = residuals.rs;

            Vector<double> r_bar = A * (rs - z.PointwiseMultiply(rp)).PointwiseDivide(s);
            Vector<double> c_bar = rd + r_bar;

            c_bar.Negate(c_bar);

            Vector<double> stepX = CholeskyFactor.Solve(c_bar);
            Vector<double> stepS = A.TransposeThisAndMultiply(stepX) - rp;
            Vector<double> stepZ = -(rs + z.PointwiseMultiply(stepS)).PointwiseDivide(s);

            return new Variables(stepX, stepZ, stepS);
        }

        #endregion

        #region Private Methods

        private ISolver<double> Factorize(Variables currentIterate)
        {
            Vector<double> z = currentIterate.z;
            Vector<double> s = currentIterate.s;

            Matrix<double> sOverZ = this.matBuilder.SparseOfDiagonalVector(z.Count, z.Count, z.PointwiseDivide(s));
            Matrix<double> Q_bar = this.Q + this.A.Multiply(sOverZ).TransposeAndMultiply(A);

            return Q_bar.Cholesky();
        }

        #endregion
    }
}
