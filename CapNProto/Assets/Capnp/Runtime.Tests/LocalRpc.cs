﻿using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class LocalRpc: TestBase
    {
        [Test]
        public void DeferredLocalAnswer()
        {
            var tcs = new TaskCompletionSource<int>();
            var impl = new TestPipelineImpl2(tcs.Task);
            var bproxy = BareProxy.FromImpl(impl);
            using (var proxy = bproxy.Cast<ITestPipeline>(true))
            using (var cap = proxy.GetCap(0, null).OutBox_Cap())
            {
                var foo = cap.Foo(123, true);
                tcs.SetResult(0);
                Assert.IsTrue(foo.Wait(TestBase.MediumNonDbgTimeout));
                Assert.AreEqual("bar", foo.Result);
            }
        }

        [Test]
        public void Embargo()
        {
            NewLocalTestbed().RunTest(Testsuite.EmbargoOnPromisedAnswer);
        }

        [Test]
        public void EmbargoError()
        {
            NewLocalTestbed().RunTest(Testsuite.EmbargoError);
        }

        [Test]
        public void EmbargoNull()
        {
            NewLocalTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [Test]
        public void CallBrokenPromise()
        {
            NewLocalTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [Test]
        public void TailCall()
        {
            NewLocalTestbed().RunTest(Testsuite.TailCall);
        }

        [Test]
        public void SendTwice()
        {
            NewLocalTestbed().RunTest(Testsuite.SendTwice);
        }

        [Test]
        public void Cancel()
        {
            NewLocalTestbed().RunTest(Testsuite.Cancel);
        }

        [Test]
        public void RetainAndRelease()
        {
            NewLocalTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [Test]
        public void PromiseResolve()
        {
            NewLocalTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [Test]
        public void Cancelation()
        {
            NewLocalTestbed().RunTest(Testsuite.Cancelation);
        }

        [Test]
        public void ReleaseOnCancel()
        {
            NewLocalTestbed().RunTest(Testsuite.ReleaseOnCancel);
        }

        [Test]
        public void Release()
        {
            NewLocalTestbed().RunTest(Testsuite.Release);
        }

        [Test]
        public void Pipeline()
        {
            NewLocalTestbed().RunTest(Testsuite.Pipeline);
        }

        [Test]
        public void Basic()
        {
            NewLocalTestbed().RunTest(Testsuite.Basic);
        }

        [Test]
        public void Ownership1()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership1);
        }

        [Test]
        public void Ownership2()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership2);
        }

        [Test]
        public void Ownership3()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership3);
        }

        [Test]
        public void ImportReceiverAnswer()
        {
            NewLocalTestbed().RunTest(Testsuite.Ownership3);
        }

        [Test]
        public void LegacyAccess()
        {
            NewLocalTestbed().RunTest(Testsuite.LegacyAccess);
        }

        [Test]
        public void EagerRace()
        {
            var impl = new TestMoreStuffImpl(new Counters());
            var tcs = new TaskCompletionSource<ITestMoreStuff>();
            using (var promise = tcs.Task.Eager(true))
            using (var cts = new CancellationTokenSource())
            {
                var bb = new BufferBlock<Task<uint>>();
                int counter = 0;

                void Generator()
                {
                    while (!cts.IsCancellationRequested)
                    {
                        bb.Post(promise.GetCallSequence((uint)Volatile.Read(ref counter)));
                        Interlocked.Increment(ref counter);
                    }

                    bb.Complete();
                }

                async Task Verifier()
                {
                    uint i = 0;
                    while (true)
                    {
                        Task<uint> t;

                        try
                        {
                            t = await bb.ReceiveAsync();
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }

                        uint j = await t;
                        Assert.AreEqual(i, j);
                        i++;
                    }
                }

                var genTask = Task.Run(() => Generator());
                var verTask = Verifier();
                SpinWait.SpinUntil(() => Volatile.Read(ref counter) >= 100);
                Task.Run(() => tcs.SetResult(impl));
                cts.Cancel();
                Assert.IsTrue(genTask.Wait(MediumNonDbgTimeout));
                Assert.IsTrue(verTask.Wait(MediumNonDbgTimeout));
            }
        }

        [Test]
        public void AwaitNoDeadlock()
        {
            for (int i = 0; i < 100; i++)
            {
                var tcs1 = new TaskCompletionSource<int>();
                var tcs2 = new TaskCompletionSource<int>();

                var t1 = Capnp.Util.StrictlyOrderedTaskExtensions.EnforceAwaitOrder(tcs1.Task);
                var t2 = Capnp.Util.StrictlyOrderedTaskExtensions.EnforceAwaitOrder(tcs2.Task);

                async Task Wait1()
                {
                    await t1;
                    await t2;
                }

                async Task Wait2()
                {
                    await t2;
                    await t1;
                }

                var w1 = Wait1();
                var w2 = Wait2();

                Task.Run(() => tcs1.SetResult(0));
                Task.Run(() => tcs2.SetResult(0));

                Assert.IsTrue(Task.WaitAll(new Task[] { w1, w2 }, MediumNonDbgTimeout));
            }
        }

        [Test]
        public void DisposedProxy()
        {
            var b = new BareProxy();
            Assert.Throws<ArgumentNullException>(() => b.Bind(null));
            var impl = new TestInterfaceImpl2();
            var proxy = Proxy.Share<ITestInterface>(impl);
            var p = (Proxy)proxy;
            Assert.Throws<InvalidOperationException>(() => p.Bind(p.ConsumedCap));
            Assert.IsFalse(p.IsDisposed);
            proxy.Dispose();
            Assert.IsTrue(p.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => { var _ = p.ConsumedCap; });
            var t = proxy.Foo(123, true);
            Assert.IsTrue(E7Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => t).Wait(MediumNonDbgTimeout));
        }
    }
}
