namespace ECS
{
    using System;

    public sealed partial class Ecs
    {
        public event Action<Entity> EntityAdded;

        public event Action<Entity> EntityRemoveStarted;
    }
}
