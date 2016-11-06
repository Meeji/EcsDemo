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

        public void Update()
        {
        }

        protected override void Initialise()
        {
        }
    }
}
