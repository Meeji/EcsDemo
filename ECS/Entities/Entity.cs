namespace ECS
{
    public class Entity
    {
        private static int nextId;

        public int Id { get; } = nextId++;
    }
}
