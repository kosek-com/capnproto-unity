﻿using Capnp.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LocalAnswer : IPromisedAnswer
    {
        readonly CancellationTokenSource _cts;

        public LocalAnswer(CancellationTokenSource cts, Task<DeserializerState> call)
        {
            _cts = cts ?? throw new ArgumentNullException(nameof(cts));
            WhenReturned = call?.EnforceAwaitOrder() ?? throw new ArgumentNullException(nameof(call));

            CleanupAfterReturn();
        }

        async void CleanupAfterReturn()
        {
            try { await WhenReturned; }
            catch { }
            finally { _cts.Dispose(); }
        }

        public StrictlyOrderedAwaitTask<DeserializerState> WhenReturned { get; }

        public bool IsTailCall => false;

        public ConsumedCapability Access(MemberAccessPath access)
        {
            return new LocalAnswerCapability(WhenReturned, access);
        }

        public ConsumedCapability Access(MemberAccessPath _, Task<IDisposable?> task)
        {
            return new LocalAnswerCapability(task.AsProxyTask());
        }

        public void Dispose()
        {
            try { _cts.Cancel(); }
            catch (ObjectDisposedException) { }
        }
    }
}