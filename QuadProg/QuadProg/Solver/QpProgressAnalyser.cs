using System;

namespace QuadProg.Solver
{
    public class QpProgressAnalyser : IQpProgressAnalyser
    {
        #region Fields

        private const double Tolerance = 1e-8;

        private const int MaxIterations = 200;

        private readonly double[] phiMinimumHistory;

        private readonly double dataInfinityNorm;

        private readonly bool includeDetailedReport;

        #endregion

        #region Constructors

        public QpProgressAnalyser(QpProblem data, bool includeDeatiledReport)
        {
            this.dataInfinityNorm = data.InfinityNorm();
            this.includeDetailedReport = includeDeatiledReport;
            this.phiMinimumHistory = new double[MaxIterations];
        }

        #endregion

        #region Properties

        public bool IncludeDetailedReport
        {
            get { return this.includeDetailedReport; }
        }

        #endregion

        #region Public Methods

        public QpProgressReport DetermineProgress(Variables iterate, Residuals residuals, double mu, int count)
        {
            var code = QpTerminationCode.InProgress;

            double residualsNorm = residuals.InfinityNorm();
            double phi = ComputeMeritFunction(residuals, residualsNorm);
            this.UpdateMeritFunctionHistory(phi, count);

            bool isMuSatisfied = (mu < Tolerance);
            bool isRnormSatisfied = (residualsNorm < Tolerance * this.dataInfinityNorm);

            if (isMuSatisfied && isRnormSatisfied)
            {
                code = QpTerminationCode.Success;
            }
            else if (count >= MaxIterations)
            {
                code = QpTerminationCode.MaxIterationsExceeded;
            }
            else if (count > 20 && phi >= 1e-8 && phi >= 1e4 * this.phiMinimumHistory[count - 1])
            {
                code = QpTerminationCode.Infeasible;
            }
            else if (count >= 30 && this.phiMinimumHistory[count] >= .5 * this.phiMinimumHistory[count - 30])
            {
                code = QpTerminationCode.Unknown;
            }

            return includeDetailedReport || code != QpTerminationCode.InProgress ?
                new QpProgressReport(code, count, phi, iterate.Clone()) :
                new QpProgressReport(code, count, phi, null);
        }

        #endregion

        #region Private Methods

        private double ComputeMeritFunction(Residuals residuals, double residualsNorm)
        {
            return (residualsNorm + Math.Abs(residuals.DualityGap)) / this.dataInfinityNorm;
        }

        private void UpdateMeritFunctionHistory(double phi, int index)
        {
            if (index == 0)
            {
                this.phiMinimumHistory[index] = phi;
            }
            else if (index < MaxIterations)
            {
                this.phiMinimumHistory[index] = Math.Min(phi, this.phiMinimumHistory[index - 1]);
            }
        }

        #endregion
    }
}
