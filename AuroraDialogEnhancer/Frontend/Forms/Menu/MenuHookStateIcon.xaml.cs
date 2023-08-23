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
    private readonly HookedGameDataProvider? _hookedGameInfoProvider;
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
        _hookedGameInfoProvider = AppServices.ServiceProvider.GetRequiredService<HookedGameDataProvider>();

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
        IconBackground.Background = Brushes.Transparent;
        Icon.Foreground           = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_OnSurface));
        Icon.Margin               = new Thickness(0);

        switch (_hookedGameInfoProvider!.HookState)
        {
            case EHookState.Hooked:
                icon = "Icon.Solid.CircleCheck";
                Icon.Foreground           = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Primary));
                Icon.Margin               = new Thickness(0);
                break;
            case EHookState.Warning:
            case EHookState.Error:
                icon = "Icon.PriorityHigh";
                IconBackground.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error));
                Icon.Foreground           = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_OnError));
                Icon.Margin               = new Thickness(0,2,0,2);
                break;
            case EHookState.Paused:
                icon = "Icon.Pause";
                Icon.Margin = new Thickness(3, 0, 3, 0);
                break;
            case EHookState.Search:
            case EHookState.Canceled:
                icon = "Icon.Spinner";
                break;
            case EHookState.None:
            default:
                icon = null;
                break;
        }

        Icon.Data = icon is null
            ? new PathGeometry()
            : (PathGeometry) Application.Current.Resources[icon];

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
