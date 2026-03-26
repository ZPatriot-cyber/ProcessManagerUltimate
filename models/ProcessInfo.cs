using ProcessManagerUltimate.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessManagerUltimate.Models
{
    public class ProcessInfo : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public MainViewModel MainViewModel { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    MainViewModel?.OnProcessSelectionChanged(Id, value);
                }
            }
        }

        private ProcessPriorityClass _priority;
        public ProcessPriorityClass Priority
        {
            get => _priority;
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    OnPropertyChanged(nameof(Priority));
                    ApplyPriority();
                }
            }
        }

        private long _memoryUsage;
        public long MemoryUsage
        {
            get => _memoryUsage;
            set { _memoryUsage = value; OnPropertyChanged(nameof(MemoryUsage)); }
        }

        public int ThreadCount { get; set; }
        public TimeSpan CpuTime { get; set; }
        public bool Responding { get; set; }

        private long _affinity;
        public long Affinity
        {
            get => _affinity;
            set
            {
                if (_affinity != value)
                {
                    _affinity = value;
                    OnPropertyChanged(nameof(Affinity));
                    OnPropertyChanged(nameof(AffinityString));
                    ApplyAffinity();
                }
            }
        }

        public string AffinityString
        {
            get
            {
                if (Affinity == 0) return "";
                var cores = new List<int>();
                for (int i = 0; i < 64; i++)
                    if ((Affinity & (1L << i)) != 0)
                        cores.Add(i);
                return string.Join(",", cores);
            }
            set
            {
                long mask = 0;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in parts)
                        if (int.TryParse(s.Trim(), out int core) && core >= 0 && core < 64)
                            mask |= (1L << core);
                }
                Affinity = mask;
            }
        }

        private void ApplyPriority()
        {
            try { Process.GetProcessById(Id).PriorityClass = _priority; } catch { }
        }

        private void ApplyAffinity()
        {
            try { Process.GetProcessById(Id).ProcessorAffinity = (IntPtr)_affinity; } catch { }
        }
    }
}