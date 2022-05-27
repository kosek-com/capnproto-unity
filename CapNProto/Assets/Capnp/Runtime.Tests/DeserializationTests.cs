﻿using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static Capnproto_test.Capnp.Test.TestStructUnion;

namespace Capnp.Net.Runtime.Tests
{
    [TestFixture]
    [Category("Coverage")]
    public class DeserializationTests
    {
        [Test]
        public void ListOfBytesAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(8, 10);
            ds.ListWriteValue(1, (byte)0x11);
            ds.ListWriteValue(2, (byte)0x22);
            ds.ListWriteValue(3, (byte)0x33);
            ds.ListWriteValue(4, (byte)0x44);
            ds.ListWriteValue(5, (byte)0x55);
            ds.ListWriteValue(6, (byte)0x66);
            ds.ListWriteValue(7, (byte)0x77);
            ds.ListWriteValue(8, (byte)0x88);
            ds.ListWriteValue(9, (byte)0x99);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(10, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual((byte)0x00, asListOfStructs[0].ReadDataByte(0));
            Assert.AreEqual((byte)0x11, asListOfStructs[1].ReadDataByte(0));
            Assert.AreEqual((byte)0x22, asListOfStructs[2].ReadDataByte(0));
            Assert.AreEqual((byte)0x33, asListOfStructs[3].ReadDataByte(0));
            Assert.AreEqual((byte)0x44, asListOfStructs[4].ReadDataByte(0));
            Assert.AreEqual((byte)0x55, asListOfStructs[5].ReadDataByte(0));
            Assert.AreEqual((byte)0x66, asListOfStructs[6].ReadDataByte(0));
            Assert.AreEqual((byte)0x77, asListOfStructs[7].ReadDataByte(0));
            Assert.AreEqual((byte)0x88, asListOfStructs[8].ReadDataByte(0));
            Assert.AreEqual((byte)0x99, asListOfStructs[9].ReadDataByte(0));
        }

        [Test]
        public void ListOfSBytesAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(8, 10);
            ds.ListWriteValue(1, (sbyte)-1);
            ds.ListWriteValue(2, (sbyte)-2);
            ds.ListWriteValue(3, (sbyte)-3);
            ds.ListWriteValue(4, (sbyte)-4);
            ds.ListWriteValue(5, (sbyte)-5);
            ds.ListWriteValue(6, (sbyte)-6);
            ds.ListWriteValue(7, (sbyte)-7);
            ds.ListWriteValue(8, (sbyte)-8);
            ds.ListWriteValue(9, (sbyte)-9);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(10, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual((sbyte)0, asListOfStructs[0].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-1, asListOfStructs[1].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-2, asListOfStructs[2].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-3, asListOfStructs[3].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-4, asListOfStructs[4].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-5, asListOfStructs[5].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-6, asListOfStructs[6].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-7, asListOfStructs[7].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-8, asListOfStructs[8].ReadDataSByte(0));
            Assert.AreEqual((sbyte)-9, asListOfStructs[9].ReadDataSByte(0));
        }

        [Test]
        public void ListOfUShortsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(16, 3);
            ds.ListWriteValue(1, (ushort)0x1111);
            ds.ListWriteValue(2, (ushort)0x2222);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(3, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual((ushort)0x0000, asListOfStructs[0].ReadDataUShort(0));
            Assert.AreEqual((ushort)0x1111, asListOfStructs[1].ReadDataUShort(0));
            Assert.AreEqual((ushort)0x2222, asListOfStructs[2].ReadDataUShort(0));
        }

        [Test]
        public void ListOfShortsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(16, 3);
            ds.ListWriteValue(1, (short)-0x1111);
            ds.ListWriteValue(2, (short)-0x2222);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(3, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual((short)0, asListOfStructs[0].ReadDataShort(0));
            Assert.AreEqual((short)-0x1111, asListOfStructs[1].ReadDataShort(0));
            Assert.AreEqual((short)-0x2222, asListOfStructs[2].ReadDataShort(0));
        }

        [Test]
        public void ListOfUIntsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(32, 2);
            ds.ListWriteValue(1, uint.MaxValue);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(2, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual(0u, asListOfStructs[0].ReadDataUInt(0));
            Assert.AreEqual(uint.MaxValue, asListOfStructs[1].ReadDataUInt(0));
        }

