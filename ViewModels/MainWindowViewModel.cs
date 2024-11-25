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
using Prefix_List_Compare.Views;

namespace Prefix_List_Compare.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Window? _thisWindow;
    [ObservableProperty] private bool _nothingToCalculateLabel = false;
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
        _thisWindow = window;
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

    private async Task CheckIfResultsAreCalculatable()
    {
        if (CurrentConfiguration != null && DesiredNetworks != null)
        {
            await CalculateResults();
            ResultsButtonState = !string.IsNullOrEmpty(NetWorksToReturn);
        }
        else ResultsButtonState = false;
    }

    private async Task CalculateResults()
    {
        if (CurrentConfiguration != null && DesiredNetworks != null)
        {
            NothingToCalculateLabel = false;
            NetWorksToReturn = string.Empty;
            NetWorksToReturn = await _calculationModel.CalculateNetworkDifferences(CurrentConfiguration, DesiredNetworks);
            if (NetWorksToReturn==string.Empty){
                NothingToCalculateLabel = true;
            }
        }
    }

    public string Greeting { get; } = "Prefix List Compare";
    private async Task CopyTextFromClipboard()
    {
        try
        {
            if (_thisWindow != null && _thisWindow.Clipboard != null){
                var clipboardText = "";
                clipboardText = await Task.Run(()=>_thisWindow.Clipboard.GetTextAsync().Result);
                if (clipboardText != null){
                    Text=string.Empty;
                    Text=Text?.Insert(0, clipboardText);
                }
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard("Internal Error","Internal Error. Please restart the application and/or your computer.",ButtonEnum.Ok).ShowAsync();

            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Exception Found", e.Message, ButtonEnum.Ok).ShowAsync();

        }
    }

    private async Task CopyTextToClipboard(string text)
    {
        try
        {
            if (_thisWindow != null && _thisWindow.Clipboard != null)
            {
                await _thisWindow.Clipboard.ClearAsync();
                await _thisWindow.Clipboard.SetTextAsync(text);
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard("No access to Clipboard","No access to Clipboard.",ButtonEnum.Ok).ShowAsync();

            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Exception Found", e.Message, ButtonEnum.Ok).ShowAsync();

        }
    }
    
    
    public async Task PasteCurrentConfiguration()
    {
        TransitionToLoading("PasteCurrentConfiguration");
        await Task.WhenAll(Task.Delay(150),CopyTextFromClipboard());
        CurrentConfiguration = string.Empty;
         if (!string.IsNullOrEmpty(Text)){
             CurrentConfiguration = CurrentConfiguration?.Insert(0,Text);
             await CheckIfResultsAreCalculatable();
         }
         TransitionToCheckmark("PasteCurrentConfiguration");
         await Task.Yield();
    }
    

    public async Task PasteDesiredNetworks()
    {
        TransitionToLoading("PasteDesiredNetworks");
        await Task.WhenAll(Task.Delay(150), CopyTextFromClipboard());
        DesiredNetworks = string.Empty;
        if (!string.IsNullOrEmpty(Text)){
             DesiredNetworks = Text?.Insert(0,Text);
             await CheckIfResultsAreCalculatable();
        }
        TransitionToCheckmark("PasteDesiredNetworks");
        await Task.Yield();
    }
    
    public async Task CopyResults()
    {
        TransitionToLoading("CopyResults");
         if (NetWorksToReturn == null)
         {
             MessageBoxManager.GetMessageBoxStandard("No differences found!","No differences found. please make sure you've copied and pasted correctly.",ButtonEnum.Ok).ShowAsync();
             return;
         }
         await Task.WhenAll(Task.Delay(150),CopyTextToClipboard(NetWorksToReturn));
         TransitionToCheckmark("CopyResults");
         await Task.Yield();
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
}


