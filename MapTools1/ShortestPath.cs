using System.Collections.Generic;
using System.Linq;

namespace MapTools
{
    public class ShortestPath
    {
        private readonly IAllowsMovement _allowsMovement;

        public ShortestPath(IAllowsMovement allowsMovement)
        {
            _allowsMovement = allowsMovement;
        }

        public IEnumerable<Path> FindPath(Position start, decimal distance)
        {
            var newPath = new Path(start);
            var resultDict = new Dictionary<Position, Path> {{newPath.EndPosition, newPath}};
            FindPaths(new Path(start), distance, resultDict);
            return resultDict.Values.Where(x => x.Distance > 0).OrderBy(x => x.Distance);
        }

        private void FindPaths(Path path, decimal distance, Dictionary<Position,Path> coveredPositions)
        {
            var adjacentSpaces = path.GetPathsToAdjacentSpaces();
            var inRangeAdjacentSpaces = adjacentSpaces
                .Where(x => x.Distance <= distance)
                .Where(x => _allowsMovement.IsAllowedPosition(x.EndPosition))
                .ToList();

            var validAdjacentSpaces = new List<Path>();
            foreach (var inRangeAdjacentSpace in inRangeAdjacentSpaces)
            {
                if (coveredPositions.ContainsKey(inRangeAdjacentSpace.EndPosition))
                {
                    if (coveredPositions[inRangeAdjacentSpace.EndPosition].Distance > inRangeAdjacentSpace.Distance)
                    {
                        coveredPositions[inRangeAdjacentSpace.EndPosition] = inRangeAdjacentSpace;
                        validAdjacentSpaces.Add(inRangeAdjacentSpace);
                    }
                }
                else
                {
                    coveredPositions.Add(inRangeAdjacentSpace.EndPosition,inRangeAdjacentSpace);
                    validAdjacentSpaces.Add(inRangeAdjacentSpace);
                }
            }

            validAdjacentSpaces.ForEach(x => FindPaths(x,distance,coveredPositions));
        }
    }
}