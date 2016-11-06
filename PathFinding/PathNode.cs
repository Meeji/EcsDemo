namespace PathFinding
{
    using System;
    using System.Collections.Generic;

    public class PathNode : IComparable<PathNode>, IEquatable<PathNode>
    {
        private readonly PathNode parent;

        private readonly Coord target;

        private int cost = -1;

        private int toParentCost = -1;

        public PathNode(Coord coord, Coord target)
        {
            this.Coord = coord;

            this.target = target;
        }

        public PathNode(Coord coord, Coord target, PathNode parent) : this(coord, target)
        {
            this.parent = parent;
        }

        public Coord Coord { get; }

        public int Cost
        {
            get
            {
                if (this.cost == -1)
                {
                    this.GenerateCost();
                }

                return this.cost;
            }
        }

        public int ToParentCost
        {
            get
            {
                if (this.toParentCost == -1)
                {
                    this.GenerateCost();
                }

                return this.toParentCost;
            }
        }

        public int CompareTo(PathNode other)
        {
            if (this.Cost < other.Cost)
            {
                return -1;
            }

            return this.Cost == other.Cost ? 0 : 1;
        }

        public bool Equals(PathNode other)
        {
            return this.Coord.Equals(other?.Coord);
        }

        public override int GetHashCode()
        {
            return this.Coord.GetHashCode();
        }

        public IEnumerable<Coord> GetPath()
        {
            if (this.parent != null)
            {
                foreach (var coord in this.parent.GetPath())
                {
                    yield return coord;
                }
            }

            yield return this.Coord;
        }

        private void GenerateCost()
        {
            if (this.parent == null)
            {
                this.toParentCost = 0;
                return;
            }

            var toParent = this.Coord.X != this.parent.Coord.X && this.Coord.Y != this.parent.Coord.Y ? 14 : 10; // 14 if diagonal move, 10 otherwise

            this.toParentCost = toParent + this.parent.ToParentCost;

            this.cost = ((Math.Abs(this.Coord.X - this.target.X) + Math.Abs(this.Coord.Y - this.target.Y)) * 10) + this.toParentCost;
        }
    }
}