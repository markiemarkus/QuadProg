namespace QuadProg.Solver
{
    public interface IQpProgressReportPublisher
    {
        void Publish(QpProgressReport report);

        void Publish(string message);
    }
}