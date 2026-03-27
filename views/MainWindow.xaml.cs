using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ProcessManagerUltimate.ViewModels;
using ProcessManagerUltimate.Models;

namespace ProcessManagerUltimate.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewModel();
            DataContext = vm;
        }

        // Обработчики для одиночного процесса (если используются)
        private void ApplyAffinity_Click(object sender, RoutedEventArgs e) => vm.ApplyAffinity();
        private void RealTime_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.RealTime);
        private void High_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.High);
        private void AboveNormal_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.AboveNormal);
        private void Normal_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.Normal);
        private void BelowNormal_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.BelowNormal);
        private void Idle_Click(object sender, RoutedEventArgs e) => vm.ChangePriority(ProcessPriorityClass.Idle);

        // Массовые операции
        private void SelectAll_Click(object sender, RoutedEventArgs e) => vm.SelectAllProcesses();
        private void DeselectAll_Click(object sender, RoutedEventArgs e) => vm.DeselectAllProcesses();
        private void ClearSelection_Click(object sender, RoutedEventArgs e) => vm.SelectedProcess = null;

        private void ApplyPriorityToSelected_Click(object sender, RoutedEventArgs e)
        {
            if (BulkPriorityCombo.SelectedItem is PriorityClassItem selected)
                vm.ApplyPriorityToSelected(selected.Value);
            else
                MessageBox.Show("Выберите приоритет из списка", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ApplyAffinityToSelected_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BulkAffinityTextBox.Text))
                vm.ApplyAffinityToSelected(BulkAffinityTextBox.Text);
            else
                MessageBox.Show("Введите номера ядер", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}