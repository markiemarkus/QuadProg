using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace QuadProg.Solver
{
    public class QpProblem
    {
        #region Fields

        private readonly Matrix<double> _Q;

        private readonly Vector<double> _c;

        private readonly Matrix<double> _A;

        private readonly Vector<double> _b;

        #endregion

        #region Constructors

        public QpProblem(Matrix<double> Q, Vector<double> g, Matrix<double> A, Vector<double> b)
        {
            this._Q = Q.Clone();
            this._c = g.Clone();
            this._A = A.Clone();
            this._b = b.Clone();
        }

        #endregion

        #region Properties

        public Matrix<double> Q
        {
            get { return this._Q.Clone(); }
        }

        public Vector<double> c
        {
            get { return this._c.Clone(); }
        }

        public Matrix<double> A
        {
            get { return this._A.Clone(); }
        }

        public Vector<double> b
        {
            get { return this._b.Clone(); }
        }

        #endregion

        #region Methods

        public double InfinityNorm()
        {
            List<double> norm = new List<double>
            {
                1.0,
                this._Q.InfinityNorm(),
                this._c.InfinityNorm(),
                this._A.InfinityNorm(),
                this._b.InfinityNorm()
            };

            return norm.Max();
        }

        #endregion

        #region Builder

        public class Builder
        {
            #region Properties

            public Matrix<double> Q { get; set; }

            public Vector<double> c { get; set; }

            public Matrix<double> A { get; set; }

            public Vector<double> b { get; set; }

            #endregion

            #region Build Methods

            public Builder WithQ(Matrix<double> value)
            {
                this.Q = value;

                return this;
            }

            public Builder WithC(Vector<double> value)
            {
                this.c = value;

                return this;
            }

            public Builder WithA(Matrix<double> value)
            {
                this.A = value;

                return this;
            }

            public Builder WithB(Vector<double> value)
            {
                this.b = value;

                return this;
            }

            public QpProblem Build()
            {
                if (this.Q == null || this.c == null || this.A == null || this.b == null)
                {
                    throw new ArgumentNullException();
                }

                return new QpProblem(this.Q, this.c, this.A, this.b);
            }

            #endregion
        }

        #endregion
    }
}