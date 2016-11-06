namespace ECS
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a system which associates components with entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISystem<out T> where T : Component
    {
        /// <summary>
        /// Fires when a component and entity are associated with each other
        /// </summary>
        event Action<Entity, T> ComponentAdded;

        /// <summary>
        /// Associates the given component with the given entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="component">The component</param>
        void AddComponent(Entity entity, Component component);

        /// <summary>
        /// Removes the component associated with the given entity
        /// </summary>
        /// <param name="entity">The entity</param>
        void RemoveComponent(Entity entity);

        /// <summary>
        /// Checks whether the given entity is associated with a component in this system
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The result</returns>
        bool EntityHasComponent(Entity entity);

        /// <summary>
        /// Returns the component associated with the given entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The component</returns>
        T GetComponentForEntity(Entity entity);

        /// <summary>
        /// Returns true if this system manages assocations with the given component type
        /// </summary>
        /// <param name="componentType">Component type</param>
        /// <returns>The result</returns>
        bool IsType(Type componentType);

        /// <summary>
        /// Enumerates all components within this system
        /// </summary>
        /// <returns>Components</returns>
        IEnumerable<T> AllComponents();

        /// <summary>
        /// Enumerates the IDs of all entities with associated components in this system
        /// </summary>
        /// <returns>Ids</returns>
        IEnumerable<int> AllEntityIds();

        /// <summary>
        /// Returns the ID of the entity associated with the given component
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>ID</returns>
        int EntityIdForComponent(Component component);
    }
}