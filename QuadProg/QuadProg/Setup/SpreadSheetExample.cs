namespace QuadProg.Setup
{
    using System.Collections.Generic;

    public class SpreadSheetExample
    {
        public string Name { get; set; }

        public double UpperConstraint { get; set; }

        public double LowerConstraint { get; set; }

        public double[,] BasisVectors { get; set; }

        public IEnumerable<double> TargetVector { get; set; }
    }
}
