﻿using Capnp.Util;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Application-level wrapper for consumer-side capabilities.
    /// The code generator will produce a Proxy specialization for each capability interface.
    /// </summary>
    public class Proxy : IDisposable, IResolvingCapability
    {
        /// <summary>
        /// Creates a new proxy object for an existing implementation or proxy, sharing its ownership.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="obj">instance to share</param>
        /// <returns></returns>
        public static T Share<T>(T obj) where T : class
        {
            if (obj is Proxy proxy)
                return proxy.Cast<T>(false);
            else
                return BareProxy.FromImpl(obj).Cast<T>(true);
        }

        bool _disposedValue = false;

        /// <summary>
        /// Completes when the capability gets resolved.
        /// </summary>
        public StrictlyOrderedAwaitTask WhenResolved
        {
            get
            {
                return ConsumedCap is IResolvingCapability resolving ?
                    resolving.WhenResolved : Task.CompletedTask.EnforceAwaitOrder();
            }
        }

        /// <summary>
        /// Returns the resolved capability
        /// </summary>
        /// <typeparam name="T">Capability interface or <see cref="BareProxy"/></typeparam>
        /// <returns>the resolved capability, or null if it did not resolve yet</returns>
        public T? GetResolvedCapability<T>() where T : class
        {
            if (ConsumedCap is IResolvingCapability resolving)
                return resolving.GetResolvedCapability<T>();
            else
                return CapabilityReflection.CreateProxy<T>(ConsumedCap) as T;
        }

        ConsumedCapability _consumedCap = NullCapability.Instance;

        /// <summary>
        /// Underlying low-level capability
        /// </summary>
        public ConsumedCapability ConsumedCap => _disposedValue ?
            throw new ObjectDisposedException(nameof(Proxy)) : _consumedCap;

        /// <summary>
        /// Whether is this a broken capability.
        /// </summary>
        public bool IsNull => _consumedCap == NullCapability.Instance;

        /// <summary>
        /// Whether <see cref="Dispose()"/> was called on this Proxy.
        /// </summary>
        public bool IsDisposed => _disposedValue;

        static async void DisposeCtrWhenReturned(CancellationTokenRegistration ctr, IPromisedAnswer answer)
        {
            try { await answer.WhenReturned; }
            catch { }
            finally { ctr.Dispose(); }
        }

        /// <summary>
        /// Calls a method of this capability.
        /// </summary>
        /// <param name="interfaceId">Interface ID to call</param>
        /// <param name="methodId">Method ID to call</param>
        /// <param name="args">Method arguments ("param struct")</param>
        /// <param name="obsoleteAndIgnored">This flag is ignored. It is there to preserve compatibility with the
        /// code generator and will be removed in future versions.</param>
        /// <param name="cancellationToken">For cancelling an ongoing method call</param>
        /// <returns>An answer promise</returns>
        /// <exception cref="ObjectDisposedException">This instance was disposed, or transport-layer stream was disposed.</exception>
        /// <exception cref="InvalidOperationException">Capability is broken.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        protected internal IPromisedAnswer Call(ulong interfaceId, ushort methodId, DynamicSerializerState args, 
            bool obsoleteAndIgnored, CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
            {
                args.Dispose();
                throw new ObjectDisposedException(nameof(Proxy));
            }

            var answer = ConsumedCap.DoCall(interfaceId, methodId, args);

            if (cancellationToken.CanBeCanceled)
            {
                DisposeCtrWhenReturned(cancellationToken.Register(answer.Dispose), answer);
            }

            return answer;
        }

        /// <summary>
        /// Constructs a null instance.
        /// </summary>
        public Proxy()
        {
#if DebugFinalizers
            CreatorStackTrace = Environment.StackTrace;
#endif
        }

        internal Proxy(ConsumedCapability cap): this()
        {
            Bind(cap);
        }

        public void Bind(ConsumedCapability cap)
        {
            if (ConsumedCap != NullCapability.Instance)
                throw new InvalidOperationException("Proxy was already bound");

            _consumedCap = cap ?? throw new ArgumentNullException(nameof(cap));
            cap.AddRef();

#if DebugFinalizers
            if (_consumedCap != null)
                _consumedCap.OwningProxy = this;
#endif
        }

        internal async Task<Skeleton> GetProvider()
        {
            var unwrapped = await ConsumedCap.Unwrap();
            return unwrapped.AsSkeleton();
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _consumedCap.Release();
                }
                else
                {
                    // When called from the Finalizer, we must not throw.
                    // But when reference counting goes wrong, ConsumedCapability.Release() will throw an InvalidOperationException.
                    // The only option here is to suppress that exception.
                    try { _consumedCap?.Release(); }
                    catch { }
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Proxy()
        {
#if DebugFinalizers
            Debugger.Log(0, "DebugFinalizers", $"Caught orphaned Proxy, created from here: {CreatorStackTrace}.");
#endif

            Dispose(false);
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Casts this Proxy to a different capability interface.
        /// </summary>
        /// <typeparam name="T">Desired capability interface</typeparam>
        /// <param name="disposeThis">Whether to Dispose() this Proxy instance</param>
        /// <returns>Proxy for desired capability interface</returns>
        /// <exception cref="InvalidCapabilityInterfaceException"><typeparamref name="T"/> did not qualify as capability interface.</exception>
        /// <exception cref="InvalidOperationException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="ArgumentException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">Problem with instatiating the Proxy (constructor threw exception).</exception>
        /// <exception cref="MemberAccessException">Caller does not have permission to invoke the Proxy constructor.</exception>
        /// <exception cref="TypeLoadException">Problem with building the Proxy type, or problem with loading some dependent class.</exception>
        public T Cast<T>(bool disposeThis) where T: class
        {
            using (disposeThis ? this : null)
            {
                return (CapabilityReflection.CreateProxy<T>(ConsumedCap) as T)!;
            }
        }

        internal Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(Proxy));

            if (ConsumedCap == null)
            {
                writer.which = CapDescriptor.WHICH.None;
                return null;
            }
            else
            {
                return ConsumedCap.Export(endpoint, writer);
            }
        }

#if DebugFinalizers
        string CreatorStackTrace { get; set; }
#endif
    }
}