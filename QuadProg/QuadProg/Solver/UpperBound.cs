namespace QuadProg.Solver
{
    public class UpperBound : Bound
    {
        #region Constructors

        public UpperBound(int position, double limit)
            : base(position, limit)
        {
        }

        #endregion

        #region Methods

        public override bool IsUnbounded
        {
            get { return double.IsPositiveInfinity(base.limit); }
        }

        public static UpperBound Empty(int position)
        {
            return new UpperBound(position, double.PositiveInfinity);
        }

        public static implicit operator double (UpperBound bound)
        {
            return bound.Limit;
        }

        public override string ToString()
        {
            return string.Format(
                "Element {0} is less than or equal to {2}.", base.position, base.limit);
        }

        public UpperBound Tighten(UpperBound newBound)
        {
            base.Validate(newBound);

            return newBound < this ? newBound : this;
        }

        #endregion
    }
}
