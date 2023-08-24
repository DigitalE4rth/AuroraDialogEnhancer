using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using Microsoft.Extensions.DependencyInjection;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Controls.GameSelector;

public partial class GameSelector
{
    private readonly ExtensionsProvider?     _extensionsProvider;
    private readonly HookedGameDataProvider? _hookedGameInfoProvider;

    private readonly Dictionary<string, ComboBoxItem>? _comboBoxItemsByGameId;
    private readonly Storyboard _spinnerStoryboard;
    private bool _isSpinnerAnimationRunning;

    private ComboBoxItem? _processingComboBoxItem;
    public event EventHandler? OnGameChanged;

    public GameSelector()
    {
        InitializeComponent();

        _spinnerStoryboard = new Storyboard();
        new SpinnerStoryboardProvider().SetStoryboard(_spinnerStoryboard, LeftIcon);

        _comboBoxItemsByGameId = new Dictionary<string, ComboBoxItem>();

#if DEBUG
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
#endif

        _extensionsProvider = AppServices.ServiceProvider.GetRequiredService<ExtensionsProvider>();
        _hookedGameInfoProvider = AppServices.ServiceProvider.GetRequiredService<HookedGameDataProvider>();
        Initialize();
    }

    public void Initialize()
    {
        foreach (var presetInfo in _extensionsProvider!.ExtensionsDictionary.Values)
        {
            var comboBoxItem = new ComboBoxItem
            {
                Content = new GameSelectorContent
                {
                    TextContent = { Text = presetInfo.DisplayName }
                },
                Tag = presetInfo.Id
            };

            _comboBoxItemsByGameId!.Add(presetInfo.Id, comboBoxItem);
            ComboBoxGames.Items.Add(comboBoxItem);
        }

        _comboBoxItemsByGameId!.TryGetValue(Properties.Settings.Default.UI_HookSettings_SelectedGameId!, out var initialItem);
        initialItem ??= _comboBoxItemsByGameId.Values.First();
        _processingComboBoxItem = initialItem;
        ComboBoxGames.SelectedItem = initialItem;
        ComboBoxGames.CustomContent = ((GameSelectorContent) initialItem.Content).TextContent.Text;

        Unloaded += GameSelector_Unloaded;
        ComboBoxGames.SelectionChanged += ComboBoxGames_SelectionChanged;
        SetIcons();
        _hookedGameInfoProvider!.OnHookStateChanged += OnHookStateChanged;
    }

    private void OnHookStateChanged(object sender, EventArgs e) => Application.Current.Dispatcher.Invoke(SetIcons);

    private void SetIcons()
    {
        SetTitleIcon();
        SetComboBoxItemIcon();
    }

    private void SetComboBoxItemIcon()
    {
        var previousContent = (GameSelectorContent) _processingComboBoxItem!.Content;
        previousContent.StopAnimation();

        if (_hookedGameInfoProvider!.HookState is EHookState.None or EHookState.Canceled)
        {
            previousContent.Icon.Data = new PathGeometry();
            return;
        }

        if (!_hookedGameInfoProvider.Id!.Equals(Properties.Settings.Default.UI_HookSettings_SelectedGameId, StringComparison.Ordinal))
        {
            previousContent.Icon.Data = new PathGeometry();
        }

        if (_hookedGameInfoProvider.Id is null) return;
        _processingComboBoxItem = _comboBoxItemsByGameId![_hookedGameInfoProvider.Id!];
        var newContent = (GameSelectorContent) _processingComboBoxItem!.Content;

        var icon = GetIconAndSetMargins(newContent.Icon);
        newContent.Icon.Data = icon is null ? new PathGeometry() : (PathGeometry) Application.Current.Resources[icon];

        if (_hookedGameInfoProvider.HookState is not (EHookState.Search or EHookState.Canceled)) return;
        newContent.BeginAnimation();
    }

    private void SetTitleIcon()
    {
        if (_isSpinnerAnimationRunning)
        {
            _isSpinnerAnimationRunning = false;
            _spinnerStoryboard.Stop();
        }

        if (_hookedGameInfoProvider!.Id is not null &&
            !_hookedGameInfoProvider.Id.Equals(Properties.Settings.Default.UI_HookSettings_SelectedGameId, StringComparison.Ordinal))
        {
            LeftIcon.Margin = new Thickness(0);
            LeftIcon.Data = (PathGeometry) Application.Current.Resources["Icon.VitalSigns"];
            return;
        }

        var icon = GetIconAndSetMargins(LeftIcon);
        LeftIcon.Data = icon is null 
            ? (PathGeometry) Application.Current.Resources["Icon.Controller"] 
            : (PathGeometry) Application.Current.Resources[icon];

        if (_hookedGameInfoProvider!.HookState is not (EHookState.Search or EHookState.Canceled)) return;
        _isSpinnerAnimationRunning = true;
        _spinnerStoryboard.Begin();
    }

    private void ComboBoxGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (ComboBoxItem) ComboBoxGames.SelectedItem;
        var selectedGameId = (string) selectedItem.Tag;

        Properties.Settings.Default.UI_HookSettings_SelectedGameId = selectedGameId;
        Properties.Settings.Default.Save();

        ComboBoxGames.CustomContent = ((GameSelectorContent) selectedItem.Content).TextContent.Text;
        SetTitleIcon();
        OnGameChanged?.Invoke(this, EventArgs.Empty);
    }

    private string? GetIconAndSetMargins(PathIcon pathIcon)
    {
        pathIcon.Margin = new Thickness(0);
        switch (_hookedGameInfoProvider!.HookState)
        {
            case EHookState.Hooked:
                return "Icon.Check";
            case EHookState.Warning:
            case EHookState.Error:
                return "Icon.PriorityHigh";
            case EHookState.Paused:
                pathIcon.Margin = new Thickness(3, 0, 3, 0);
                return "Icon.Pause";
            case EHookState.Search:
            case EHookState.Canceled:
                return "Icon.Spinner";
            case EHookState.None:
            default:
                return null;
        }
    }

    private void GameSelector_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= GameSelector_Unloaded;
        ComboBoxGames.SelectionChanged -= ComboBoxGames_SelectionChanged;
        _hookedGameInfoProvider!.OnHookStateChanged -= OnHookStateChanged;
    }
}
