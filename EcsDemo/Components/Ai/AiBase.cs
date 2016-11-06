namespace WitcherDemo.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ECS;

    using PathFinding;

    public abstract class AiBase : IAi
    {
        private Random rng;

        public Entity Entity { get; set; }

        public static double DistanceBetween(Coord p1, Coord p2)
        {
            if (p1.Equals(p2))
            {
                return 0.0;
            }

            return Math.Sqrt(Math.Pow(Math.Abs(p1.X - p2.X), 2) + Math.Pow(Math.Abs(p1.Y - p2.Y), 2));
        }

        public abstract Coord Update(IEnumerable<Coord> trees, IEnumerable<Coord> monsters, Coord witcher, Coord me);

        protected Coord Wander(IEnumerable<Coord> avoid, Coord me)
        {
            if (this.rng == null)
            {
                this.rng = new Random(this.Entity.Id);
            }

            // Decided whether to move at all
            if (this.rng.Next(0, 2) == 0)
            {
                return me;
            }

            Coord next;

            switch (this.rng.Next(0, 3))
            {
                case 0:
                    next = new Coord(me.X, me.Y + 1);
                    break;
                case 1:
                    next = new Coord(me.X, me.Y - 1);
                    break;
                case 2:
                    next = new Coord(me.X + 1, me.Y);
                    break;
                case 3:
                    next = new Coord(me.X - 1, me.Y);
                    break;
                default:
                    return me;
            }

            return avoid.Contains(next) ? me : next;
        }
    }
}