namespace QuadProg.Solver
{
    using System;

    public class BoxConstraint
    {
        #region Fields

        private const double APriori = 1;

        private readonly LowerBound lowerBound;

        private readonly UpperBound upperBound;

        private readonly int position;

        #endregion

        #region Constructors

        public BoxConstraint(int position, LowerBound lb, UpperBound ub)
        {
            if (lb == null || ub == null)
            {
                throw new ArgumentNullException("Bounds cannot be null.");
            }

            this.lowerBound = lb;
            this.upperBound = ub;
            this.position = position;
        }

        #endregion

        #region Properties

        public bool IsFeasible
        {
            get { return this.upperBound > this.lowerBound; }
        }

        #endregion

        #region Methods

        public BoxConstraint Add(Bound bound)
        {
            var newLowerBound = bound as LowerBound;

            if (newLowerBound != null)
            {
                return this.Add(newLowerBound);
            }

            var newUpperBound = bound as UpperBound;

            if (newUpperBound != null)
            {
                return this.Add(newUpperBound);
            }

            throw new NotSupportedException("Unsupported bound type.");
        }

        private BoxConstraint Add(LowerBound bound)
        {
            LowerBound newLowerBound = this.lowerBound.Tighten(bound);

            if (newLowerBound == this.lowerBound)
            {
                return this;
            }

            return this.ToBuilder().WithLowerBound(newLowerBound).Build();
        }

        private BoxConstraint Add(UpperBound bound)
        {
            UpperBound newUpperBound = this.upperBound.Tighten(bound);

            if (newUpperBound == this.upperBound)
            {
                return this;
            }

            return this.ToBuilder().WithUpperBound(newUpperBound).Build();
        }

        public double ChooseStartingValue()
        {
            if (this.lowerBound.IsUnbounded && this.upperBound.IsUnbounded)
            {
                return APriori;
            }

            if (this.lowerBound.IsUnbounded)
            {
                return Math.Min(APriori, this.upperBound);
            }

            if (this.upperBound.IsUnbounded)
            {
                return Math.Max(APriori, this.lowerBound);
            }

            return (this.upperBound + this.lowerBound) / 2;
        }

        public Builder ToBuilder()
        {
            return new Builder()
            {
                Position = this.position,
                LowerBound = this.lowerBound,
                UpperBound = this.upperBound
            };
        }

        #endregion

        #region Builder

        public class Builder
        {
            #region Properties

            public LowerBound LowerBound { get; set; }

            public UpperBound UpperBound { get; set; }

            public int Position { get; set; }

            #endregion

            #region Methods

            public Builder WithLowerBound(LowerBound lb)
            {
                this.LowerBound = lb;

                return this;
            }

            public Builder WithUpperBound(UpperBound ub)
            {
                this.UpperBound = ub;

                return this;
            }

            public Builder WithPosition(int position)
            {
                this.Position = position;

                return this;
            }

            public BoxConstraint Build()
            {
                return new BoxConstraint(this.Position, this.LowerBound, this.UpperBound);
            }

            #endregion
        }

        #endregion
    }
}
