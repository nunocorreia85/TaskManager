using System;
using TaskManager.Core.Enums;

namespace TaskManager.Core
{
    public class Process
    {
        public Process(long id, Priority priority)
        {
            Id = id;
            Priority = priority;
            Ticks = DateTimeOffset.UtcNow.Ticks;
        }

        public long Ticks { get; }

        public long Id { get; }

        public Priority Priority { get; }
    }
}