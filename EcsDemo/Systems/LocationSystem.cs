namespace WitcherDemo.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ECS;

    using PathFinding;

    using WitcherDemo.Components;

    public class LocationSystem : System<HasLocation>
    {
        private readonly Dictionary<Coord, List<Entity>> entityLocations = new Dictionary<Coord, List<Entity>>();

        public override void AddComponent(Entity entity, HasLocation component)
        {
            this.AddToLocation(new Coord(component.X, component.Y), entity);
            component.ChangedLocation += this.ChangedLocation;
            base.AddComponent(entity, component);
        }

        public override void RemoveComponent(Entity entity)
        {
            this.entityLocations[this.CoordForEntity(entity)].Remove(entity);
            base.RemoveComponent(entity);
        }

        public IEnumerable<Entity> EntitiesAtSameLocation(Entity entity)
        {
            var coord = this.CoordForEntity(entity);
            return this.entityLocations[coord].Where(e => e != entity);
        }

        public bool IsEntityAt(int x, int y)
        {
            var coord = new Coord(x, y);
            return this.entityLocations.ContainsKey(coord) && this.entityLocations[coord].Count > 0;
        }

        private void AddToLocation(Coord coord, Entity entity)
        {
            if (!this.entityLocations.ContainsKey(coord))
            {
                this.entityLocations[coord] = new List<Entity>();
            }

            this.entityLocations[coord].Add(entity);
        }

        private Coord CoordForEntity(Entity entity)
        {
            var location = this.GetComponentForEntity(entity);
            return new Coord(location.X, location.Y);
        }

        private void ChangedLocation(int oldX, int oldY, int x, int y, Entity entity)
        {
            this.entityLocations[new Coord(oldX, oldY)].Remove(entity);
            this.AddToLocation(new Coord(x, y), entity);
        }
    }
}
