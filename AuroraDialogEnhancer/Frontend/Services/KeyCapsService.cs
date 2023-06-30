using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Frontend.Providers;

namespace AuroraDialogEnhancer.Frontend.Services;

public class KeyCapsService
{
    private readonly DefaultUiElementsProvider _defaultUiElementsProvider;

    public KeyCapsService()
    {
        _defaultUiElementsProvider = new DefaultUiElementsProvider();
    }

    private UIElement JoinKeys(TriggerViewModel triggerViewModel)
    {
        return _defaultUiElementsProvider.GetKeyCap(string.Join(" + ", triggerViewModel.KeyNames));
    }
    
    private List<UIElement> JoinKeys(IReadOnlyList<TriggerViewModel> triggerViewModels)
    {
        var result = new List<UIElement>();
        for (var i = 0; i < triggerViewModels.Count; ++i)
        {
            result.Add(JoinKeys(triggerViewModels[i]));

            if (i != triggerViewModels.Count - 1)
            {
                result.Add(_defaultUiElementsProvider.GetDivider());
            }
        }

        return result;
    }

    public void SetKeyCaps(ContentControl control, ActionViewModel actionViewModel)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };
        var elementsToAdd = JoinKeys(actionViewModel.TriggerViewModels);
        elementsToAdd.ForEach(element => panel.Children.Add(element));
        control.Content = panel;
    }

    public void SetKeyCaps(Panel panel, TriggerViewModel triggerViewModel)
    {
        panel.Children.Clear();
        panel.Children.Add(JoinKeys(triggerViewModel));
    }
}
