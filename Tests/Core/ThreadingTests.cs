using System.Threading;
using NUnit.Framework;
using System;
using SMART.Core.BusinessLayer.Threading;

namespace SMART.Test.Core
{
    [TestFixture]
    public class ThreadingTests
    {
        //TODO: Think of a better way to test this???

        [Test]
        public void queueing_a_callback_to_the_abortable_threadpool_should_return_a_workitem()
        {
            // Assign
            bool r = false;
            WaitCallback callback = (s) => { r = true;};
            
            // Act
            var item = AbortableThreadPool.QueueUserWorkItem(callback, null);
            
            //TODO: ugly sleep here, change AbortableThreadPool to return a IAsyncResult instead
            Thread.Sleep(100);
            // Assert
            Assert.IsTrue(r);
            Assert.IsNotNull(item);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void abortablethreadpool_should_throw_an_exception_if_no_callback_exist()
        {
            // Assign
            
            // Act
            AbortableThreadPool.QueueUserWorkItem(null);

            // Assert

        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void abortablethreadpool_cancel_should_throw_if_argument_is_null()
        {
            // Assign

            // Act
            AbortableThreadPool.Cancel(null, ThreadPoolAbortMethod.Terminate);

            // Assert

        }

        [Test]
        public void canceling_should_set_item_trytostop_to_true_if_graceful_was_selected()
        {
            ManualResetEvent r = new ManualResetEvent(false);
            WaitCallback callback = (sender) => { r.Reset();
                                                    r.WaitOne();
            };
            
            var item  = AbortableThreadPool.QueueUserWorkItem(callback);
            Thread.Sleep(10);
            bool stopped = false;
            item.Stopped += (s, e) => { stopped = true; };

            Assert.IsFalse(item.TryToStop);
            var status = AbortableThreadPool.Cancel(item, ThreadPoolAbortMethod.Gracefull);
            Assert.AreEqual(WorkItemStatus.Aborted, status);
            r.Set();
   
        }

        [Test]
        [Ignore]
        public void terminate_abort_thread_exception_will_be_raised()
        {
            // Assign
            bool called = false;
            WaitCallback callback = (s) =>
                                        {
                                            try
                                            {
                                                Thread.Sleep(1000);
                                            }
                                            catch (ThreadAbortException e)
                                            {

                                                called = true;
                                                Thread.ResetAbort();
                                            }
                                        };
            // Act
            var item = AbortableThreadPool.QueueUserWorkItem(callback);
            Thread.Sleep(100);
          
            AbortableThreadPool.Cancel(item, ThreadPoolAbortMethod.Terminate);
            // Assert
            Thread.Sleep(10);
            Assert.IsTrue(called);
        }
    }
}