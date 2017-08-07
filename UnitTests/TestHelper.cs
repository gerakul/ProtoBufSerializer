using Gerakul.ProtoBufSerializer;
using Google.Protobuf;
using System;
using System.IO;
using System.Linq;

namespace UnitTests
{
    public static class TestHelper
    {
        public static byte[] GetSerializedEtalonMess()
        {
            string base64 = "CAES9AQJWtcwqWwhlEsVexQWQBiF//////////8BIPjLm4u0/P///wEowQIwiLTk9MsDOMcBQI/ogqXgNU3IAAAAUQgCEAc0AAAAXeuuzOBh+EWLH4b///9oAXI" + 
                "ScXdlIDEyMyAhQCMg0LnRhtGDegUBAmT/A6oGGFrXMKlsIZTLMzMzMzMz8z8AAAAAAAAIQLIGDAAAsMCamZk/AABAQLoGEAH+//////////8B/////wfCBhT///////////8BAv" +
                "//////////f8oGBwEA/////w/SBg2QTv///////////wEC2gYJoJwB/////w8G4gYRlZPYn+5H////////////AQDqBgxA4gEA////fwAAAADyBhjLBPtxHwEAAAAAAAAAAAAA/" +
                "////////3/6BgzAHf7/////fwAAAICCBxjAHf7///////////////9/AAAAAAAAAICKBwMBAQCSBwCSBxJxd2UgMTIzICFAIyDQudGG0YOSBwZxd2VydHmaBwMBAgOaBwUBAmT/" +
                "A5oHAwAAAMoMGFrXMKlsIZTL////////7//////////vf9IMDAAAsMCamZk/AABAQNoMFQH+//////////8BgICAgPj/////AeIMFP///////////wEC//////////9/6gwHCgD" +
                "/////D/IMDAD///////////8BAvoMCaCcAf7///8PAIINEZWT2J/uR////////////wEAig0MQOIBAP///38AAAAAkg0YywT7cR8BAAAAAAAAAAAAAP////////9/mg0MwB3+/w" +
                "AAAAAAAACAog0YwB3+////////////////fwAAAAAAAAAAqgYY5CjMY2ddUzQzMzMzMzPzvwAAAAAAAAAAsgb0BAkAAAAAAADwPxV7FBZAGIX//////////wEg+Mubi7T8////A" +
                "SjBAjCItOT0ywM4xwFAj+iCpeA1TcgAAABRCAIQBzQAAABd667M4GH4RYsfhv///2gBchJxd2UgMTIzICFAIyDQudGG0YN6BQECZP8DqgYYWtcwqWwhlMszMzMzMzPzPwAAAAAA" +
                "AAhAsgYMAACwwJqZmT8AAEBAugYQAf7//////////wH/////B8IGFP///////////wEC//////////9/ygYHAQD/////D9IGDZBO////////////AQLaBgmgnAH/////DwbiBhG" +
                "Vk9if7kf///////////8BAOoGDEDiAQD///9/AAAAAPIGGMsE+3EfAQAAAAAAAAAAAAD/////////f/oGDMAd/v////9/AAAAgIIHGMAd/v///////////////38AAAAAAAAAgI" +
                "oHAwEBAJIHAJIHEnF3ZSAxMjMgIUAjINC50YbRg5IHBnF3ZXJ0eZoHAwECA5oHBQECZP8DmgcDAAAAygwYWtcwqWwhlMv////////v/////////+9/0gwMAACwwJqZmT8AAEBA2" +
                "gwVAf7//////////wGAgICA+P////8B4gwU////////////AQL//////////3/qDAcKAP////8P8gwMAP///////////wEC+gwJoJwB/v///w8Agg0RlZPYn+5H////////////" +
                "AQCKDQxA4gEA////fwAAAACSDRjLBPtxHwEAAAAAAAAAAAAA/////////3+aDQzAHf7/AAAAAAAAAICiDRjAHf7///////////////9/AAAAAAAAAACyBvQECVrXMKlsIZRLFc3" +
                "MjD8Yhf//////////ASD4y5uLtPz///8BKMECMIi05PTLAzjHAUCP6IKl4DVNyAAAAFEIAhAHNAAAAF3rrszgYfhFix+G////aAFyEnF3ZSAxMjMgIUAjINC50YbRg3oFAQJk/w" +
                "OqBhha1zCpbCGUyzMzMzMzM/M/AAAAAAAACECyBgwAALDAmpmZPwAAQEC6BhAB/v//////////Af////8HwgYU////////////AQL//////////3/KBgcBAP////8P0gYNkE7//" +
                "/////////8BAtoGCaCcAf////8PBuIGEZWT2J/uR////////////wEA6gYMQOIBAP///38AAAAA8gYYywT7cR8BAAAAAAAAAAAAAP////////9/+gYMwB3+/////38AAACAggcY" +
                "wB3+////////////////fwAAAAAAAACAigcDAQEAkgcAkgcScXdlIDEyMyAhQCMg0LnRhtGDkgcGcXdlcnR5mgcDAQIDmgcFAQJk/wOaBwMAAADKDBha1zCpbCGUy////////+/" +
                "/////////73/SDAwAALDAmpmZPwAAQEDaDBUB/v//////////AYCAgID4/////wHiDBT///////////8BAv//////////f+oMBwoA/////w/yDAwA////////////AQL6DAmgnA" +
                "H+////DwCCDRGVk9if7kf///////////8BAIoNDEDiAQD///9/AAAAAJINGMsE+3EfAQAAAAAAAAAAAAD/////////f5oNDMAd/v8AAAAAAAAAgKINGMAd/v///////////////" +
                "38AAAAAAAAAALIG/gQJWtcwqWwhlEsVexQWQBiF//////////8BIPjLm4u0/P///wEowQIwiLTk9MsDOMcBQI/ogqXgNU3IAAAAUQgCEAc0AAAAXeuuzOBh+EWLH4b///9oAXIS" +
                "cXdlIDEyMyAhQCMg0LnRhtGDegUBAmT/A6oGGFrXMKlsIZTLMzMzMzMz8z8AAAAAAAAIQLIGDAAAsMCamZk/AABAQLoGGgH+//////////8B/////weAgICA+P////8BwgYU///" +
                "/////////AQL//////////3/KBgcBAP////8P0gYNkE7///////////8BAtoGCaCcAf////8PBuIGEZWT2J/uR////////////wEA6gYMQOIBAP///38AAAAA8gYYywT7cR8BAA" +
                "AAAAAAAAAAAP////////9/+gYMwB3+/////38AAACAggcYwB3+////////////////fwAAAAAAAACAigcDAQEAkgcAkgcScXdlIDEyMyAhQCMg0LnRhtGDkgcGcXdlcnR5mgcDA" +
                "QIDmgcFAQJk/wOaBwMAAADKDBha1zCpbCGUy////////+//////////73/SDAwAALDAmpmZPwAAQEDaDBUB/v//////////AYCAgID4/////wHiDBT///////////8BAv//////" +
                "////f+oMBwoA/////w/yDAwA////////////AQL6DAmgnAH+////DwCCDRGVk9if7kf///////////8BAIoNDEDiAQD///9/AAAAAJINGMsE+3EfAQAAAAAAAAAAAAD////////" +
                "/f5oNDMAd/v8AAAAAAAAAgKINGMAd/v///////////////38AAAAAAAAAAMoMFP//////////fwKAgICAgICAgIAB6hIGMTIzcXdl";

            return Convert.FromBase64String(base64);
        }

