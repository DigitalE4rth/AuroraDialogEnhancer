using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.Backend.Extensions;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.About;

public partial class AboutPage : Page
{
    private readonly ExtensionsProvider _extensionsProvider;

    public AboutPage(ExtensionsProvider extensionsProvider)
    {
        _extensionsProvider = extensionsProvider;
        InitializeComponent();
        InitializeExtensions();
    }

    private void InitializeExtensions()
    {
        var isLeft = true;
        var leftExtensions = new List<ExtensionViewModel>();
        var rightExtensions = new List<ExtensionViewModel>();
        foreach (var extension in _extensionsProvider.ExtensionsDictionary!.Values)
        {
            if (isLeft)
            {
                leftExtensions.Add(new ExtensionViewModel(extension));
                isLeft = false;
                continue;
            }

            rightExtensions.Add(new ExtensionViewModel(extension));
            isLeft = true;
        }

        ContainerExtensionsLeft.ItemsSource = leftExtensions;
        ContainerExtensionsRight.ItemsSource = rightExtensions;
    }

    private void Button_Link_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start((string) ((CardButton) sender).Tag);
    }
}

