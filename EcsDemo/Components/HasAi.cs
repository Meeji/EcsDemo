namespace WitcherDemo.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ECS;

    using PathFinding;

    public class HasAi : AsyncUpdatableComponent
    {
        private readonly IAi ai;

        public HasAi(IAi ai)
        {
            this.ai = ai;
        }

        public override Action Update()
        {
            var actors = this.Ecs.EntitiesWithComponent<IsActor>();
            var actorSystem = this.Ecs.GetSystem<IsActor>();
            var locationSystem = this.Ecs.GetSystem<HasLocation>();

            var trees = new List<Coord>();
            var monsters = new List<Coord>();
            Coord witcher = null;
            Coord me = null;

            foreach (var actor in actors)
            {
                var location = locationSystem.GetComponentForEntity(actor);
                var coord = new Coord(location.X, location.Y);

                if (actor.Id == this.Entity.Id)
                {
                    me = coord;
                }

                switch (actorSystem.GetComponentForEntity(actor).Type)
                {
                    case ActorType.Witcher:
                        witcher = coord;
                        break;
                    case ActorType.Monster:
                        monsters.Add(coord);
                        break;
                    case ActorType.Tree:
                        trees.Add(coord);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var newLocation = this.ai.Update(trees, monsters, witcher, me);

            // Update our location, if we're still part of the service
            return () =>
                {
                    if (locationSystem.EntityHasComponent(this.Entity))
                    {
                        locationSystem.GetComponentForEntity(this.Entity).SetLocation(newLocation.X, newLocation.Y);
                    }
                };
        }

        public override Task<Action> UpdateAsync()
        {
            return Task.Run((Func<Action>)this.Update);
        }

        protected override void Initialise()
        {
            this.ai.Entity = this.Entity;
        }
    }
}
