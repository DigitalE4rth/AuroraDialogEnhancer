using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Interpreters;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.ViewModels;
using AuroraDialogEnhancer.Frontend.Controls.Cards;
using WhyOrchid.Controls;

namespace AuroraDialogEnhancer.Frontend.Forms.KeyBinding;

public partial class AutoSkipEditorWindow
{
    private readonly KeyboardHookManagerRecordService _keyboardHookManagerRecordService;
    private readonly KeyCapsService                   _keyCapsService;
    private readonly KeyInterpreterService            _keyInterpreterService;
    private readonly ModifierKeysProvider             _modifierKeysProvider;
    private readonly MouseHookManagerRecordService    _mouseHookManagerRecordService;

    private readonly Dictionary<CardDropDownWithArtificialFocus, TriggerViewModel> _cardDropDownButtonsDictionary;
    private TriggerViewModel                 _processingTrigger;
    private CardDropDownWithArtificialFocus? _processingCardDropDown;
    private AutoSkipDataContext              _autoSkipDataContext;
    private bool            _isRecording;
    private bool            _isCardButtonPristine;
    private readonly object _focusLock;

    public AutoSkipEditorWindow(KeyboardHookManagerRecordService hookManagerRecordServiceBase,
                                KeyCapsService                   keyCapsService,
                                KeyInterpreterService            keyInterpreterService, 
                                ModifierKeysProvider             modifierKeysProvider,
                                MouseHookManagerRecordService    mouseHookManagerRecordService)
    {
        _keyboardHookManagerRecordService = hookManagerRecordServiceBase;
        _keyCapsService                   = keyCapsService;
        _keyInterpreterService            = keyInterpreterService;
        _modifierKeysProvider             = modifierKeysProvider;
        _mouseHookManagerRecordService    = mouseHookManagerRecordService;
        
        Owner                 = Application.Current.MainWindow;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        _autoSkipDataContext           = new AutoSkipDataContext();
        _cardDropDownButtonsDictionary = new Dictionary<CardDropDownWithArtificialFocus, TriggerViewModel>();
        _processingTrigger             = new TriggerViewModel();
        _focusLock                     = new object();

        Unloaded    += TriggerEditorWindow_Unloaded;
        Deactivated += TriggerEditorWindow_Deactivated;
        Activated   += TriggerEditorWindow_Activated;

        InitializeComponent();
    }

    #region Initialization
    public void Initialize(AutoSkipConfigViewModel viewModel)
    {
        var viewModelSnapShot = new AutoSkipConfigViewModel(viewModel);
        _autoSkipDataContext  = new AutoSkipDataContext(viewModelSnapShot);
        DataContext           = _autoSkipDataContext;

        InitializeTriggers();
        SetCardActionElements();

        var skipModeItem = ComboBoxSkipMode.Items.OfType<ComboBoxItem>().First(item => (ESkipMode) item.Tag == viewModel.SkipMode);
        ComboBoxSkipMode.SelectedItem = skipModeItem;
        ComboBoxSkipMode.SelectionChanged += ComboBoxSkipMode_SelectionChanged;
        SetContainersSkipModeVisibility(viewModel.SkipMode);

        var skipStartConditionItem = ComboBoxSkipStartCondition.Items.OfType<ComboBoxItem>().First(item => (ESkipStartCondition) item.Tag == viewModel.StartCondition);
        ComboBoxSkipStartCondition.SelectedItem = skipStartConditionItem;
        ComboBoxSkipStartCondition.SelectionChanged += ComboBoxSkipStartConditionOnSelectionChanged;
    }

    public AutoSkipConfigViewModel GetResult()
    {
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels = _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Distinct(new TriggerViewModelComparer()).ToList();
        return _autoSkipDataContext.ViewModel;
    }

    private void TriggerEditorWindow_Activated(object sender, EventArgs e)
    {
        lock (_focusLock)
        {
            if (!_isRecording) return;
            _keyboardHookManagerRecordService.Start();
        }
    }

    private void TriggerEditorWindow_Deactivated(object sender, EventArgs e)
    {
        lock (_focusLock)
        {
            if (!_isRecording) return;
            _keyboardHookManagerRecordService.Stop();
        }
    }
    #endregion

