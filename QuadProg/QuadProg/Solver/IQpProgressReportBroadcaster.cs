namespace QuadProg.Solver
{
    public interface IQpProgressReportBroadcaster : IQpProgressReportPublisher
    {
        event System.EventHandler<QpReportPublishedEventArgs> OnQpReportCompleted;

        event System.EventHandler<QpMessageUpdateEventArgs> OnQpMessageUpdate;
    }
}
