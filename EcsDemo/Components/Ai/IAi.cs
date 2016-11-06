namespace WitcherDemo.Components
{
    using System.Collections.Generic;

    using ECS;

    using PathFinding;

    public interface IAi
    {
        Entity Entity { get; set; }

        Coord Update(IEnumerable<Coord> trees, IEnumerable<Coord> monsters, Coord witcher, Coord me);
    }
}