namespace Binac.Tests
{
    [TestClass]
    public sealed class BinacNumberTest
    {
        [DataTestMethod]
        [DataRow(0x20_00_00_00, 0.5)]
        [DataRow(0x60_00_00_00, -0.5)] // This is not complimentary number. Or it is?
        [DataRow(0x10_00_00_00, 0.25)]
        [DataRow(0x50_00_00_00, -0.25)]
        [DataRow(0x08_00_00_00, 0.125)]
        [DataRow(0x48_00_00_00, -0.125)]
        [DataRow(0x30_00_00_00, 0.75)]
        [DataRow(0x70_00_00_00, -0.75)]
        [DataRow(0x00_00_00_00, 0.0)]
        [DataRow(0x40_00_00_00, -0.0)]
        public void ConversionToDouble(int value, double expected)
        {
            var num = new BinacNumber(value);

            var result = (double)num;

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(0x20_00_00_00, 0x10_00_00_00, 0.75)]
        [DataRow(0x10_00_00_00, 0x60_00_00_00, -0.25)]
        [DataRow(0x10_00_00_00, 0x50_00_00_00, 0.0)]
        public void Addition(int term1, int term2, double expected)
        {
            var first = new BinacNumber(term1);
            var second = new BinacNumber(term2);

            var result = first + second;
            
            Assert.AreEqual(expected, (double)result);
        }
        [DataTestMethod]
        [DataRow(0x20_00_00_00, 0x10_00_00_00, 0.25)]
        [DataRow(0x10_00_00_00, 0x20_00_00_00, -0.25)]
        [DataRow(0x10_00_00_00, 0x10_00_00_00, 0.0)]
        public void Substract(int term1, int term2, double expected)
        {
            var first = new BinacNumber(term1);
            var second = new BinacNumber(term2);

            var result = first - second;
            
            Assert.AreEqual(expected, (double)result);
        }

        [DataTestMethod]
        [DataRow(0x20_00_00_00, 0x10_00_00_00, 0.125)]
        [DataRow(0x10_00_00_00, 0x60_00_00_00, -0.125)]
        [DataRow(0x10_00_00_00, 0x50_00_00_00, -0.0625)]
        [DataRow(0x20_00_00_00, 0x20_00_00_00, 0.25)]
        public void Multiply(int term1, int term2, double expected)
        {
            var first = new BinacNumber(term1);
            var second = new BinacNumber(term2);

            var result = first * second;

            Assert.AreEqual(expected, (double)result);
        }

        [DataTestMethod]
        [DataRow(0x20_00_00_00, 0x10_00_00_00, 0.5)]
        [DataRow(0x60_00_00_00, 0x10_00_00_00, -0.5)]
        [DataRow(0x20_00_00_00, 0x50_00_00_00, -0.5)]
        public void Divide(int term1, int term2, double expected)
        {
            var first = new BinacNumber(term1);
            var second = new BinacNumber(term2);

            var result = first / second;

            Assert.AreEqual(expected, (double)result);
        }
    }
}
