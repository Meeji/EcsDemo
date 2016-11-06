namespace ECS
{
    public sealed partial class Ecs : IEcs
    {
        private readonly IEntityConfiguratorFactory entityConfiguratorFactory;

        public Ecs() : this(new EntityConfiguratorFactory())
        {
        }

        // CTOR that allows us to inject the EntityConfigurationFactory for future unit testing.
        internal Ecs(IEntityConfiguratorFactory entityConfiguratorFactory)
        {
            this.entityConfiguratorFactory = entityConfiguratorFactory;
        }
    }
}
