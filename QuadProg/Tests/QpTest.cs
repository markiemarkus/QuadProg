namespace Tests
{
    using MathNet.Numerics.LinearAlgebra;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using QuadProg.Solver;

    [TestClass]
    public class QpTest
    {
        private QpProblem problem;

        private const int n = 200;
        
        [TestInitialize]
        public void Setup()
        {
            // Arrange
            Matrix<double> Q = Matrix<double>.Build.DenseIdentity(n);

            Vector<double> c = Vector<double>.Build.Random(n);
            Matrix<double> topA = Matrix<double>.Build.DenseIdentity(n);
            var A = topA.Append(-topA);

            Vector<double> b = -10 * Vector<double>.Build.Dense(2 * n, 1);

            Vector<double> x = Vector<double>.Build.Dense(n, 1);

            this.problem = new QpProblem.Builder()
                .WithQ(Q)
                .WithA(A)
                .WithC(c)
                .WithB(b)
                .Build();
        }

        [TestMethod]
        public void AcSolver()
        {
            // Arrange
            IQpProgressReportBroadcaster publisher = new QpProgressReportPublisher();
            IQpInitialPointStrategy warmStarter = new QpWarmStarter();
            var listener = new ConsoleOutputService();
            listener.Subscribe(publisher);

            var solver = new PredictorCorrectorSolver(
                new QpPreSolver(problem.A, problem.b), 
                new QpWarmStarter(), 
                new QpProgressAnalyser(problem, true), 
                new QpProgressReportPublisher());

            // Act
            QpProgressReport solution = solver.Solve(problem);

            // Assert
            var check = solution.X + problem.c;
            Assert.IsTrue( check.Norm(2) < (1e-8 * n));
        }

        [TestMethod]
        public void AlgLibSolver()
        {
            var builder = Vector<double>.Build;

            double[,] Q = this.problem.Q.ToArray();
            double[] b = this.problem.c.ToArray();
            double[] s = builder.Dense(n, 1).ToArray();
            
            double[,] c = this.problem.A
                .Transpose()
                .Append(this.problem.b.ToColumnMatrix())
                .ToArray();

            var ct = new int[2*n];
            for (int i = 0; i < ct.Length; i++) { ct[i] = 1; }

            double[] x;
            alglib.minqpstate state;
            alglib.minqpreport rep;

            // create solver, set quadratic/linear terms
            alglib.minqpcreate(n, out state);
            alglib.minqpsetquadraticterm(state, Q);
            alglib.minqpsetlinearterm(state, b);
            alglib.minqpsetlc(state, c, ct);

            // Set scale of the parameters.
            alglib.minqpsetscale(state, s);

            // solve problem with BLEIC-based QP solver
            // default stopping criteria are used.
            alglib.minqpsetalgobleic(state, 0.0, 0.0, 0.0, 0);
            alglib.minqpoptimize(state);
            alglib.minqpresults(state, out x, out rep);
            System.Console.WriteLine("{0}", alglib.ap.format(x, 1));

            // Assert
            var check = builder.DenseOfArray(x) + this.problem.c;
            Assert.IsTrue(check.Norm(2) < (1e-8 * n));
        }
    }
}
