using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Google.Protobuf;

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
    }
}
