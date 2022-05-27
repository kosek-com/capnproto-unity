using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class CapabilityReflectionTests
    {
        [Test]
        public void ValidateCapabilityInterface()
        {
            Assert.Throws<ArgumentNullException>(() => CapabilityReflection.ValidateCapabilityInterface(null));
            CapabilityReflection.ValidateCapabilityInterface(typeof(ITestInterface));
            Assert.Throws<InvalidCapabilityInterfaceException>(() => CapabilityReflection.ValidateCapabilityInterface(typeof(CapabilityReflectionTests)));
        }

        [Test]
        public void IsValidCapabilityInterface()
        {
            Assert.Throws<ArgumentNullException>(() => CapabilityReflection.IsValidCapabilityInterface(null));
            Assert.IsTrue(CapabilityReflection.IsValidCapabilityInterface(typeof(ITestInterface)));
            Assert.IsFalse(CapabilityReflection.IsValidCapabilityInterface(typeof(CapabilityReflectionTests)));
        }
    }
}
