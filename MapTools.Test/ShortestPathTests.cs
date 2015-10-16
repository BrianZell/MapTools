using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class ShortestPathTests
    {
        [Test]
        public void FindPath_StartPosition_IsTheSameAsProvidedStartPosition()
        {
            var startPosition = new Position(3, 3, 0);
            var sut = new ShortestPath(new Map());
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
            var sut = new ShortestPath(new Map());
            var results = sut.FindPath(startPosition, 1);

            Assert.That(results.Select(x => x.EndPosition), Is.EquivalentTo(expectedResults));
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
            var sut = new ShortestPath(new Map());
            var results = sut.FindPath(startPosition, 2);
            
            Assert.That(Sort(results.Select(x => x.EndPosition)), Is.EquivalentTo(Sort(expectedResults)));
            results.ToList().ForEach(Console.WriteLine);
        }

        public IOrderedEnumerable<Position> Sort(IEnumerable<Position> positions)
        {
            return positions.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z);
        }
    }

    public class Map
    {
    }

    public class Path
    {
        private const decimal StraightDistance = 1.0M;
        private const decimal DiagonalDistance = 1.4142M;

        private readonly List<Position> _pathPositions = new List<Position>();
        
        public Path(Position start)
        {
            StartPosition = start;
            EndPosition = start;
            Distance = 0.0M;
        }

        private Path(Path source, Position position, decimal distance)
        {
            this.StartPosition = source.StartPosition;
            this.EndPosition = position;
            this.Distance = source.Distance + distance;
            this._pathPositions.Add(position);
        }

        public override string ToString()
        {
            return string.Format("{0} => {1} : {2}", StartPosition, EndPosition, Distance);
        }

        public Position StartPosition { get; private set; }

        public Position EndPosition { get; private set; }

        public decimal Distance { get; private set; }

        public IReadOnlyCollection<Position> PathPositions {
            get { return _pathPositions.AsReadOnly(); }
        }

        public Path North()
        {
            return new Path(this,this.EndPosition.North(), StraightDistance);
        }

        public Path NorthWest()
        {
            return new Path(this, this.EndPosition.NorthWest(), DiagonalDistance);
        }

        public Path NorthEast()
        {
            return new Path(this, this.EndPosition.NorthEast(), DiagonalDistance);
        }

        public Path South()
        {
            return new Path(this, this.EndPosition.South(), StraightDistance);
        }

        public Path SouthWest()
        {
            return new Path(this, this.EndPosition.SouthWest(), DiagonalDistance);
        }

        public Path SouthEast()
        {
            return new Path(this, this.EndPosition.SouthEast(), DiagonalDistance);
        }

        public Path East()
        {
            return new Path(this, this.EndPosition.East(), StraightDistance);
        }

        public Path West()
        {
            return new Path(this, this.EndPosition.West(), StraightDistance);
        }
    }

    public class ShortestPath
    {
        public ShortestPath(Map map)
        {
        }

        public IEnumerable<Path> FindPath(Position start, decimal distance)
        {
            return FindPaths(new Path(start), distance, new List<Position>(new [] {start}));
        }

        private IEnumerable<Path> FindPaths(Path path, decimal distance, List<Position> coveredPositions)
        {
            var adjacentSpaces = new List<Path>
                       {
                           path.North(),
                           path.South(),
                           path.East(),
                           path.West(),
                           path.NorthEast(),
                           path.NorthWest(),
                           path.SouthWest(),
                           path.SouthEast(),
                       };

            var validAdjacentSpaces = adjacentSpaces
                                        .Where(x => x.Distance <= distance && !coveredPositions.Contains(x.EndPosition))
                                        .ToList();

            coveredPositions.AddRange(validAdjacentSpaces.Select(x => x.EndPosition));
            var results = new List<Path>(validAdjacentSpaces);
            results.AddRange(validAdjacentSpaces.SelectMany(x => FindPaths(x, distance, coveredPositions)));

            return results;
        }
    }
}
