namespace QuadProg.Setup
{
    using MathNet.Numerics.LinearAlgebra;
    using QuadProg.Solver;
    using System;
    using System.Text;

    public class SpreadSheetExampleRunner
    {
        #region Methods

        public QpProgressReport  Run(SpreadSheetExample example)
        {
            Matrix<double> basisVectors = Matrix<double>.Build.DenseOfArray(example.BasisVectors);
            Vector<double> targetVector = Vector<double>.Build.DenseOfEnumerable(example.TargetVector);

            var assembler = new QpLeastSquaresFormulator();

            QpProblem problem = assembler
                .WithBasisVectors(basisVectors)
                .WithTargetVector(targetVector)
                .WithLowerConstraints(example.LowerConstraint)
                .WithUpperConstraints(example.UpperConstraint)
                .SetupProblem();

            IQpProgressReportBroadcaster publisher = new QpProgressReportPublisher();
            IQpInitialPointStrategy warmStarter = new QpWarmStarter();
            
            var listener = new ConsoleOutputService();
            listener.Subscribe(publisher);

            var solverHack = new PredictorCorrectorSolver(
                new QpPreSolver(problem.A, problem.b), 
                warmStarter, 
                new QpProgressAnalyser(problem, true), 
                publisher);

            return solverHack.Solve(problem);
        }

        public void PrintResults(QpProgressReport results, SpreadSheetExample example, Int64 elapsedMs, StringBuilder stringBuilder)
        {
            this.Display(new String('*', 50), stringBuilder);
            this.Display(example.Name, stringBuilder);
            this.Display(string.Format("Iterations: {0}", results.Iterations), stringBuilder);
            this.Display(string.Format("Elapsed time: {0}ms", elapsedMs), stringBuilder);
            this.Display("Solution: " + results.SolveStatus.ToString(), stringBuilder);
            this.Display(results.X, stringBuilder);
            this.Display(string.Empty, stringBuilder);
        }

        #endregion

        #region Private Methods

        private void Display(string message, StringBuilder sb)
        {
            Console.WriteLine(message);
            sb.AppendLine(message);
        }

        private void Display(Vector<double> vector, StringBuilder sb)
        {
            if (vector == null) return;

            Console.WriteLine(vector);

            foreach (double element in vector)
            {
                sb.AppendLine(element.ToString());
            }
        }

        #endregion
    }
}
