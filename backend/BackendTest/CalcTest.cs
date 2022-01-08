using Xunit;

namespace BackendTest
{
    public class CalcTest
    {
        [Fact]
        public void Add_When2Integers_ShouldReturnCorrectInteger()
        {
            var result = Calc.Add(1, 1);
            Assert.Equal(2, result);
        }
    }
}
