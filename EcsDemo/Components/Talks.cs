namespace WitcherDemo.Components
{
    using System;
    using System.Collections.Generic;

    using ECS;

    using RandomTalk = System.Tuple<System.Collections.Generic.IList<string>, double>;

    public partial class Talks : UpdatableComponent
    {
        private readonly List<RandomTalk> randomTalk = new List<RandomTalk>();

        private readonly Action<string> logger;

        private Random rng;

        private Action<string> talk;

        private string leftBuffer;

        public Talks(Action<string> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Registers a list of strings, from which the character will occasionally 
        /// </summary>
        /// <param name="strings">Selection of dialogue</param>
        /// <param name="chance">Chance between 0.0 and 1.0</param>
        /// <returns></returns>
        public Talks RandomlySay(IList<string> strings, double chance)
        {
            this.randomTalk.Add(new RandomTalk(strings, chance));
            return this;
        }

        /// <summary>
        /// Allows you to put words into the mouth of the entity
        /// </summary>
        /// <param name="say">String describing what the entity should say</param>
        /// <param name="chance"></param>
        /// <returns>An action which will trigger the character to say this phrase</returns>
        public Action GetSpeaker(string say, double chance = 1.0)
        {
            return () => this.Talk(say, chance);
        }

        /// <summary>
        /// Allows you to put words into the mouth of the entity
        /// </summary>
        /// <param name="say">A selection of phrases</param>
        /// <param name="chance">Chance a phrase will be spoken</param>
        /// <returns>An action which will trigger the character to say one of the phrases at random</returns>
        public Action GetSpeaker(IList<string> say, double chance = 1.0)
        {
            return () => this.Talk(say, chance);
        }

        public override Action Update()
        {
            foreach (var phrase in this.randomTalk)
            {
                this.Talk(phrase.Item1, phrase.Item2);
            }

            return null;
        }

        protected override void Initialise()
        {
            this.leftBuffer = !this.Ecs.HasComponent<HasName>(this.Entity)
                ? string.Empty
                : this.Ecs.GetComponent<HasName>(this.Entity).Name + ": ";

            this.talk = s => this.logger($"{this.leftBuffer}\"{s}\"");

            this.rng = new Random(this.Entity.Id); // Had problems with rngs spitting out the same number. Should help.
        }

        private void Talk(string phrase, double chance)
        {
            if (this.ShouldTalk(chance))
            {
                this.Talk(phrase);
            }
        }

        private void Talk(IList<string> phrases, double chance)
        {
            if (this.ShouldTalk(chance))
            {
                this.Talk(phrases);
            }
        }

        private void Talk(string phrase)
        {
            this.talk(phrase);
        }

        private void Talk(IList<string> phrases)
        {
            this.talk(this.RandomFrom(phrases));
        }

        private bool ShouldTalk(double chance)
        {
            if (chance >= 1.0)
            {
                return true;
            }

            if (chance <= 0.0)
            {
                return false;
            }

            return this.rng.NextDouble() <= chance;
        }

        private string RandomFrom(IList<string> strings)
        {
            return strings[this.rng.Next(strings.Count)];
        }
    }
}
