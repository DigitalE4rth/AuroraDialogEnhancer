﻿using System.Reflection;
using System.Windows.Forms;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Frontend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.AppConfig.NotifyIcon;

public class NotifyIconService
{
    private readonly System.Windows.Forms.NotifyIcon _notifyIcon;
    private readonly UiService _uiService;

    public NotifyIconService(UiService uiService)
    {
        _uiService  = uiService;
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title,
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Statics.AppConstants.Locations.AssemblyExe)
        };

        _notifyIcon.Click       += NotifyIcon_OnClick;
        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
    }

    public void Initialize()
    {
        _notifyIcon.Visible = true;
    }

    private void NotifyIcon_OnClick(object sender, System.EventArgs e)
    {
        if (((MouseEventArgs)e).Button == MouseButtons.Left) return;
        var notifyWindow = AppServices.ServiceProvider.GetRequiredService<NotifyMenuWindow>();
        notifyWindow.Show();
        notifyWindow.Activate();
    }

    private void NotifyIcon_DoubleClick(object sender, System.EventArgs e)
    {
        _uiService.ShowMainWindow(true);
    }

    public void Dispose()
    {
        _notifyIcon.Click       -= NotifyIcon_OnClick;
        _notifyIcon.DoubleClick -= NotifyIcon_DoubleClick;
        _notifyIcon.Dispose();
    }
}
