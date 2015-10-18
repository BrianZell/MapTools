using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace MapTools.Test
{
    public class ShortestPathTests
    {
        private Mock<IAllowsMovement> _mockMap = new Mock<IAllowsMovement>();

        [SetUp]
        public void SetUp()
        {
            _mockMap.Setup(x => x.IsAllowedPosition(It.IsAny<Position>())).Returns(true);
        }

        [Test]
        public void FindPath_StartPosition_IsTheSameAsProvidedStartPosition()
        {
            var startPosition = new Position(3, 3, 0);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(startPosition, 1);

            Assert.That(results.Select(x => x.StartPosition).Distinct().Single(), Is.EqualTo(startPosition));
        }

        [Test]
        public void FindPath_WhenDistanceIsOne_OnlyReturnsAdjacentSpaces()
        {
            var expectedResults = new[]{
                                          new Position(3,4,0),
                                          new Position(2,3,0),
                                          new Position(3,2,0),
                                          new Position(4,3,0),
                                       };

            var startPosition = new Position(3, 3, 0);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(startPosition, 1);

            Assert.That(Sort(results.Select(x => x.EndPosition)), Is.EquivalentTo(Sort(expectedResults)));
        }

        [Test]
        public void FindPath_WhenDistanceIsTwo_ReturnsTwoAdjacentSpacesAndOneDiagonal()
        {
            var expectedResults = new[]{
                                          new Position(3,1,0),
                                          new Position(3,2,0),
                                          new Position(3,4,0),
                                          new Position(3,5,0),
                                          new Position(1,3,0),
                                          new Position(2,3,0),
                                          new Position(4,3,0),
                                          new Position(5,3,0),
                                          new Position(2,2,0),
                                          new Position(4,2,0),
                                          new Position(4,4,0),
                                          new Position(2,4,0),
                                       };

            var startPosition = new Position(3, 3, 0);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(startPosition, 2);
            
            Assert.That(Sort(results.Select(x => x.EndPosition)), Is.EquivalentTo(Sort(expectedResults)));
            results.ToList().ForEach(Console.WriteLine);
        }

        /* 5|_|_|_|_|_|
         * 4|_|_|_|_|_|
         * 3|_|_|X|_|_|
         * 2|_|_|_|_|_|
         * 1|_|_|_|_|_|
         *   1 2 3 4 5  */
        [TestCase(1, 5, 3),   TestCase(2, 5, 2.5), TestCase(3, 5, 2), TestCase(4, 5, 2.5), TestCase(5, 5, 3)]
        [TestCase(1, 4, 2.5), TestCase(2, 4, 1.5), TestCase(3, 4, 1), TestCase(4, 4, 1.5), TestCase(5, 4, 2.5)]
        [TestCase(1, 3, 2),   TestCase(2, 3, 1),                      TestCase(4, 3, 1),   TestCase(5, 3, 2)]
        [TestCase(1, 2, 2.5), TestCase(2, 2, 1.5), TestCase(3, 2, 1), TestCase(4, 2, 1.5), TestCase(5, 2, 2.5)]
        [TestCase(1, 1, 3),   TestCase(2, 1, 2.5), TestCase(3, 1, 2), TestCase(4, 1, 2.5), TestCase(5, 1, 3)]
        public void FindPath_WhenDistanceIsThreeFromPos3x3_ReturnsAdjacentSpacesByShortestPath(int x, int y, decimal expectedMaxDistance)
        {
            var positionToCheck = new Position(x, y, 0);

            var startPosition = new Position(3, 3, 0);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(startPosition, 3);


            var path = results.Single(p => p.EndPosition == positionToCheck);
            path.PathPositions.ToList().ForEach(p => Debug.WriteLine(p));
            Assert.That(path.Distance, Is.InRange(expectedMaxDistance - 0.25M, expectedMaxDistance));
        }

        [Test]
        public void FindPath_WhenPathIsBlocked_DoesNotIncludedBlockedPositions()
        {
            var position = new Position(3, 3, 0);
            _mockMap.Setup(t => t.IsAllowedPosition(position)).Returns(false);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(position.North(), 3);

            var path = results.SingleOrDefault(t => t.EndPosition == position);
            Assert.That(path, Is.Null);
        }

        /* 5|_|_|_|_|_|
         * 4|_|_|_|O|_|
         * 3|_|O|X|_|_|
         * 2|_|_|O|_|_|
         * 1|_|_|_|_|_|
         *   1 2 3 4 5  */
        [TestCase(1, 5, 3),   TestCase(2, 5, 2.5), TestCase(3, 5, 2), TestCase(4, 5, 2.5), TestCase(5, 5, 3.5)]
        [TestCase(1, 4, 2.5), TestCase(2, 4, 1.5), TestCase(3, 4, 1),                      TestCase(5, 4, 2.5)]
        [TestCase(1, 3, 3),                                           TestCase(4, 3, 1),   TestCase(5, 3, 2)]
        [TestCase(1, 2, 2.5), TestCase(2, 2, 1.5),                    TestCase(4, 2, 1.5), TestCase(5, 2, 2.5)]
        [TestCase(1, 1, 3),   TestCase(2, 1, 2.5), TestCase(3, 1, 3), TestCase(4, 1, 2.5), TestCase(5, 1, 3)]
        public void FindPath_WhenPathIsBlockedNavigatesAroundBlocks_ReturnsAdjacentSpacesByShortestPath(int x, int y, decimal expectedMaxDistance)
        {
            _mockMap.Setup(t => t.IsAllowedPosition(new Position(2, 3, 0))).Returns(false);
            _mockMap.Setup(t => t.IsAllowedPosition(new Position(3, 2, 0))).Returns(false);
            _mockMap.Setup(t => t.IsAllowedPosition(new Position(4, 4, 0))).Returns(false);
            var sut = new ShortestPath(_mockMap.Object);
            var results = sut.FindPath(new Position(3, 3, 0), 4);

            var path = results.Single(p => p.EndPosition == new Position(x, y, 0));
            path.PathPositions.ToList().ForEach(p => Debug.WriteLine(p));
            Assert.That(path.Distance, Is.InRange(expectedMaxDistance - 0.25M, expectedMaxDistance));
        }

        public IOrderedEnumerable<Position> Sort(IEnumerable<Position> positions)
        {
            return positions.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z);
        }
    }
}