        [Test]
        public void ListOfIntsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(32, 2);
            ds.ListWriteValue(0, int.MinValue);
            ds.ListWriteValue(1, int.MaxValue);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(2, asListOfStructs.Count);
            Assert.AreEqual(ObjectKind.Value, asListOfStructs[0].Kind);
            Assert.AreEqual(int.MinValue, asListOfStructs[0].ReadDataInt(0));
            Assert.AreEqual(int.MaxValue, asListOfStructs[1].ReadDataInt(0));
        }

        [Test]
        public void ListOfULongsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(64, 2);
            ds.ListWriteValue(1, ulong.MaxValue);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(2, asListOfStructs.Count);
            Assert.AreEqual(0ul, asListOfStructs[0].ReadDataULong(0));
            Assert.AreEqual(ulong.MaxValue, asListOfStructs[1].ReadDataULong(0));
            Assert.Throws<IndexOutOfRangeException>(() => asListOfStructs[-1].ReadDataUShort(0));
            Assert.Throws<IndexOutOfRangeException>(() => asListOfStructs[3].ReadDataUShort(0));
            CollectionAssert.AreEqual(new ulong[] { 0, ulong.MaxValue }, asListOfStructs.Select(_ => _.ReadDataULong(0)).ToArray());
        }

        [Test]
        public void ListOfLongsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(64, 2);
            ds.ListWriteValue(0, long.MinValue);
            ds.ListWriteValue(1, long.MaxValue);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(2, asListOfStructs.Count);
            Assert.AreEqual(long.MinValue, asListOfStructs[0].ReadDataLong(0));
            Assert.AreEqual(long.MaxValue, asListOfStructs[1].ReadDataLong(0));
        }

        [Test]
        public void ListOfFloatsAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(32, 6);
            ds.ListWriteValue(0, 1.0f);
            ds.ListWriteValue(1, float.MinValue);
            ds.ListWriteValue(2, float.MaxValue);
            ds.ListWriteValue(3, float.NaN);
            ds.ListWriteValue(4, float.NegativeInfinity);
            ds.ListWriteValue(5, float.PositiveInfinity);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(6, asListOfStructs.Count);
            Assert.AreEqual(1.0f, asListOfStructs[0].ReadDataFloat(0));
            Assert.AreEqual(float.MinValue, asListOfStructs[1].ReadDataFloat(0));
            Assert.AreEqual(float.MaxValue, asListOfStructs[2].ReadDataFloat(0));
            Assert.AreEqual(float.NaN, asListOfStructs[3].ReadDataFloat(0));
            Assert.AreEqual(float.NegativeInfinity, asListOfStructs[4].ReadDataFloat(0));
            Assert.AreEqual(float.PositiveInfinity, asListOfStructs[5].ReadDataFloat(0));
        }

        [Test]
        public void ListOfDoublesAsListOfStructs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfValues(64, 6);
            ds.ListWriteValue(0, 1.0);
            ds.ListWriteValue(1, double.MinValue);
            ds.ListWriteValue(2, double.MaxValue);
            ds.ListWriteValue(3, double.NaN);
            ds.ListWriteValue(4, double.NegativeInfinity);
            ds.ListWriteValue(5, double.PositiveInfinity);

            DeserializerState d = ds;
            var asListOfStructs = d.RequireList().Cast(_ => _);
            Assert.AreEqual(6, asListOfStructs.Count);
            Assert.AreEqual(1.0, asListOfStructs[0].ReadDataDouble(0));
            Assert.AreEqual(double.MinValue, asListOfStructs[1].ReadDataDouble(0));
            Assert.AreEqual(double.MaxValue, asListOfStructs[2].ReadDataDouble(0));
            Assert.AreEqual(double.NaN, asListOfStructs[3].ReadDataDouble(0));
            Assert.AreEqual(double.NegativeInfinity, asListOfStructs[4].ReadDataDouble(0));
            Assert.AreEqual(double.PositiveInfinity, asListOfStructs[5].ReadDataDouble(0));
        }

        [Test]
        public void ListOfStructsAsListOfBools()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(3, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, false);
            ds.ListBuildStruct(2).WriteData(0, true);

            DeserializerState d = ds;
            var asListOfBools = d.RequireList().CastBool();
            Assert.AreEqual(3, asListOfBools.Count);
            Assert.AreEqual(false, asListOfBools[0]);
            Assert.AreEqual(false, asListOfBools[1]);
            Assert.AreEqual(true, asListOfBools[2]);
        }

        [Test]
        public void ListOfStructsAsListOfBytes()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(3, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, (byte)0x11);
            ds.ListBuildStruct(2).WriteData(0, (byte)0x22);

            DeserializerState d = ds;
            var asListOfBytes = d.RequireList().CastByte();
            Assert.AreEqual(3, asListOfBytes.Count);
            Assert.AreEqual(0, asListOfBytes[0]);
            Assert.AreEqual((byte)0x11, asListOfBytes[1]);
            Assert.AreEqual((byte)0x22, asListOfBytes[2]);
        }

        [Test]
        public void ListOfStructsAsListOfSBytes()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, sbyte.MinValue);

            DeserializerState d = ds;
            var asListOfSBytes = d.RequireList().CastSByte();
            Assert.AreEqual(2, asListOfSBytes.Count);
            Assert.AreEqual((sbyte)0, asListOfSBytes[0]);
            Assert.AreEqual(sbyte.MinValue, asListOfSBytes[1]);
        }

        [Test]
        public void ListOfStructsAsListOfUShorts()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, ushort.MaxValue);

            DeserializerState d = ds;
            var asListOfUShorts = d.RequireList().CastUShort();
            Assert.AreEqual(2, asListOfUShorts.Count);
            Assert.AreEqual((ushort)0, asListOfUShorts[0]);
            Assert.AreEqual(ushort.MaxValue, asListOfUShorts[1]);
        }

        [Test]
        public void ListOfStructsAsListOfShorts()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(0).WriteData(0, short.MinValue);
            ds.ListBuildStruct(1).WriteData(0, short.MaxValue);

            DeserializerState d = ds;
            var asListOfShorts = d.RequireList().CastShort();
            Assert.AreEqual(2, asListOfShorts.Count);
            Assert.AreEqual(short.MinValue, asListOfShorts[0]);
            Assert.AreEqual(short.MaxValue, asListOfShorts[1]);
        }

        [Test]
        public void ListOfStructsAsListOfUInts()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, uint.MaxValue);

            DeserializerState d = ds;
            var asListOfUInts = d.RequireList().CastUInt();
            Assert.AreEqual(2, asListOfUInts.Count);
            Assert.AreEqual(0u, asListOfUInts[0]);
            Assert.AreEqual(uint.MaxValue, asListOfUInts[1]);
        }

        [Test]
        public void ListOfStructsAsListOfInts()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(0).WriteData(0, int.MinValue);
            ds.ListBuildStruct(1).WriteData(0, int.MaxValue);

            DeserializerState d = ds;
            var asListOfInts = d.RequireList().CastInt();
            Assert.AreEqual(2, asListOfInts.Count);
            Assert.AreEqual(int.MinValue, asListOfInts[0]);
            Assert.AreEqual(int.MaxValue, asListOfInts[1]);
        }

        [Test]
        public void ListOfStructsAsListOfULongs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(1).WriteData(0, ulong.MaxValue);

            DeserializerState d = ds;
            var asListOfULongs = d.RequireList().CastULong();
            Assert.AreEqual(2, asListOfULongs.Count);
            Assert.AreEqual(0ul, asListOfULongs[0]);
            Assert.AreEqual(ulong.MaxValue, asListOfULongs[1]);
        }

        [Test]
        public void ListOfStructsAsListOfLongs()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(0).WriteData(0, long.MinValue);
            ds.ListBuildStruct(1).WriteData(0, long.MaxValue);

            DeserializerState d = ds;
            var asListOfLongs = d.RequireList().CastLong();
            Assert.AreEqual(2, asListOfLongs.Count);
            Assert.AreEqual(long.MinValue, asListOfLongs[0]);
            Assert.AreEqual(long.MaxValue, asListOfLongs[1]);
        }

        [Test]
        public void ListOfStructsAsListOfFloats()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(0).WriteData(0, float.NaN);
            ds.ListBuildStruct(1).WriteData(0, float.PositiveInfinity);

            DeserializerState d = ds;
            var asListOfFloats = d.RequireList().CastFloat();
            Assert.AreEqual(2, asListOfFloats.Count);
            Assert.AreEqual(float.NaN, asListOfFloats[0]);
            Assert.AreEqual(float.PositiveInfinity, asListOfFloats[1]);
        }

        [Test]
        public void ListOfStructsAsListOfDoubles()
        {
            var ds = new DynamicSerializerState(MessageBuilder.Create(128));
            ds.SetListOfStructs(2, 1, 0);
            ds.ListBuildStruct(0).WriteData(0, double.NegativeInfinity);
            ds.ListBuildStruct(1).WriteData(0, double.MaxValue);

            DeserializerState d = ds;
            var asListOfDoubles = d.RequireList().CastDouble();
            Assert.AreEqual(2, asListOfDoubles.Count);
            Assert.AreEqual(double.NegativeInfinity, asListOfDoubles[0]);
            Assert.AreEqual(double.MaxValue, asListOfDoubles[1]);
        }

        [Test]
        public void NestedLists()
        {
            var expected = new int[][] {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5 },
                new int[] { 6 } };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            Assert.Throws<NotSupportedException>(() => ld.CastText());
            var result = ld.Cast2D<int>();
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                CollectionAssert.AreEqual(expected[i], result[i].ToArray());
            }

            Assert.Throws<NotSupportedException>(() => ld.Cast2D<decimal>());
        }

        [Test]
        public void LinearListWrongUse()
        {
            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(new int[] { 1, 2, 3 });
            DeserializerState d = dss;
            var ld = d.RequireList();
            Assert.Throws<NotSupportedException>(() => ld.CastList());
            Assert.Throws<NotSupportedException>(() => ld.CastCapList<ITestInterface>());
        }

        [Test]
        public void NestedLists3D()
        {
            var expected = new int[][][] {
                new int[][]
                {
                    new int[] { 1, 2, 3 },
                    new int[] { 4, 5 },
                    new int[] { 6 }
                },
                new int[][]
                {
                    new int[] { 1, 2, 3 },
                    new int[0]
                },
                new int[0][]
            };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.Cast3D<int>();
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = result[i];
                Assert.AreEqual(expected[i].Length, inner.Count);

                for (int j = 0; j < expected[i].Length; j++)
                {
                    var inner2 = inner[j];
                    CollectionAssert.AreEqual(expected[i][j], inner2.ToArray());
                }
            }
        }

        [Test]
        public void NestedListsND()
        {
            var expected = new int[][][] {
                new int[][]
                {
                    new int[] { 1, 2, 3 },
                    new int[] { 4, 5 },
                    new int[] { 6 } 
                },
                new int[][]
                {
                    new int[] { 1, 2, 3 },
                    new int[0]
                },
                new int[0][] 
            };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();            
            var result = (IReadOnlyList<object>)ld.CastND<int>(3);
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = (IReadOnlyList<object>)result[i];
                Assert.AreEqual(expected[i].Length, inner.Count);

                for (int j = 0; j < expected[i].Length; j++)
                {
                    var inner2 = (IReadOnlyList<int>)inner[j];
                    CollectionAssert.AreEqual(expected[i][j], inner2.ToArray());
                }
            }
        }

        [Test]
        public void NestedLists3DStruct()
        {
            var expected = new List<List<List<SomeStruct.WRITER>>>();

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            for (int i = 0; i < 3; i++)
            {
                expected.Add(new List<List<SomeStruct.WRITER>>());

                for (int j = 0; j <= i; j++)
                {
                    expected[i].Add(new List<SomeStruct.WRITER>());

                    for (int k = 0; k <= j; k++)
                    {
                        var x = b.CreateObject<SomeStruct.WRITER>();
                        x.SomeText = $"{i}, {j}, {k}";
                        expected[i][j].Add(x);
                    }
                }
            }

            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.Cast3D<SomeStruct.READER>(_ => new SomeStruct.READER(_));
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = result[i];
                Assert.AreEqual(i + 1, inner.Count);

                for (int j = 0; j < inner.Count; j++)
                {
                    var inner2 = inner[j];
                    Assert.AreEqual(j + 1, inner2.Count);

                    for (int k = 0; k < inner2.Count; k++)
                    {
                        Assert.AreEqual($"{i}, {j}, {k}", inner2[k].SomeText);
                    }
                }
            }
        }

        [Test]
        public void NestedListsNDStruct()
        {
            var expected = new List<List<List<SomeStruct.WRITER>>>();

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            for (int i = 0; i < 3; i++)
            {
                expected.Add(new List<List<SomeStruct.WRITER>>());

                for (int j = 0; j <= i; j++)
                {
                    expected[i].Add(new List<SomeStruct.WRITER>());

                    for (int k = 0; k <= j; k++)
                    {
                        var x = b.CreateObject<SomeStruct.WRITER>();
                        x.SomeText = $"{i}, {j}, {k}";
                        expected[i][j].Add(x);
                    }
                }
            }

            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = (IReadOnlyList<object>)ld.CastND<SomeStruct.READER>(3, _ => new SomeStruct.READER(_));
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = (IReadOnlyList<object>)result[i];
                Assert.AreEqual(i + 1, inner.Count);

                for (int j = 0; j < inner.Count; j++)
                {
                    var inner2 = (IReadOnlyList<SomeStruct.READER>)inner[j];
                    Assert.AreEqual(j + 1, inner2.Count);

                    for (int k = 0; k < inner2.Count; k++)
                    {
                        Assert.AreEqual($"{i}, {j}, {k}", inner2[k].SomeText);
                    }
                }
            }
        }

        [Test]
        public void NestedLists2DStruct()
        {
            var expected = new List<List<SomeStruct.WRITER>>();

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            for (int i = 0; i < 3; i++)
            {
                expected.Add(new List<SomeStruct.WRITER>());

                for (int j = 0; j <= i; j++)
                {
                    var x = b.CreateObject<SomeStruct.WRITER>();
                    x.SomeText = $"{i}, {j}";
                    expected[i].Add(x);
                }
            }

            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.Cast2D(_ => new SomeStruct.READER(_));
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = result[i];
                Assert.AreEqual(i + 1, inner.Count);

                for (int j = 0; j < inner.Count; j++)
                {
                    Assert.AreEqual($"{i}, {j}", inner[j].SomeText);
                }
            }
        }

        [Test]
        public void ListOfEnums()
        {
            var expected = new TestEnum[] { TestEnum.bar, TestEnum.baz, TestEnum.corge };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.CastEnums(_ => (TestEnum)_);
            CollectionAssert.AreEqual(expected, result.ToArray());
        }

        [Test]
        public void NestedLists2DEnum()
        {
            var expected = new TestEnum[][]
            {
                new TestEnum[] { TestEnum.bar, TestEnum.baz, TestEnum.corge },
                new TestEnum[] { TestEnum.corge, TestEnum.foo, TestEnum.garply }
            };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.CastEnums2D(_ => (TestEnum)_);
            Assert.AreEqual(expected.Length, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                CollectionAssert.AreEqual(expected[i], result[i].ToArray());
            }
        }

        [Test]
        public void NestedLists3DEnum()
        {
            var expected = new TestEnum[][][] {
                new TestEnum[][]
                {
                    new TestEnum[] { TestEnum.qux, TestEnum.quux, TestEnum.grault },
                    new TestEnum[] { TestEnum.garply, TestEnum.foo },
                    new TestEnum[] { TestEnum.corge }
                },
                new TestEnum[][]
                {
                    new TestEnum[] { TestEnum.baz, TestEnum.bar },
                    new TestEnum[0]
                },
                new TestEnum[0][]
            };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = ld.CastEnums3D(_ => (TestEnum)_);
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = result[i];
                Assert.AreEqual(expected[i].Length, inner.Count);

                for (int j = 0; j < expected[i].Length; j++)
                {
                    var inner2 = inner[j];
                    CollectionAssert.AreEqual(expected[i][j], inner2.ToArray());
                }
            }
        }

        [Test]
        public void NestedListsNDEnum()
        {
            var expected = new TestEnum[][][] {
                new TestEnum[][]
                {
                    new TestEnum[] { TestEnum.qux, TestEnum.quux, TestEnum.grault },
                    new TestEnum[] { TestEnum.garply, TestEnum.foo },
                    new TestEnum[] { TestEnum.corge }
                },
                new TestEnum[][]
                {
                    new TestEnum[] { TestEnum.baz, TestEnum.bar },
                    new TestEnum[0]
                },
                new TestEnum[0][]
            };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var ld = d.RequireList();
            var result = (IReadOnlyList<object>)ld.CastEnumsND(3, _ => (TestEnum)_);
            Assert.AreEqual(3, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var inner = (IReadOnlyList<object>)result[i];
                Assert.AreEqual(expected[i].Length, inner.Count);

                for (int j = 0; j < expected[i].Length; j++)
                {
                    var inner2 = (IReadOnlyList<TestEnum>)inner[j];
                    CollectionAssert.AreEqual(expected[i][j], inner2.ToArray());
                }
            }
        }

        [Test]
        public void NestedLists2DVoid()
        {
            var b = MessageBuilder.Create();
            var s = b.CreateObject<ListOfPointersSerializer<ListOfEmptySerializer>>();
            s.Init(3);
            s[0].Init(4);
            s[1].Init(5);
            s[2].Init(6);
            DeserializerState d = s;
            var voids = d.RequireList().CastVoid2D();
            CollectionAssert.AreEqual(new int[] { 4, 5, 6 }, voids.ToArray());
        }

        [Test]
        public void NestedLists3DVoid()
        {
            var expected = new int[][] {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5 },
                new int[] { 6 } };

            var b = MessageBuilder.Create();
            var s = b.CreateObject<ListOfPointersSerializer<ListOfPointersSerializer<ListOfEmptySerializer>>>();
            s.Init(expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                s[i].Init(expected[i], (l, j) => l.Init(j));
            }
            DeserializerState d = s;
            var voids = d.RequireList().CastVoid3D();
            Assert.AreEqual(expected.Length, voids.Count);
            for (int i = 0; i < expected.Length; i++)
            {
                CollectionAssert.AreEqual(expected[i], voids[i].ToArray());
            }
        }

        [Test]
        public void ListOfEmpty()
        {
            var expected = new TestEnum[] { TestEnum.bar, TestEnum.baz, TestEnum.corge };

            var b = MessageBuilder.Create();
            var loes = b.CreateObject<ListOfEmptySerializer>();
            loes.Init(12345678);
            DeserializerState d = loes;
            var ld = d.RequireList();
            Assert.AreEqual(ListKind.ListOfEmpty, ld.Kind);
            if (!(ld is ListOfEmptyDeserializer loed))
            {
                Assert.Fail("List did not deserialize back to ListOfEmptyDeserializer");
                return;
            }
            Assert.AreEqual(12345678, loed.Count);
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = loed[-1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = loed[12345678]; });
            _ = loed[12345677];
            var kind = loed.Cast(_ => _.Kind).Take(1).Single();
            Assert.AreEqual(ObjectKind.Nil, kind);
        }

        [Test]
        public void DeserializerStateBadConv()
        {
            SerializerState s = null;
            Assert.Throws<ArgumentNullException>(() => { var _ = (DeserializerState)s; });
            s = new DynamicSerializerState();
            Assert.Throws<InvalidOperationException>(() => { var _ = (DeserializerState)s; });
        }

        [Test]
        public void BadPointers()
        {
            var data = new ulong[1];
            var wf = new WireFrame(new Memory<ulong>[] { new Memory<ulong>(data) });
            var d0 = DeserializerState.CreateRoot(wf);
            Assert.AreEqual(ObjectKind.Nil, d0.Kind);
            WirePointer p = default;
            p.BeginStruct(1, 0);
            p.Offset = 0;
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfBits, 64);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfBytes, 8);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfEmpty, 6400);
            data[0] = p;
            var d1 = DeserializerState.CreateRoot(wf);
            p.BeginList(ListKind.ListOfInts, 2);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfLongs, 1);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfPointers, 1);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfShorts, 4);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.BeginList(ListKind.ListOfStructs, 1);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.SetFarPointer(0, 0, false);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.SetFarPointer(1, 0, false);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.SetFarPointer(0, 1, false);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));
            p.SetFarPointer(0, 0, true);
            data[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf));

            var data2 = new ulong[3];
            var wf2 = new WireFrame(new Memory<ulong>[] { new Memory<ulong>(data2) });
            p.BeginList(ListKind.ListOfStructs, 1);
            data2[0] = p;
            data2[1] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf2));

            p.SetFarPointer(0, 1, true);
            data2[0] = p;
            data2[1] = 0;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf2));

            p.SetFarPointer(0, 1, false);
            data2[1] = p;
            data2[2] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf2));

            p.SetFarPointer(0, 2, true);
            data2[0] = p;
            Assert.Throws<DeserializationException>(() => DeserializerState.CreateRoot(wf2));
        }

        [Test]
        public void ReadCap1()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(0, 1);
            DeserializerState ds = dss;
            Assert.Throws<ArgumentOutOfRangeException>(() => ds.ReadCap(-1));
            Assert.Throws<InvalidOperationException>(() => ds.ReadCap(0));
        }

        [Test]
        public void ReadCap2()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var dss1 = mb.CreateObject<DynamicSerializerState>();
            var dss2 = mb.CreateObject<DynamicSerializerState>();
            dss2.SetStruct(1, 1);
            dss1.SetStruct(0, 2);
            dss1.Link(0, dss2);
            dss1.LinkToCapability(1, 7);
            var d = (DeserializerState)dss1;
            Assert.Throws<Rpc.RpcException>(() => d.ReadCap(0));
            Assert.Throws<Rpc.RpcException>(() => d.ReadCap(1));
        }

        [Test]
        public void Read1()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(1, 0);
            dss.WriteData(0, ulong.MaxValue);
            var d = (DeserializerState)dss;
            Assert.AreEqual(ushort.MaxValue, d.ReadDataUShort(48));
            Assert.Throws<ArgumentOutOfRangeException>(() => d.ReadDataUShort(49));
            Assert.AreEqual((ushort)0, d.ReadDataUShort(64));
            Assert.IsNotNull(d.StructReadPointer(7));
            Assert.Throws<DeserializationException>(() => d.RequireCapList<ITestInterface>());
        }

        [Test]
        public void Read2()
        {
            var mb = MessageBuilder.Create();
            var dss = mb.CreateObject<ListOfBitsSerializer>();
            dss.Init(50);
            var d = (DeserializerState)dss;
            Assert.Throws<DeserializationException>(() => d.ReadDataUInt(0));
            Assert.Throws<DeserializationException>(() => d.StructReadPointer(0));
        }

        [Test]
        public void ReadCapList()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var dss = mb.CreateObject<DynamicSerializerState>();
            dss.SetStruct(0, 1);
            var loc = mb.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            loc.Init(1);
            loc[0] = new TestInterfaceImpl2();
            dss.LinkObject(0, loc);
            var d = (DeserializerState)dss;
            var cl = d.ReadCapList<ITestInterface>(0);
            Assert.AreEqual(1, cl.Count);
            Assert.IsNotNull(cl[0]);
        }

        [Test]
        public void ReadCap()
        {
            var dss = DynamicSerializerState.CreateForRpc();
            dss.SetStruct(0, 1);
            dss.LinkObject<ITestInterface>(0, new TestInterfaceImpl2());
            var d = (DeserializerState)dss;
            Assert.IsNotNull(d.ReadCap(0));
            Assert.IsNotNull(d.ReadCap<ITestInterface>(0));
        }

        [Test]
        public void RequireCap1()
        {
            var dss = DynamicSerializerState.CreateForRpc();
            dss.SetStruct(1, 1);
            var d = (DeserializerState)dss;
            Assert.Throws<DeserializationException>(() => d.RequireCap<ITestInterface>());
        }

        [Test]
        public void RequireCap2()
        {
            DeserializerState d = default;
            d.Kind = ObjectKind.Capability;
            Assert.Throws<InvalidOperationException>(() => d.RequireCap<ITestInterface>());
        }
    }
}
