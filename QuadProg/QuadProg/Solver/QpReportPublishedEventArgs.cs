namespace QuadProg.Solver
{
    public class QpReportPublishedEventArgs : System.EventArgs
    {
        private readonly QpProgressReport report;

        public QpReportPublishedEventArgs(QpProgressReport report)
        {
            this.report = report;
        }

        public QpProgressReport ProgressReport
        {
            get { return this.report; }
        }
    }
}
