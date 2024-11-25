using Avalonia.Controls;
using Avalonia.Interactivity;
using Prefix_List_Compare.ViewModels;

namespace Prefix_List_Compare.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel(this);
    }
}