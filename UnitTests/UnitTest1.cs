using Gerakul.ProtoBufSerializer;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSerialization()
        {
            Assert.IsTrue(TestHelper.SerializeAndParseFromTest(TestHelper.CreateSuperMessDescriptor()));
        }

        [TestMethod]
        public void TestAllPackedSerialization()
        {
            Assert.IsTrue(TestHelper.SerializeAndParseFromTest(TestHelper.CreateSuperMessAllPackedDescriptor()));
        }

        [TestMethod]
        public void TestAllUnpackedSerialization()
        {
            Assert.IsTrue(TestHelper.SerializeAndParseFromTest(TestHelper.CreateSuperMessAllUnpackedDescriptor()));
        }

        [TestMethod]
        public void TestDeserialization()
        {
            Assert.IsTrue(TestHelper.WriteToAndDeserializeTest(TestHelper.CreateSuperMessDescriptor()));
        }

        [TestMethod]
        public void TestAllPackedDeserialization()
        {
            Assert.IsTrue(TestHelper.WriteToAndDeserializeTest(TestHelper.CreateSuperMessAllPackedDescriptor()));
        }
        
        [TestMethod]
        public void TestAllUnpackedDeserialization()
        {
            Assert.IsTrue(TestHelper.WriteToAndDeserializeTest(TestHelper.CreateSuperMessAllUnpackedDescriptor()));
        }

        [TestMethod]
        public void TestSerDeser()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessDescriptor(), TestHelper.CreateSuperMessDescriptor()));
        }

        [TestMethod]
        public void TestSerDeserPacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessDescriptor(), TestHelper.CreateSuperMessAllPackedDescriptor()));
        }

        [TestMethod]
        public void TestSerDeserUnpacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessDescriptor(), TestHelper.CreateSuperMessAllUnpackedDescriptor()));
        }

        [TestMethod]
        public void TestSerPackedDeser()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllPackedDescriptor(), TestHelper.CreateSuperMessDescriptor()));
        }

        [TestMethod]
        public void TestSerPackedDeserPacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllPackedDescriptor(), TestHelper.CreateSuperMessAllPackedDescriptor()));
        }

        [TestMethod]
        public void TestSerPackedDeserUnpacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllPackedDescriptor(), TestHelper.CreateSuperMessAllUnpackedDescriptor()));
        }

        [TestMethod]
        public void TestSerUnpackedDeser()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllUnpackedDescriptor(), TestHelper.CreateSuperMessDescriptor()));
        }

        [TestMethod]
        public void TestSerUnpackedDeserPacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllUnpackedDescriptor(), TestHelper.CreateSuperMessAllPackedDescriptor()));
        }

        [TestMethod]
        public void TestSerUnpackedDeserUnpacked()
        {
            Assert.IsTrue(TestHelper.SerializeAndDeserializeTest(TestHelper.CreateSuperMessAllUnpackedDescriptor(), TestHelper.CreateSuperMessAllUnpackedDescriptor()));
        }

        [TestMethod]
        public void TestReadLenDelimited()
        {
            var messages = new Test.Mess[3];

            messages[0] = TestHelper.CreateTestMess();
            messages[0].DoubleVal = 1;

            messages[1] = TestHelper.CreateTestMess();
            messages[1].FloatVal = 1.1F;

            messages[2] = TestHelper.CreateTestMess();
            messages[2].Int32Arr.Add(int.MinValue);

            var descr = TestHelper.CreateMessDescriptor();

            var buff = descr.WriteLenDelimitedStream(messages);

            var buffers = BasicDeserializer.ReadLenDelimited(buff).ToArray();

            var messages1 = buffers.Select(x => descr.Read(x)).ToArray();

            var eq1 = messages[0].Equals(messages1[0]);
            var eq2 = messages[1].Equals(messages1[1]);
            var eq3 = messages[2].Equals(messages1[2]);

            Assert.IsTrue(eq1 && eq2 && eq3);
        }
    }
}
