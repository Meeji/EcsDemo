namespace ECS
{
    using System;

    public sealed partial class Ecs
    {
        public bool HasComponent(Entity entity, Type component)
        {
            foreach (var system in this.systems)
            {
                if (system.IsType(component))
                {
                    return system.EntityHasComponent(entity);
                }
            }

            return false;
        }

        public bool HasComponent<T>(Entity entity) where T : Component
        {
            return this.HasComponent(entity, typeof(T));
        }

        public T GetComponent<T>(Entity entity) where T : Component
        {
            var type = typeof(T);

            foreach (var system in this.systems)
            {
                if (system.IsType(type))
                {
                    if (system.EntityHasComponent(entity))
                    {
                        return (T)system.GetComponentForEntity(entity);
                    }

                    break;
                }
            }

            throw new InvalidOperationException("Entity does not have this component");
        }
    }
}
