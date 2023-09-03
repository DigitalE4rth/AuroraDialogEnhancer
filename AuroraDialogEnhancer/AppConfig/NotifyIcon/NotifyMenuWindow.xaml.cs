using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AuroraDialogEnhancer.AppConfig.Statics;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Frontend.Services;
using WhyOrchid.Controls;
using Application = System.Windows.Application;
using Button = WhyOrchid.Controls.Button;
using Control = System.Windows.Forms.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace AuroraDialogEnhancer.AppConfig.NotifyIcon;
    
public partial class NotifyMenuWindow
{
    private readonly CoreService            _coreService;
    private readonly ExtensionConfigService _extensionConfigService;
    private readonly ExtensionsProvider     _extensionProvider;
    private readonly HookedGameDataProvider _hookedGameDataProvider;
    private readonly UiService              _uiService;

    private readonly Dictionary<string, Button> _buttonsByGameId;
    private Button? _processingButton;
    private string  _latestGameId;

    public NotifyMenuWindow(CoreService            coreService,
                            ExtensionConfigService extensionConfigService,
                            ExtensionsProvider     extensionProvider,
                            HookedGameDataProvider hookedGameDataProvider,
                            UiService              uiService)
    {
        Unloaded += NotifyMenuWindow_Unloaded;

        _coreService            = coreService;
        _extensionConfigService = extensionConfigService;
        _hookedGameDataProvider = hookedGameDataProvider;
        _extensionProvider      = extensionProvider;
        _uiService              = uiService;
        _buttonsByGameId        = new Dictionary<string, Button>();
        _latestGameId           = Properties.Settings.Default.App_HookSettings_SelectedGameId;

        InitializeComponent();
        InitializeGameButtons();
    }

    private void InitializeGameButtons()
    {
        foreach (var presetInfo in _extensionProvider.ExtensionsDictionary.Values)
        {
            var gameButton = new Button
            {
                MinHeight = 30,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Style = (Style) Application.Current.Resources["ButtonText"],
                Content = new NotifyGameContent
                {
                    TextContent = { Text = presetInfo.Name }
                },
                Tag = presetInfo.Id
            };

            gameButton.Click += GameButton_Click;
            _buttonsByGameId.Add(presetInfo.Id, gameButton);
            ContainerGameButtons.Children.Add(gameButton);
        }

        if (_extensionProvider.ExtensionsDictionary.Count == 0)
        {
            ButtonScreenshots.Visibility = Visibility.Collapsed;
            SeparatorExit.Visibility = Visibility.Collapsed;
        }

        _buttonsByGameId.TryGetValue(Properties.Settings.Default.App_HookSettings_SelectedGameId!, out var initialButton);
        initialButton ??= _buttonsByGameId.Values.First();
        _processingButton = initialButton;

        SetHookStateIcon();
        _hookedGameDataProvider.OnHookStateChanged += OnHookDataStateChanged;
    }

    private void GameButton_Click(object sender, RoutedEventArgs e)
    {
        _latestGameId = (string) ((Button) sender).Tag;

        if (!_uiService.IsMainWindowShown())
        {
            Properties.Settings.Default.App_HookSettings_SelectedGameId = _latestGameId;
            Properties.Settings.Default.Save();
        }

        Task.Run(() => _coreService.RestartAutoDetection(_latestGameId).ConfigureAwait(false));
        CloseNotifyMenuWindow();
    }

    private void OnHookDataStateChanged(object sender, EventArgs e) => Application.Current.Dispatcher.Invoke(SetHookStateIcon);

