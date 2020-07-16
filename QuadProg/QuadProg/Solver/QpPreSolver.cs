namespace QuadProg.Solver
{
    using MathNet.Numerics.LinearAlgebra;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QpPreSolver : IQpPreSolver
    {
        #region Fields

        private readonly Matrix<double> a;

        private readonly Vector<double> b;

        #endregion

        #region Constructors

        public QpPreSolver(Matrix<double> a, Vector<double> b)
        {
            this.a = a;
            this.b = b;
        }

        #endregion

        #region Methods

        public QpProgressReport PreSolve()
        {
            bool success = true;

            var constraintSummary = new ConstraintSummary(this.a);

            foreach (int colIndex in constraintSummary.EmptyColIndices)
            {
                success &= this.ProcessEmptyCol(colIndex, this.b);
            }

            List<BoxConstraint> boxConstraints = SetupBoxConstraints(this.a.RowCount);

            foreach (int colIndex in constraintSummary.SingleElementColIndices)
            {
                Bound bound = this.ProcessSingleElementCol(colIndex, constraintSummary, this.b);
                boxConstraints[bound.Position] = boxConstraints[bound.Position].Add(bound);
            }

            Vector<double> initialX;
            success &= this.ChooseStartingX(boxConstraints, out initialX);

            foreach (int colIndex in constraintSummary.MultiElementColIndices)
            {
                success &= this.ProcessMultiElementCol(colIndex, constraintSummary, this.b);
            }

            return CompileReport(success, initialX);
        }

        #endregion

        #region Private Methods

        private static bool IsAboutEqual(double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }

        private List<BoxConstraint> SetupBoxConstraints(int numVars)
        {
            var boxConstraints = new List<BoxConstraint>();

            for (int i = 0; i < numVars; i++)
            {
                boxConstraints.Add(new BoxConstraint(i, LowerBound.Empty(i), UpperBound.Empty(i)));
            }

            return boxConstraints;
        }

        private QpProgressReport CompileReport(bool success, Vector<double> initialX)
        {
            if (success)
            {
                return new QpProgressReport(QpTerminationCode.InProgress, 0, 0, new Variables(initialX, null, null));
            }

            return new QpProgressReport(QpTerminationCode.Infeasible, 0, 0, null);
        }

        private bool ProcessMultiElementCol(int colIndex, ConstraintSummary constraintSummary, Vector<double> b)
        {
            // TODO check multivariable constraints
            return true;
        }

        private Bound ProcessSingleElementCol(int colIndex, ConstraintSummary constraintSummary, Vector<double> b)
        {
            var element = constraintSummary.GetColElements(colIndex).Single();

            if (element.Item3 > 0)
            {
                return new LowerBound(element.Item1, b[colIndex] / element.Item3);
            }

            return new UpperBound(element.Item1, b[colIndex] / element.Item3);
        }

        private bool ProcessEmptyCol(int colIndex, Vector<double> b)
        {
            if (IsAboutEqual(b[colIndex], 0))
            {
                // TODO clear row.
                return true;
            }

            return false;
        }

        private bool ChooseStartingX(List<BoxConstraint> boxConstraints, out Vector<double> initialX)
        {
            bool success = true;
            var startingValues = new List<double>();

            foreach (BoxConstraint boxConstraint in boxConstraints)
            {
                if (boxConstraint.IsFeasible)
                {
                    startingValues.Add(boxConstraint.ChooseStartingValue());
                }
                else
                {
                    success = false;
                    break;
                }
            }

            initialX = success ? Vector<double>.Build.DenseOfEnumerable(startingValues) : null;

            return success;
        }

        #endregion

        #region Private Class

        private class ConstraintSummary
        {
            #region Fields

            private readonly Dictionary<int, List<Tuple<int, int, double>>> elementsByCol;

            #endregion

            #region Constructors

            public ConstraintSummary(Matrix<double> a)
            {
                this.elementsByCol = a.EnumerateIndexed(Zeros.AllowSkip)
                 .GroupBy(element => element.Item2)
                 .ToDictionary(grp => grp.Key, grp => grp.ToList());

                this.EmptyColIndices = new List<int>();
                this.SingleElementColIndices = new List<int>();
                this.MultiElementColIndices = new List<int>();

                this.Initialise(a.ColumnCount);
            }

            #endregion

            #region Properties

            public List<int> EmptyColIndices { get; private set; }

            public List<int> SingleElementColIndices { get; private set; }

            public List<int> MultiElementColIndices { get; private set; }

            #endregion

            #region Methods

            public List<Tuple<int, int, double>> GetColElements(int col)
            {
                return this.elementsByCol[col].ToList();
            }

            private void Initialise(int colCount)
            {
                for (int i = 0; i < colCount; i++)
                {
                    if (elementsByCol.ContainsKey(i))
                    {
                        var colElements = elementsByCol[i];

                        if (colElements.Count == 1)
                        {
                            this.SingleElementColIndices.Add(i);
                        }
                        else
                        {
                            this.MultiElementColIndices.Add(i);
                        }
                    }
                    else
                    {
                        this.EmptyColIndices.Add(i);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
