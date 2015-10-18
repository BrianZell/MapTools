using System.Collections.Generic;
using System.Linq;

namespace MapTools
{
    public class Path
    {
        public const decimal StraightDistance = 1.0M;
        public const decimal DiagonalDistance = 1.4142M;

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
            this._pathPositions.AddRange(source._pathPositions.Concat(new [] {position}));
        }

        public Position StartPosition { get; private set; }

        public Position EndPosition { get; private set; }

        public decimal Distance { get; private set; }

        public IReadOnlyCollection<Position> PathPositions {
            get { return _pathPositions.AsReadOnly(); }
        }

        public IList<Path> GetPathsToAdjacentSpaces()
        {
            return new[]
                       {
                           new Path(this,this.EndPosition.North(), StraightDistance),
                           new Path(this, this.EndPosition.South(), StraightDistance),
                           new Path(this, this.EndPosition.West(), StraightDistance),
                           new Path(this, this.EndPosition.East(), StraightDistance),
                           new Path(this, this.EndPosition.NorthWest(), DiagonalDistance),
                           new Path(this, this.EndPosition.NorthEast(), DiagonalDistance),
                           new Path(this, this.EndPosition.SouthWest(), DiagonalDistance),
                           new Path(this, this.EndPosition.SouthEast(), DiagonalDistance),
                       };
        }

        public override string ToString()
        {
            return string.Format("{0} => {1} : {2}", StartPosition, EndPosition, Distance);
        }
    }
}