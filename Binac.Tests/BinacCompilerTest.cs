namespace Binac.Tests
{
    [TestClass]
    public sealed class BinacCompilerTest
    {
        [TestMethod]
        public void ParseMemoryLocations()
        {
            var compiler = new BinacCompiler();

            var opcodes = compiler.ParseCode("05(123)");

            Assert.IsTrue(opcodes.Length == 1);
            Assert.AreEqual(new BinacOperation { Code = 5, MemoryAddress = 83 }, opcodes[0]);
        }
        [TestMethod]
        public void ParseMultipleOpcodes()
        {
            var compiler = new BinacCompiler();

            var opcodes = compiler.ParseCode("05(123) S(122)");

            Assert.IsTrue(opcodes.Length == 2);
            Assert.AreEqual(new BinacOperation { Code = 5, MemoryAddress = 83 }, opcodes[0]);
            Assert.AreEqual(new BinacOperation { Code = 13, MemoryAddress = 82 }, opcodes[1]);
        }
    }
}
