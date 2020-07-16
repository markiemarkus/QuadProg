namespace QuadProg.Solver
{
    public abstract class Bound
    {
        #region Fields

        protected readonly int position;

        protected readonly double limit;

        #endregion

        #region Constructors

        protected Bound(int position, double limit)
        {
            this.position = position;
            this.limit = limit;
        }

        #endregion

        #region Properties

        public abstract bool IsUnbounded { get; }
        
        public int Position
        {
            get { return this.position; }
        }

        public double Limit
        {
            get { return this.limit; }
        }

        #endregion

        #region Methods

        protected void Validate(Bound bound)
        {
            if (bound.Position != this.position)
            {
                throw new System.IndexOutOfRangeException("Bound positions do not agree.");
            }
        }

        #endregion
    }
}
