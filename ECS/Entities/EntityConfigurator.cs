namespace ECS
{
    public class EntityConfigurator : IEntityConfigurator
    {
        private readonly IEcs ecs;

        internal EntityConfigurator(IEcs ecs, Entity entity)
        {
            this.ecs = ecs;
            this.Entity = entity;
        }

        public Entity Entity { get; }

        public EntityConfigurator WithComponent<T>() where T : Component, new()
        {
            return this.WithComponent(new T());
        }

        public EntityConfigurator WithComponent<T>(T component) where T : Component
        {
            this.ecs.GetSystem<T>().AddComponent(this.Entity, component);
            component.InitFromEcs(this.ecs);
            return this;
        }
    }
}
