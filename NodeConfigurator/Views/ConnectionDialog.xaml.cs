using System.Windows;
using NodeConfigurator.ViewModels;

namespace NodeConfigurator.Views
{
    public partial class ConnectionDialog : Window
    {
        public ConnectionDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Save password to ViewModel
            if (DataContext is ConnectionViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
            DialogResult = true;
            Close();
        }
    }
}
