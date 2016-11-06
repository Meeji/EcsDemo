namespace WitcherDemo
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Logger is essentially an overwriting ring buffer from which values can be read singularly or in blocks, or written to singularly.
    /// TODO: Worth making generic?
    /// </summary>
    public class Logger
    {
        private readonly string[] log;

        private int index;

        public Logger(int limit)
        {
            this.LogLimit = limit;
            this.log = new string[limit];
        }

        public Logger() : this(100)
        {
        }

        public int LogLimit { get; }

        /// <summary>
        /// Gets the last x logs from the buffer. If numberOfLines is greater than the size of the buffer it throws an InvalidOperationException.
        /// Will return nulls for blank values.
        /// </summary>
        /// <param name="numberOfLines"></param>
        /// <returns>Logs</returns>
        public IEnumerable<string> GetLogs(int numberOfLines)
        {
            if (numberOfLines > this.LogLimit)
            {
                throw new InvalidOperationException("Tried to read more lines than are contained in the log buffer");
            }

            var readIndex =
                this.AdvanceIndex(
                    this.index < numberOfLines
                        ? this.LogLimit - (numberOfLines - this.index)
                        : this.index - numberOfLines);

            for (var i = 0; i < numberOfLines; i++)
            {
                yield return this.log[readIndex];
                readIndex = this.AdvanceIndex(readIndex);
            }
        }

        /// <summary>
        /// Gets the last log added to the buffer. Will return null if there are no logs in the buffer
        /// </summary>
        /// <returns>Log</returns>
        public string GetLastLog()
        {
            return this.log[this.index];
        }

        /// <summary>
        /// Adds a log to the buffer
        /// </summary>
        /// <param name="newLog"></param>
        public void AddLog(string newLog)
        {
            this.index = this.AdvanceIndex(this.index);
            this.log[this.index] = newLog;
        }

        private int AdvanceIndex(int idx)
        {
            return idx >= this.LogLimit - 1 ? 0 : idx + 1;
        }
    }
}
