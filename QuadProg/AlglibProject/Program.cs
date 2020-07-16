namespace AlglibProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            QuadProg();

            NonLinearOptimisation();
        }

        static void Test1()
        {
            //
            // We have two samples - x and y, and want to measure dependency between them
            //
            double[] x = new double[] { 0, 1, 4, 9, 16, 25, 36, 49, 64, 81 };
            double[] y = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double v;

            //
            // Three dependency measures are calculated:
            // * covariation
            // * Pearson correlation
            // * Spearman rank correlation
            //
            v = alglib.cov2(x, y);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 82.5
            v = alglib.pearsoncorr2(x, y);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 0.9627
            v = alglib.spearmancorr2(x, y);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 1.000
            System.Console.ReadLine();
        }

        public static int QuadProg()
        {
            //
            // This example demonstrates minimization of F(x0,x1) = x0^2 + x1^2 -6*x0 - 4*x1
            // subject to linear constraint x0+x1<=2
            //
            // Exact solution is [x0,x1] = [1.5,0.5]
            //
            // IMPORTANT: this solver minimizes  following  function:
            //     f(x) = 0.5*x'*A*x + b'*x.
            // Note that quadratic term has 0.5 before it. So if you want to minimize
            // quadratic function, you should rewrite it in such way that quadratic term
            // is multiplied by 0.5 too.
            // For example, our function is f(x)=x0^2+x1^2+..., but we rewrite it as 
            //     f(x) = 0.5*(2*x0^2+2*x1^2) + ....
            // and pass diag(2,2) as quadratic term - NOT diag(1,1)!
            //
            double[,] a = new double[,] { { 2, 0 }, { 0, 2 } };
            double[] b = new double[] { -6, -4 };
            double[] s = new double[] { 1, 1 };
            double[,] c = new double[,] { { 1.0, 1.0, 2.0 } };
            int[] ct = new int[] { -1 };
            double[] x;
            alglib.minqpstate state;
            alglib.minqpreport rep;

            // create solver, set quadratic/linear terms
            alglib.minqpcreate(2, out state);
            alglib.minqpsetquadraticterm(state, a);
            alglib.minqpsetlinearterm(state, b);
            alglib.minqpsetlc(state, c, ct);

            // Set scale of the parameters.
            // It is strongly recommended that you set scale of your variables.
            // Knowing their scales is essential for evaluation of stopping criteria
            // and for preconditioning of the algorithm steps.
            // You can find more information on scaling at http://www.alglib.net/optimization/scaling.php
            alglib.minqpsetscale(state, s);

            // solve problem with BLEIC-based QP solver
            // default stopping criteria are used.
            alglib.minqpsetalgobleic(state, 0.0, 0.0, 0.0, 0);
            alglib.minqpoptimize(state);
            alglib.minqpresults(state, out x, out rep);
            System.Console.WriteLine("{0}", alglib.ap.format(x, 1)); // EXPECTED: [1.500,0.500]

            // solve problem with QuickQP solver, default stopping criteria are used
            // Oops! It does not support general linear constraints, -5 returned as completion code!
            alglib.minqpsetalgoquickqp(state, 0.0, 0.0, 0.0, 0, true);
            alglib.minqpoptimize(state);
            alglib.minqpresults(state, out x, out rep);
            System.Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: -5
            System.Console.ReadLine();
            return 0;
        }

        public static void function1_grad(double[] x, ref double func, double[] grad, object obj)
        {
            // this callback calculates f(x0,x1) = 100*(x0+3)^4 + (x1-3)^4
            // and its derivatives df/d0 and df/dx1
            func = 100 * System.Math.Pow(x[0] + 3, 4) + System.Math.Pow(x[1] - 3, 4);
            grad[0] = 400 * System.Math.Pow(x[0] + 3, 3);
            grad[1] = 4 * System.Math.Pow(x[1] - 3, 3);
        }

        public static int NonLinearOptimisation()
        {
            //
            // This example demonstrates minimization of f(x,y) = 100*(x+3)^4+(y-3)^4
            // using LBFGS method.
            //
            double[] x = new double[] { 0, 0 };
            double epsg = 0.0000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            alglib.minlbfgsstate state;
            alglib.minlbfgsreport rep;

            alglib.minlbfgscreate(1, x, out state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, function1_grad, null, null);
            alglib.minlbfgsresults(state, out x, out rep);

            System.Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: 4
            System.Console.WriteLine("{0}", alglib.ap.format(x, 2)); // EXPECTED: [-3,3]
            System.Console.ReadLine();
            return 0;
        }
    }
}
