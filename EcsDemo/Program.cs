namespace WitcherDemo
{
    public static class Program
    {
        private const int BoundsX = 32;

        private const int BoundsY = 22;

        // Creates a logger that'll print its messages to the console next to the game
        private static readonly Logger Logger = new Logger();

        private static void Main()
        {
            new Game(32, 22).Run();
        }
    }
}