        public static Test.SuperMess GetEtalonMess()
        {
            Test.SuperMess mess = new Test.SuperMess()
            {
                Int32Val = 1,
                MessVal = CreateTestMess(),
                StringVal = "123qwe"
            };

            mess.DoubleArr.Add(1.234e-56);
            mess.DoubleArr.Add(-1.2);
            mess.DoubleArr.Add(0);

            mess.Int64Packed.Add(long.MaxValue);
            mess.Int64Packed.Add(2);
            mess.Int64Packed.Add(long.MinValue);

            var t1 = CreateTestMess();
            t1.DoubleVal = 1;
            mess.MessArr.Add(t1);

            var t2 = CreateTestMess();
            t2.FloatVal = 1.1F;
            mess.MessArr.Add(t2);

            var t3 = CreateTestMess();
            t3.Int32Arr.Add(int.MinValue);
            mess.MessArr.Add(t3);

            return mess;
        }

        public static Test.Mess CreateTestMess()
        {
            Test.Mess t = new Test.Mess()
            {
                DoubleVal = 1.234e56,
                FloatVal = 2.345F,
                Int32Val = -123,
                Int64Val = -123456789000L,
                UInt32Val = 321,
                UInt64Val = 123456789000L,
                SInt32Val = -100,
                SInt64Val = -923456789000L,
                Fixed32Val = 200,
                Fixed64Val = 223456789000L,
                SFixed32Val = -523456789,
                SFixed64Val = -523456789000L,
                BoolVal = true,
                StringVal = "qwe 123 !@# йцу",
                BytesVal = ByteString.CopyFrom(new byte[] { 1, 2, 100, 255, 3 })
            };

            t.DoubleArr.Add(-1.234e56);
            t.DoubleArr.Add(1.2);
            t.DoubleArr.Add(3);

            t.FloatArr.Add(-5.5F);
            t.FloatArr.Add(1.2F);
            t.FloatArr.Add(3F);

            t.Int32Arr.Add(1);
            t.Int32Arr.Add(-2);
            t.Int32Arr.Add(int.MaxValue);

            t.Int64Arr.Add(-1);
            t.Int64Arr.Add(2);
            t.Int64Arr.Add(long.MaxValue);

            t.UInt32Arr.Add(1);
            t.UInt32Arr.Add(0);
            t.UInt32Arr.Add(uint.MaxValue);

            t.UInt64Arr.Add(10000);
            t.UInt64Arr.Add(ulong.MaxValue);
            t.UInt64Arr.Add(2);

            t.SInt32Arr.Add(10000);
            t.SInt32Arr.Add(int.MinValue);
            t.SInt32Arr.Add(3);

            t.SInt64Arr.Add(-1234567890123);
            t.SInt64Arr.Add(long.MinValue);
            t.SInt64Arr.Add(0);

            t.Fixed32Arr.Add(123456);
            t.Fixed32Arr.Add(int.MaxValue);
            t.Fixed32Arr.Add(0);

            t.Fixed64Arr.Add(1234567890123);
            t.Fixed64Arr.Add(0);
            t.Fixed64Arr.Add(long.MaxValue);

            t.SFixed32Arr.Add(-123456);
            t.SFixed32Arr.Add(int.MaxValue);
            t.SFixed32Arr.Add(int.MinValue);

            t.SFixed64Arr.Add(-123456);
            t.SFixed64Arr.Add(long.MaxValue);
            t.SFixed64Arr.Add(long.MinValue);

            t.BoolArr.Add(true);
            t.BoolArr.Add(true);
            t.BoolArr.Add(false);

            t.StringArr.Add("");
            t.StringArr.Add("qwe 123 !@# йцу");
            t.StringArr.Add("qwerty");

            t.BytesArr.Add(ByteString.CopyFrom(new byte[] { 1, 2, 3 }));
            t.BytesArr.Add(ByteString.CopyFrom(new byte[] { 1, 2, 100, 255, 3 }));
            t.BytesArr.Add(ByteString.CopyFrom(new byte[] { 0, 0, 0 }));

            t.DoublePacked.Add(-1.234e56);
            t.DoublePacked.Add(double.MinValue);
            t.DoublePacked.Add(double.MaxValue);

            t.FloatPacked.Add(-5.5F);
            t.FloatPacked.Add(1.2F);
            t.FloatPacked.Add(3F);

            t.Int32Packed.Add(1);
            t.Int32Packed.Add(-2);
            t.Int32Packed.Add(int.MinValue);

            t.Int64Packed.Add(-1);
            t.Int64Packed.Add(2);
            t.Int64Packed.Add(long.MaxValue);

            t.UInt32Packed.Add(10);
            t.UInt32Packed.Add(0);
            t.UInt32Packed.Add(uint.MaxValue);

            t.UInt64Packed.Add(0);
            t.UInt64Packed.Add(ulong.MaxValue);
            t.UInt64Packed.Add(2);

            t.SInt32Packed.Add(10000);
            t.SInt32Packed.Add(int.MaxValue);
            t.SInt32Packed.Add(0);

            t.SInt64Packed.Add(-1234567890123);
            t.SInt64Packed.Add(long.MinValue);
            t.SInt64Packed.Add(0);

            t.Fixed32Packed.Add(123456);
            t.Fixed32Packed.Add(int.MaxValue);
            t.Fixed32Packed.Add(0);

            t.Fixed64Packed.Add(1234567890123);
            t.Fixed64Packed.Add(0);
            t.Fixed64Packed.Add(long.MaxValue);

            t.SFixed32Packed.Add(-123456);
            t.SFixed32Packed.Add(0);
            t.SFixed32Packed.Add(int.MinValue);

            t.SFixed64Packed.Add(-123456);
            t.SFixed64Packed.Add(long.MaxValue);
            t.SFixed64Packed.Add(0);

            t.BoolPacked.Add(false);
            t.BoolPacked.Add(true);
            t.BoolPacked.Add(true);

            return t;
        }

