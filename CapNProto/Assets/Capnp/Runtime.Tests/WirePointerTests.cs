using NUnit.Framework;
using System;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class WirePointerTests
    {
        [Test]
        public void Struct()
        {
            var wp = default(WirePointer);
            wp.BeginStruct(17, 71);
            wp.Offset = -321;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.Struct, wp.Kind);
            Assert.AreEqual(17, wp.StructDataCount);
            Assert.AreEqual(71, wp.StructPtrCount);
            Assert.AreEqual(-321, wp.Offset);
        }

        [Test]
        public void StructAsListTag()
        {
            var wp = default(WirePointer);
            wp.BeginStruct(17, 71);
            wp.ListOfStructsElementCount = 555;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.Struct, wp.Kind);
            Assert.AreEqual(17, wp.StructDataCount);
            Assert.AreEqual(71, wp.StructPtrCount);
            Assert.AreEqual(555, wp.ListOfStructsElementCount);
        }

        [Test]
        public void ListOfEmpty()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfEmpty, 112);
            wp.Offset = 517;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfEmpty, wp.ListKind);
            Assert.AreEqual(112, wp.ListElementCount);
            Assert.AreEqual(517, wp.Offset);
        }

        [Test]
        public void ListOfBits()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfBits, 888);
            wp.Offset = -919;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfBits, wp.ListKind);
            Assert.AreEqual(888, wp.ListElementCount);
            Assert.AreEqual(-919, wp.Offset);
        }

        [Test]
        public void ListOfBytes()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfBytes, 1023);
            wp.Offset = 1027;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfBytes, wp.ListKind);
            Assert.AreEqual(1023, wp.ListElementCount);
            Assert.AreEqual(1027, wp.Offset);
        }

        [Test]
        public void ListOfShorts()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfShorts, 12345);
            wp.Offset = -12345;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfShorts, wp.ListKind);
            Assert.AreEqual(12345, wp.ListElementCount);
            Assert.AreEqual(-12345, wp.Offset);
        }

        [Test]
        public void ListOfInts()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfInts, 89400);
            wp.Offset = 111000;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfInts, wp.ListKind);
            Assert.AreEqual(89400, wp.ListElementCount);
            Assert.AreEqual(111000, wp.Offset);
        }

        [Test]
        public void ListOfLongs()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfLongs, 34500);
            wp.Offset = 8100999;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfLongs, wp.ListKind);
            Assert.AreEqual(34500, wp.ListElementCount);
            Assert.AreEqual(8100999, wp.Offset);
        }

        [Test]
        public void ListOfPointers()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfPointers, 12999777);
            wp.Offset = -11222000;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfPointers, wp.ListKind);
            Assert.AreEqual(12999777, wp.ListElementCount);
            Assert.AreEqual(-11222000, wp.Offset);
        }

        [Test]
        public void ListOfStructs()
        {
            var wp = default(WirePointer);
            wp.BeginList(ListKind.ListOfStructs, 77000);
            wp.Offset = -99888;
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.List, wp.Kind);
            Assert.AreEqual(ListKind.ListOfStructs, wp.ListKind);
            Assert.AreEqual(77000, wp.ListElementCount);
            Assert.AreEqual(-99888, wp.Offset);
        }

        [Test]
        public void Far()
        {
            var wp = default(WirePointer);
            wp.SetFarPointer(29, 777, false);
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.Far, wp.Kind);
            Assert.IsFalse(wp.IsDoubleFar);
            Assert.AreEqual(29u, wp.TargetSegmentIndex);
            Assert.AreEqual(777, wp.LandingPadOffset);
        }

        [Test]
        public void DoubleFar()
        {
            var wp = default(WirePointer);
            wp.SetFarPointer(92, 891, true);
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.Far, wp.Kind);
            Assert.IsTrue(wp.IsDoubleFar);
            Assert.AreEqual(92u, wp.TargetSegmentIndex);
            Assert.AreEqual(891, wp.LandingPadOffset);
        }

        [Test]
        public void Capability()
        {
            var wp = default(WirePointer);
            wp.SetCapability(123456);
            ulong v = wp;

            wp = v;
            Assert.AreEqual(PointerKind.Other, wp.Kind);
            Assert.AreEqual(0u, wp.OtherPointerKind);
            Assert.AreEqual(123456u, wp.CapabilityIndex);
        }

        [Test]
        public void OffsetOutOfBounds()
        {
            var wp = default(WirePointer);
            wp.BeginStruct(12345, 54321);
            Assert.Throws<ArgumentOutOfRangeException>(() => wp.Offset = 1 << 30);
            Assert.Throws<ArgumentOutOfRangeException>(() => wp.Offset = int.MinValue);
        }

        [Test]
        public void ElementCountOutOfBounds()
        {
            var wp = default(WirePointer);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => wp.BeginList(ListKind.ListOfBytes, 1 << 29));
            wp.BeginList(ListKind.ListOfInts, 1 << 29 - 1);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => wp.BeginList(ListKind.ListOfBytes, -1));
        }

        [Test]
        public void FarPointerOffsetOutOfBounds()
        {
            var wp = default(WirePointer);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => wp.SetFarPointer(1, 1 << 29, false));
        }
    }
}
