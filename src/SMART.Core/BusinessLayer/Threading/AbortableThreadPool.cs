using System;
using System.Collections.Generic;
using System.Threading;

namespace SMART.Core.BusinessLayer.Threading
{
    public static class AbortableThreadPool
    {
        private static readonly LinkedList<WorkItem> callbacks = new LinkedList<WorkItem>();
        private static readonly Dictionary<WorkItem, Thread> threads = new Dictionary<WorkItem, Thread>();

        public static WorkItem QueueUserWorkItem(WaitCallback callback, object state)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            var item = new WorkItem(callback, state, ExecutionContext.Capture());
            lock (callbacks) callbacks.AddLast(item);
            ThreadPool.QueueUserWorkItem(HandleItem);
            return item;
        }
        public static WorkItem QueueUserWorkItem(WaitCallback callback)
        {
            return QueueUserWorkItem(callback, new object());
        }

        private static void HandleItem(object ignored)
        {
            WorkItem item = null;

            try
            {
                lock (callbacks)
                {
                    if (callbacks.Count > 0)
                    {
                        item = callbacks.First.Value;
                        callbacks.RemoveFirst();
                    }
                    if (item == null) return;
                    threads.Add(item, Thread.CurrentThread);
                }
                item.TryToStop = false;
                ExecutionContext.Run(item.Context, delegate { item.WaitCallback(item); }, null);
            }
            finally
            {
                lock (callbacks)
                    if (item != null) threads.Remove(item);
            }
        }
    
        public static WorkItemStatus Cancel(WorkItem item, ThreadPoolAbortMethod abortMethod)
        {
            if (item == null) throw new ArgumentNullException("item");

            lock (callbacks)
            {
                var node = callbacks.Find(item);
                if (node != null)
                {
                    callbacks.Remove(node);
                    return WorkItemStatus.Queued;
                }
                if (threads.ContainsKey(item))
                {
                    switch (abortMethod)
                    {
                        case ThreadPoolAbortMethod.Terminate:
                            {
                                threads[item].Abort();
                                threads.Remove(item);
                                return WorkItemStatus.Aborted;
                            }
                        case ThreadPoolAbortMethod.Gracefull:
                            {
                                item.TryToStop = true;
                                return WorkItemStatus.Aborted;

                            }
                    }
                    return WorkItemStatus.Executing;
                }
                return WorkItemStatus.Completed;
            }
        }

    }
}