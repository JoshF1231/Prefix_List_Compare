﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Prefix_List_Compare.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string? _text = "";
    [ObservableProperty] private int _caretIndex;

    public string Greeting { get; } = "Welcome to Avalonia!";
    
    [RelayCommand]
    private async Task CopyText(CancellationToken token)
    {
        try
        {
            await DoSetClipboardTextAsync(Text);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("SHeit", "sheit", ButtonEnum.Ok).ShowAsync();
        }
    }
    [RelayCommand]
    public async Task PasteText()
    {
        try
        {
            if (await DoGetClipboardTextAsync() is { } pastedText){
                Text = string.Empty;
                Text = Text?.Insert(CaretIndex, pastedText);
            }
            if (Text != null)
                await MessageBoxManager.GetMessageBoxStandard("nooo", Text, ButtonEnum.Ok).ShowAsync();
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("nooo", "nooo", ButtonEnum.Ok).ShowAsync();

        }
    }
    
    private async Task DoSetClipboardTextAsync(string? text)
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel. 

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See DepInject project for a sample of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
            throw new NullReferenceException("Missing Clipboard instance.");

        await provider.SetTextAsync(text);
    }
    private async Task<string?> DoGetClipboardTextAsync()
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel. 

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See DepInject project for a sample of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
            throw new NullReferenceException("Missing Clipboard instance.");

        return await provider.GetTextAsync();
    }
}