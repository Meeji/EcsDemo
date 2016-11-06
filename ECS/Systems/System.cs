namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class System<T> : ISystem<T> where T : Component
    {
        private readonly IDictionary<int, T> components = new Dictionary<int, T>();

        public event Action<Entity, T> ComponentAdded;

        public virtual void AddComponent(Entity entity, T component)
        {
            this.components.Add(entity.Id, component);
            this.ComponentAdded?.Invoke(entity, component);
        }

        public void AddComponent(Entity entity, Component component)
        {
            this.AddComponent(entity, (T)component);
        }

        public virtual void RemoveComponent(Entity entity)
        {
            this.components.Remove(entity.Id);
        }

        public bool EntityHasComponent(Entity entity)
        {
            return this.components.ContainsKey(entity.Id);
        }

        public T GetComponentForEntity(Entity entity)
        {
            return this.components[entity.Id];
        }

        public bool IsType(Type componentType)
        {
            return componentType == typeof(T);
        }

        public IEnumerable<T> AllComponents()
        {
            return this.components.Select(c => c.Value);
        }

        public IEnumerable<int> AllEntityIds()
        {
            return this.components.Keys;
        }

        public int EntityIdForComponent(Component component)
        {
            return this.components.First(c => c.Value == component).Key;
        }
    }
}
