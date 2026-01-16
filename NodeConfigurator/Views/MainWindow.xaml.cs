using System.Windows;
using System.Windows.Controls;
using NodeConfigurator.ViewModels;

namespace NodeConfigurator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NodeTreeView.SelectedItemChanged += NodeTreeView_SelectedItemChanged;
        }

        private void NodeTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel viewModel && e.NewValue is TreeNodeViewModel selectedNode)
            {
                viewModel.SelectedNode = selectedNode;
            }
        }
    }
}
