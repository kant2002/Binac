namespace Binac.Tests
{
    [TestClass]
    public sealed class BinacNumberTest
    {
        [Ignore]
        [DataTestMethod]
        [DataRow(30_00_00_00, 0.5)]
        public void ConversionToDouble(int value, double expected)
        {
            var num = new BinacNumber(value);

            var result = (double)num;

            Assert.AreEqual(expected, result);
        }
    }
}
