using System.Diagnostics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using QuadProg.Setup;
using QuadProg.Solver;
using System;

namespace QuadProg
{
    class Program
    {
        static void Main(string[] args)
        {
            // Using managed code only
            Control.UseManaged();
            Console.WriteLine(Control.LinearAlgebraProvider);

            var m = Matrix<double>.Build.Random(1000, 1000);
            var v = Vector<double>.Build.Random(1000);

            var w = Stopwatch.StartNew();
            var y1 = m.Solve(v);
            Console.WriteLine("Solved:{0}", w.Elapsed);
            Console.WriteLine(y1);

            // Using the Intel MKL native provider
            Control.UseNativeMKL();
            Console.WriteLine(Control.LinearAlgebraProvider);

            w.Restart();
            var y2 = m.Solve(v);
            Console.WriteLine("Solved:{0}", w.Elapsed);
            Console.WriteLine(y2);


            w.Restart();
            Console.WriteLine(w.Elapsed);
            SpreadSheetExamples.RunExamples();
            Console.WriteLine(w.Elapsed);

            System.Console.ReadLine();
        }

        private static void MuckAround()
        {
            var vectorB = new double[] { 0, -2, -6, -3, 0, 5, 1 };

            var arrayA = new double[,]
            {
                {  1, -1,  0, 1, 0, 2, 1 },
                {  0,  0, -1, 1, 0, 3, 0 },
            };

            var b = Vector<double>.Build.DenseOfArray(vectorB);

            var a = Matrix<double>.Build.DenseOfArray(arrayA);

            var preSolver = new QpPreSolver(a, b);

            QpProgressReport report = preSolver.PreSolve();
            Console.WriteLine(report.X);
        }
    }
}
