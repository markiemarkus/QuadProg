namespace QuadProg.Solver
{
    public class LowerBound : Bound
    {
        #region Constructors

        public LowerBound(int position, double limit)
            : base(position, limit)
        {
        }

        #endregion

        #region Methods

        public static LowerBound Empty(int position)
        {
            return new LowerBound(position, double.NegativeInfinity);
        }

        public static implicit operator double(LowerBound bound)
        {
            return bound.Limit;
        }

        public override string ToString()
        {
            return string.Format(
                "Element {0} is greater than or equal to {2}.", base.position, base.limit);
        }

        public override bool IsUnbounded
        {
            get { return double.IsNegativeInfinity(base.limit); }
        }

        public LowerBound Tighten(LowerBound newBound)
        {
            base.Validate(newBound);

            return newBound > this ? newBound : this;
        }

        #endregion
    }
}
