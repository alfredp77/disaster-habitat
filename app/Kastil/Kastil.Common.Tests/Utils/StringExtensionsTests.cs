using Kastil.Common.Utils;
using NUnit.Framework;

namespace Kastil.Common.Tests.Utils
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ShouldExtractPriceWithoutDollarSign()
        {
            Assert.That("$20".GetTrailingNumbers(), Is.EqualTo(20));
            Assert.That("$ 50".GetTrailingNumbers(), Is.EqualTo(50));
            Assert.That("$100.5".GetTrailingNumbers(), Is.EqualTo(100));
            Assert.That("$a 20xx".GetTrailingNumbers(), Is.EqualTo(20));
        }
    }
}
