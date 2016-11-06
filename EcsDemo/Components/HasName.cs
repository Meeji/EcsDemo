namespace WitcherDemo.Components
{
    using ECS;

    public class HasName : Component
    {
        public HasName(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        protected override void Initialise()
        {
        }
    }
}
