namespace QuadProg.Solver
{
    public class QpProgressReportPublisher : IQpProgressReportBroadcaster
    {
        #region Events

        public event System.EventHandler<QpReportPublishedEventArgs> OnQpReportCompleted;

        public event System.EventHandler<QpMessageUpdateEventArgs> OnQpMessageUpdate;

        #endregion

        #region Interface Methods

        public virtual void Publish(QpProgressReport report)
        {
            if (this.OnQpReportCompleted != null)
            {
                this.OnQpReportCompleted(this, new QpReportPublishedEventArgs(report));
            }
        }

        public virtual void Publish(string message)
        {
            if (this.OnQpMessageUpdate != null)
            {
                this.OnQpMessageUpdate(this, new QpMessageUpdateEventArgs(message));
            }
        }

        #endregion
    }
}