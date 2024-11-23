﻿using System;
 using System.Collections.Generic;
 using System.Collections.ObjectModel;
 using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Tmds.DBus.Protocol;

namespace Prefix_List_Compare.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string? _text = "";
    [ObservableProperty] private int _caretIndex;
    [ObservableProperty] private string? _currentConfiguration;
    [ObservableProperty] private string? _desiredNetworks;
    [ObservableProperty] private string? _netWorksToReturn;
    private List<String> _buttonNames = new();
    
    private AvaloniaDictionary<string,bool> _iconStates = new();

    public AvaloniaDictionary<string, bool> IconStates
    {
        get => _iconStates;
        set
        {
            _iconStates = value;
            OnPropertyChanged(nameof(IconStates));
        }
    }

    private bool _currentConfigurationIconState;
    private bool _desiredNetworksIconState;
    private bool _copyResultsIconState;

    public MainWindowViewModel()
    {
        CopyResultsIconState = false;
        CurrentConfigurationIconState = false;
        DesiredNetworksIconState = false;
        InitializeButtonNames();
        foreach (var buttonName in _buttonNames)
        {
            IconStates.Add(buttonName, false); // represents icon states ( the loading and checkmark icons)
        }
    }

    private void InitializeButtonNames()
    {
        _buttonNames.Add("PasteCurrentConfigurationLoading");
        _buttonNames.Add("PasteCurrentConfigurationCheckmark");
        _buttonNames.Add("PasteDesiredNetworksLoading");
        _buttonNames.Add("PasteDesiredNetworksCheckmark");
        _buttonNames.Add("CopyResultsLoading");
        _buttonNames.Add("CopyResultsCheckmark");
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
    public bool GetButtonState(string buttonName)
    {
        return true;
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
        IconStates["PasteDesiredNetworksCheckmark"] = false;
        IconStates["PasteDesiredNetworksLoading"] = true;
        OnPropertyChanged(nameof(IconStates));
        await Task.WhenAny(Task.Delay(150), PasteText());
        DesiredNetworks = Text;
        IconStates["PasteDesiredNetworksLoading"] = false;
        IconStates["PasteDesiredNetworksCheckmark"] = true;
        OnPropertyChanged(nameof(IconStates));
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


