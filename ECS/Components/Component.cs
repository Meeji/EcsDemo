namespace ECS
{
    /// <summary>
    /// Base class for a component
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Parent ECS container
        /// </summary>
        protected IEcs Ecs { get; private set; }

        /// <summary>
        /// Associated entity
        /// </summary>
        protected Entity Entity { get; private set; }

        /// <summary>
        /// Is run from within the ECS container when the component is registered
        /// </summary>
        /// <param name="ecs"></param>
        internal void InitFromEcs(IEcs ecs)
        {
            this.Ecs = ecs;
            this.Entity = this.Ecs.GetEntityForComponent(this, this.GetType());
            this.Initialise();
        }

        /// <summary>
        /// Initialisation logic that relies on this.Ecs or this.Entity should go here and NOT in the constructor of the component.
        /// </summary>
        protected abstract void Initialise();
    }
}
