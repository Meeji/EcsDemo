namespace ECS
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a component which CAN be updated asynchronously once per tick (that is, every time update is called on the ECS container)
    /// However, this will only happen if the system the component is registered with is itself registered as asyncUpdatable.
    /// </summary>
    public abstract class AsyncUpdatableComponent : UpdatableComponent
    {
        /// <summary>
        /// Updates the component 1 tick. Returns a task which itself returns an action which is run after all components are updated. 
        /// Any changes to the ECS's state should run in the action.
        /// </summary>
        /// <returns>Task which returns an action synchronously</returns>
        public abstract Task<Action> UpdateAsync();
    }
}
