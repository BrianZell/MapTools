using NUnit.Framework;

namespace MapTools.Test
{
    public class PositionTests
    {
        [Test]
        public void SamePositionsAreEqual()
        {
            var sut = new Position(1, 1, 1);
            var sut2 = new Position(1, 1, 1);

            Assert.That(sut,Is.EqualTo(sut2));
        }

        [Test]
        public void DifferentPositionsAreNotEqual()
        {
            var sut = new Position(1, 1, 1);
            var sut2 = new Position(1, 1, 0);

            Assert.That(sut, Is.Not.EqualTo(sut2));
        }
    }
}