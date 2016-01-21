using MathNet.Numerics.LinearAlgebra.Factorization;
using System;

namespace QuadProg.Solver
{
    public class PredictorCorrectorSolver : IQpAlgorithm
    {
        #region Fields

        private readonly IQpPreSolver preSolver;

        private readonly IQpInitialPointStrategy warmStartStrategy;

        private readonly IQpProgressAnalyser statusReporter;

        private readonly IQpProgressReportPublisher publisher;

        #endregion

        #region Constructors

        public PredictorCorrectorSolver(
            IQpPreSolver preSolver,
            IQpInitialPointStrategy warmStartStrategy,
            IQpProgressAnalyser terminateStrategy,
            IQpProgressReportPublisher publisher)
        {
            this.preSolver = preSolver;
            this.warmStartStrategy = warmStartStrategy;
            this.statusReporter = terminateStrategy;
            this.publisher = publisher;
        }

        #endregion

        #region Methods

        public QpProgressReport Solve(QpProblem data)
        {
            QpProgressReport report = this.preSolver.PreSolve();
            if (report.SolveStatus != QpTerminationCode.InProgress) return report;

            // Set up the problem from the warm start strategy
            Variables currentIterate = this.warmStartStrategy.GenerateInitialPoint(data, report.X);
            NewtonSystem newtonEquations = this.warmStartStrategy.InitialNewtonEquations;
            Residuals residuals = this.warmStartStrategy.InitialResiduals;

            double mu = currentIterate.Mu();

            int count = 0;

            do
            {
                // Analyse and report on the algorithm progress
                report = this.statusReporter.DetermineProgress(currentIterate, residuals, mu, count);
                this.publisher.Publish(report);

                if (report.SolveStatus != QpTerminationCode.InProgress) break;

                // ~~~~~ Predictor Step ~~~~~
                // Factorise the system of equations using a Newton method
                ISolver<double> choleskyFactor = newtonEquations.Factorize(currentIterate, count);
                Variables step = newtonEquations.ComputeStep(currentIterate, residuals, choleskyFactor);

                // Calculate the largest permissable step length (alpha affine) 
                double alpha = currentIterate.GetLargestAlphaForStep(step);

                // Calculate the complementarity measure and associated centering parameter.
                double muAffine = currentIterate.MuGivenStep(step, alpha);
                double sigma = Math.Pow(muAffine / mu, 3);

                // ~~~~~ Corrector Step ~~~~~
                // Apply second order step corrections and a re-centering adjustment.
                residuals.ApplyCorrection(step, sigma, mu);

                // Compute the corrected step and largest permitted step length.
                step = newtonEquations.ComputeStep(currentIterate, residuals, choleskyFactor);
                alpha = currentIterate.GetLargestAlphaForStep(step);

                // Finally take the step and calculate the new complementarity measure.
                currentIterate.ApplyStep(step, alpha);
                residuals.Update(currentIterate);
                mu = currentIterate.Mu();

                count++;

            } while (report.SolveStatus == QpTerminationCode.InProgress && count < 10000);

            return report;
        }

        #endregion
    }
}