using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Capnp.Rpc;
using NUnit.Framework;
using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;

namespace Capnp.Net.Runtime.Tests
{

    [TestFixture]
    [Category("Coverage")]
    public class TcpRpcPorted: TestBase
    {
        [Test]
        public void Basic()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Basic);
        }

        [Test]
        public void Pipeline()
        {
            NewLocalhostTcpTestbed(TcpRpcTestOptions.ClientTracer).RunTest(Testsuite.Pipeline);
        }

        [Test]
        public void Release()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Release);
        }

        [Test]
        public void ReleaseOnCancel()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                //client.WhenConnected.Wait();

                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    ((Proxy)main).WhenResolved.WrappedTask.Wait(MediumNonDbgTimeout);

                    // Since we have a threaded model, there is no way to deterministically provoke the situation
                    // where Cancel and Finish message cross paths. Instead, we'll do a lot of such requests and
                    // later on verify that the handle count is 0.

                    for (int i = 0; i < 1000; i++)
                    {
                        var cts = new CancellationTokenSource();
                        var task = main.GetHandle(cts.Token);
                        cts.Cancel();
                        task.ContinueWith(t =>
                        {
                            try
                            {
                                t.Result.Dispose();
                            }
                            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                            {
                            }
                            cts.Dispose();
                        });
                    }

                    Thread.Sleep(ShortTimeout);

                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                }
            }
        }

        [Test]
        public void TailCall()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.TailCall);
        }

        [Test]
        public void Cancelation()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Cancelation);
        }

        [Test]
        public void PromiseResolve()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [Test]
        public void RetainAndRelease()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [Test]
        public void Cancel()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Cancel);
        }

        [Test]
        public void SendTwice()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.SendTwice);
        }

        [Test]
        public void Embargo()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoOnPromisedAnswer);
        }

        [Test]
        public void EmbargoError()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoError);
        }

        [Test]
        public void EmbargoNull()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [Test]
        public void CallBrokenPromise()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [Test]
        public void BootstrapReuse()
        {
            (var server, var client) = SetupClientServerPair();

            var counters = new Counters();
            var impl = new TestInterfaceImpl(counters);

            using (server)
            using (client)
            {
                //client.WhenConnected.Wait();

                server.Main = impl;
                for (int i = 0; i < 10; i++)
                {
                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        ((Proxy)main).WhenResolved.WrappedTask.Wait(MediumNonDbgTimeout);
                    }
                    Assert.IsFalse(impl.IsDisposed);
                }
            }

            Assert.IsTrue(impl.IsDisposed);
        }

        [Test]
        public void Ownership1()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership1);
        }

        [Test]
        public void Ownership2()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership2);
        }

        [Test]
        public void Ownership3()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership3);
        }
    }
}
