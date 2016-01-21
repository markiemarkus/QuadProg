using System.Collections.Generic;

namespace QuadProg.Setup
{
    public class SpreadSheetExample
    {
        public string Name { get; set; }

        public double UpperConstraint { get; set; }

        public double LowerConstraint { get; set; }

        public double[,] BasisVectors { get; set; }

        public IEnumerable<double> TargetVector { get; set; }
    }
}
