namespace WitcherDemo.Components
{
    using System.Collections.Generic;

    using PathFinding;

    public static class ExtensionMethods
    {
        public static Coord Closest(this IEnumerable<Coord> coords, Coord closeTo)
        {
            var score = double.MaxValue;
            Coord coordOut = null;

            foreach (var coord in coords)
            {
                var newScore = AiBase.DistanceBetween(coord, closeTo);
                if (newScore < score)
                {
                    score = newScore;
                    coordOut = coord;
                }
            }

            return coordOut;
        }
    }
}
