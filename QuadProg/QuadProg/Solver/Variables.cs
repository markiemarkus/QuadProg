namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;
    using System;

    public class Variables
    {
        #region Constructors

        public Variables(Vector<double> x, Vector<double> z, Vector<double> s)
        {
            this.x = x;
            this.z = z;
            this.s = s;
        }

        #endregion

        #region Properties

        public int ComplementaryVariablesCount
        {
            get { return z.Count; }
        }

        // TODO Address public setter
        public Vector<double> x { get; set; }
        public Vector<double> z { get; set; }
        public Vector<double> s { get; set; }

        #endregion

        #region Public Methods

        /** compute complementarity gap, obtained by taking the inner
        product of the complementary vectors and dividing by the total
        number of components */
        public double Mu()
        {
            return this.z.DotProduct(this.s) / this.ComplementaryVariablesCount;
        }

        /** compute the complementarity gap resulting from a step of length
        * alpha along direction "step" */
        public double MuGivenStep(Variables step, double alpha)
        {
            // Compute the affine duality gap
            return (z + alpha * step.z).DotProduct(s + alpha * step.s) / this.ComplementaryVariablesCount;
        }

        /** Calculate the largest alpha in (0,1] such that the nonnegative
        * variables stay nonnegative in the given search direction. */
        public double GetLargestAlphaForStep(Variables step)
        {
            // Compute alpha
            double alpha = 1;

            alpha = GetLargestPermittedAlpha(this.z, step.z, alpha);
            alpha = GetLargestPermittedAlpha(this.s, step.s, alpha);

            return alpha;
        }

        public Variables Clone()
        {
            Vector<double> xClone = this.x == null ? null : this.x.Clone();
            Vector<double> zClone = this.z == null ? null : this.z.Clone();
            Vector<double> sClone = this.s == null ? null : this.s.Clone();

            return new Variables(xClone, zClone, sClone);
        }

        public void ApplyStep(Variables step, double alpha)
        {
            const double eta = 0.95;

            step.Scale(eta * alpha);
            this.Add(step);
        }

        public void UpdateMultipliersWithoutViolation(Variables step)
        {
            this.z = ApplyStepWithoutViolation(this.z, step.z);
            this.s = ApplyStepWithoutViolation(this.s, step.s);
        }

        #endregion

        #region Private Methods

        private static double GetLargestPermittedAlpha(Vector<double> vector, Vector<double> step, double alpha)
        {
            foreach (Tuple<int, double> valueByIndex in step.EnumerateIndexed(Zeros.AllowSkip))
            {
                if (valueByIndex.Item2 < 0)
                {
                    alpha = Math.Min(alpha, (-vector[valueByIndex.Item1] / valueByIndex.Item2));
                }
            }

            return alpha;
        }

        private static Vector<double> ApplyStepWithoutViolation(Vector<double> vector, Vector<double> step)
        {
            const double beta = 1.0;

            Vector<double> result = vector + step;

            for (int i = 0; i < result.Count; i++)
            {
                result[i] = Math.Max(result[i], beta);
            }

            return result;
        }

        private void Scale(double scalingFactor)
        {
            this.x.Multiply(scalingFactor, this.x);
            this.z.Multiply(scalingFactor, this.z);
            this.s.Multiply(scalingFactor, this.s);
        }

        private void Add(Variables step)
        {
            this.x.Add(step.x, this.x);
            this.z.Add(step.z, this.z);
            this.s.Add(step.s, this.s);
        }

        #endregion
    }
}
