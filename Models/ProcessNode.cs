using System.Collections.ObjectModel;

namespace ProcessManagerUltimate.Models
{
    public class ProcessNode
    {
        public ProcessInfo Process { get; set; }
        public ObservableCollection<ProcessNode> Children { get; set; }
            = new ObservableCollection<ProcessNode>();
    }
}