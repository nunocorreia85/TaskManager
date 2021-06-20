using System.Collections.Generic;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Interfaces
{
    public interface IOperatingSystem
    {
        bool Add(Process process, AddMethod method);
        void Kill(long processId);
        void KillGroup(Priority priority);
        void KillAll();
        ICollection<Process> List(SortBy sortBy);
    }
}