namespace ECS
{
    using System;

    /// <summary>
    /// Defines a component which CAN be updated once per tick (that is, every time update is called on the ECS container)
    /// However, this will only happen if the system the component is registered with is itself registered as updatable.
    /// </summary>
    public abstract class UpdatableComponent : Component
    {
        /// <summary>
        /// Updates the component 1 tick. Returns an action which is run after all components are updated. Any changes to the ECS's state should run in the action.
        /// </summary>
        /// <returns>Action which makes changes to the ECS state.</returns>
        public abstract Action Update();
    }
}
