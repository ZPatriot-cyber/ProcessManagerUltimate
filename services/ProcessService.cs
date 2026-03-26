using ProcessManagerUltimate.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessManagerUltimate.Services
{
    public class ProcessService
    {
        public async Task<List<ProcessInfo>> GetProcessesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<ProcessInfo>();
                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        var process = Process.GetProcessById(p.Id);
                        list.Add(new ProcessInfo
                        {
                            Id = p.Id,
                            Name = p.ProcessName,
                            Priority = process.PriorityClass,
                            MemoryUsage = p.WorkingSet64,
                            ThreadCount = p.Threads.Count,
                            CpuTime = p.TotalProcessorTime,
                            Responding = p.Responding,
                            Affinity = process.ProcessorAffinity.ToInt64()
                        });
                    }
                    catch { }
                }
                return list.OrderBy(p => p.Name).ToList();
            });
        }

        public List<ThreadInfo> GetThreads(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);

                return process.Threads.Cast<ProcessThread>()
                    .Select(t => new ThreadInfo
                    {
                        Id = t.Id,
                        Priority = t.PriorityLevel,
                        State = t.ThreadState,
                        CpuTime = t.TotalProcessorTime
                    })
                    .ToList();
            }
            catch
            {
                return new List<ThreadInfo>();
            }
        }

        public void SetPriority(int pid, ProcessPriorityClass priority)
        {
            try
            {
                Process.GetProcessById(pid).PriorityClass = priority;
            }
            catch { }
        }

        public long GetAffinity(int pid)
        {
            try
            {
                return Process.GetProcessById(pid).ProcessorAffinity.ToInt64();
            }
            catch
            {
                return 0;
            }
        }

        public void SetAffinity(int pid, long mask)
        {
            try
            {
                Process.GetProcessById(pid).ProcessorAffinity = (System.IntPtr)mask;
            }
            catch { }
        }
    }
}