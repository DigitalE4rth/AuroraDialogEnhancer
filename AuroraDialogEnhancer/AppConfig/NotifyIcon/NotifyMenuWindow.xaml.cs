using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Extensions;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Utils;
using AuroraDialogEnhancer.Frontend.Services;
using WhyOrchid.Controls;
using Application = System.Windows.Application;
using Button = WhyOrchid.Controls.Button;
using Control = System.Windows.Forms.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace AuroraDialogEnhancer.AppConfig.NotifyIcon;
    
public partial class NotifyMenuWindow
{
    private readonly CoreService            _coreService;
    private readonly ExtensionConfigService _extensionConfigService;
    private readonly ExtensionsProvider     _extensionProvider;
    private readonly ProcessDataProvider    _processDataProvider;
    private readonly UiService              _uiService;

    private readonly Dictionary<string, Button> _buttonsByGameId;
    private Button? _processingButton;
    private string  _latestGameId;

    public NotifyMenuWindow(CoreService            coreService,
                            ExtensionConfigService extensionConfigService,
                            ExtensionsProvider     extensionProvider,
                            ProcessDataProvider    processDataProvider,
                            UiService              uiService)
    {
        Unloaded += NotifyMenuWindow_Unloaded;

        _coreService            = coreService;
        _extensionConfigService = extensionConfigService;
        _extensionProvider      = extensionProvider;
        _processDataProvider    = processDataProvider;
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
        _processDataProvider.OnHookStateChanged += OnHookDataStateChanged;
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

        if (_processDataProvider.Id is null)
        {
            previousContent.Icon.Data = new PathGeometry();
            return;
        }

        if (!_processDataProvider.Id.Equals(Properties.Settings.Default.App_HookSettings_SelectedGameId, StringComparison.Ordinal))
        {
            previousContent.Icon.Data = new PathGeometry();
        }

        if (_processDataProvider.Id is null) return;
        _processingButton = _buttonsByGameId[_processDataProvider.Id];
        var newContent = (NotifyGameContent) _processingButton.Content;

        var icon = GetIconAndSetMargins(newContent.Icon);
        newContent.Icon.Data = icon is null ? new PathGeometry() : (PathGeometry) Application.Current.Resources[icon];

        if (_processDataProvider.HookState is not (EHookState.Search or EHookState.Canceled)) return;
        newContent.BeginAnimation();
    }

    private string? GetIconAndSetMargins(PathIcon pathIcon)
    {
        pathIcon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Large;
        switch (_processDataProvider.HookState)
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

    private void Button_OpenMainWindow_OnClick(object sender, RoutedEventArgs e) => _uiService.ShowMainWindow(true);

    private void Button_Screenshots_OnClick(object sender, RoutedEventArgs e)
    {
        new FolderProcessStartService().Open(_extensionConfigService.GetScreenshotsLocation(_latestGameId));
        CloseNotifyMenuWindow();
    }

    private void Button_Exit_OnClick(object sender, RoutedEventArgs e)
    {
        CloseNotifyMenuWindow();
        Application.Current.Dispatcher.Invoke(Application.Current.Shutdown, DispatcherPriority.Send);
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var matrix = PresentationSource.FromVisual(this)!.CompositionTarget.TransformFromDevice;
        var adaptedMousePoint = matrix.Transform(new Point(Control.MousePosition.X, Control.MousePosition.Y));
        
        var activeScreen = Screen.FromPoint(Control.MousePosition);
        var adaptedLeftTop = matrix.Transform(new Point(activeScreen.WorkingArea.Left, activeScreen.WorkingArea.Top));
        var adaptedRightBottom = matrix.Transform(new Point(activeScreen.WorkingArea.Right, activeScreen.WorkingArea.Bottom));
        
        var adaptedWorkingArea = new Rectangle(
            (int) adaptedLeftTop.X,
            (int) adaptedLeftTop.Y,
            (int) (adaptedRightBottom.X - adaptedLeftTop.X),
            (int) (adaptedRightBottom.Y - adaptedLeftTop.Y));

        if (adaptedMousePoint.X + arrangeBounds.Width > adaptedWorkingArea.Width + adaptedWorkingArea.X &&
            adaptedMousePoint.Y - adaptedWorkingArea.X - arrangeBounds.Width > 0)
        {
            adaptedMousePoint.X -= arrangeBounds.Width;
            Left = adaptedMousePoint.X + Container.Margin.Right;
        }
        else
        {
            Left = adaptedMousePoint.X - Container.Margin.Left;
        }

        if (adaptedMousePoint.Y + arrangeBounds.Height > adaptedWorkingArea.Height + adaptedWorkingArea.Y &&
            adaptedMousePoint.Y - adaptedWorkingArea.Y - arrangeBounds.Height > 0)
        {
            adaptedMousePoint.Y -= arrangeBounds.Height;
            Top = adaptedMousePoint.Y + Container.Margin.Bottom;
        }
        else
        {
            Top = adaptedMousePoint.Y - Container.Margin.Top;
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
        _processDataProvider.OnHookStateChanged -= OnHookDataStateChanged;
    }
}
