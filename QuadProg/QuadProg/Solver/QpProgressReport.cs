using MathNet.Numerics.LinearAlgebra;

namespace QuadProg.Solver
{
    public class QpProgressReport
    {
        #region Fields

        private readonly QpTerminationCode solveStatus;

        private readonly Variables currentIterate;

        private readonly int iterations;

        private readonly double meritFunction;

        #endregion

        #region Constructors

        public QpProgressReport(QpTerminationCode status, int iterations, double meritFunction, Variables currentIterate)
        {
            this.solveStatus = status;
            this.iterations = iterations;
            this.currentIterate = currentIterate == null ? null : currentIterate.Clone();
            this.meritFunction = meritFunction;
        }

        #endregion

        #region Properties

        public QpTerminationCode SolveStatus
        {
            get { return this.solveStatus; }
        }

        public double MeritFunction
        {
            get { return this.meritFunction; }
        }

        public bool IsDetailedReport
        {
            get { return this.currentIterate != null; }
        }

        public Vector<double> X
        {
            get
            {
                return this.IsDetailedReport ? this.currentIterate.x.Clone() : null;
            }
        }

        public Vector<double> SlackVector
        {
            get
            {
                return this.IsDetailedReport ? this.currentIterate.s.Clone() : null;
            }
        }

        public Vector<double> LagrangeMultiplier
        {
            get
            {
                return this.IsDetailedReport ? this.currentIterate.z.Clone() : null;
            }
        }

        public double Iterations
        {
            get { return this.iterations; }
        }

        #endregion
    }
}
