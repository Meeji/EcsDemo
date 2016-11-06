namespace WitcherDemo
{
    using System;
    using System.Linq;
    using System.Timers;

    using Components;
    using ECS;
    using PathFinding;
    using Renderer;
    using WitcherDemo.Systems;

    /// <summary>
    /// Monolithic set-everything-up main class. Wouldn't be necessary with a good IOC solution.
    /// Fulfils all dependencies and builds the ECS; creates game entities and hooks up events.
    /// </summary>
    public class Game
    {
        // Creates a logger that'll print its messages to the console next to the game
        private static readonly Logger Logger = new Logger();

        private readonly int boundsX;

        private readonly int boundsY;

        public Game(int boundsX, int boundsY)
        {
            this.boundsX = boundsX;
            this.boundsY = boundsY;
        }

        public void Run(bool debug = false)
        {
            var ecs = this.CreateEcs();
            this.CreateEntity(ecs, 250, ActorType.Tree);
            var geralt = this.CreateGeralt(ecs);

            if (debug)
            {
                // For debugging purposes, we'll kill Geralt if he runs headlong into a tree.
                ecs.ConfigureEntity(geralt).WithComponent(new KilledBy(ActorType.Tree));
            }

            this.SetupMonsterDiedEvent(ecs, geralt);
            this.SetupMonstersAppearEvent(ecs);
            this.CreateEntity(ecs, 5, ActorType.Monster);
            this.EnterGameLoop(ecs, geralt);
        }

        private void EnterGameLoop(IEcs ecs, Entity geralt)
        {
            var renderer = new Renderer.Renderer(ecs, this.boundsX, this.boundsY, new RendererCharacters(), Logger, () => ecs.GetComponent<HasMoney>(geralt).Money);

            // Simulation loop - 5 updates a second
            var timer = new Timer(200);
            timer.Elapsed += async (sender, e) =>
            {
                timer.Stop();
                await ecs.UpdateAsync();

                if (ecs.GetSystem<IsActor>().AllComponents().Count(c => c.Type == ActorType.Monster) < 3)
                {
                    this.CreateEntity(ecs, 5, ActorType.Monster);
                }

                renderer.Render();
                timer.Start();
            };
            timer.Start();

            // Press a key to quit
            Console.ReadKey();
        }

        private void SetupMonsterDiedEvent(IEcs ecs, Entity geralt)
        {
            // Get a trigger for Geralt to comment sometimes after a kill.
            var geraltGloats = ecs.GetComponent<Talks>(geralt)
                .GetSpeaker(Talks.GeraltGetsMoney, 0.2);

            // When Geralt kills a monster
            ecs.EntityRemoveStarted += entity =>
            {
                var type = ecs.GetComponent<IsActor>(entity).Type;

                if (type == ActorType.Monster)
                {
                    Logger.AddLog($" * {ecs.GetComponent<HasName>(entity).Name} has been killed!");
                    ecs.GetComponent<HasMoney>(geralt).AddMoney(5);
                    geraltGloats();
                }

                // For debugging also print a message if Geralt dies
                if (type == ActorType.Witcher)
                {
                    Logger.AddLog($" * * {ecs.GetComponent<HasName>(entity).Name} has been killed! (wait, what?)");
                }
            };
        }

        private void SetupMonstersAppearEvent(IEcs ecs)
        {
            var lastMonsterEnteredAtTick = 0;
            ecs.GetSystem<IsActor>().ComponentAdded += (entity, component) =>
            {
                // Only want to announce once per tick and if the entity is a monster
                if (ecs.Tick != lastMonsterEnteredAtTick && component.Type == ActorType.Monster)
                {
                    Logger.AddLog(" * New monsters have entered the fray!");
                    lastMonsterEnteredAtTick = ecs.Tick;
                }
            };
        }

        private Entity CreateGeralt(IEcs ecs)
        {
            // Make a pathfinder for Geralt's AI that knows where the trees are.
            var trees =
                ecs.EntitiesWithComponent<IsActor>()
                    .Where(e => ecs.GetComponent<IsActor>(e).Type == ActorType.Tree)
                    .Select(e => ecs.GetComponent<HasLocation>(e))
                    .Select(l => new Coord(l.X, l.Y));

            var pathFinder = new PathFinder(new Coord(this.boundsX, this.boundsY), trees);

            return
                ecs.NewEntity()
                    .WithComponent(new HasName("Geralt"))
                    .WithComponent(this.FindSafeLocation(ecs.GetSystem<HasLocation>() as LocationSystem, new Random()))
                    .WithComponent(new Renders('G', ConsoleColor.DarkMagenta))
                    .WithComponent(new IsActor(ActorType.Witcher))
                    .WithComponent(new HasAi(new WitcherAi(pathFinder)))
                    .WithComponent(new Talks(Logger.AddLog).RandomlySay(Talks.GeraltSpeaks, 0.02))
                    .WithComponent<HasMoney>()
                    .Entity;
        }

        private IEcs CreateEcs()
        {
            return
                new Ecs().WithSystem<HasName>()
                    .WithSystem<HasMoney>()
                    .WithCustomSystem<LocationSystem, HasLocation>()
                    .WithSystem<Renders>()
                    .WithSystem<IsActor>()
                    .WithUpdatableSystem<KilledBy>()
                    .WithUpdatableSystem<Talks>()
                    .WithAsyncUpdatableSystem<HasAi>();
        }

        private void CreateEntity(IEcs ecs, int number, ActorType actor)
        {
            var rng = new Random();
            var locationSystem = ecs.GetSystem<HasLocation>() as LocationSystem;

            for (var i = 0; i < number; i++)
            {
                var entity = ecs.NewEntity()
                    .WithComponent(this.FindSafeLocation(locationSystem, rng));

                if (actor == ActorType.Tree)
                {
                    this.MakeEntityIntoTree(entity);
                }
                else
                {
                    this.MakeEntityIntoDrowner(entity);
                }
            }
        }

        private HasLocation FindSafeLocation(LocationSystem locationSystem, Random rng)
        {
            var x = rng.Next(0, this.boundsX);
            var y = rng.Next(0, this.boundsY);

            while (locationSystem.IsEntityAt(x, y))
            {
                x = rng.Next(0, this.boundsX);
                y = rng.Next(0, this.boundsY);
            }

            return new HasLocation(x, y, this.boundsX, this.boundsY);
        }

        private void MakeEntityIntoDrowner(EntityConfigurator drowner)
        {
            drowner.WithComponent(new Renders('D', ConsoleColor.DarkYellow))
                .WithComponent(new IsActor(ActorType.Monster))
                .WithComponent(new HasAi(new DrownerAi()))
                .WithComponent(new KilledBy(ActorType.Witcher))
                .WithComponent(new HasName($"Drowner {drowner.Entity.Id}"))
                .WithComponent(new Talks(Logger.AddLog).RandomlySay(Talks.DrownerSpeaks, 0.01));
        }

        private void MakeEntityIntoTree(EntityConfigurator tree)
        {
            tree.WithComponent(new IsActor(ActorType.Tree)).WithComponent(new Renders('T', ConsoleColor.DarkGreen));
        }
    }
}
