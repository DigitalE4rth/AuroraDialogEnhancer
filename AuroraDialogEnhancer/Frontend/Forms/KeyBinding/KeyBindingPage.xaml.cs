using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Frontend.Services;
using AuroraDialogEnhancerExtensions.KeyBinding;
using Microsoft.Extensions.DependencyInjection;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.KeyBinding;

public partial class KeyBindingPage : Page
{
    private readonly KeyBindingProfileService _keyBindingProfileService;
    private readonly KeyCapsService           _keyCapsService;

    private KeyBindingProfileViewModel? _keyBindingProfileViewModel;

    public KeyBindingPage(KeyCapsService           keyCapsService,
                          KeyBindingProfileService keyBindingProfileService)
    {
        _keyBindingProfileService = keyBindingProfileService;
        _keyCapsService           = keyCapsService;

        InitializeComponent();

        Unloaded += KeyBindingPage_Unloaded;
        InitializeKeyBindingProfile();
    }

    private void InitializeKeyBindingProfile()
    {
        UnloadComponentEvents();
        _keyBindingProfileViewModel = _keyBindingProfileService.GetViewModel(Properties.Settings.Default.UI_HookSettings_SelectedGameId);
        InitializeProfileValues();
        InitializeComponentEvents();
    }

    private void GameSelector_OnGameChanged(object sender, EventArgs e) => InitializeKeyBindingProfile();

    private void InitializeComponentEvents()
    {
        GameSelector.OnGameChanged                   += GameSelector_OnGameChanged;
        ComboBoxSingleBehaviour.SelectionChanged     += ComboBoxSingleBehaviour_OnSelectionChanged;
        ComboBoxNumericBehaviour.SelectionChanged    += ComboBoxNumericBehaviour_OnSelectionChanged;
        ComboBoxCursorBehaviour.SelectionChanged     += ComboBoxCursorBehaviour_OnSelectionChanged;
        ComboBoxHiddenCursorSetting.SelectionChanged += ComboBoxHiddenCursorSettingOnSelectionChanged;
    }

    private void UnloadComponentEvents()
    {
        GameSelector.OnGameChanged                   -= GameSelector_OnGameChanged;
        ComboBoxSingleBehaviour.SelectionChanged     -= ComboBoxSingleBehaviour_OnSelectionChanged;
        ComboBoxNumericBehaviour.SelectionChanged    -= ComboBoxNumericBehaviour_OnSelectionChanged;
        ComboBoxCursorBehaviour.SelectionChanged     -= ComboBoxCursorBehaviour_OnSelectionChanged;
        ComboBoxHiddenCursorSetting.SelectionChanged -= ComboBoxHiddenCursorSettingOnSelectionChanged;
    }

    public void InitializeProfileValues()
    {
        ToggleCycleThrough.IsChecked = _keyBindingProfileViewModel!.IsCycleThrough;
        ToggleHideCursor.IsChecked = _keyBindingProfileViewModel.IsCursorHideOnManualClick;

        var singleBehaviour = ComboBoxSingleBehaviour.Items.OfType<ComboBoxItem>().First(item => (ESingleDialogOptionBehaviour)item.Tag == _keyBindingProfileViewModel.SingleDialogOptionBehaviour);
        ComboBoxSingleBehaviour.SelectedItem = singleBehaviour;

        var numericBehaviour = ComboBoxNumericBehaviour.Items.OfType<ComboBoxItem>().First(item => (ENumericActionBehaviour)item.Tag == _keyBindingProfileViewModel.NumericActionBehaviour);
        ComboBoxNumericBehaviour.SelectedItem = numericBehaviour;

        var cursorBehaviour = ComboBoxCursorBehaviour.Items.OfType<ComboBoxItem>().First(item => (ECursorBehaviour)item.Tag == _keyBindingProfileViewModel.CursorBehaviour);
        ComboBoxCursorBehaviour.SelectedItem = cursorBehaviour;

        var hiddenCursorSetting = ComboBoxHiddenCursorSetting.Items.OfType<ComboBoxItem>().First(item => (EHiddenCursorSetting)item.Tag == _keyBindingProfileViewModel.HiddenCursorSetting);
        ComboBoxHiddenCursorSetting.SelectedItem = hiddenCursorSetting;

        InitializeKeyCaps();
    }

