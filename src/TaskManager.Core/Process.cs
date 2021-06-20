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
            Created = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset Created { get; }

        public long Id { get; }

        public Priority Priority { get; }
    }
}