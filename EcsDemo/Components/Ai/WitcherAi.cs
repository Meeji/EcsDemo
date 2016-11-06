namespace WitcherDemo.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ECS;

    using PathFinding;

    public class WitcherAi : AiBase, IAi
    {
        private readonly PathFinder pathFinder;

        public WitcherAi(PathFinder pathFinder)
        {
            this.pathFinder = pathFinder;
        }

        public override Coord Update(IEnumerable<Coord> trees, IEnumerable<Coord> monsters, Coord witcher, Coord me)
        {
            var allMonsters = monsters.ToList();

            // Monsters? What monsters?
            if (allMonsters.Count == 0)
            {
                return this.Wander(trees, me);
            }

            var closestMonster = allMonsters.Closest(me);

            var path = this.pathFinder.FindPath(me, closestMonster).ToList();

            // Can't reach monster!
            if (path.Count < 2)
            {
                return this.Wander(trees, me);
            }

            return path[1]; // First node in path is the one we're on.
        }
    }
}
