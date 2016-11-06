namespace WitcherDemo.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ECS;

    using PathFinding;

    public class DrownerAi : AiBase, IAi
    {
        public override Coord Update(IEnumerable<Coord> trees, IEnumerable<Coord> monsters, Coord witcher, Coord me)
        {
            // Witcher? What witcher?
            if (witcher == null || AiBase.DistanceBetween(me, witcher) > 5)
            {
                return this.Wander(trees, me);
            }

            // Try to escape Geralt!
            var x = me.X;
            var y = me.Y;
            if (witcher.X < me.X)
            {
                x += 1;
            }

            if (witcher.X > me.X)
            {
                x -= 1;
            }

            if (witcher.Y < me.Y)
            {
                y += 1;
            }

            if (witcher.Y > me.Y)
            {
                y -= 1;
            }

            var newLocation = new Coord(x, y);

            // If we've got our back to a tree, cower. Otherwise, run!
            return trees.Contains(newLocation) ? me : newLocation;
        }
    }
}