    #region Elements
    private void ComboBoxSkipMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _autoSkipDataContext.ViewModel.SkipMode = (ESkipMode) ((ComboBoxItem) ComboBoxSkipMode.SelectedItem).Tag;
        SetContainersSkipModeVisibility(_autoSkipDataContext.ViewModel.SkipMode);
    }

    private void SetContainersSkipModeVisibility(ESkipMode skipMode)
    {
        switch (skipMode)
        {
            case ESkipMode.Replies:
                ContainerSkipReplies.Visibility = Visibility.Visible;
                ContainerSkipRegular.Visibility = Visibility.Collapsed;
                break;
            case ESkipMode.Everything:
            case ESkipMode.Text:
            default:
                ContainerSkipReplies.Visibility = Visibility.Collapsed;
                ContainerSkipRegular.Visibility = Visibility.Visible;
                break;
        }
    }

    private void ComboBoxSkipStartConditionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _autoSkipDataContext.ViewModel.StartCondition = (ESkipStartCondition) ((ComboBoxItem) ComboBoxSkipStartCondition.SelectedItem).Tag;
    }
    #endregion

    #region Triggers rendering
    private void InitializeTriggers()
    {
        if (_autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Count == 0)
        {
            return;
        }

        UniformGridTriggers.Rows = _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Count;
        UniformGridTriggers.Children.Clear();
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.ForEach(triggerViewModel =>
        {
            var cardDropDown = GenerateCardDropDown(triggerViewModel);
            _cardDropDownButtonsDictionary.Add(cardDropDown, triggerViewModel);
            UniformGridTriggers.Children.Add(cardDropDown);
        });
    }

    private void SetCardActionElements()
    {
        CardButtonAction.Items.Add
        (
            new ComboBoxItem
            {
                Content = new ComboBoxContent
                {
                    TextContent = { Text = Properties.Localization.Resources.TriggerEditor_Edit },
                    Icon = { Data = (Geometry)Application.Current.Resources["I.S.Edit"] }
                },
                MinWidth = 100.0
            }
        );

        CardButtonAction.Items.Add
        (
            new ComboBoxItem
            {
                Content = new ComboBoxContent
                {
                    TextContent =
                    {
                        Text = Properties.Localization.Resources.TriggerEditor_Delete,
                        Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error))
                    },
                    Icon =
                    {
                        Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error)),
                        Data = (Geometry) Application.Current.Resources["I.S.TrashCan"]
                    }
                },
                MinWidth = 100.0
            }
        );

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(((Grid)CardButtonAction.RightIcon).Width + CardButtonAction.ContentPadding.Right, 0, 0, 0),
        };
        _keyCapsService.SetKeyCaps(panel, _autoSkipDataContext.SkipKeys);
        CardButtonAction.BodyContent = panel;

        CardButtonAction.SelectionChanged += CardButtonAction_SelectionChanged;
    }

    private void CardButtonAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cardButton = (CardDropDownWithArtificialFocus)sender;
        var selectedIndex = cardButton.SelectedIndex;
        if (selectedIndex == -1) return;

        switch (selectedIndex)
        {
            case 0:
                cardButton.IsArtificiallyFocused = true;
                _processingCardDropDown = cardButton;
                _processingTrigger = _autoSkipDataContext.SkipKeys;
                StartRecording();
                break;
            case 1:
                _autoSkipDataContext.SkipKeys = new TriggerViewModel();
                ((StackPanel) cardButton.BodyContent).Children.Clear();
                break;
        }

        cardButton.SelectedIndex = -1;
    }

    private CardDropDownWithArtificialFocus GenerateCardDropDown(TriggerViewModel triggerViewModel)
    {
        var cardDropDown = new CardDropDownWithArtificialFocus
        {
            Height = 45.0,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0,0,0,5),
            Items =
            {
                new ComboBoxItem 
                { 
                    Content = new ComboBoxContent
                    {
                        TextContent = { Text = Properties.Localization.Resources.TriggerEditor_MoveUp },
                        Icon = { Data = (Geometry) Application.Current.Resources["I.R.ExpandLess"] }
                    }, 
                    MinWidth = 100.0
                },
                new ComboBoxItem
                {
                    Content = new ComboBoxContent
                    {
                        TextContent = { Text = Properties.Localization.Resources.TriggerEditor_MoveDown },
                        Icon = { Data = (Geometry) Application.Current.Resources["I.R.ExpandMore"] }
                    }
                },
                new ComboBoxItem
                {
                    Content = new ComboBoxContent
                    {
                        TextContent = { Text = Properties.Localization.Resources.TriggerEditor_Edit },
                        Icon = { Data = (Geometry) Application.Current.Resources["I.S.Edit"] }
                    }
                },
                new ComboBoxItem
                {
                    Content = new ComboBoxContent
                    {
                        TextContent =
                        {
                            Text = Properties.Localization.Resources.TriggerEditor_Delete,
                            Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error))
                        },
                        Icon =
                        {
                            Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(WhyOrchid.Properties.Settings.Default.Color_Error)),
                            Data = (Geometry) Application.Current.Resources["I.S.TrashCan"]
                        }
                    }
                }
            },
            RightIcon = new Grid
            {
                Width = WhyOrchid.Properties.Settings.Default.FontStyle_Medium,
                Children =
                {
                    new PathIcon
                    {
                        Width = WhyOrchid.Properties.Settings.Default.FontStyle_Small,
                        Style = (Style) Application.Current.Resources["IconSmall"],
                        Data  = (Geometry) Application.Current.Resources["I.R.ExpandMore"]
                    }
                }
            }
        };

        var panel = new StackPanel
        {
            Orientation         = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin              = new Thickness(((Grid) cardDropDown.RightIcon).Width + cardDropDown.ContentPadding.Right, 0, 0, 0),
        };
        _keyCapsService.SetKeyCaps(panel, triggerViewModel);

        cardDropDown.BodyContent = panel;
        cardDropDown.SelectionChanged += CardDropDown_SelectionChanged;

        return cardDropDown;
    }

    private void CardDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cardButton = (CardDropDownWithArtificialFocus)sender;
        var selectedIndex = cardButton.SelectedIndex;
        if (selectedIndex == -1) return;

        switch (selectedIndex)
        {
            case 0:
                ActionMoveUp(cardButton);
                break;
            case 1:
                ActionMoveDown(cardButton);
                break;
            case 2:
                ActionEdit(cardButton);
                break;
            case 3:
                ActionDelete(cardButton);
                break;
        }

        cardButton.SelectedIndex = -1;
    }
    #endregion

    #region Main actions
    private void Button_AddTrigger_OnClick(object sender, RoutedEventArgs e)
    {
        Keyboard.ClearFocus();

        _isCardButtonPristine = true;

        var triggerViewModel = new TriggerViewModel();
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Add(triggerViewModel);

        var cardDropDown = GenerateCardDropDown(triggerViewModel);
        _cardDropDownButtonsDictionary.Add(cardDropDown, triggerViewModel);

        _processingTrigger = triggerViewModel;
        _processingCardDropDown = cardDropDown;

        UniformGridTriggers.Rows += 1;
        UniformGridTriggers.Children.Add(cardDropDown);
        cardDropDown.IsArtificiallyFocused = true;

        StartRecording();
    }

    private void Button_Save_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        _keyboardHookManagerRecordService.Stop();
        DialogResult = false;
    }

    private void Button_StopRecording_OnClick(object sender, RoutedEventArgs e)
    {
        StopRecording(true);
        ClearProcessingObjects();
    }
    #endregion

    #region PopUp actions
    private void ActionMoveUp(CardDropDownWithArtificialFocus cardButton)
    {
        var index = UniformGridTriggers.Children.IndexOf(cardButton);

        if (index == 0) return;

        var itemToShift = _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels[index];
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.RemoveAt(index);
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Insert(index - 1, itemToShift);

        UniformGridTriggers.Children.RemoveAt(index);
        UniformGridTriggers.Children.Insert(index - 1, cardButton);

        cardButton.IsArtificiallyFocused = false;
    }

    private void ActionMoveDown(CardDropDownWithArtificialFocus cardButton)
    {
        var index = UniformGridTriggers.Children.IndexOf(cardButton);

        if (index == _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Count - 1) return;

        var itemToShift = _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels[index];
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.RemoveAt(index);
        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Insert(index + 1, itemToShift);

        UniformGridTriggers.Children.RemoveAt(index);
        UniformGridTriggers.Children.Insert(index + 1, cardButton);

        cardButton.IsArtificiallyFocused = false;
    }

    private void ActionEdit(CardDropDownWithArtificialFocus cardButton)
    {
        cardButton.IsArtificiallyFocused = true;

        _processingCardDropDown = cardButton;
        _processingTrigger = _cardDropDownButtonsDictionary[cardButton];

        StartRecording();
    }

    private void ActionDelete(CardDropDownWithArtificialFocus cardButton)
    {
        var index = UniformGridTriggers.Children.IndexOf(cardButton);

        _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.RemoveAt(index);
        _cardDropDownButtonsDictionary.Remove(cardButton);
        UniformGridTriggers.Children.RemoveAt(index);

        UniformGridTriggers.Rows = _autoSkipDataContext.ViewModel.ActivationKeys.TriggerViewModels.Count;
    }
    #endregion

    #region Recording
    private void StartRecording()
    {
        if (_isRecording) return;

        _isRecording = true;
        GridRecord.Visibility = Visibility.Visible;
        _mouseHookManagerRecordService.RegisterAllHighLevelKeys();

        KeyDown += TriggerEditWindow_KeyDown;

        _keyboardHookManagerRecordService.OnKeyDown   += KeyboardHookManagerRecordService_OnKeyDown;
        _keyboardHookManagerRecordService.OnKeyUp     += KeyboardHookManagerRecordService_OnKeyUp;
        _mouseHookManagerRecordService.OnMouseKeyDown += MouseHookManagerRecordService_OnMouseKeyDown;


        _keyboardHookManagerRecordService.Start();
        _mouseHookManagerRecordService.Start();
    }

    private void TriggerEditWindow_KeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;
    }

    private void MouseHookManagerRecordService_OnMouseKeyDown(object sender, EHighMouseKey e)
    {
        Dispatcher.Invoke(() => 
        { 
            StopRecording();

            _processingTrigger.KeyCodes = new List<GenericKey> { new MouseKey(e) };
            _processingTrigger.KeyNames = new List<string>     { _keyInterpreterService.InterpretKey(e) };

            _keyCapsService.SetKeyCaps((StackPanel) _processingCardDropDown!.BodyContent, _processingTrigger);
            ClearProcessingObjects();
        });
    }

    private void KeyboardHookManagerRecordService_OnKeyDown(object sender, HashSet<int> e)
    {
        Dispatcher.Invoke(() =>
        {
            var modifierKeys = e.Where(key => _modifierKeysProvider.IsModifierKey(key)).ToList();
            var regularKeys = e.Except(modifierKeys).ToList();

            if (regularKeys.Any())
            {
                var lastRegularKey = regularKeys.Last();

                if (lastRegularKey == KeyInterop.VirtualKeyFromKey(Key.Escape))
                {
                    StopRecording(true);
                    ClearProcessingObjects();
                    return;
                }

                StopRecording();

                var modifierAndRegularKeys = new List<KeyboardKey>(modifierKeys.Select(keyCode => new KeyboardKey(keyCode))) { new(lastRegularKey) };

                _processingTrigger.KeyCodes = new List<GenericKey>(modifierAndRegularKeys);
                _processingTrigger.KeyNames = modifierAndRegularKeys.Select(_keyInterpreterService.InterpretKey).ToList();

                _keyCapsService.SetKeyCaps((StackPanel)_processingCardDropDown!.BodyContent, _processingTrigger);

                ClearProcessingObjects();
                return;
            }

            var highModifierKeys = modifierKeys.Select(keyCode => new KeyboardKey(keyCode)).ToList();

            _processingTrigger.KeyCodes = new List<GenericKey>(highModifierKeys);
            _processingTrigger.KeyNames = highModifierKeys.Select(_keyInterpreterService.InterpretKey).ToList();

            _keyCapsService.SetKeyCaps((StackPanel) _processingCardDropDown!.BodyContent, _processingTrigger);
        });
    }

    private void KeyboardHookManagerRecordService_OnKeyUp(object sender, HashSet<int> e)
    {
        if (!_isRecording) return;

        var modifierKeys = e.Where(key => _modifierKeysProvider.IsModifierKey(key)).ToList();
        var regularKeys = e.Except(modifierKeys).ToList();

        if (regularKeys.Any()) return;

        Dispatcher.Invoke(() =>
        {
            StopRecording();
            ClearProcessingObjects();
        });
    }

    private void ClearProcessingObjects()
    {
        _processingCardDropDown = null;
    }

    private void StopRecording(bool isCanceled = false)
    {
        if (!_isRecording) return;

        _keyboardHookManagerRecordService.Stop();
        _mouseHookManagerRecordService.Stop();

        _keyboardHookManagerRecordService.OnKeyDown   -= KeyboardHookManagerRecordService_OnKeyDown;
        _keyboardHookManagerRecordService.OnKeyUp     -= KeyboardHookManagerRecordService_OnKeyUp;
        _mouseHookManagerRecordService.OnMouseKeyDown -= MouseHookManagerRecordService_OnMouseKeyDown;
        
        _processingCardDropDown!.IsArtificiallyFocused = false;

        if (isCanceled && _isCardButtonPristine)
        {
            ActionDelete(_processingCardDropDown);
            _isCardButtonPristine = false;
        }

        GridRecord.Visibility = Visibility.Collapsed;

        KeyDown -= TriggerEditWindow_KeyDown;
        _isRecording = false;
    }
    #endregion

    #region Cleanup
    private void TriggerEditorWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        StopRecording(true);
        ClearProcessingObjects();
        Unloaded    -= TriggerEditorWindow_Unloaded;
        Activated   -= TriggerEditorWindow_Activated;
        Deactivated -= TriggerEditorWindow_Deactivated;

        foreach (var cardDropDown in _cardDropDownButtonsDictionary.Keys)
        {
            cardDropDown.SelectionChanged -= CardDropDown_SelectionChanged;
        }

        CardButtonAction.SelectionChanged           -= CardButtonAction_SelectionChanged;
        ComboBoxSkipMode.SelectionChanged           -= ComboBoxSkipMode_SelectionChanged;
        ComboBoxSkipStartCondition.SelectionChanged -= ComboBoxSkipStartConditionOnSelectionChanged;
    }
    #endregion
}
