using System.Windows;

namespace TextFileVacuumer.Views;

public partial class MainWindow : Window
{
    public MainWindow(ViewModels.MainWindowViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
