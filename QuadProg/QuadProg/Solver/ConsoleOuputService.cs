using System;

namespace QuadProg.Solver
{
    public class ConsoleOutputService
    {
        #region Public Methods

        public void Subscribe(IQpProgressReportBroadcaster broadcaster)
        {
            broadcaster.OnQpReportCompleted += PrintQpReport;
            broadcaster.OnQpMessageUpdate += PrintQpMessage;
        }

        #endregion

        #region Private Methods

        private void PrintQpReport(object sender, QpReportPublishedEventArgs e)
        {
            QpProgressReport report = e.ProgressReport;

            if (report.Iterations == 0)
            {
                this.PrintTitle(report);
            }

            this.PrintTableData(report);

            if (report.SolveStatus != QpTerminationCode.InProgress)
            {
                this.PrintSeparator();
                this.PrintMessage(string.Empty);
            }
        }

        private void PrintQpMessage(object sender, QpMessageUpdateEventArgs e)
        {
            string report = e.Message;

            this.PrintMessage(report);
        }

        private void PrintTitle(QpProgressReport report)
        {
            this.PrintSeparator();

            string[] headers = new[] { "Iteration", "Merit Function", "Solve Status" };
            var stringHeaders = string.Format("{0,-17}{1,-17}{2,-17}", headers[0], headers[1], headers[2]);

            this.PrintMessage(stringHeaders);
            this.PrintSeparator();
        }

        private void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        private void PrintSeparator()
        {
            this.PrintMessage(new string('=', 54));
        }

        private void PrintTableData(QpProgressReport report)
        {
            string phi;

            if (Math.Abs(report.MeritFunction) > 10000 || Math.Abs(report.MeritFunction) < 0.01)
            {
                phi = report.MeritFunction.ToString("0.00000E+0");
            }
            else
            {
                phi = report.MeritFunction.ToString("0.###");
            }

            var stringHeaders = string.Format(
                "{0,-17}{1,-17}{2,-17}",
                report.Iterations,
                phi,
                report.SolveStatus);

            this.PrintMessage(stringHeaders);
        }

        #endregion
    }
}
