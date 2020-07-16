namespace QuadProg.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class SpreadSheetExamples
    {
        public static void RunExamples()
        {
            List<SpreadSheetExample> examples = BuildExamples();
            var runner = new SpreadSheetExampleRunner();

            var csvText = new StringBuilder();

            var watch = new Stopwatch();
            watch.Restart(); System.Threading.Thread.Sleep(10);

            foreach (SpreadSheetExample example in examples)
            {
                watch.Restart();
                var results = runner.Run(example);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                runner.PrintResults(results, example, elapsedMs, csvText);
            }

            File.WriteAllText(@"C:\Users\Mark\Documents\Result.csv", csvText.ToString());
        }

        private static List<SpreadSheetExample> BuildExamples()
        {
            var examples = new List<SpreadSheetExample>();

            examples.Add(new SpreadSheetExample
            {
                Name = "QP_Base: Simple Unconstrained optimisation problem",
                LowerConstraint = -10000,
                UpperConstraint = -9999,

                BasisVectors = new double[,]   
                { 
                    {  1,  2,  1 },
                    {  0,  1,  0 },
                    {  0,  4,  2 }
                },

                TargetVector = new double[] { 7, 1, 10 },
            });

            examples.Add(new SpreadSheetExample
            {
                Name = "QP_Boxed: Simple optimisation problem subject to inequality constraints",
                LowerConstraint = 3,
                UpperConstraint = 2,

                BasisVectors = new double[,]   
                { 
                    {  1,  2,  1 },
                    {  0,  1,  0 },
                    {  0,  4,  2 }
                },

                TargetVector = new double[] { 7, 1, 10 },
            });

            examples.Add(new SpreadSheetExample
            {
                Name = "QP_Subspace: Optimisation where basis vectors do not span the entire vector space of the target profile.",
                LowerConstraint = 0,
                UpperConstraint = 2,

                BasisVectors = new double[,]   
                { 
                    {  1,  2,  1 },
                    {  0,  1,  0 },
                    {  1,  0,  3 },
                    {  1,  0,  4 },
                    {  1,  0,  5 },
                    {  0,  4,  2 }
                },

                TargetVector = new double[] { 7, 1, 0, 2, 2, 10 },
            });

            List<SpreadSheetExample> fileExamples = ReadFromFile();

            return examples.Concat(fileExamples).ToList();
        }

        private static List<SpreadSheetExample> ReadFromFile()
        {
            var largeExamples = new List<SpreadSheetExample>();

            var spreadsheet = new ExcelSpreadSheet();
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var iconPath = Path.Combine(outPutDirectory, @"QuadraticProgramming2.xlsx");


            var book = spreadsheet.GetExcelWorkBook(iconPath);

            {
                var vectors = spreadsheet.ReadRange(book, "QP_Raw50", "BasisVectors");
                var target2d = spreadsheet.ReadRange(book, "QP_Raw50", "TargetProfile");
                var target = FlattenArray(target2d);

                largeExamples.Add(new SpreadSheetExample()
                {
                    Name = "QP_Soln50: 50 Vector problem",
                    BasisVectors = vectors,
                    TargetVector = target,
                    LowerConstraint = 0,
                    UpperConstraint = 2,
                });
            }

            {
                var vectors = spreadsheet.ReadRange(book, "QP_Raw300", "BasisVectors");
                var target2d = spreadsheet.ReadRange(book, "QP_Raw300", "TargetProfile");
                var target = FlattenArray(target2d);

                largeExamples.Add(new SpreadSheetExample()
                {
                    Name = "QP_Soln300: 300 Vector problem",
                    BasisVectors = vectors,
                    TargetVector = target,
                    LowerConstraint = 0,
                    UpperConstraint = 20,
                });
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return largeExamples;
        }

        private static List<double> FlattenArray(double[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            List<double> ret = new List<double>(width * height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ret.Add(array[i, j]);
                }
            }
            return ret;
        }
    }
}
