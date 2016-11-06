namespace PathFinding
{
    using System.Collections.Generic;
    using System.Linq;

    public class PathFinder
    {
        private readonly Coord gridSize;

        private readonly HashSet<Coord> impassables;

        public PathFinder(Coord gridSize) : this(gridSize, new Coord[0])
        {
        }

        public PathFinder(Coord gridSize, IEnumerable<Coord> impassables)
        {
            this.gridSize = gridSize;
            this.impassables = new HashSet<Coord>(impassables);
        }

        public IEnumerable<Coord> FindPath(Coord start, Coord end)
        {
            var closed = new HashSet<PathNode>();
            var open = new HashSet<PathNode>();

            var current = new PathNode(start, end);
            closed.Add(current);

            var neighbours = this.CreateNeighbours(current, end);
            neighbours.ExceptWith(closed);
            open.UnionWith(neighbours);

            while (!current.Coord.Equals(end))
            {
                if (open.Count == 0)
                {
                    return new Coord[0];
                }

                current = open.OrderBy(p => p.Cost).First();

                neighbours = this.CreateNeighbours(current, end);
                neighbours.ExceptWith(closed);
                open.UnionWith(neighbours);
                open.Remove(current);
                closed.Add(current);
            }

            return current.GetPath();
        }

        private ISet<PathNode> CreateNeighbours(PathNode pathNode, Coord target)
        {
            var coord = pathNode.Coord;

            return new[]
            {
                new Coord(coord.X - 1, coord.Y + 1), // top left
                new Coord(coord.X, coord.Y + 1),     // top
                new Coord(coord.X + 1, coord.Y + 1), // top right
                new Coord(coord.X - 1, coord.Y),     // left
                new Coord(coord.X + 1, coord.Y),     // right
                new Coord(coord.X - 1, coord.Y - 1), // bottom left
                new Coord(coord.X, coord.Y - 1),     // bottom
                new Coord(coord.X + 1, coord.Y - 1), // bottom right
            }
                .Where(c => c.X >= 0 && c.Y >= 0 && c.X < this.gridSize.X && c.Y < this.gridSize.Y) // Within bounds
                .Where(c => !this.impassables.Contains(c))                                          // Not impassable
                .Select(c => new PathNode(c, target, pathNode))                                     // As PathNodes
                .ToHashSet();                                                                       // Into a set
        }
    }
}
