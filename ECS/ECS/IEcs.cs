namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for The ECS container
    /// </summary>
    public interface IEcs
    {
        /// <summary>
        /// Event that firest when an entity is added to the container.
        /// </summary>
        event Action<Entity> EntityAdded;

        /// <summary>
        /// Event that fires before an entity is removed from the container.
        /// </summary>
        event Action<Entity> EntityRemoveStarted;

        /// <summary>
        /// Number of ticks of the simulation that have run.
        /// </summary>
        int Tick { get; }

        /// <summary>
        /// Runs a tick of the simulation by updating all components synchronously.
        /// </summary>
        void Update();

        /// <summary>
        /// Runs a tick of the simulation by updating all components asynchronously.
        /// </summary>
        Task UpdateAsync();

        /// <summary>
        /// Checks whether an entity has a component of the given type
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="component">Type of the component to check</param>
        /// <returns></returns>
        bool HasComponent(Entity entity, Type component);

        /// <summary>
        /// Checks whether an entity has a component of the given type
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns></returns>
        bool HasComponent<T>(Entity entity) where T : Component;

        /// <summary>
        /// Gets the component of the given type associated with the given entity
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The component</returns>
        T GetComponent<T>(Entity entity) where T : Component;

        /// <summary>
        /// Creates and registers a system for the given component type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithSystem<T>() where T : Component;

        /// <summary>
        /// Creates and registers a system for the given component type and registers the system as updatable
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithUpdatableSystem<T>() where T : UpdatableComponent;

        /// <summary>
        /// Creates and registers a system for the given component type and registers the system as asynchronously updatable
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithAsyncUpdatableSystem<T>() where T : AsyncUpdatableComponent;

        /// <summary>
        /// Instantiates the given system and registers it for the given component type
        /// </summary>
        /// <typeparam name="T1">System type</typeparam>
        /// <typeparam name="T2">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithCustomSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : Component;

        /// <summary>
        /// Instantiates the given system and registers it for the given component type and registers the system as updatable
        /// </summary>
        /// <typeparam name="T1">System type</typeparam>
        /// <typeparam name="T2">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithCustomUpdatableSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : UpdatableComponent;

        /// <summary>
        /// Instantiates the given system and registers it for the given component type and registers the system as asynchronously updatable
        /// </summary>
        /// <typeparam name="T1">System type</typeparam>
        /// <typeparam name="T2">Component type</typeparam>
        /// <returns>The ECS container</returns>
        IEcs WithCustomAsyncUpdatableSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : AsyncUpdatableComponent;

        /// <summary>
        /// Registers the supplied system
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="system">The system</param>
        /// <returns>The ECS container</returns>
        IEcs WithSystem<T>(ISystem<T> system) where T : Component;

        /// <summary>
        /// Registers the supplied system as updatable
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="system">The system</param>
        /// <returns>The ECS container</returns>
        IEcs WithUpdatableSystem<T>(ISystem<T> system) where T : UpdatableComponent;

        /// <summary>
        /// Registers the supplied system as asynchronously updatable
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="system">The system</param>
        /// <returns>The ECS container</returns>
        IEcs WithAsyncUpdatableSystem<T>(ISystem<T> system) where T : AsyncUpdatableComponent;

        /// <summary>
        /// Returns the system for the given component type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>The system</returns>
        ISystem<T> GetSystem<T>() where T : Component;

        /// <summary>
        /// Returns the system for the given component type
        /// </summary>
        /// <param name="type">Component type</param>
        /// <returns>The system</returns>
        ISystem<Component> GetSystem(Type type);

        /// <summary>
        /// Creates and registers a new entity, and returns it wrapped in a configurator
        /// </summary>
        /// <returns>Wrapped entity</returns>
        IEntityConfigurator NewEntity();

        /// <summary>
        /// Registers the given entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns></returns>
        IEntityConfigurator AddEntity(Entity entity);

        /// <summary>
        /// Removes the given entity from The ECS container and removes all components associated with it. Return the entity when done
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The entity</returns>
        Entity RemoveEntity(Entity entity);

        /// <summary>
        /// Checks whether the given entity is registered
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns></returns>
        bool HasEntity(Entity entity);

        /// <summary>
        /// Wraps the given entity in a configurator
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The wrapped entity</returns>
        IEntityConfigurator ConfigureEntity(Entity entity);

        /// <summary>
        /// Returns an enumerable of all entities with associated components of the given type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Enumerable of entities</returns>
        IEnumerable<Entity> EntitiesWithComponent<T>() where T : Component;

        /// <summary>
        /// Returns the entity associated with the given component
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="component">The component</param>
        /// <returns>The entity</returns>
        Entity GetEntityForComponent<T>(T component) where T : Component;

        /// <summary>
        /// Returns the entity associated with the given component
        /// </summary>
        /// <param name="component">The component</param>
        /// <param name="componentType">Type of component</param>
        /// <returns></returns>
        Entity GetEntityForComponent(Component component, Type componentType);
    }
}