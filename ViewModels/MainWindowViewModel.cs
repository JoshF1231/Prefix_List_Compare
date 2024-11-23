﻿using System;
 using System.Collections.Generic;
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
    [ObservableProperty] private string? _currentConfiguration;
    [ObservableProperty] private string? _desiredNetworks;
    [ObservableProperty] private string? _netWorksToReturn;
    private bool _copyResultsIconState;
    private bool _currentConfigurationIconState;
    private bool _desiredNetworksIconState;


    public bool CopyResultsIconState
    {
        get => _copyResultsIconState;
        set
        {
            if (_copyResultsIconState != value)
            {
                _copyResultsIconState = value;
                OnPropertyChanged();  // Notify the UI that the property value has changed
            }
        }
    }
    public bool CurrentConfigurationIconState
    {
        get => _currentConfigurationIconState;
        set
        {
            if (_currentConfigurationIconState != value)
            {
                _currentConfigurationIconState = value;
                OnPropertyChanged();  // Notify the UI that the property value has changed
            }
        }
    }
    public bool DesiredNetworksIconState
    {
        get => _desiredNetworksIconState;
        set
        {
            if (_desiredNetworksIconState != value)
            {
                _desiredNetworksIconState = value;
                OnPropertyChanged();  // Notify the UI that the property value has changed
            }
        }
    }
    
    public bool GetButtonState(string buttonName)
    {
        return true;
    }
    public MainWindowViewModel()
    {
        CopyResultsIconState = false;
        CurrentConfigurationIconState = false;
        DesiredNetworksIconState = false;
    }
    public string Greeting { get; } = "Prefix List Compare";
    
    [RelayCommand]
    private async Task CopyText()
    {
        try
        {
            await DoSetClipboardTextAsync(NetWorksToReturn);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Could not paste to clipboard", "Could not paste to clipboard. Try restarting the program or your computer.", ButtonEnum.Ok).ShowAsync();
        }
    }
    [RelayCommand]
    private async Task PasteText()
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
            await MessageBoxManager.GetMessageBoxStandard("Could not copy from Clipboard!", "Make sure you have the text copied! (Ctrl+C)", ButtonEnum.Ok).ShowAsync();
        }
    }

    public async Task PasteCurrentConfiguration()
    {
        await PasteText();
        CurrentConfiguration = Text;
    }
    public async Task PasteDesiredNetworks()
    {
        await PasteText();
        DesiredNetworks = Text;
    }
    
    public async Task CopyResults()
    {
        NetWorksToReturn = "123";
        CopyResultsIconState = false;
        await Task.WhenAll(Task.Delay(150),CopyText());
        CopyResultsIconState = true;
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


