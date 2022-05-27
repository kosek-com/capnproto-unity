﻿using System;

namespace Capnp.Rpc.Interception
{
    class CensorCapability : RefCountingCapability
    {
        public CensorCapability(ConsumedCapability interceptedCapability, IInterceptionPolicy policy)
        {
            InterceptedCapability = interceptedCapability;
            interceptedCapability.AddRef();
            Policy = policy;
        }

        public ConsumedCapability InterceptedCapability { get; }
        public IInterceptionPolicy Policy { get; }

        protected override void ReleaseRemotely()
        {
            InterceptedCapability.Release();
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cc = new CallContext(this, interfaceId, methodId, args);
            Policy.OnCallFromAlice(cc);
            return cc.Answer;
        }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            writer.which = CapDescriptor.WHICH.SenderHosted;
            writer.SenderHosted = endpoint.AllocateExport(AsSkeleton(), out bool _);
            return null;
        }
    }
}