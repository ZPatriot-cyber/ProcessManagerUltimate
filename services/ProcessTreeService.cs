using ProcessManagerUltimate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ProcessManagerUltimate.Services
{
    public class ProcessTreeService
    {
        public List<ProcessNode> BuildTree(List<ProcessInfo> processes)
        {
            var dict = processes.ToDictionary(p => p.Id, p => new ProcessNode { Process = p });
            var parents = new Dictionary<int, int>();
            var searcher = new ManagementObjectSearcher("SELECT ProcessId, ParentProcessId FROM Win32_Process");

            foreach (ManagementObject obj in searcher.Get())
            {
                int pid = Convert.ToInt32(obj["ProcessId"]);
                int parent = Convert.ToInt32(obj["ParentProcessId"]);
                parents[pid] = parent;
            }

            var roots = new List<ProcessNode>();

            foreach (var node in dict.Values)
            {
                if (parents.ContainsKey(node.Process.Id) && dict.ContainsKey(parents[node.Process.Id]))
                {
                    dict[parents[node.Process.Id]].Children.Add(node);
                }
                else
                {
                    roots.Add(node);
                }
            }

            return roots;
        }
    }
}