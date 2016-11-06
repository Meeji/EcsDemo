namespace PathFinding
{
    using System;

    public class Coord : Tuple<int, int> // Inherit tuple's equality and hash features
    {
        public Coord(int x, int y) : base(x, y)
        {
        }

        public int X => this.Item1;

        public int Y => this.Item2;
    }
}