using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public abstract class KeyBindingProfileDto
{
    #region Utilities
    public virtual bool IsCursorHideOnManualClick { get; set; } = false;
    public virtual bool IsCycleThrough            { get; set; } = true;
    public virtual ESingleDialogOptionBehaviourDto SingleDialogOptionBehaviourDto { get; set; } = ESingleDialogOptionBehaviourDto.Highlight;
    public virtual ENumericActionBehaviourDto      NumericActionBehaviourDto      { get; set; } = ENumericActionBehaviourDto.Highlight;
    public virtual ECursorBehaviourDto             CursorBehaviourDto             { get; set; } = ECursorBehaviourDto.Hide;
    public virtual EHiddenCursorSettingDto         HiddenCursorSettingDto         { get; set; } = EHiddenCursorSettingDto.YCoordinate;
    #endregion

    #region General
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> PauseResume { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Reload      { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Screenshot  { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> HideCursor  { get; set; } = new(0);
    #endregion

    #region Controls
    public virtual List<List<GenericKeyDto>> Select          { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]              
    public virtual List<List<GenericKeyDto>> Previous        { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]              
    public virtual List<List<GenericKeyDto>> Next            { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]           
    public virtual List<ClickablePointDto>   ClickablePoints { get; set; } = new(0);
    #endregion

    #region Numeric
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> One   { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Two   { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Three { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Four  { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Five  { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Six   { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Seven { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Eight { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Nine  { get; set; } = new(0);
    [XmlArrayItem(ElementName = "ArrayOfKeys")]
    public virtual List<List<GenericKeyDto>> Ten   { get; set; } = new(0);
    #endregion
}
