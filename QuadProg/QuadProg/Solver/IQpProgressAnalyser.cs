namespace QuadProg.Solver
{
    public interface IQpProgressAnalyser
    {
        bool IncludeDetailedReport { get; }

        QpProgressReport DetermineProgress(Variables iterate, Residuals residuals, double mu, int count);
    }
}