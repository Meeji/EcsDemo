namespace WitcherDemo.Components
{
    using ECS;

    public class IsActor : Component
    {
        public IsActor(ActorType type)
        {
            this.Type = type;
        }

        public ActorType Type { get; }

        protected override void Initialise()
        {
        }
    }
}