    private void SetHookStateIcon()
    {
        var previousContent = (NotifyGameContent) _processingButton!.Content;
        previousContent.StopAnimation();

        if (_hookedGameDataProvider.Id is null)
        {
            previousContent.Icon.Data = new PathGeometry();
            return;
        }

        if (!_hookedGameDataProvider.Id.Equals(Properties.Settings.Default.App_HookSettings_SelectedGameId, StringComparison.Ordinal))
        {
            previousContent.Icon.Data = new PathGeometry();
        }

        if (_hookedGameDataProvider.Id is null) return;
        _processingButton = _buttonsByGameId[_hookedGameDataProvider.Id];
        var newContent = (NotifyGameContent) _processingButton.Content;

        var icon = GetIconAndSetMargins(newContent.Icon);
        newContent.Icon.Data = icon is null ? new PathGeometry() : (PathGeometry) Application.Current.Resources[icon];

        if (_hookedGameDataProvider.HookState is not (EHookState.Search or EHookState.Canceled)) return;
        newContent.BeginAnimation();
    }

    private string? GetIconAndSetMargins(PathIcon pathIcon)
    {
        pathIcon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Large;
        switch (_hookedGameDataProvider.HookState)
        {
            case EHookState.Hooked:
                return "I.S.Check";
            case EHookState.Warning:
            case EHookState.Error:
                pathIcon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Medium;
                return "I.R.PriorityHigh";
            case EHookState.Paused:
                pathIcon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Small;
                return "I.S.Pause";
            case EHookState.Search:
            case EHookState.Canceled:
                return "I.S.Spinner";
            case EHookState.None:
            default:
                return null;
        }
    }

    private void Button_OpenMainWindow_OnClick(object sender, RoutedEventArgs e) => _uiService.ShowMainWindow();

    private void Button_Screenshots_OnClick(object sender, RoutedEventArgs e)
    {
        var screenshotsFolder = _extensionConfigService.GetScreenshotsLocation(_latestGameId);
        if (!Directory.Exists(screenshotsFolder))
        {
            Directory.CreateDirectory(screenshotsFolder);
        }

        Process.Start(new ProcessStartInfo
        {
            Arguments = screenshotsFolder,
            UseShellExecute = true,
            FileName = Global.StringConstants.ExplorerName
        });

        CloseNotifyMenuWindow();
    }

    private void Button_Exit_OnClick(object sender, RoutedEventArgs e)
    {
        CloseNotifyMenuWindow();
        Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var activeScreen = Screen.FromPoint(Control.MousePosition);

        double xPositionToSet = Control.MousePosition.X;
        double yPositionToSet = Control.MousePosition.Y;

        if (xPositionToSet + arrangeBounds.Width > activeScreen.WorkingArea.Width + activeScreen.WorkingArea.X &&
            xPositionToSet - activeScreen.WorkingArea.X - arrangeBounds.Width > 0)
        {
            xPositionToSet -= arrangeBounds.Width;
            Left = xPositionToSet + Container.Margin.Right;
        }
        else
        {
            Left = xPositionToSet - Container.Margin.Left;
        }

        if (yPositionToSet + arrangeBounds.Height > activeScreen.WorkingArea.Height + activeScreen.WorkingArea.Y &&
            yPositionToSet - activeScreen.WorkingArea.Y - arrangeBounds.Height > 0)
        {
            yPositionToSet -= arrangeBounds.Height;
            Top = yPositionToSet + Container.Margin.Bottom;
        }
        else
        {
            Top = yPositionToSet - Container.Margin.Top;
        }

        return base.ArrangeOverride(arrangeBounds);
    }

    private void Shadow_OnMouseDown(object sender, MouseButtonEventArgs e) => CloseNotifyMenuWindow();

    private void Shadow_OnTouchDown(object sender, TouchEventArgs e) => CloseNotifyMenuWindow();

    private void Shadow_OnStylusDown(object sender, StylusDownEventArgs e) => CloseNotifyMenuWindow();

    private void CloseNotifyMenuWindow()
    {
        Hide();
        Close();
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        CloseNotifyMenuWindow();
    }

    private void NotifyMenuWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= NotifyMenuWindow_Unloaded;
        _hookedGameDataProvider.OnHookStateChanged -= OnHookDataStateChanged;
    }
}
