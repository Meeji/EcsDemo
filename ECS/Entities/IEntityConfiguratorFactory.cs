namespace ECS
{
    /// <summary>
    /// Interface for a factory that creates IEntityConfigurators
    /// </summary>
    internal interface IEntityConfiguratorFactory
    {
        /// <summary>
        /// Creates an IEntityConfigurator using a given ECS and Entity as a base
        /// </summary>
        /// <param name="ecs">The ECS</param>
        /// <param name="entity">The Entity</param>
        /// <returns></returns>
        IEntityConfigurator Create(IEcs ecs, Entity entity);
    }
}