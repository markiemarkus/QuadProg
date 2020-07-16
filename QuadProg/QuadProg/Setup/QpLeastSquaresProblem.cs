namespace QuadProg.Setup
{
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using QuadProg.Solver;

    public class QpLeastSquaresFormulator
    {
        #region Properties

        public Matrix<double> BasisVectors { get; set; }

        public Vector<double> TargetVector { get; set; }

        public double LowerConstraint { get; set; }

        public double UpperConstraint { get; set; }

        #endregion

        #region Public Methods

        public QpLeastSquaresFormulator WithBasisVectors(Matrix<double> basisVectors)
        {
            this.BasisVectors = basisVectors;

            return this;
        }

        public QpLeastSquaresFormulator WithTargetVector(Vector<double> targetVector)
        {
            this.TargetVector = targetVector;

            return this;
        }

        public QpLeastSquaresFormulator WithLowerConstraints(double value)
        {
            this.LowerConstraint = value;

            return this;
        }

        public QpLeastSquaresFormulator WithUpperConstraints(double value)
        {
            this.UpperConstraint = value;

            return this;
        }

        public QpProblem SetupProblem()
        {
            Matrix<double> Q = AssembleQMatrix();
            Vector<double> g = AssembleCVector();
            Matrix<double> A = AssembleAMatrix();
            Vector<double> b = AssembleBVector();

            return new QpProblem.Builder()
                .WithQ(Q)
                .WithA(A)
                .WithC(g)
                .WithB(b)
                .Build();
        }

        #endregion

        #region Private methods

        private Matrix<double> AssembleQMatrix()
        {
            var rows = this.BasisVectors.RowCount;

            Matrix<double> lowerOnes = DenseMatrix.Create(
                rows,
                rows,
                (r, c) => (r < c) ? 0 : 1);

            return 2 * BasisVectors.TransposeThisAndMultiply(
                lowerOnes.TransposeThisAndMultiply(
                    lowerOnes * BasisVectors));
        }

        private Vector<double> AssembleCVector()
        {
            var rows = this.BasisVectors.RowCount;

            Matrix<double> Lower = DenseMatrix.Create(
                rows,
                rows,
                (r, c) => (r < c) ? 0 : 1);

            return -2 * Lower.TransposeThisAndMultiply(Lower * this.BasisVectors).TransposeThisAndMultiply(this.TargetVector);
        }

        private Matrix<double> AssembleAMatrix()
        {
            int cols = BasisVectors.ColumnCount;

         // Matrix<double> topA = Matrix<double>.Build.DenseIdentity(cols);
            Matrix<double> topA = Matrix<double>.Build.SparseIdentity(cols);

            return topA.Append(-1 * topA);
        }

        private Vector<double> AssembleBVector()
        {
            var cols = this.BasisVectors.ColumnCount;

            return Vector<double>.Build.Dense(cols * 2, (r) =>
                (r < cols) ? this.LowerConstraint  : -1 * this.UpperConstraint);
        }

        #endregion
    }
}
