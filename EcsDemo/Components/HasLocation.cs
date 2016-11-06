namespace WitcherDemo.Components
{
    using System;

    using ECS;

    public class HasLocation : Component
    {
        private readonly int boundX;

        private readonly int boundY;

        public HasLocation(int x, int y, int boundsX, int boundY)
        {
            this.X = x;
            this.Y = y;
            this.boundX = boundsX;
            this.boundY = boundY;
        }

        public event Action<int, int, int, int, Entity> ChangedLocation;

        public int Y { get; private set; }

        public int X { get; private set; }

        public void SetLocation(int x, int y)
        {
            if (x < 0 || y < 0 || y >= this.boundY || x >= this.boundX)
            {
                return;
            }

            this.ChangedLocation?.Invoke(this.X, this.Y, x, y, this.Entity);

            this.X = x;
            this.Y = y;
        }

        protected override void Initialise()
        {
        }
    }
}
