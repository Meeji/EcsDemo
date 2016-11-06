namespace WitcherDemo.Components
{
    using System;
    using System.Linq;

    using ECS;

    using WitcherDemo.Systems;

    public class KilledBy : UpdatableComponent
    {
        private readonly ActorType type;

        public KilledBy(ActorType type)
        {
            this.type = type;
        }

        public override Action Update()
        {
            var locationSystem = this.Ecs.GetSystem<HasLocation>() as LocationSystem;

            if (locationSystem == null)
            {
                throw new Exception("Location system couldn't be retrieved and cast"); // TODO: Custom exception type?
            }

            if (
                locationSystem.EntitiesAtSameLocation(this.Entity)
                    .Any(e => this.Ecs.GetComponent<IsActor>(e).Type == this.type))
            {
                return () =>
                {
                    if (Ecs.HasEntity(this.Entity))
                    {
                        this.Ecs.RemoveEntity(this.Entity); // Blarg ded
                    }
                };
            }

            return null;
        }

        protected override void Initialise()
        {
        }
    }
}
