namespace ECS
{
    internal class EntityConfiguratorFactory : IEntityConfiguratorFactory
    {
        public IEntityConfigurator Create(IEcs ecs, Entity entity)
        {
            return new EntityConfigurator(ecs, entity);
        }
    }
}