    private void InitializeKeyCaps()
    {
        _keyCapsService.SetKeyCaps(CardButtonReload,      _keyBindingProfileViewModel!.Reload);
        _keyCapsService.SetKeyCaps(CardButtonPauseResume, _keyBindingProfileViewModel.PauseResume);
        _keyCapsService.SetKeyCaps(CardButtonScreenshot,  _keyBindingProfileViewModel.Screenshot);
        _keyCapsService.SetKeyCaps(CardButtonHideCursor,  _keyBindingProfileViewModel.HideCursor);

        _keyCapsService.SetKeyCaps(CardButtonSelect,     _keyBindingProfileViewModel.Select);
        _keyCapsService.SetKeyCaps(CardButtonPrevious,   _keyBindingProfileViewModel.Previous);
        _keyCapsService.SetKeyCaps(CardButtonNext,       _keyBindingProfileViewModel.Next);
        _keyCapsService.SetKeyCaps(CardButtonAutoDialog, _keyBindingProfileViewModel.AutoDialog);
        _keyCapsService.SetKeyCaps(CardButtonHideUi,     _keyBindingProfileViewModel.HideUi);
        _keyCapsService.SetKeyCaps(CardButtonFullScreenPopUp, _keyBindingProfileViewModel.FullScreenPopUp);

        _keyCapsService.SetKeyCaps(CardButtonOne,   _keyBindingProfileViewModel.One);
        _keyCapsService.SetKeyCaps(CardButtonTwo,   _keyBindingProfileViewModel.Two);
        _keyCapsService.SetKeyCaps(CardButtonThree, _keyBindingProfileViewModel.Three);
        _keyCapsService.SetKeyCaps(CardButtonFour,  _keyBindingProfileViewModel.Four);
        _keyCapsService.SetKeyCaps(CardButtonFive,  _keyBindingProfileViewModel.Five);
        _keyCapsService.SetKeyCaps(CardButtonSix,   _keyBindingProfileViewModel.Six);
        _keyCapsService.SetKeyCaps(CardButtonSeven, _keyBindingProfileViewModel.Seven);
        _keyCapsService.SetKeyCaps(CardButtonEight, _keyBindingProfileViewModel.Eight);
        _keyCapsService.SetKeyCaps(CardButtonNine,  _keyBindingProfileViewModel.Nine);
        _keyCapsService.SetKeyCaps(CardButtonTen,   _keyBindingProfileViewModel.Ten);
    }

