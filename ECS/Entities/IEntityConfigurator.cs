namespace ECS
{
    /// <summary>
    /// A class which allows the fluent configuration of an entity's components
    /// </summary>
    public interface IEntityConfigurator
    {
        /// <summary>
        /// The entity being configured
        /// </summary>
        Entity Entity { get; }

        /// <summary>
        /// Instatiates the given component type and associates it with the entity 
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>The entity configurator</returns>
        EntityConfigurator WithComponent<T>() where T : Component, new();

        /// <summary>
        /// Associates the given component with the entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="component">The component</param>
        /// <returns>The entity configurator</returns>
        EntityConfigurator WithComponent<T>(T component) where T : Component;
    }
}