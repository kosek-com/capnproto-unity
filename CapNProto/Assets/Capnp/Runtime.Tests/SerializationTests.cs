﻿using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Capnproto_test.Capnp.Test.TestStructUnion;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class SerializationTests
    {
        [Test]
        public void ListOfBits1()
        {
            void CheckList(IEnumerable<bool> items)
            {
                int i = 0;
                foreach (bool bit in items)
                {
                    if (i == 63 || i == 66 || i == 129)
                        Assert.IsTrue(bit);
                    else
                        Assert.IsFalse(bit);

                    ++i;
                }
                Assert.AreEqual(130, i);
            }

            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfBitsSerializer>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            // Assert.AreEqual(0, list.Count); // Bug or feature? Uninitialized list's Count is -1
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.Throws<InvalidOperationException>(() => { list[0] = false; });
            list.Init(130);
            list[63] = true;
            list[65] = true;
            list[66] = true;
            list[65] = false;
            list[129] = true;
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[130]; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[130] = false; });
            Assert.IsFalse(list[0]);
            Assert.IsTrue(list[63]);
            Assert.IsFalse(list[64]);
            Assert.IsFalse(list[65]);
            Assert.IsTrue(list[66]);
            Assert.IsTrue(list[129]);
            var list2 = b.CreateObject<ListOfBitsSerializer>();
            list2.Init(null);
            list2.Init(list.ToArray());
            Assert.IsFalse(list2[0]);
            Assert.IsTrue(list2[63]);
            Assert.IsFalse(list2[64]);
            Assert.IsFalse(list2[65]);
            Assert.IsTrue(list2[66]);
            Assert.IsTrue(list2[129]);
            CheckList(list2);
            Assert.Throws<InvalidOperationException>(() => list.Init(4));
            DeserializerState d = list2;
            var list3 = d.RequireList().CastBool();
            CheckList(list3);
        }

        [Test]
        public void ListOfBits2()
        {
            var b = MessageBuilder.Create();
            var wrong = b.CreateObject<DynamicSerializerState>();
            wrong.SetListOfValues(8, 7);
            wrong.Allocate();
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValue(0, true));
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValues(new bool[7]));
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfValues(1, 70);
            list.ListWriteValue(0, true);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(-1, true));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(70, true));
            var values = new bool[70];
            values[63] = true;
            values[65] = true;
            list.ListWriteValues(values, true);
            var los = list.Rewrap<ListOfBitsSerializer>();
            Assert.IsTrue(!los[63]);
            Assert.IsFalse(!los[64]);
            Assert.IsTrue(!los[65]);
            Assert.IsFalse(!los[66]);
            Assert.Throws<ArgumentNullException>(() => list.ListWriteValues(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValues(new bool[1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValues(new bool[71]));
        }

        [Test]
        public void ListOfCaps()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var list = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.Throws<InvalidOperationException>(() => { list[0] = null; });
            list.Init(5);
            Assert.Throws<InvalidOperationException>(() => list.Init(1));
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[5]; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[-1] = null; });
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var c2 = new Counters();
            var cap2 = new TestInterfaceImpl(c2);
            list[0] = null;
            list[1] = cap1;
            list[2] = cap2;
            list[3] = cap1;
            list[4] = cap2;
            list[3] = null;
            Assert.IsTrue(list.All(p => p is Rpc.Proxy));
            var proxies = list.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(1, c2.CallCount);
            list[4].Foo(123u, true).Wait();
            Assert.AreEqual(2, c2.CallCount);

            var list2 = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            list2.Init(null);
            list2.Init(list.ToArray());
            proxies = list2.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list2[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(3, c2.CallCount);
            list2[4].Foo(123u, true).Wait();
            Assert.AreEqual(4, c2.CallCount);

            DeserializerState d = list2;
            var list3 = d.RequireList().CastCapList<ITestInterface>();
            proxies = list3.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list3[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(5, c2.CallCount);
            list3[4].Foo(123u, true).Wait();
            Assert.AreEqual(6, c2.CallCount);
        }

        [Test]
        public void ListOfEmpty()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfEmptySerializer>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            list.Init(987654321);
            Assert.AreEqual(987654321, list.Count);
            Assert.Throws<InvalidOperationException>(() => list.Init(42));
            DeserializerState d = list;
            int list2 = d.RequireList().CastVoid();
            Assert.AreEqual(987654321, list2);
        }

        [Test]
        public void ListOfPointers()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var list = b.CreateObject<ListOfPointersSerializer<DynamicSerializerState>>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.Throws<InvalidOperationException>(() => { list[0] = null; });
            list.Init(7);
            Assert.Throws<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(7, list.Count);
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[-1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[7] = null; });
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var obj1 = b.CreateObject<DynamicSerializerState>();
            obj1.SetObject(cap1);
            var obj2 = b.CreateObject<DynamicSerializerState>();
            obj2.SetStruct(1, 1);
            var lobs = b.CreateObject<ListOfBitsSerializer>();
            lobs.Init(1);
            var obj3 = lobs.Rewrap<DynamicSerializerState>();
            list[1] = obj1;
            list[2] = obj2;
            list[3] = obj3;
            Assert.IsNotNull(list[0]);
            Assert.AreEqual(ObjectKind.Nil, list[0].Kind);
            Assert.AreEqual(obj1, list[1]);
            Assert.AreEqual(obj2, list[2]);
            Assert.AreEqual(obj3, list[3]);
            var list2 = list.ToArray();
            Assert.IsNotNull(list2[0]);
            Assert.AreEqual(ObjectKind.Nil, list2[0].Kind);
            Assert.AreEqual(obj1, list2[1]);
            Assert.AreEqual(obj2, list2[2]);
            Assert.AreEqual(obj3, list2[3]);

            DeserializerState d = list;
            var list3 = d.RequireList().Cast(_ => _);
            Assert.AreEqual(7, list3.Count);
            Assert.IsNotNull(list3[0]);
            Assert.AreEqual(ObjectKind.Nil, list3[0].Kind);
            Assert.AreEqual(ObjectKind.Capability, list3[1].Kind);
            Assert.AreEqual(ObjectKind.Struct, list3[2].Kind);
            Assert.AreEqual(ObjectKind.ListOfBits, list3[3].Kind);
        }

        [Test]
        public void ListOfPrimitives()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.Throws<InvalidOperationException>(() => { list[0] = 1.0f; });
            list.Init(4);
            Assert.Throws<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            list[0] = 0.0f;
            list[1] = 1.0f;
            list[2] = 2.0f;
            list[3] = 3.0f;
            Assert.AreEqual(0.0f, list[0]);
            Assert.AreEqual(1.0f, list[1]);
            Assert.AreEqual(2.0f, list[2]);
            Assert.AreEqual(3.0f, list[3]);

            var list2 = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            list2.Init(null);
            list2.Init(list.ToArray());
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual(0.0f, list2[0]);
            Assert.AreEqual(1.0f, list2[1]);
            Assert.AreEqual(2.0f, list2[2]);
            Assert.AreEqual(3.0f, list2[3]);

            DeserializerState d = list2;
            var list3 = d.RequireList().CastFloat();
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual(0.0f, list3[0]);
            Assert.AreEqual(1.0f, list3[1]);
            Assert.AreEqual(2.0f, list3[2]);
            Assert.AreEqual(3.0f, list3[3]);
        }

        [Test]
        public void ListOfBytes()
        {
            var b = MessageBuilder.Create();
            var wrong = b.CreateObject<DynamicSerializerState>();
            wrong.SetListOfValues(1, 64);
            wrong.Allocate();
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValue(0, (byte)1));
            Assert.Throws<InvalidOperationException>(() => wrong.ListGetBytes());
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfValues(8, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(-1, (byte)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(64, (byte)1));
            list.ListWriteValue(1, (byte)1);
            list.ListWriteValue(2, (byte)2);
            CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, list.ListGetBytes().ToArray());            
        }

        [Test]
        public void ListOfUShortsWrongUse()
        {
            var b = MessageBuilder.Create();
            var wrong = b.CreateObject<DynamicSerializerState>();
            wrong.SetListOfValues(1, 64);
            wrong.Allocate();
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValue(0, (ushort)1));
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfValues(16, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(-1, (ushort)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(64, (ushort)1));
        }

        [Test]
        public void ListOfUShortsFromSegment()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            var array = new ushort[] { 1, 2, 3, 4, 5, 6 };
            var segment = new ArraySegment<ushort>(array, 1, 3);
            list.Init(segment);
            CollectionAssert.AreEqual(segment.ToArray(), list.ToArray());
        }

        [Test]
        public void ListOfUShortsFromDeser()
        {
            var b = MessageBuilder.Create();
            var list0 = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            var expected = new ushort[] { 2, 3, 4 };
            list0.Init(expected);
            var deser = ((DeserializerState)list0).RequireList().CastUShort();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            list.Init(deser);
            CollectionAssert.AreEqual(expected, list.ToArray());
        }

        [Test]
        public void ListOfUShortsFromSer()
        {
            var b = MessageBuilder.Create();
            var list0 = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            var expected = new ushort[] { 2, 3, 4 };
            list0.Init(expected);
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            list.Init(list0);
            CollectionAssert.AreEqual(expected, list.ToArray());
        }

        [Test]
        public void ListOfUShortsFromList()
        {
            var b = MessageBuilder.Create();
            var list0 = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            var expected = new List<ushort> { 2, 3, 4 };
            list0.Init(expected);
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            list.Init(list0);
            CollectionAssert.AreEqual(expected, list.ToArray());
        }

        [Test]
        public void ListOfUInts()
        {
            var b = MessageBuilder.Create();
            var wrong = b.CreateObject<DynamicSerializerState>();
            wrong.SetListOfValues(1, 64);
            wrong.Allocate();
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValue(0, 1u));
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfValues(32, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(-1, 1u));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(64, 1u));
        }

        [Test]
        public void ListOfULongs()
        {
            var b = MessageBuilder.Create();
            var wrong = b.CreateObject<DynamicSerializerState>();
            wrong.SetListOfValues(1, 64);
            wrong.Allocate();
            Assert.Throws<InvalidOperationException>(() => wrong.ListWriteValue(0, 1ul));
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfValues(64, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(-1, 1ul));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.ListWriteValue(64, 1ul));
        }

        [Test]
        public void ListOfStructs1()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfStructsSerializer<SomeStruct.WRITER>>();

            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            list.Init(4);
            Assert.Throws<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[5]; });
            list[0].SomeText = "0";
            list[1].SomeText = "1";
            list[2].SomeText = "2";
            list[3].SomeText = "3";
            Assert.AreEqual("0", list[0].SomeText);
            Assert.AreEqual("3", list[3].SomeText);

            var list2 = b.CreateObject<ListOfStructsSerializer<SomeStruct.WRITER>>();
            list2.Init(list.ToArray(), (dst, src) => { dst.SomeText = src.SomeText; dst.MoreText = src.MoreText; });
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual("0", list2[0].SomeText);
            Assert.AreEqual("3", list2[3].SomeText);

            DeserializerState d = list2;
            var list3 = d.RequireList().Cast(_ => new SomeStruct.READER(_));
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual("0", list3[0].SomeText);
            Assert.AreEqual("3", list3[3].SomeText);
        }

        [Test]
        public void ListOfStructs2()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<DynamicSerializerState>();
            list.SetListOfStructs(3, 4, 5);
            list.SetListOfStructs(3, 4, 5);
            Assert.Throws<InvalidOperationException>(() => list.SetListOfStructs(1, 4, 5));
            Assert.Throws<InvalidOperationException>(() => list.SetListOfStructs(3, 1, 5));
            Assert.Throws<InvalidOperationException>(() => list.SetListOfStructs(3, 4, 1));
            Assert.Throws<InvalidOperationException>(() => list.StructWriteData(0, 1, 1));
        }

        [Test]
        public void ListOfStructs3()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfStructsSerializer<SomeStruct.WRITER>>();
            Assert.Throws<InvalidOperationException>(() => list.Any());
            list.Init(3);
            int i = 0;
            foreach (var item in list)
            {
                item.SomeText = i.ToString();
                Assert.AreEqual(item, list[i]);
                ++i;
            }
            Assert.AreEqual("0", list[0].SomeText);
            Assert.AreEqual("1", list[1].SomeText);
            Assert.AreEqual("2", list[2].SomeText);
        }

        [Test]
        public void ListOfText()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfTextSerializer>();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.Throws<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.Throws<InvalidOperationException>(() => { list[0] = "foo"; });
            list.Init(4);
            Assert.Throws<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[5]; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[-1] = null; });
            list[0] = "0";
            list[2] = null;
            list[3] = "3";
            Assert.AreEqual("0", list[0]);
            Assert.IsNull(list[1]);
            Assert.IsNull(list[2]);
            Assert.AreEqual("3", list[3]);

            var list2 = b.CreateObject<ListOfTextSerializer>();
            list2.Init(list.ToArray());
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual("0", list2[0]);
            Assert.IsNull(list2[1]);
            Assert.IsNull(list2[2]);
            Assert.AreEqual("3", list2[3]);

            DeserializerState d = list2;
            var tmp = d.RequireList();
            var list3 = tmp.CastText2();
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual("0", list3[0]);
            Assert.IsNull(list3[1]);
            Assert.IsNull(list3[2]);
            Assert.AreEqual("3", list3[3]);
        }

        [Test]
        public void CapnpSerializableListBool()
        {
            var expected = new bool[] { true, false, true };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfBitsSerializer>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<bool>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListByte()
        {
            var expected = new byte[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<byte>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<byte>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListSByte()
        {
            var expected = new sbyte[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<sbyte>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<sbyte>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListUShort()
        {
            var expected = new ushort[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<ushort>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListShort()
        {
            var expected = new short[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<short>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<short>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListInt()
        {
            var expected = new int[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<int>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<int>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListUInt()
        {
            var expected = new uint[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<uint>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<uint>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListLong()
        {
            var expected = new long[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<long>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<long>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListULong()
        {
            var expected = new ulong[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ulong>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<ulong>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListFloat()
        {
            var expected = new float[] { -1.0f, 2.0f, -3.0f };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<float>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableListDouble()
        {
            var expected = new double[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<double>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<double>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [Test]
        public void CapnpSerializableStruct()
        {
            var b = MessageBuilder.Create();
            var obj = b.CreateObject<SomeStruct.WRITER>();
            obj.SomeText = "hello";
            obj.MoreText = "world";
            DeserializerState d = obj;
            var obj2 = CapnpSerializable.Create<SomeStruct>(d);
            Assert.AreEqual("hello", obj2.SomeText);
            Assert.AreEqual("world", obj2.MoreText);
        }

        [Test]
        public void CapnpSerializableCapList()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var wr = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var c2 = new Counters();
            var cap2 = new TestInterfaceImpl(c2);
            wr.Init(new ITestInterface[] { null, cap1, cap2 });
            DeserializerState d = wr;
            var list = CapnpSerializable.Create<IReadOnlyList<ITestInterface>>(d);
            list[1].Foo(123u, true);
            Assert.AreEqual(1, c1.CallCount);
            list[2].Foo(123u, true);
            Assert.AreEqual(1, c2.CallCount);
        }

        [Test]
        public void CapnpSerializableString()
        {
            string expected = "1, 2, 3";
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<byte>>();
            var bytes = Encoding.UTF8.GetBytes(expected);
            list.Init(bytes.Length + 1);
            bytes.CopyTo(list.Data);
            DeserializerState d = list;
            var str = CapnpSerializable.Create<string>(d);
            Assert.AreEqual(expected, str);
        }

        [Test]
        public void CapnpSerializableAnyPointer()
        {
            var b = MessageBuilder.Create();
            var obj = b.CreateObject<SomeStruct.WRITER>();
            obj.SomeText = "hello";
            obj.MoreText = "world";
            DeserializerState d = obj;
            var any = CapnpSerializable.Create<AnyPointer>(d);
            var obj2 = new SomeStruct.READER(any.State);
            Assert.AreEqual("hello", obj2.SomeText);
            Assert.AreEqual("world", obj2.MoreText);

        }

        class Unconstructible1 : ICapnpSerializable
        {
            public Unconstructible1(int annoyingParameter)
            {

            }

            public void Deserialize(DeserializerState state)
            {
                throw new NotImplementedException();
            }

            public void Serialize(SerializerState state)
            {
                throw new NotImplementedException();
            }
        }

        class Unconstructible2 : ICapnpSerializable
        {
            public Unconstructible2()
            {
                throw new NotImplementedException();
            }

            public void Deserialize(DeserializerState state)
            {
                throw new NotImplementedException();
            }

            public void Serialize(SerializerState state)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void CapnpSerializableWrongUse()
        {
            var d = default(DeserializerState);
            Assert.Throws<ArgumentException>(() => CapnpSerializable.Create<SerializationTests>(d));
            Assert.Throws<ArgumentException>(() => CapnpSerializable.Create<IReadOnlyList<SerializationTests>>(d));
            Assert.Throws<ArgumentException>(() => CapnpSerializable.Create<IReadOnlyList<DeserializerState>>(d));
            Assert.Throws<ArgumentException>(() => CapnpSerializable.Create<Unconstructible1>(d));
            Assert.Throws<ArgumentException>(() => CapnpSerializable.Create<Unconstructible2>(d));
        }

        [Test]
        public void DynamicSerializerStateBytes()
        {
            var expected = new byte[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastByte().ToArray());
        }

        [Test]
        public void DynamicSerializerStateSBytes()
        {
            var expected = new List<sbyte> { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastSByte().ToArray());
        }

        [Test]
        public void DynamicSerializerStateShorts()
        {
            var expected = new List<short> { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastShort().ToArray());
        }

        [Test]
        public void DynamicSerializerStateUShorts()
        {
            var expected = new ushort[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastUShort().ToArray());
        }

        [Test]
        public void DynamicSerializerStateInts()
        {
            var expected = new List<int> { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastInt().ToArray());
        }

        [Test]
        public void DynamicSerializerStateUInts()
        {
            var expected = new uint[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastUInt().ToArray());
        }

        [Test]
        public void DynamicSerializerStateLongs()
        {
            var expected = new List<long> { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastLong().ToArray());
        }

        [Test]
        public void DynamicSerializerStateULongs()
        {
            var expected = new ulong[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastULong().ToArray());
        }

        [Test]
        public void DynamicSerializerStateFloats()
        {
            var expected = new float[] { 1.0f, 2.0f, 3.0f };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastFloat().ToArray());
        }

        [Test]
        public void DynamicSerializerStateDoubles()
        {
            var expected = new double[] { 1.0, 2.0, 3.0 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastDouble().ToArray());
        }

        [Test]
        public void DynamicSerializerStateBools()
        {
            var expected = new bool[] { true, true, false };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastBool().ToArray());
        }

        [Test]
        public void DynamicSerializerStateStrings()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastText2().ToArray());
        }

        [Test]
        public void DynamicSerializerStateObjects()
        {
            var c = new Counters();
            var cap = new TestInterfaceImpl(c);
            var expected = new object[] { null, "foo", cap };

            var b = MessageBuilder.Create();
            b.InitCapTable();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var list = d.RequireList().Cast(_ => _);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list[0].Kind == ObjectKind.Nil);
            Assert.AreEqual("foo", list[1].RequireList().CastText());
            var proxy = list[2].RequireCap<ITestInterface>();
            proxy.Foo(123u, true);
            Assert.AreEqual(1, c.CallCount);
        }

        [Test]
        public void DynamicSerializerStateFromDeserializerState()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var lots = b.CreateObject<ListOfTextSerializer>();
            lots.Init(expected);
            DeserializerState d = lots;
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(d);
            DeserializerState d2 = dss;
            CollectionAssert.AreEqual(expected, d2.RequireList().CastText2().ToArray());
        }

        [Test]
        public void DynamicSerializerStateFromSerializerState()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var lots = b.CreateObject<ListOfTextSerializer>();
            lots.Init(expected);
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(lots);
            DeserializerState d2 = dss;
            CollectionAssert.AreEqual(expected, d2.RequireList().CastText2().ToArray());
        }

        [Test]
        public void EmptyListContract()
        {
            var list = new EmptyList<string>();
            Assert.AreEqual(0, list.Count);
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[-1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = list[0]; });
            Assert.AreEqual(0, list.ToArray().Length);
        }

        class TestSerializerStateStruct11 : SerializerState
        {
            public TestSerializerStateStruct11()
            {
                SetStruct(1, 1);
            }
        }

        class TestSerializerStateStruct20 : SerializerState
        {
            public TestSerializerStateStruct20()
            {
                SetStruct(2, 0);
            }
        }

        class TestSerializerStateStruct11X : SerializerState
        {
            public TestSerializerStateStruct11X()
            {
                SetStruct(1, 1);
            }
        }

        [Test]
        public void SerializerStateInvalidRewrap1()
        {
            var dss = new DynamicSerializerState(MessageBuilder.Create());
            dss.SetListOfValues(8, 1);
            Assert.Throws<InvalidOperationException>(() => dss.Rewrap<TestSerializerStateStruct11>());
        }

        [Test]
        public void SerializerStateInvalidRewrap2()
        {
            var dss = new DynamicSerializerState();
            dss.SetStruct(1, 0);
            Assert.Throws<InvalidOperationException>(() => dss.Rewrap<TestSerializerStateStruct11>());
        }

        [Test]
        public void SerializerStateInvalidRewrap3()
        {
            var dss = new DynamicSerializerState();
            dss.SetStruct(0, 1);
            Assert.Throws<InvalidOperationException>(() => dss.Rewrap<TestSerializerStateStruct11>());
        }

        [Test]
        public void SerializerStateInvalidRewrap4()
        {
            var dss = new DynamicSerializerState();
            dss.SetStruct(1, 0);
            Assert.Throws<InvalidOperationException>(() => dss.Rewrap<ListOfTextSerializer>());
        }

        [Test]
        public void UnboundSerializerState()
        {
            var dss = new DynamicSerializerState();
            dss.SetStruct(1, 0);
            Assert.Throws<InvalidOperationException>(() => dss.WriteData(0, 0));
        }

        [Test]
        public void LinkBadUsage1()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(0, 1);
            Assert.Throws<ArgumentNullException>(() => dss.Link(0, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.Link(-1, dss));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.Link(1, dss));
            dss.Link(0, dss);
            Assert.Throws<InvalidOperationException>(() => dss.Link(0, mb.CreateObject<DynamicSerializerState>()));
        }

        [Test]
        public void LinkBadUsage2()
        {
            var dss = new DynamicSerializerState(MessageBuilder.Create());
            dss.SetListOfPointers(1);
            Assert.Throws<ArgumentNullException>(() => dss.Link(0, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.Link(-1, dss));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.Link(1, dss));
        }

        [Test]
        public void NoCapTable()
        {
            var dss = new DynamicSerializerState(MessageBuilder.Create());
            dss.SetStruct(0, 1);
            Assert.Throws<InvalidOperationException>(() => dss.ReadCap(0));
            Assert.Throws<InvalidOperationException>(() => dss.ProvideCapability(new TestInterfaceImpl2()));
            Assert.Throws<InvalidOperationException>(() => dss.ProvideCapability(new TestInterface_Skeleton()));
        }

        [Test]
        public void StructReadCap1()
        {
            var dss = DynamicSerializerState.CreateForRpc();
            dss.SetStruct(0, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.ReadCap(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.ReadCap(100));
            Assert.IsTrue(dss.ReadCap(0).IsNull);
            dss.LinkToCapability(1, 99);
            dss.Link(2, dss);
            dss.Allocate();
            Assert.IsTrue(dss.ReadCap(0).IsNull);
            Assert.IsTrue(dss.ReadCap<ITestCallOrder>(0) is Proxy proxy && proxy.IsNull);
            Assert.Throws<Rpc.RpcException>(() => dss.ReadCap(1));
            Assert.Throws<Rpc.RpcException>(() => dss.ReadCap(2));
        }

        [Test]
        public void StructReadCap2()
        {
            var dss = DynamicSerializerState.CreateForRpc();
            dss.SetListOfStructs(1, 1, 1);
            dss.Allocate();
            Assert.Throws<InvalidOperationException>(() => dss.ReadCap(0));
        }

        [Test]
        public void Rewrap()
        {
            var mb = MessageBuilder.Create();
            var list1 = mb.CreateObject<ListOfStructsSerializer<TestSerializerStateStruct11>>();
            list1.Init(3);
            var list2 = list1.Rewrap<ListOfStructsSerializer<TestSerializerStateStruct11X>>();
            list2[0].WriteData(0, 0);
            Assert.Throws<InvalidOperationException>(() => list2.Rewrap<TestSerializerStateStruct11>());
            var obj = mb.CreateObject<TestSerializerStateStruct11>();
            var obj2 = obj.Rewrap<TestSerializerStateStruct11X>();
            Assert.Throws<InvalidOperationException>(() => obj2.Rewrap<TestSerializerStateStruct20>());
        }

        [Test]
        public void AllocatedNil()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.Allocate();
            Assert.Throws<InvalidOperationException>(() => dss.SetCapability(0));
            dss.SetCapability(null);
            Assert.Throws<InvalidOperationException>(() => dss.SetListOfPointers(1));
            Assert.Throws<InvalidOperationException>(() => dss.SetListOfStructs(1, 1, 1));
            Assert.Throws<InvalidOperationException>(() => dss.SetListOfValues(8, 1));
            Assert.Throws<InvalidOperationException>(() => dss.SetObject(mb.CreateObject<TestSerializerStateStruct11>()));
            dss.SetObject(null);
            Assert.Throws<InvalidOperationException>(() => dss.SetStruct(1, 1));
        }

        [Test]
        public void SetCapability1()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(1, 1);
            Assert.Throws<InvalidOperationException>(() => dss.SetCapability(null));
            Assert.Throws<InvalidOperationException>(() => dss.SetCapability(0));
        }

        [Test]
        public void SetCapability2()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetCapability(7);
            dss.SetCapability(7);
            Assert.Throws<InvalidOperationException>(() => dss.SetCapability(8));
            Assert.Throws<InvalidOperationException>(() => dss.SetCapability(null));
        }

        [Test]
        public void StructReadData()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            Assert.Throws<InvalidOperationException>(() => dss.StructReadData(0, 1));
            dss.SetStruct(2, 0);
            Assert.AreEqual(0ul, dss.StructReadData(0, 64));
            dss.Allocate();
            Assert.AreEqual(0ul, dss.StructReadData(0, 64));
            Assert.AreEqual(0ul, dss.StructReadData(256, 64));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.StructReadData(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.StructReadData(1, 64));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.StructReadData(0, 65));
            Assert.Throws<OverflowException>(() => dss.StructReadData(ulong.MaxValue, 2));
        }

        [Test]
        public void TryGetPointer()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            Assert.Throws<InvalidOperationException>(() => dss.TryGetPointer(0));
            dss.SetStruct(0, 1);
            Assert.IsNull(dss.TryGetPointer(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss.TryGetPointer(1));
        }

        [Test]
        public void ListBuildStruct()
        {
            var mb = MessageBuilder.Create();
            var dss1 = mb.CreateObject<DynamicSerializerState>();
            dss1.SetStruct(1, 1);
            Assert.Throws<InvalidOperationException>(() => dss1.ListBuildStruct(0));
            var dss2 = mb.CreateObject<DynamicSerializerState>();
            dss2.SetListOfStructs(2, 1, 1);
            var s0 = dss2.ListBuildStruct(0);
            Assert.AreEqual(s0, dss2.ListBuildStruct(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss2.ListBuildStruct(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss2.ListBuildStruct(2));
            var s1 = dss2.ListBuildStruct<TestSerializerStateStruct11>(1);
            Assert.AreEqual(s1, dss2.ListBuildStruct<TestSerializerStateStruct11>(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss2.ListBuildStruct<TestSerializerStateStruct11>(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => dss2.ListBuildStruct<TestSerializerStateStruct11>(2));
        }

        [Test]
        public void LinkObject()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(0, 5);
            var s1 = mb.CreateObject<SomeStruct.WRITER>();
            s1.SomeText = "foo";
            s1.MoreText = "bar";
            dss.LinkObject(0, s1);
            var s2 = mb.CreateObject<SomeStruct.WRITER>();
            s2.SomeText = "baz";
            s2.MoreText = "foobar";
            var d = (DeserializerState)s2;
            dss.LinkObject(1, d);
            var s3 = mb.CreateObject<SomeStruct.WRITER>();
            s3.SomeText = "0";
            var s4 = mb.CreateObject<SomeStruct.WRITER>();
            s4.SomeText = "1";
            var arr = new SomeStruct.WRITER[] { s3, s4 };
            dss.LinkObject(2, arr);
            Assert.Throws<InvalidOperationException>(() => dss.LinkObject(3, new object()));
            dss.LinkObject(3, new SomeStruct() { SomeText = "corge" });

            var t1 = dss.BuildPointer(0).Rewrap<SomeStruct.WRITER>();
            Assert.AreEqual("foo", t1.SomeText);
            Assert.AreEqual("bar", t1.MoreText);
            var t2 = dss.BuildPointer(1).Rewrap<SomeStruct.WRITER>();
            Assert.AreEqual("baz", t2.SomeText);
            Assert.AreEqual("foobar", t2.MoreText);
            var l = dss.BuildPointer(2).Rewrap<ListOfPointersSerializer<SomeStruct.WRITER>>();
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual("0", l[0].SomeText);
            Assert.AreEqual("1", l[1].SomeText);
            Assert.AreEqual("corge", dss.BuildPointer(3).Rewrap<SomeStruct.WRITER>().SomeText);
            Assert.AreEqual(ObjectKind.Nil, dss.BuildPointer(4).Kind);
        }

        [Test]
        public void Disposing()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.Dispose();
            dss.Dispose();
            var dss2 = DynamicSerializerState.CreateForRpc();
            var cap = new TestInterfaceImpl2();
            dss2.ProvideCapability(cap);
            dss2.Dispose();
            Assert.IsTrue(cap.IsDisposed);
            dss2.Dispose();
        }

        [Test]
        public void ProvideCapability()
        {
            var dss = DynamicSerializerState.CreateForRpc();
            var impl = new TestInterfaceImpl2();
            var p1 = (Proxy)Proxy.Share<ITestInterface>(impl);
            var p2 = (Proxy)Proxy.Share<ITestInterface>(impl);
            Assert.AreEqual(0u, dss.ProvideCapability(p1));
            Assert.AreEqual(0u, dss.ProvideCapability(p2.ConsumedCap));
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            Assert.AreEqual(0u, dss.ProvideCapability(CapabilityReflection.CreateSkeleton(impl)));
#pragma warning restore CS0618 // Typ oder Element ist veraltet
            Assert.IsTrue(p1.IsDisposed);
            Assert.IsFalse(p2.IsDisposed);
            p2.Dispose();
            Assert.IsFalse(impl.IsDisposed);
            dss.Dispose();
            Assert.IsTrue(impl.IsDisposed);
        }
    }
}
