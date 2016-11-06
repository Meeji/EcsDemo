namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed partial class Ecs
    {
        private readonly Dictionary<int, Entity> entityLookup = new Dictionary<int, Entity>();

        public IEntityConfigurator NewEntity()
        {
            return this.AddEntity(new Entity());
        }

        public IEntityConfigurator AddEntity(Entity entity)
        {
            this.entityLookup.Add(entity.Id, entity);
            this.EntityAdded?.Invoke(entity);
            return this.entityConfiguratorFactory.Create(this, entity);
        }

        public Entity RemoveEntity(Entity entity)
        {
            this.EntityRemoveStarted?.Invoke(entity);

            this.entityLookup.Remove(entity.Id);
            foreach (var system in this.systems)
            {
                system.RemoveComponent(entity);
            }

            return entity;
        }

        public bool HasEntity(Entity entity)
        {
            return this.entityLookup.ContainsKey(entity.Id);
        }

        public IEntityConfigurator ConfigureEntity(Entity entity)
        {
            return this.entityConfiguratorFactory.Create(this, entity);
        }

        public IEnumerable<Entity> EntitiesWithComponent<T>() where T : Component
        {
            return this.GetSystem<T>().AllEntityIds().Select(id => this.entityLookup[id]);
        }

        public Entity GetEntityForComponent<T>(T component) where T : Component
        {
            var entityId = this.GetSystem<T>().EntityIdForComponent(component);
            return this.entityLookup[entityId];
        }

        public Entity GetEntityForComponent(Component component, Type componentType)
        {
            var entityId = this.GetSystem(componentType).EntityIdForComponent(component);
            return this.entityLookup[entityId];
        }
    }
}
