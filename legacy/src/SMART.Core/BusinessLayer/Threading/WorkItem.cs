using System;
using System.Threading;

namespace SMART.Core.BusinessLayer.Threading
{
    public class WorkItem
    {
        public WaitCallback WaitCallback { get; private set; }
        public Object State { get; private set; }
        public ExecutionContext Context { get; private set; }
        public bool TryToStop { get; set; }
        public int Position { get; set; }
        public event EventHandler Stopped;

        public WorkItem(WaitCallback waitCallback, object state, ExecutionContext context)
        {
            WaitCallback = waitCallback;
            Context = context;
            State = state;
        }
    }
}