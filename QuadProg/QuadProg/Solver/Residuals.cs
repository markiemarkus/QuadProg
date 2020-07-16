namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;
    using System.Collections.Generic;
    using System.Linq;

    public class Residuals
    {
        #region Fields

        private readonly Matrix<double> Q;
        private readonly Vector<double> c;
        private readonly Matrix<double> A;
        private readonly Vector<double> b;

        #endregion

        #region Constructors

        public Residuals(QpProblem data, Variables iterate)
        {
            this.Q = data.Q;
            this.c = data.c;
            this.A = data.A;
            this.b = data.b;

            this.Initialise();
            this.Update(iterate);
        }

        #endregion

        #region Properties

        public double DualityGap { get; private set; }

        public Vector<double> rd { get; private set; }
        public Vector<double> rp { get; private set; }
        public Vector<double> rs { get; private set; }

        #endregion

        #region Public Methods

        public void Update(Variables newIterate)
        {
            Vector<double> x = newIterate.x;
            Vector<double> z = newIterate.z;
            Vector<double> s = newIterate.s;

            this.Q.Multiply(x, this.rd);
            this.rd.Add(this.c, this.rd);
            this.DualityGap = this.rd.DotProduct(x) - this.b.DotProduct(z);
            this.rd.Subtract(this.A * z, this.rd);

            s.Add(this.b, this.rp);
            this.rp.Subtract(A.TransposeThisAndMultiply(x), this.rp);
            s.PointwiseMultiply(z, this.rs);
        }

        public double InfinityNorm()
        {
            List<double> norm = new List<double>
            {
                this.rd.InfinityNorm(),
                this.rp.InfinityNorm(),
            };

            return norm.Max();
        }

        public void ApplyCorrection(Variables step, double sigma, double mu)
        {
            this.rs = this.rs + step.s.PointwiseMultiply(step.z) - (sigma * mu);
        }

        #endregion

        #region Private Methods

        private void Initialise()
        {
            this.rd = Vector<double>.Build.Dense(this.Q.ColumnCount);
            this.rp = Vector<double>.Build.Dense(this.b.Count);
            this.rs = Vector<double>.Build.Dense(this.b.Count);
        }

        #endregion
    }
}
