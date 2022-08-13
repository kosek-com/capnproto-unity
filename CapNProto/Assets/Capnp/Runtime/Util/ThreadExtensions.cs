using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Capnp.Util
{
    public static class ThreadExtensions
    {
        public static void SafeJoin(this Thread thread, int timeout = 5000)
        {
            if (!thread.Join(timeout))
            {
                string name = thread.Name ?? thread.ManagedThreadId.ToString();

                try
                {
                    Console.WriteLine($"Unable to join thread {name}. Thread is in state {thread.ThreadState}.");
                    thread.Abort();
                    if (!thread.Join(timeout))
                    {
                        Console.WriteLine($"Still unable to join thread {name} after Abort(). Thread is in state {thread.ThreadState}.");
                    }
                }
                catch
                {
                }
            }
        }
    }
}
