using LiveCharts;
using LiveCharts.Wpf;
using ProcessManagerUltimate.Models;
using ProcessManagerUltimate.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Generic;

namespace ProcessManagerUltimate.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ProcessService service = new ProcessService();
        private readonly CpuMonitorService cpuService = new CpuMonitorService();
        private readonly ProcessTreeService treeService = new ProcessTreeService();
        private readonly DispatcherTimer timer;

        public ObservableCollection<ProcessInfo> Processes { get; } = new ObservableCollection<ProcessInfo>();
        public ObservableCollection<ProcessNode> ProcessTree { get; } = new ObservableCollection<ProcessNode>();
        public ObservableCollection<ThreadInfo> Threads { get; } = new ObservableCollection<ThreadInfo>();
        public ObservableCollection<ProcessInfo> TopMemory { get; } = new ObservableCollection<ProcessInfo>();

        public SeriesCollection CpuSeries { get; set; }
        private ChartValues<double> cpuValues = new ChartValues<double>();

        public ObservableCollection<bool> CoreSelection { get; }
        public int CoreCount { get; }

        private ProcessInfo _selectedProcess;
        public ProcessInfo SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (_selectedProcess != value)
                {
                    _selectedProcess = value;
                    if (value != null)
                    {
                        LoadThreads();
                        LoadAffinity();
                    }
                    else
                    {
                        Threads.Clear();
                        for (int i = 0; i < CoreCount; i++)
                            CoreSelection[i] = false;
                    }
                    OnPropertyChanged(nameof(SelectedProcess));
                }
            }
        }

        private int _selectedProcessesCount;
        public int SelectedProcessesCount
        {
            get => _selectedProcessesCount;
            set { _selectedProcessesCount = value; OnPropertyChanged(nameof(SelectedProcessesCount)); }
        }

        private HashSet<int> _selectedPids = new HashSet<int>();

        // Список приоритетов для выпадающего списка (русские названия)
        public List<PriorityClassItem> RussianPriorityClasses { get; } = new List<PriorityClassItem>
        {
            new PriorityClassItem { Name = "Низкий", Value = ProcessPriorityClass.Idle },
            new PriorityClassItem { Name = "Ниже среднего", Value = ProcessPriorityClass.BelowNormal },
            new PriorityClassItem { Name = "Средний", Value = ProcessPriorityClass.Normal },
            new PriorityClassItem { Name = "Выше среднего", Value = ProcessPriorityClass.AboveNormal },
            new PriorityClassItem { Name = "Высокий", Value = ProcessPriorityClass.High },
            new PriorityClassItem { Name = "Реального времени", Value = ProcessPriorityClass.RealTime }
        };

        public MainViewModel()
        {
            CoreCount = Environment.ProcessorCount;
            CoreSelection = new ObservableCollection<bool>(Enumerable.Repeat(true, CoreCount).ToList());

            CpuSeries = new SeriesCollection
            {
                new LineSeries { Title = "CPU %", Values = cpuValues, PointGeometry = null }
            };

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += async (s, e) =>
            {
                await LoadProcessesAsync();
                UpdateCpu();
            };
            timer.Start();
        }

        private async Task LoadProcessesAsync()
        {
            try
            {
                var freshList = await service.GetProcessesAsync();
                var freshDict = freshList.ToDictionary(p => p.Id);

                for (int i = Processes.Count - 1; i >= 0; i--)
                {
                    var existing = Processes[i];
                    if (!freshDict.ContainsKey(existing.Id))
                    {
                        Processes.RemoveAt(i);
                        _selectedPids.Remove(existing.Id);
                    }
                }

                foreach (var fresh in freshList)
                {
                    var existing = Processes.FirstOrDefault(p => p.Id == fresh.Id);
                    if (existing != null)
                    {
                        existing.MemoryUsage = fresh.MemoryUsage;
                        existing.ThreadCount = fresh.ThreadCount;
                        existing.CpuTime = fresh.CpuTime;
                        existing.Responding = fresh.Responding;
                    }
                    else
                    {
                        fresh.MainViewModel = this;
                        Processes.Add(fresh);
                    }
                }

                TopMemory.Clear();
                foreach (var p in Processes.OrderByDescending(p => p.MemoryUsage).Take(10))
                    TopMemory.Add(p);

                var tree = treeService.BuildTree(Processes.ToList());
                ProcessTree.Clear();
                foreach (var n in tree)
                    ProcessTree.Add(n);

                if (SelectedProcess != null)
                {
                    var stillAlive = Processes.FirstOrDefault(p => p.Id == SelectedProcess.Id);
                    if (stillAlive == null)
                        SelectedProcess = null;
                }

                SelectedProcessesCount = _selectedPids.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки процессов: {ex.Message}");
            }
        }

        private void UpdateCpu()
        {
            try
            {
                var usage = cpuService.GetUsage();
                if (usage.Count == 0) return;
                double avg = usage.Average();
                if (cpuValues.Count > 60) cpuValues.RemoveAt(0);
                cpuValues.Add(avg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления CPU: {ex.Message}");
            }
        }

        private void LoadThreads()
        {
            Threads.Clear();
            if (SelectedProcess == null) return;
            try
            {
                var list = service.GetThreads(SelectedProcess.Id);
                foreach (var t in list) Threads.Add(t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки потоков: {ex.Message}");
            }
        }

        private void LoadAffinity()
        {
            if (SelectedProcess == null) return;
            try
            {
                long mask = SelectedProcess.Affinity;
                for (int i = 0; i < CoreCount; i++)
                    CoreSelection[i] = (mask & (1L << i)) != 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки affinity: {ex.Message}");
            }
        }

        public void ApplyAffinity()
        {
            if (SelectedProcess == null) return;
            try
            {
                long mask = 0;
                for (int i = 0; i < CoreCount; i++)
                    if (CoreSelection[i])
                        mask |= (1L << i);
                SelectedProcess.Affinity = mask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка применения affinity: {ex.Message}");
            }
        }

        public void ChangePriority(ProcessPriorityClass priority)
        {
            if (SelectedProcess == null) return;
            try
            {
                SelectedProcess.Priority = priority;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка изменения приоритета: {ex.Message}");
            }
        }

        public void OnProcessSelectionChanged(int processId, bool isSelected)
        {
            if (isSelected)
                _selectedPids.Add(processId);
            else
                _selectedPids.Remove(processId);
            SelectedProcessesCount = _selectedPids.Count;
        }

        public void SelectAllProcesses()
        {
            foreach (var p in Processes)
                p.IsSelected = true;
        }

        public void DeselectAllProcesses()
        {
            foreach (var p in Processes)
                p.IsSelected = false;
        }

        public void ApplyPriorityToSelected(ProcessPriorityClass priority)
        {
            foreach (var p in Processes.Where(p => p.IsSelected))
            {
                try { p.Priority = priority; } catch { }
            }
        }

        public void ApplyAffinityToSelected(string affinityString)
        {
            foreach (var p in Processes.Where(p => p.IsSelected))
            {
                try { p.AffinityString = affinityString; } catch { }
            }
        }

        public void StopTimer() => timer?.Stop();
    }
}