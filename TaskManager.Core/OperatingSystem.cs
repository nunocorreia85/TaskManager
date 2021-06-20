using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core
{
    public class OperatingSystem : IOperatingSystem
    {
        private readonly ISettings _settings;
        private readonly ILogger<OperatingSystem> _logger;

        public OperatingSystem(ISettings settings, ILogger<OperatingSystem> logger)
        {
            _settings = settings;
            _logger = logger;
            Processes = new ConcurrentDictionary<long, Process>();
        }

        private ConcurrentDictionary<long, Process> Processes { get; }

        public bool Add(Process process, AddMethod method)
        {
            _logger.LogInformation($"Add new process with id {process.Id}");
            return method switch
            {
                AddMethod.Fifo => AddProcessFifoMethod(process),
                AddMethod.Priority => AddProcessPriorityMethod(process),
                _ => AddProcessDefaultMethod(process)
            };
        }

        public void Kill(long processId)
        {
            if (Processes.TryRemove(processId, out _))
                _logger.LogError("Failed to remove process id {processId}", processId);
        }

        public void KillGroup(Priority priority)
        {
            foreach (var (id, process) in Processes)
                if (process.Priority == priority)
                    Kill(id);
        }

        public void KillAll()
        {
            Processes.Clear();
        }

        public ICollection<Process> List(SortBy sortBy)
        {
            if (sortBy == SortBy.Id) return Processes.Values;

            var sortedList = new SortedList<long, Process>();

            foreach (var (_, process) in Processes)
                switch (sortBy)
                {
                    case SortBy.Priority:
                        sortedList.Add((int) process.Priority, process);
                        break;
                    case SortBy.CreationTime:
                        sortedList.Add((int) process.Created.Ticks, process);
                        break;
                }

            return sortedList.Values;
        }

        private bool AddProcessPriorityMethod(Process newProcess)
        {
            if (_settings.ProcessesMaximumCapacity == Processes.Count)
            {
                _logger.LogInformation("Max capacity reached");

                var sortedList = new SortedList<long, Process>();
                foreach (var process in Processes)
                    if (newProcess.Priority < process.Value.Priority)
                        sortedList.Add(process.Value.Created.Ticks * (int) process.Value.Priority, process.Value);
                var last = sortedList.LastOrDefault();
                if (last.Value == null)
                {
                    _logger.LogInformation("Could not find a process with lowest priority that is the oldest");
                    return false;
                }

                if (!Processes.TryRemove(last.Key, out _))
                {
                    _logger.LogError("Failed to remove a process");
                    return false;
                }
            }

            if (!Processes.TryAdd(newProcess.Id, newProcess))
            {
                _logger.LogInformation("Cannot add the new process since it already exists");
                return false;
            }

            return true;
        }

        private bool AddProcessFifoMethod(Process newProcess)
        {
            if (_settings.ProcessesMaximumCapacity == Processes.Count)
            {
                _logger.LogInformation("Max capacity reached");

                var lastProcess = Processes.OrderByDescending(pair => pair.Value.Created.Ticks).FirstOrDefault();

                if (!Processes.TryRemove(lastProcess.Value.Id, out _))
                {
                    _logger.LogError("Failed to remove the oldest process");
                    return false;
                }
            }

            if (!Processes.TryAdd(newProcess.Id, newProcess))
                _logger.LogError("Cannot add the new process since it already exists");
            return true;
        }

        private bool AddProcessDefaultMethod(Process newProcess)
        {
            if (_settings.ProcessesMaximumCapacity == Processes.Count)
            {
                _logger.LogInformation("Reached max capacity won’t accept any new process");
                return false;
            }

            Processes.TryAdd(newProcess.Id, newProcess);
            return true;
        }
    }
}