        public static MessageDescriptor<Test.Mess> CreateMessDescriptor()
        {
            FieldSetting<Test.Mess>[] settings = new FieldSetting<Test.Mess>[]
            {
                FieldSetting<Test.Mess>.CreateDouble(Test.Mess.DoubleValFieldNumber, x => x.DoubleVal, (x, y) => x.DoubleVal = y),
                FieldSetting<Test.Mess>.CreateFloat(Test.Mess.FloatValFieldNumber, x => x.FloatVal, (x, y) => x.FloatVal = y),
                FieldSetting<Test.Mess>.CreateInt32(Test.Mess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                FieldSetting<Test.Mess>.CreateInt64(Test.Mess.Int64ValFieldNumber, x => x.Int64Val, (x, y) => x.Int64Val = y),
                FieldSetting<Test.Mess>.CreateUInt32(Test.Mess.UInt32ValFieldNumber, x => x.UInt32Val, (x, y) => x.UInt32Val = y),
                FieldSetting<Test.Mess>.CreateUInt64(Test.Mess.UInt64ValFieldNumber, x => x.UInt64Val, (x, y) => x.UInt64Val = y),
                FieldSetting<Test.Mess>.CreateSInt32(Test.Mess.SInt32ValFieldNumber, x => x.SInt32Val, (x, y) => x.SInt32Val = y),
                FieldSetting<Test.Mess>.CreateSInt64(Test.Mess.SInt64ValFieldNumber, x => x.SInt64Val, (x, y) => x.SInt64Val = y),
                FieldSetting<Test.Mess>.CreateFixed32(Test.Mess.Fixed32ValFieldNumber, x => x.Fixed32Val, (x, y) => x.Fixed32Val = y),
                FieldSetting<Test.Mess>.CreateFixed64(Test.Mess.Fixed64ValFieldNumber, x => x.Fixed64Val, (x, y) => x.Fixed64Val = y),
                FieldSetting<Test.Mess>.CreateSFixed32(Test.Mess.SFixed32ValFieldNumber, x => x.SFixed32Val, (x, y) => x.SFixed32Val = y),
                FieldSetting<Test.Mess>.CreateSFixed64(Test.Mess.SFixed64ValFieldNumber, x => x.SFixed64Val, (x, y) => x.SFixed64Val = y),
                FieldSetting<Test.Mess>.CreateBool(Test.Mess.BoolValFieldNumber, x => x.BoolVal, (x, y) => x.BoolVal = y),
                FieldSetting<Test.Mess>.CreateString(Test.Mess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y),
                FieldSetting<Test.Mess>.CreateBytes(Test.Mess.BytesValFieldNumber, x => x.BytesVal.ToByteArray(), (x, y) => x.BytesVal = ByteString.CopyFrom(y)),

                FieldSetting<Test.Mess>.CreateDoubleArray(Test.Mess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatArray(Test.Mess.FloatArrFieldNumber, x => x.FloatArr, (x, y) => x.FloatArr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32Array(Test.Mess.Int32ArrFieldNumber, x => x.Int32Arr, (x, y) => x.Int32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64Array(Test.Mess.Int64ArrFieldNumber, x => x.Int64Arr, (x, y) => x.Int64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32Array(Test.Mess.UInt32ArrFieldNumber, x => x.UInt32Arr, (x, y) => x.UInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64Array(Test.Mess.UInt64ArrFieldNumber, x => x.UInt64Arr, (x, y) => x.UInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32Array(Test.Mess.SInt32ArrFieldNumber, x => x.SInt32Arr, (x, y) => x.SInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64Array(Test.Mess.SInt64ArrFieldNumber, x => x.SInt64Arr, (x, y) => x.SInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32Array(Test.Mess.Fixed32ArrFieldNumber, x => x.Fixed32Arr, (x, y) => x.Fixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64Array(Test.Mess.Fixed64ArrFieldNumber, x => x.Fixed64Arr, (x, y) => x.Fixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32Array(Test.Mess.SFixed32ArrFieldNumber, x => x.SFixed32Arr, (x, y) => x.SFixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64Array(Test.Mess.SFixed64ArrFieldNumber, x => x.SFixed64Arr, (x, y) => x.SFixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolArray(Test.Mess.BoolArrFieldNumber, x => x.BoolArr, (x, y) => x.BoolArr.Add(y)),
                FieldSetting<Test.Mess>.CreateStringArray(Test.Mess.StringArrFieldNumber, x => x.StringArr, (x, y) => x.StringArr.Add(y)),
                FieldSetting<Test.Mess>.CreateBytesArray(Test.Mess.BytesArrFieldNumber, x => x.BytesArr.Select(y => y.ToByteArray()), (x, y) => x.BytesArr.Add(ByteString.CopyFrom(y))),

                FieldSetting<Test.Mess>.CreateDoublePackedArray(Test.Mess.DoublePackedFieldNumber, x => x.DoublePacked, (x, y) => x.DoublePacked.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatPackedArray(Test.Mess.FloatPackedFieldNumber, x => x.FloatPacked, (x, y) => x.FloatPacked.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32PackedArray(Test.Mess.Int32PackedFieldNumber, x => x.Int32Packed, (x, y) => x.Int32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64PackedArray(Test.Mess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32PackedArray(Test.Mess.UInt32PackedFieldNumber, x => x.UInt32Packed, (x, y) => x.UInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64PackedArray(Test.Mess.UInt64PackedFieldNumber, x => x.UInt64Packed, (x, y) => x.UInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32PackedArray(Test.Mess.SInt32PackedFieldNumber, x => x.SInt32Packed, (x, y) => x.SInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64PackedArray(Test.Mess.SInt64PackedFieldNumber, x => x.SInt64Packed, (x, y) => x.SInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32PackedArray(Test.Mess.Fixed32PackedFieldNumber, x => x.Fixed32Packed, (x, y) => x.Fixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64PackedArray(Test.Mess.Fixed64PackedFieldNumber, x => x.Fixed64Packed, (x, y) => x.Fixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32PackedArray(Test.Mess.SFixed32PackedFieldNumber, x => x.SFixed32Packed, (x, y) => x.SFixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64PackedArray(Test.Mess.SFixed64PackedFieldNumber, x => x.SFixed64Packed, (x, y) => x.SFixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolPackedArray(Test.Mess.BoolPackedFieldNumber, x => x.BoolPacked, (x, y) => x.BoolPacked.Add(y)),
            };

            return MessageDescriptor<Test.Mess>.Create(settings);
        }

        public static MessageDescriptor<Test.Mess> CreateMessAllPackedDescriptor()
        {
            FieldSetting<Test.Mess>[] settings = new FieldSetting<Test.Mess>[]
            {
                FieldSetting<Test.Mess>.CreateDouble(Test.Mess.DoubleValFieldNumber, x => x.DoubleVal, (x, y) => x.DoubleVal = y),
                FieldSetting<Test.Mess>.CreateFloat(Test.Mess.FloatValFieldNumber, x => x.FloatVal, (x, y) => x.FloatVal = y),
                FieldSetting<Test.Mess>.CreateInt32(Test.Mess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                FieldSetting<Test.Mess>.CreateInt64(Test.Mess.Int64ValFieldNumber, x => x.Int64Val, (x, y) => x.Int64Val = y),
                FieldSetting<Test.Mess>.CreateUInt32(Test.Mess.UInt32ValFieldNumber, x => x.UInt32Val, (x, y) => x.UInt32Val = y),
                FieldSetting<Test.Mess>.CreateUInt64(Test.Mess.UInt64ValFieldNumber, x => x.UInt64Val, (x, y) => x.UInt64Val = y),
                FieldSetting<Test.Mess>.CreateSInt32(Test.Mess.SInt32ValFieldNumber, x => x.SInt32Val, (x, y) => x.SInt32Val = y),
                FieldSetting<Test.Mess>.CreateSInt64(Test.Mess.SInt64ValFieldNumber, x => x.SInt64Val, (x, y) => x.SInt64Val = y),
                FieldSetting<Test.Mess>.CreateFixed32(Test.Mess.Fixed32ValFieldNumber, x => x.Fixed32Val, (x, y) => x.Fixed32Val = y),
                FieldSetting<Test.Mess>.CreateFixed64(Test.Mess.Fixed64ValFieldNumber, x => x.Fixed64Val, (x, y) => x.Fixed64Val = y),
                FieldSetting<Test.Mess>.CreateSFixed32(Test.Mess.SFixed32ValFieldNumber, x => x.SFixed32Val, (x, y) => x.SFixed32Val = y),
                FieldSetting<Test.Mess>.CreateSFixed64(Test.Mess.SFixed64ValFieldNumber, x => x.SFixed64Val, (x, y) => x.SFixed64Val = y),
                FieldSetting<Test.Mess>.CreateBool(Test.Mess.BoolValFieldNumber, x => x.BoolVal, (x, y) => x.BoolVal = y),
                FieldSetting<Test.Mess>.CreateString(Test.Mess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y),
                FieldSetting<Test.Mess>.CreateBytes(Test.Mess.BytesValFieldNumber, x => x.BytesVal.ToByteArray(), (x, y) => x.BytesVal = ByteString.CopyFrom(y)),

                FieldSetting<Test.Mess>.CreateDoublePackedArray(Test.Mess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatPackedArray(Test.Mess.FloatArrFieldNumber, x => x.FloatArr, (x, y) => x.FloatArr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32PackedArray(Test.Mess.Int32ArrFieldNumber, x => x.Int32Arr, (x, y) => x.Int32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64PackedArray(Test.Mess.Int64ArrFieldNumber, x => x.Int64Arr, (x, y) => x.Int64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32PackedArray(Test.Mess.UInt32ArrFieldNumber, x => x.UInt32Arr, (x, y) => x.UInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64PackedArray(Test.Mess.UInt64ArrFieldNumber, x => x.UInt64Arr, (x, y) => x.UInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32PackedArray(Test.Mess.SInt32ArrFieldNumber, x => x.SInt32Arr, (x, y) => x.SInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64PackedArray(Test.Mess.SInt64ArrFieldNumber, x => x.SInt64Arr, (x, y) => x.SInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32PackedArray(Test.Mess.Fixed32ArrFieldNumber, x => x.Fixed32Arr, (x, y) => x.Fixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64PackedArray(Test.Mess.Fixed64ArrFieldNumber, x => x.Fixed64Arr, (x, y) => x.Fixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32PackedArray(Test.Mess.SFixed32ArrFieldNumber, x => x.SFixed32Arr, (x, y) => x.SFixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64PackedArray(Test.Mess.SFixed64ArrFieldNumber, x => x.SFixed64Arr, (x, y) => x.SFixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolPackedArray(Test.Mess.BoolArrFieldNumber, x => x.BoolArr, (x, y) => x.BoolArr.Add(y)),
                FieldSetting<Test.Mess>.CreateStringArray(Test.Mess.StringArrFieldNumber, x => x.StringArr, (x, y) => x.StringArr.Add(y)),
                FieldSetting<Test.Mess>.CreateBytesArray(Test.Mess.BytesArrFieldNumber, x => x.BytesArr.Select(y => y.ToByteArray()), (x, y) => x.BytesArr.Add(ByteString.CopyFrom(y))),

                FieldSetting<Test.Mess>.CreateDoublePackedArray(Test.Mess.DoublePackedFieldNumber, x => x.DoublePacked, (x, y) => x.DoublePacked.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatPackedArray(Test.Mess.FloatPackedFieldNumber, x => x.FloatPacked, (x, y) => x.FloatPacked.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32PackedArray(Test.Mess.Int32PackedFieldNumber, x => x.Int32Packed, (x, y) => x.Int32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64PackedArray(Test.Mess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32PackedArray(Test.Mess.UInt32PackedFieldNumber, x => x.UInt32Packed, (x, y) => x.UInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64PackedArray(Test.Mess.UInt64PackedFieldNumber, x => x.UInt64Packed, (x, y) => x.UInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32PackedArray(Test.Mess.SInt32PackedFieldNumber, x => x.SInt32Packed, (x, y) => x.SInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64PackedArray(Test.Mess.SInt64PackedFieldNumber, x => x.SInt64Packed, (x, y) => x.SInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32PackedArray(Test.Mess.Fixed32PackedFieldNumber, x => x.Fixed32Packed, (x, y) => x.Fixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64PackedArray(Test.Mess.Fixed64PackedFieldNumber, x => x.Fixed64Packed, (x, y) => x.Fixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32PackedArray(Test.Mess.SFixed32PackedFieldNumber, x => x.SFixed32Packed, (x, y) => x.SFixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64PackedArray(Test.Mess.SFixed64PackedFieldNumber, x => x.SFixed64Packed, (x, y) => x.SFixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolPackedArray(Test.Mess.BoolPackedFieldNumber, x => x.BoolPacked, (x, y) => x.BoolPacked.Add(y)),
            };

            return MessageDescriptor<Test.Mess>.Create(settings);
        }

        public static MessageDescriptor<Test.Mess> CreateMessAllUnpackedDescriptor()
        {
            FieldSetting<Test.Mess>[] settings = new FieldSetting<Test.Mess>[]
            {
                FieldSetting<Test.Mess>.CreateDouble(Test.Mess.DoubleValFieldNumber, x => x.DoubleVal, (x, y) => x.DoubleVal = y),
                FieldSetting<Test.Mess>.CreateFloat(Test.Mess.FloatValFieldNumber, x => x.FloatVal, (x, y) => x.FloatVal = y),
                FieldSetting<Test.Mess>.CreateInt32(Test.Mess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                FieldSetting<Test.Mess>.CreateInt64(Test.Mess.Int64ValFieldNumber, x => x.Int64Val, (x, y) => x.Int64Val = y),
                FieldSetting<Test.Mess>.CreateUInt32(Test.Mess.UInt32ValFieldNumber, x => x.UInt32Val, (x, y) => x.UInt32Val = y),
                FieldSetting<Test.Mess>.CreateUInt64(Test.Mess.UInt64ValFieldNumber, x => x.UInt64Val, (x, y) => x.UInt64Val = y),
                FieldSetting<Test.Mess>.CreateSInt32(Test.Mess.SInt32ValFieldNumber, x => x.SInt32Val, (x, y) => x.SInt32Val = y),
                FieldSetting<Test.Mess>.CreateSInt64(Test.Mess.SInt64ValFieldNumber, x => x.SInt64Val, (x, y) => x.SInt64Val = y),
                FieldSetting<Test.Mess>.CreateFixed32(Test.Mess.Fixed32ValFieldNumber, x => x.Fixed32Val, (x, y) => x.Fixed32Val = y),
                FieldSetting<Test.Mess>.CreateFixed64(Test.Mess.Fixed64ValFieldNumber, x => x.Fixed64Val, (x, y) => x.Fixed64Val = y),
                FieldSetting<Test.Mess>.CreateSFixed32(Test.Mess.SFixed32ValFieldNumber, x => x.SFixed32Val, (x, y) => x.SFixed32Val = y),
                FieldSetting<Test.Mess>.CreateSFixed64(Test.Mess.SFixed64ValFieldNumber, x => x.SFixed64Val, (x, y) => x.SFixed64Val = y),
                FieldSetting<Test.Mess>.CreateBool(Test.Mess.BoolValFieldNumber, x => x.BoolVal, (x, y) => x.BoolVal = y),
                FieldSetting<Test.Mess>.CreateString(Test.Mess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y),
                FieldSetting<Test.Mess>.CreateBytes(Test.Mess.BytesValFieldNumber, x => x.BytesVal.ToByteArray(), (x, y) => x.BytesVal = ByteString.CopyFrom(y)),

                FieldSetting<Test.Mess>.CreateDoubleArray(Test.Mess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatArray(Test.Mess.FloatArrFieldNumber, x => x.FloatArr, (x, y) => x.FloatArr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32Array(Test.Mess.Int32ArrFieldNumber, x => x.Int32Arr, (x, y) => x.Int32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64Array(Test.Mess.Int64ArrFieldNumber, x => x.Int64Arr, (x, y) => x.Int64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32Array(Test.Mess.UInt32ArrFieldNumber, x => x.UInt32Arr, (x, y) => x.UInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64Array(Test.Mess.UInt64ArrFieldNumber, x => x.UInt64Arr, (x, y) => x.UInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32Array(Test.Mess.SInt32ArrFieldNumber, x => x.SInt32Arr, (x, y) => x.SInt32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64Array(Test.Mess.SInt64ArrFieldNumber, x => x.SInt64Arr, (x, y) => x.SInt64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32Array(Test.Mess.Fixed32ArrFieldNumber, x => x.Fixed32Arr, (x, y) => x.Fixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64Array(Test.Mess.Fixed64ArrFieldNumber, x => x.Fixed64Arr, (x, y) => x.Fixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32Array(Test.Mess.SFixed32ArrFieldNumber, x => x.SFixed32Arr, (x, y) => x.SFixed32Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64Array(Test.Mess.SFixed64ArrFieldNumber, x => x.SFixed64Arr, (x, y) => x.SFixed64Arr.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolArray(Test.Mess.BoolArrFieldNumber, x => x.BoolArr, (x, y) => x.BoolArr.Add(y)),
                FieldSetting<Test.Mess>.CreateStringArray(Test.Mess.StringArrFieldNumber, x => x.StringArr, (x, y) => x.StringArr.Add(y)),
                FieldSetting<Test.Mess>.CreateBytesArray(Test.Mess.BytesArrFieldNumber, x => x.BytesArr.Select(y => y.ToByteArray()), (x, y) => x.BytesArr.Add(ByteString.CopyFrom(y))),

                FieldSetting<Test.Mess>.CreateDoubleArray(Test.Mess.DoublePackedFieldNumber, x => x.DoublePacked, (x, y) => x.DoublePacked.Add(y)),
                FieldSetting<Test.Mess>.CreateFloatArray(Test.Mess.FloatPackedFieldNumber, x => x.FloatPacked, (x, y) => x.FloatPacked.Add(y)),
                FieldSetting<Test.Mess>.CreateInt32Array(Test.Mess.Int32PackedFieldNumber, x => x.Int32Packed, (x, y) => x.Int32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateInt64Array(Test.Mess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt32Array(Test.Mess.UInt32PackedFieldNumber, x => x.UInt32Packed, (x, y) => x.UInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateUInt64Array(Test.Mess.UInt64PackedFieldNumber, x => x.UInt64Packed, (x, y) => x.UInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt32Array(Test.Mess.SInt32PackedFieldNumber, x => x.SInt32Packed, (x, y) => x.SInt32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSInt64Array(Test.Mess.SInt64PackedFieldNumber, x => x.SInt64Packed, (x, y) => x.SInt64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed32Array(Test.Mess.Fixed32PackedFieldNumber, x => x.Fixed32Packed, (x, y) => x.Fixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateFixed64Array(Test.Mess.Fixed64PackedFieldNumber, x => x.Fixed64Packed, (x, y) => x.Fixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed32Array(Test.Mess.SFixed32PackedFieldNumber, x => x.SFixed32Packed, (x, y) => x.SFixed32Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateSFixed64Array(Test.Mess.SFixed64PackedFieldNumber, x => x.SFixed64Packed, (x, y) => x.SFixed64Packed.Add(y)),
                FieldSetting<Test.Mess>.CreateBoolArray(Test.Mess.BoolPackedFieldNumber, x => x.BoolPacked, (x, y) => x.BoolPacked.Add(y)),
            };

            return MessageDescriptor<Test.Mess>.Create(settings);
        }

        public static MessageDescriptor<Test.SuperMess> CreateSuperMessDescriptor()
        {
            FieldSetting<Test.SuperMess>[] settings = new FieldSetting<Test.SuperMess>[]
            {
                        FieldSetting<Test.SuperMess>.CreateInt32(Test.SuperMess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                        FieldSetting<Test.SuperMess>.CreateMessage(Test.SuperMess.MessValFieldNumber, x => x.MessVal, (x, y) => x.MessVal = y, CreateMessDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateDoubleArray(Test.SuperMess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateMessageArray(Test.SuperMess.MessArrFieldNumber, x => x.MessArr, (x, y) => x.MessArr.Add(y), CreateMessDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateInt64PackedArray(Test.SuperMess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateString(Test.SuperMess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y)
            };

            return MessageDescriptor<Test.SuperMess>.Create(settings);
        }

        public static MessageDescriptor<Test.SuperMess> CreateSuperMessAllPackedDescriptor()
        {
            FieldSetting<Test.SuperMess>[] settings = new FieldSetting<Test.SuperMess>[]
            {
                        FieldSetting<Test.SuperMess>.CreateInt32(Test.SuperMess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                        FieldSetting<Test.SuperMess>.CreateMessage(Test.SuperMess.MessValFieldNumber, x => x.MessVal, (x, y) => x.MessVal = y, CreateMessAllPackedDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateDoublePackedArray(Test.SuperMess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateMessageArray(Test.SuperMess.MessArrFieldNumber, x => x.MessArr, (x, y) => x.MessArr.Add(y), CreateMessAllPackedDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateInt64PackedArray(Test.SuperMess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateString(Test.SuperMess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y)
            };

            return MessageDescriptor<Test.SuperMess>.Create(settings);
        }

        public static MessageDescriptor<Test.SuperMess> CreateSuperMessAllUnpackedDescriptor()
        {
            FieldSetting<Test.SuperMess>[] settings = new FieldSetting<Test.SuperMess>[]
            {
                        FieldSetting<Test.SuperMess>.CreateInt32(Test.SuperMess.Int32ValFieldNumber, x => x.Int32Val, (x, y) => x.Int32Val = y),
                        FieldSetting<Test.SuperMess>.CreateMessage(Test.SuperMess.MessValFieldNumber, x => x.MessVal, (x, y) => x.MessVal = y, CreateMessAllUnpackedDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateDoubleArray(Test.SuperMess.DoubleArrFieldNumber, x => x.DoubleArr, (x, y) => x.DoubleArr.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateMessageArray(Test.SuperMess.MessArrFieldNumber, x => x.MessArr, (x, y) => x.MessArr.Add(y), CreateMessAllUnpackedDescriptor()),
                        FieldSetting<Test.SuperMess>.CreateInt64Array(Test.SuperMess.Int64PackedFieldNumber, x => x.Int64Packed, (x, y) => x.Int64Packed.Add(y)),
                        FieldSetting<Test.SuperMess>.CreateString(Test.SuperMess.StringValFieldNumber, x => x.StringVal, (x, y) => x.StringVal = y)
            };

            return MessageDescriptor<Test.SuperMess>.Create(settings);
        }

        public static bool SerializeAndParseFromTest(MessageDescriptor<Test.SuperMess> descriptor)
        {
            var mess = TestHelper.GetEtalonMess();

            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var writer = descriptor.CreateWriter(ms))
                {
                    writer.Write(mess);
                }

                ms.Flush();

                buff = ms.ToArray();
            }

            var mess1 = Test.SuperMess.Parser.ParseFrom(buff);

            return mess.Equals(mess1);
        }

        public static bool WriteToAndDeserializeTest(MessageDescriptor<Test.SuperMess> descriptor)
        {
            var mess = TestHelper.GetEtalonMess();

            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                mess.WriteTo(ms);
                ms.Flush();

                buff = ms.ToArray();
            }

            Test.SuperMess mess1;
            using (var reader = descriptor.CreateReader(new MemoryStream(buff), true))
            {
                mess1 = reader.Read();
            }

            return mess.Equals(mess1);
        }

        public static bool SerializeAndDeserializeTest(MessageDescriptor<Test.SuperMess> descriptorSer, 
            MessageDescriptor<Test.SuperMess> descriptorDeser)
        {
            var mess = TestHelper.GetEtalonMess();

            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var writer = descriptorSer.CreateWriter(ms))
                {
                    writer.Write(mess);
                }

                ms.Flush();

                buff = ms.ToArray();
            }

            Test.SuperMess mess1;
            using (var reader = descriptorDeser.CreateReader(new MemoryStream(buff), true))
            {
                mess1 = reader.Read();
            }

           return mess.Equals(mess1);
        }
    }
}
