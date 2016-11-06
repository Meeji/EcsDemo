namespace WitcherDemo.Renderer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using ECS;

    using WitcherDemo.Components;

    using Pixel = System.Tuple<char, System.ConsoleColor>;

    public class Renderer
    {
        private readonly int boundsX;

        private readonly int boundsY;

        private readonly IEcs ecs;

        private readonly RendererCharacters rendererCharacters;

        private readonly Logger logger;

        private readonly Func<int> moneyRetriever;

        private Pixel[][] lastFrame;

        public Renderer(IEcs ecs, int boundsX, int boundsY, RendererCharacters rendererCharacters, Logger logger, Func<int> moneyRetriever)
        {
            this.ecs = ecs;
            this.boundsX = boundsX;
            this.boundsY = boundsY;
            this.rendererCharacters = rendererCharacters;
            this.logger = logger;
            this.moneyRetriever = moneyRetriever;
        }

        public async void Render()
        {
            Console.CursorVisible = false;
            var frame = this.BlankFrame();

            if (this.lastFrame == null)
            {
                Console.Clear();
                this.lastFrame = this.BlankFrame();
                this.BlitFrame(this.lastFrame);
            }

            // Draw various elements to the frame asynchronously (making sure none try to draw to the same pixel!)
            var gameTask = Task.Run(() => this.RenderGameIntoFrame(frame, 1, 1));
            var logsTask = Task.Run(() => this.RenderLogsIntoFrame(frame));
            var moneyTask = Task.Run(() => this.RenderMoneyIntoFrame(frame));
            await gameTask;
            await logsTask;
            await moneyTask;

            this.BlitDifferences(frame, this.lastFrame);
            this.lastFrame = frame;
        }

        private void BlitFrame(Pixel[][] frame)
        {
            Console.Clear();
            for (var i = 0; i < frame.Length; i++)
            {
                for (var j = 0; j < frame[i].Length; j++)
                {
                    Console.SetCursorPosition(i, j);
                    this.BlitPixel(frame[i][j]);
                }
            }
        }

        private void BlitDifferences(Pixel[][] newFrame, Pixel[][] oldFrame)
        {
            // We blit the difference rather than the whole new frame to stop flicker
            for (var i = 0; i < newFrame.Length; i++)
            {
                for (var j = 0; j < newFrame[i].Length; j++)
                {
                    var pixel = newFrame[i][j];
                    if (!pixel.Equals(oldFrame[i][j]))
                    {
                        Console.SetCursorPosition(i, j);
                        this.BlitPixel(pixel);
                    }
                }
            }
        }

        private void BlitPixel(Pixel pixel)
        {
            Console.ForegroundColor = pixel.Item2;
            Console.Write(pixel.Item1);
            Console.ResetColor();
        }

        private void RenderGameIntoFrame(Pixel[][] frame, int offSetX, int offSetY)
        {
            foreach (var entity in this.ecs.EntitiesWithComponent<Renders>())
            {
                if (this.ecs.HasComponent<HasLocation>(entity))
                {
                    var location = this.ecs.GetComponent<HasLocation>(entity);
                    var renders = this.ecs.GetComponent<Renders>(entity);
                    frame[location.X + offSetX][location.Y + offSetY] = new Pixel(renders.Render, renders.Colour);
                }
            }
        }

        private void RenderLogsIntoFrame(Pixel[][] frame)
        {
            var logLines = this.boundsY - 4;
            var logs = this.logger.GetLogs(logLines).Where(l => l != null).Select(l => l.Take(75 - this.boundsX));

            var line = this.boundsY - 3;
            foreach (var log in logs)
            {
                var startX = this.boundsX + 3;
                foreach (var character in log)
                {
                    frame[startX++][line] = new Pixel(character, ConsoleColor.Yellow);
                }

                line -= 1;
            }
        }

        private void RenderMoneyIntoFrame(Pixel[][] frame)
        {
            var money = $"Geralt's Orens: {this.moneyRetriever()}".ToCharArray();

            for (var i = 0; i < money.Length; i++)
            {
                frame[78 - (money.Length - i)][this.boundsY] = new Pixel(money[i], ConsoleColor.Yellow);
            }
        }

        private Pixel[][] BlankFrame()
        {
            var frame = new Pixel[80][];
            for (var i = 0; i < 80; i++)
            {
                frame[i] = new Pixel[this.boundsY + 2];
                for (var j = 0; j < this.boundsY + 2; j++)
                {
                    frame[i][j] = j < this.boundsY + 1 && j > 0 && i > 0 && i < this.boundsX + 1
                                      ? new Pixel('.', ConsoleColor.White)
                                      : new Pixel(' ', ConsoleColor.White);
                }
            }

            // Add frame corners
            frame[0][0] = new Pixel(this.rendererCharacters.CornerTopLeft, ConsoleColor.DarkRed);
            frame[79][0] = new Pixel(this.rendererCharacters.CornerTopRight, ConsoleColor.DarkRed);
            frame[0][this.boundsY + 1] = new Pixel(this.rendererCharacters.CornerBottomLeft, ConsoleColor.DarkRed);
            frame[79][this.boundsY + 1] = new Pixel(this.rendererCharacters.CornerBottomRight, ConsoleColor.DarkRed);

            // Add frame top and bottom
            for (var i = 1; i < 79; i++)
            {
                frame[i][0] = new Pixel(this.rendererCharacters.RightLeft, ConsoleColor.DarkRed);
                frame[i][this.boundsY + 1] = new Pixel(this.rendererCharacters.RightLeft, ConsoleColor.DarkRed);
            }

            // Add frame sides and center bar
            for (var i = 1; i < this.boundsY + 1; i++)
            {
                // Sides
                frame[0][i] = new Pixel(this.rendererCharacters.UpDown, ConsoleColor.DarkRed);
                frame[79][i] = new Pixel(this.rendererCharacters.UpDown, ConsoleColor.DarkRed);

                // Bar
                frame[this.boundsX + 1][i] = new Pixel(this.rendererCharacters.UpDown, ConsoleColor.DarkRed);
            }

            // Put in bar junctions
            frame[this.boundsX + 1][0] = new Pixel(this.rendererCharacters.JuntionRightLeftDown, ConsoleColor.DarkRed);
            frame[this.boundsX + 1][this.boundsY + 1] = new Pixel(this.rendererCharacters.JuntionRightLeftUp, ConsoleColor.DarkRed);

            return frame;
        }
    }
}
