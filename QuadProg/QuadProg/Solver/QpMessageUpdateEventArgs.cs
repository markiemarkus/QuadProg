namespace QuadProg.Solver
{
    using System;

    public class QpMessageUpdateEventArgs : EventArgs
    {
        private readonly string message;

        public QpMessageUpdateEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return this.message; }
        }
    }
}
