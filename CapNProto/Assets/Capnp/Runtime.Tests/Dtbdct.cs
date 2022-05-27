using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class Dtbdct: TestBase
    {
        [Test]
        public void EmbargoOnPromisedAnswer()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoOnPromisedAnswer);
        }

        [Test]
        public void EmbargoOnImportedCap()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoOnImportedCap);
        }

        [Test]
        public void EmbargoError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoError);
        }

        [Test]
        public void EmbargoNull()
        {
            NewDtbdctTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [Test]
        public void CallBrokenPromise()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [Test]
        public void TailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.TailCall);
        }

        [Test]
        public void SendTwice()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SendTwice);
        }

        [Test]
        public void Cancel()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Cancel);
        }

        [Test]
        public void RetainAndRelease()
        {
            NewDtbdctTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [Test]
        public void PromiseResolve()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [Test]
        public void PromiseResolveLate()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolveLate);
        }

        [Test]
        public void PromiseResolveError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.PromiseResolveError);
        }

        [Test]
        public void Cancelation()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Cancelation);
        }

        [Test]
        public void ReleaseOnCancel()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ReleaseOnCancel);
        }

        [Test]
        public void Release()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Release);
        }

        [Test]
        public void Pipeline()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Pipeline);
        }

        [Test]
        public void Basic()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Basic);
        }

        [Test]
        public void BootstrapReuse()
        {
            NewDtbdctTestbed().RunTest(Testsuite.BootstrapReuse);
        }

        [Test]
        public void Ownership1()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership1);
        }

        [Test]
        public void Ownership2()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership2);
        }

        [Test]
        public void Ownership3()
        {
            NewDtbdctTestbed().RunTest(Testsuite.Ownership3);
        }

        [Test]
        public void SillySkeleton()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SillySkeleton);
        }

        [Test]
        public void ImportReceiverAnswer()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverAnswer);
        }

        [Test]
        public void ImportReceiverAnswerError()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverAnswerError);
        }

        [Test]
        public void ImportReceiverAnswerCanceled()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ImportReceiverCanceled);
        }

        [Test]
        public void ButNoTailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ButNoTailCall);
        }

        [Test]
        public void SecondIsTailCall()
        {
            NewDtbdctTestbed().RunTest(Testsuite.SecondIsTailCall);
        }

        [Test]
        public void ReexportSenderPromise()
        {
            NewDtbdctTestbed().RunTest(Testsuite.ReexportSenderPromise);
        }

        [Test]
        public void CallAfterFinish1()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallAfterFinish1);
        }

        [Test]
        public void CallAfterFinish2()
        {
            NewDtbdctTestbed().RunTest(Testsuite.CallAfterFinish2);
        }

        [Test]
        public void LegacyAccess()
        {
            NewDtbdctTestbed().RunTest(Testsuite.LegacyAccess);
        }
    }
}
