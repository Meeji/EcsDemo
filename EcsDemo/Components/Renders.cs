namespace WitcherDemo.Components
{
    using System;

    using ECS;
 
    public class Renders : Component
    {
        public Renders(char render, ConsoleColor colour = ConsoleColor.White)
        {
            this.Render = render;
            this.Colour = colour;
        }

        public ConsoleColor Colour { get; }

        public char Render { get; }

        protected override void Initialise()
        {
        }
    }
}
