using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBinding;
public abstract class KeyBindingProfileDefault : KeyBindingProfileDto
{
    #region General
    public override List<List<GenericKey>> PauseResume { get; set; } = new() { new List<GenericKey> { new KeyboardKey(192) } }; // ~
    public override List<List<GenericKey>> Reload      { get; set; } = new() { new List<GenericKey> { new KeyboardKey(116) } }; // F5
    public override List<List<GenericKey>> Screenshot  { get; set; } = new() { new List<GenericKey> { new KeyboardKey(123) } }; // F12
    #endregion

    #region Numeric
    public override List<List<GenericKey>> One   { get; set; } = new() { new List<GenericKey> { new KeyboardKey(49) } }; // 1
    public override List<List<GenericKey>> Two   { get; set; } = new() { new List<GenericKey> { new KeyboardKey(50) } }; // 2
    public override List<List<GenericKey>> Three { get; set; } = new() { new List<GenericKey> { new KeyboardKey(51) } }; // 3
    public override List<List<GenericKey>> Four  { get; set; } = new() { new List<GenericKey> { new KeyboardKey(52) } }; // 4
    public override List<List<GenericKey>> Five  { get; set; } = new() { new List<GenericKey> { new KeyboardKey(53) } }; // 5
    public override List<List<GenericKey>> Six   { get; set; } = new() { new List<GenericKey> { new KeyboardKey(54) } }; // 6
    public override List<List<GenericKey>> Seven { get; set; } = new() { new List<GenericKey> { new KeyboardKey(55) } }; // 7
    public override List<List<GenericKey>> Eight { get; set; } = new() { new List<GenericKey> { new KeyboardKey(56) } }; // 8
    public override List<List<GenericKey>> Nine  { get; set; } = new() { new List<GenericKey> { new KeyboardKey(57) } }; // 9
    public override List<List<GenericKey>> Ten   { get; set; } = new() { new List<GenericKey> { new KeyboardKey(48) } }; // 0
    #endregion
}
