using System.Collections.Generic;

namespace AuroraDialogEnhancerExtensions.KeyBindings;
public abstract class KeyBindingProfileDtoDefault : KeyBindingProfileDto
{
    #region General
    public override List<List<GenericKeyDto>> PauseResume { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(192) } }; // ~
    public override List<List<GenericKeyDto>> Reload      { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(116) } }; // F5
    public override List<List<GenericKeyDto>> Screenshot  { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(123) } }; // F12
    #endregion

    #region Numeric
    public override List<List<GenericKeyDto>> One   { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(49) } }; // 1
    public override List<List<GenericKeyDto>> Two   { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(50) } }; // 2
    public override List<List<GenericKeyDto>> Three { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(51) } }; // 3
    public override List<List<GenericKeyDto>> Four  { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(52) } }; // 4
    public override List<List<GenericKeyDto>> Five  { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(53) } }; // 5
    public override List<List<GenericKeyDto>> Six   { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(54) } }; // 6
    public override List<List<GenericKeyDto>> Seven { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(55) } }; // 7
    public override List<List<GenericKeyDto>> Eight { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(56) } }; // 8
    public override List<List<GenericKeyDto>> Nine  { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(57) } }; // 9
    public override List<List<GenericKeyDto>> Ten   { get; set; } = new() { new List<GenericKeyDto> { new KeyboardKeyDto(48) } }; // 0
    #endregion
}
