using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Frontend.Controls.GameSelector;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Frontend.Forms.Menu;

public partial class MenuHookStateIcon
{
    private readonly ProcessDataProvider? _hookedGameInfoProvider;
    private readonly Storyboard _spinnerStoryboard;
    private bool _isSpinnerAnimationRunning;

    public MenuHookStateIcon()
    {
        InitializeComponent();

        _spinnerStoryboard = new Storyboard();
        new SpinnerStoryboardProvider().SetStoryboard(_spinnerStoryboard, Icon);

#if DEBUG
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
#endif
        _hookedGameInfoProvider = AppServices.ServiceProvider.GetRequiredService<ProcessDataProvider>();

        SetIcon();
        Unloaded += OnUnloaded;
        _hookedGameInfoProvider.OnHookStateChanged += OnHookStateChanged;
    }

    private void OnHookStateChanged(object sender, System.EventArgs e) => Application.Current.Dispatcher.Invoke(SetIcon);

    private void SetIcon()
    {
        if (_isSpinnerAnimationRunning)
        {
            _isSpinnerAnimationRunning = false;
            _spinnerStoryboard.Stop();
        }

        string? icon;
        Icon.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_OnSurface));
        Icon.Height     = Icon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Large;
        IconHighlight.Visibility = Visibility.Collapsed;

        switch (_hookedGameInfoProvider!.HookState)
        {
            case EHookState.Hooked:
                icon = "I.S.CircleCheck";
                Icon.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Primary));
                IconHighlight.Visibility = Visibility.Visible;
                break;
            case EHookState.Warning:
            case EHookState.Error:
                icon = "I.S.Error";
                Icon.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error));
                IconHighlight.Visibility = Visibility.Visible;
                break;
            case EHookState.Paused:
                icon = "I.S.Pause";
                Icon.Height = WhyOrchid.Properties.Settings.Default.FontStyle_Small;
                break;
            case EHookState.Search:
            case EHookState.Canceled:
                icon = "I.S.Spinner";
                break;
            case EHookState.None:
            default:
                icon = null;
                break;
        }

        var iconData = icon is null ? new PathGeometry() : (PathGeometry) Application.Current.Resources[icon];

        Icon.Data = iconData;
        IconHighlight.Data = iconData;

        if (_hookedGameInfoProvider.HookState is not (EHookState.Search or EHookState.Canceled)) return;
        _isSpinnerAnimationRunning = true;
        _spinnerStoryboard.Begin();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _hookedGameInfoProvider!.OnHookStateChanged -= OnHookStateChanged;
        Unloaded -= OnUnloaded;
    }
}
