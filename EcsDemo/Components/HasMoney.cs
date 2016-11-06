namespace WitcherDemo.Components
{
    using ECS;

    public class HasMoney : Component
    {
        public int Money { get; private set; }

        public void AddMoney(int amount)
        {
            this.Money += amount;
        }

        public void SetMoney(int amount)
        {
            this.Money = amount;
        }

        protected override void Initialise()
        {
        }
    }
}
