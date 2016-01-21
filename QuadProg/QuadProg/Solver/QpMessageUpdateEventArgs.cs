using System;

namespace QuadProg.Solver
{
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