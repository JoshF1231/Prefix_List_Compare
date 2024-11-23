﻿using System;
 using System.Collections.Generic;
 using System.Collections.ObjectModel;
 using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Tmds.DBus.Protocol;
using Prefix_List_Compare.Models;
namespace Prefix_List_Compare.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Window _thisWindow;
    [ObservableProperty] private string? _text = "";
    [ObservableProperty] private int _caretIndex;
    [ObservableProperty] private string? _currentConfiguration;
    [ObservableProperty] private string? _desiredNetworks;
    [ObservableProperty] private string? _netWorksToReturn;
    [ObservableProperty] private bool? _resultsButtonState;
    private List<String> _buttonNames = new();
    private readonly CalculationModel _calculationModel = new(); 
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

    public MainWindowViewModel(Window? window = null)
    {
        if (window != null)
        {
            _thisWindow = window;
        }
        _resultsButtonState = false;
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

    private async void CheckIfResultsAreCalculatable()
    {
        if (CurrentConfiguration != null && DesiredNetworks != null)
        {
            //await CalculateResults();
            ResultsButtonState = true;
        }
        else ResultsButtonState = false;
    }

    private async Task CalculateResults()
    {
        if (CurrentConfiguration != null && DesiredNetworks != null)
        {
            NetWorksToReturn = await _calculationModel.CalculateNetworkDifferences(CurrentConfiguration, DesiredNetworks);
        }
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
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Could not copy from Clipboard!", "Make sure you have the text copied! (Ctrl+C)", ButtonEnum.Ok).ShowAsync();
        }
    }


    public async Task Clipboardtime()
    {
        var clipboardText = "";
        try
        {
            if (_thisWindow != null && _thisWindow.Clipboard != null){
            clipboardText = await _thisWindow.Clipboard.GetTextAsync();
            Text=string.Empty;
            Text=Text?.Insert(0, clipboardText);
            }
        }
        catch
        {
            return;
        }
        return;
    }
    public async Task PasteCurrentConfiguration()
    {
        TransitionToLoading("PasteCurrentConfiguration");
        await Task.WhenAny(Task.Delay(150), Clipboardtime());
        CurrentConfiguration = string.Empty;
        if (!string.IsNullOrEmpty(Text)){
            CurrentConfiguration = CurrentConfiguration?.Insert(0,Text);
            TransitionToCheckmark("PasteCurrentConfiguration");
            CheckIfResultsAreCalculatable();
        }
    }
    public async Task PasteDesiredNetworks()
    {
        TransitionToLoading("PasteDesiredNetworks");
        await Task.WhenAny(Task.Delay(150), Clipboardtime());
        DesiredNetworks = string.Empty;
        if (!string.IsNullOrEmpty(Text)){
            DesiredNetworks = Text?.Insert(0,Text);
            TransitionToCheckmark("PasteDesiredNetworks");
            CheckIfResultsAreCalculatable();
        }
    }
    
    public async Task CopyResults()
    {
        TransitionToLoading("CopyResults");
        if (NetWorksToReturn == null)
        {
            MessageBoxManager.GetMessageBoxStandard("No differences found!","No differences found. please make sure you've copied and pasted correctly.",ButtonEnum.Ok).ShowAsync();
            return;
        }
        await Task.WhenAll(Task.Delay(150),CopyText());
        TransitionToCheckmark("CopyResults");
    }

    private void TransitionToLoading(string buttonName)
    {
        IconStates[$"{buttonName}Checkmark"] = false;
        IconStates[$"{buttonName}Loading"] = true;
        OnPropertyChanged(nameof(IconStates));
    }

    private void TransitionToCheckmark(string buttonName)
    {
        IconStates[$"{buttonName}Loading"] = false;
        IconStates[$"{buttonName}Checkmark"] = true;
        OnPropertyChanged(nameof(IconStates));
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


