using System.Collections.Generic;

namespace TaskManager.Core.Comparer
{
    internal class ProcessDateComparer : IComparer<Process>
    {
        public int Compare(Process x, Process y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Created.CompareTo(y.Created);
        }
    }
}