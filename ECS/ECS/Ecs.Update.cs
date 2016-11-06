namespace ECS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    // TODO: Share code more? Not sure I want to try and interleave sync and async too much
    public sealed partial class Ecs
    {
        public int Tick { get; private set; }

        public void Update()
        {
            var finalActions = new List<Action>(this.asyncUpdatableSystems.Count + this.updatableSystems.Count);
            foreach (var system in this.asyncUpdatableSystems.Concat(this.updatableSystems))
            {
                foreach (var component in system.AllComponents())
                {
                    var action = component.Update();

                    if (action != null)
                    {
                        finalActions.Add(action);
                    }
                }
            }

            foreach (var action in finalActions)
            {
                action();
            }
        }

        /// <summary>
        /// Runs one 'tick' of the simulation.
        /// First starts running update on all async components, then runs update on synchronous components.
        /// Finally waits for async components to finish and finally runs their final actions.
        /// </summary>
        public async Task UpdateAsync()
        {
            var finalActions = new List<Action>(this.asyncUpdatableSystems.Count + this.updatableSystems.Count);
            var tasks = new List<Task<Action>>(this.asyncUpdatableSystems.Count);

            // Start running all asynchronous updates...
            foreach (var system in this.asyncUpdatableSystems)
            {
                foreach (var component in system.AllComponents())
                {
                    var task = component.AsyncUpdate();
                    tasks.Add(task);
                }
            }

            // Run all synchronous tasks and add their final actions to queue.
            foreach (var system in this.updatableSystems)
            {
                foreach (var component in system.AllComponents())
                {
                    var action = component.Update();

                    if (action != null)
                    {
                        finalActions.Add(action);
                    }
                }
            }

            // Await end of all async tasks and add their final actions to queue.
            foreach (var task in tasks)
            {
                var action = await task;

                if (action != null)
                {
                    finalActions.Add(action);
                }
            }

            // Execute all final actions.
            foreach (var action in finalActions)
            {
                action();
            }

            this.Tick += 1;
        }
    }
}
