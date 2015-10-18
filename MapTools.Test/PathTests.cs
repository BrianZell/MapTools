using NUnit.Framework;

namespace MapTools.Test
{
    public class PathTests
    {
        [Test]
        public void SingleItemPath_SetsStartPosition()
        {
            var startPos = new Position(1, 1, 1);
            var sut = new Path(startPos);
            
            Assert.That(sut.StartPosition, Is.EqualTo(startPos));
        }

        [Test]
        public void SingleItemPath_SetsEndPosition()
        {
            var startPos = new Position(1, 1, 1);
            var sut = new Path(startPos);

            Assert.That(sut.EndPosition, Is.EqualTo(startPos));
        }
    }
}