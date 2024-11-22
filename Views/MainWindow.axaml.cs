using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia.Input.Platform;
using Prefix_List_Compare.ViewModels;

namespace Prefix_List_Compare.Views;

public partial class MainWindow : Window
{
    private string sourceText = "";
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }

    private void PasteButton(object? sender, RoutedEventArgs e)
    {
    }

    private void CopyButton(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}