    private void ComboBoxSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((WhyOrchid.Controls.ComboBox) sender).SelectedIndex == -1) return;
        ((WhyOrchid.Controls.ComboBox) sender).SelectedIndex = -1;

        var selectedAction = (EKeyBindingSettingsAction) ((ComboBoxItem) e.AddedItems[0]).Tag;

        switch (selectedAction)
        {
            case EKeyBindingSettingsAction.Default:
                _keyBindingProfileService.SaveDefault(Properties.Settings.Default.UI_HookSettings_SelectedGameId);
                break;
            case EKeyBindingSettingsAction.Clear:
            default:
                _keyBindingProfileService.SavePristine(Properties.Settings.Default.UI_HookSettings_SelectedGameId);
                break;
        }

        InitializeKeyBindingProfile();
    }

    #region Actions
    #region Utilities
    private void ToggleCycleThrough_OnClick(object sender, RoutedEventArgs e)
    {
        _keyBindingProfileViewModel!.IsCycleThrough = (bool)ToggleCycleThrough.IsChecked!;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }

    private void ToggleHideCursor_OnClick(object sender, RoutedEventArgs e)
    {
        _keyBindingProfileViewModel!.IsCursorHideOnManualClick = (bool)ToggleHideCursor.IsChecked!;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }

    private void ComboBoxSingleBehaviour_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _keyBindingProfileViewModel!.SingleDialogOptionBehaviour = (ESingleDialogOptionBehaviour)((ComboBoxItem)ComboBoxSingleBehaviour.SelectedItem).Tag;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }

    private void ComboBoxNumericBehaviour_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _keyBindingProfileViewModel!.NumericActionBehaviour = (ENumericActionBehaviour)((ComboBoxItem)ComboBoxNumericBehaviour.SelectedItem).Tag;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }

    private void ComboBoxCursorBehaviour_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _keyBindingProfileViewModel!.CursorBehaviour = (ECursorBehaviour)((ComboBoxItem)ComboBoxCursorBehaviour.SelectedItem).Tag;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }

    private void ComboBoxHiddenCursorSettingOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _keyBindingProfileViewModel!.HiddenCursorSetting = (EHiddenCursorSetting)((ComboBoxItem)ComboBoxHiddenCursorSetting.SelectedItem).Tag;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);
    }
    #endregion

    #region General
    private void CardButton_Reload_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton) sender, _keyBindingProfileViewModel!.Reload);
    }

    private void CardButton_PauseResume_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.PauseResume);
    }

    private void CardButton_Screenshot_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Screenshot);
    }

    private void CardButton_HideCursor_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.HideCursor);
    }
    #endregion

    #region Controls
    private void CardButton_Select_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Select);
    }

    private void CardButton_Previous_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Previous);
    }

    private void CardButton_Next_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Next);
    }

    private void CardButton_AutoDialog_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.AutoDialog);
    }

    private void CardButton_HideUi_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.HideUi);
    }

    private void CardButton_FullScreenPopUp_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.FullScreenPopUp);
    }
    #endregion

    #region Numeric
    private void CardButton_First_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.One);
    }

    private void CardButton_Second_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Two);
    }

    private void CardButton_Third_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Three);
    }

    private void CardButton_Fourth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Four);
    }

    private void CardButton_Fifth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Five);
    }

    private void CardButton_Sixth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Six);
    }

    private void CardButton_Seventh_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Seven);
    }

    private void CardButton_Eighth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Eight);
    }

    private void CardButton_Ninth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Nine);
    }

    private void CardButton_Tenth_OnClick(object sender, RoutedEventArgs e)
    {
        EditActionViewModel((CardButton)sender, _keyBindingProfileViewModel!.Ten);
    }
    #endregion
    #endregion

    private void EditActionViewModel(CardButton cardButton, ActionViewModel actionViewModel)
    {
        var window = AppServices.ServiceProvider.GetRequiredService<TriggerEditorWindow>();
        window.Initialize(cardButton.Title.ToString(), actionViewModel);

        if (window.ShowDialog() != true) return;

        var result = window.GetResult()!;
        result.TriggerViewModels = result.TriggerViewModels.Distinct().ToList();

        RemoveDuplicates(result);

        actionViewModel.TriggerViewModels = result.TriggerViewModels;
        _keyBindingProfileService.SaveAndApplyIfHookIsActive(Properties.Settings.Default.UI_HookSettings_SelectedGameId, _keyBindingProfileViewModel!);

        InitializeProfileValues();
    }

    private void RemoveDuplicates(ActionViewModel sourceVm)
    {
        RemoveDuplicates(_keyBindingProfileViewModel!.Reload,     sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.PauseResume, sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Screenshot,  sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.HideCursor,  sourceVm);

        RemoveDuplicates(_keyBindingProfileViewModel.Select,     sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Previous,   sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Next,       sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.AutoDialog, sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.HideUi,     sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.FullScreenPopUp, sourceVm);

        RemoveDuplicates(_keyBindingProfileViewModel.One,   sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Two,   sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Three, sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Four,  sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Five,  sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Six,   sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Seven, sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Eight, sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Nine,  sourceVm);
        RemoveDuplicates(_keyBindingProfileViewModel.Ten,   sourceVm);
    }

    private void RemoveDuplicates(ActionViewModel targetVm, ActionViewModel sourceVm)
    {
        var duplicates = from target in targetVm.TriggerViewModels
                         from source in sourceVm.TriggerViewModels
                         where target.Equals(source)
                         select target;

        targetVm.TriggerViewModels = targetVm.TriggerViewModels.Except(duplicates).ToList();
    }

    private void KeyBindingPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= KeyBindingPage_Unloaded;
        UnloadComponentEvents();
    }
}
