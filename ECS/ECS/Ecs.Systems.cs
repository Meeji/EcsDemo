namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed partial class Ecs
    {
        private readonly List<ISystem<Component>> systems = new List<ISystem<Component>>();

        private readonly List<ISystem<UpdatableComponent>> updatableSystems = new List<ISystem<UpdatableComponent>>();

        private readonly List<ISystem<AsyncUpdatableComponent>> asyncUpdatableSystems = new List<ISystem<AsyncUpdatableComponent>>();

        public IEcs WithSystem<T>() where T : Component
        {
            return this.WithCustomSystem<System<T>, T>();
        }

        public IEcs WithUpdatableSystem<T>() where T : UpdatableComponent
        {
            return this.WithCustomUpdatableSystem<System<T>, T>();
        }

        public IEcs WithAsyncUpdatableSystem<T>() where T : AsyncUpdatableComponent
        {
            return this.WithCustomAsyncUpdatableSystem<System<T>, T>();
        }

        public IEcs WithCustomSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : Component
        {
            return this.WithSystem(new T1());
        }

        public IEcs WithCustomUpdatableSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : UpdatableComponent
        {
            return this.WithUpdatableSystem(new T1());
        }

        public IEcs WithCustomAsyncUpdatableSystem<T1, T2>() where T1 : ISystem<T2>, new() where T2 : AsyncUpdatableComponent
        {
            return this.WithAsyncUpdatableSystem(new T1());
        }

        public IEcs WithSystem<T>(ISystem<T> system) where T : Component
        {
            this.systems.Add(system);
            return this;
        }

        public IEcs WithUpdatableSystem<T>(ISystem<T> system) where T : UpdatableComponent
        {
            this.systems.Add(system);
            this.updatableSystems.Add(system);
            return this;
        }

        public IEcs WithAsyncUpdatableSystem<T>(ISystem<T> system) where T : AsyncUpdatableComponent
        {
            this.systems.Add(system);
            this.asyncUpdatableSystems.Add(system);
            return this;
        }

        public ISystem<T> GetSystem<T>() where T : Component
        {
            var type = typeof(T);

            return this.systems.Where(system => system.IsType(type)).Cast<ISystem<T>>().FirstOrDefault();
        }

        public ISystem<Component> GetSystem(Type type)
        {
            return this.systems.Where(system => system.IsType(type)).Cast<ISystem<Component>>().FirstOrDefault();
        }
    }
}
