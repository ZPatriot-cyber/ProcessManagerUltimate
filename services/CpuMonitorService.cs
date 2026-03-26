using System.Collections.Generic;
using System.Diagnostics;

namespace ProcessManagerUltimate.Services
{
    public class CpuMonitorService
    {
        private readonly List<PerformanceCounter> counters;

        public CpuMonitorService()
        {
            counters = new List<PerformanceCounter>();
            int cores = System.Environment.ProcessorCount;

            for (int i = 0; i < cores; i++)
            {
                counters.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString()));
            }
        }

        public List<float> GetUsage()
        {
            var list = new List<float>();

            foreach (var c in counters)
            {
                list.Add(c.NextValue());
            }

            return list;
        }
    }
}