using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models;

[Serializable]
public class KeyBindingProfile
{
    #region Utilities
    public virtual bool IsCursorHideOnManualClick { get; set; } = false;
    public virtual bool IsCycleThrough            { get; set; } = true;
    public virtual ESingleDialogOptionBehaviour SingleDialogOptionBehaviour { get; set; } = ESingleDialogOptionBehaviour.Highlight;
    public virtual ENumericActionBehaviour      NumericActionBehaviour      { get; set; } = ENumericActionBehaviour.Highlight;
    public virtual ECursorBehaviour             CursorBehaviour             { get; set; } = ECursorBehaviour.Hide;
    #endregion

    #region General
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> PauseResume { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Reload      { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Screenshot  { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> HideCursor  { get; set; } = new();
    #endregion

    #region Controls
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Select   { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Previous { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Next     { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")]
    public virtual List<List<GenericKey>> Last     { get; set; } = new();
    #endregion

    #region Interaction Points
    public virtual List<InteractionPoint> InteractionPoints { get; set; } = new();
    #endregion

    #region Scripts
    public virtual AutoSkipConfig AutoSkipConfig { get; set; } = new();
    #endregion

    #region Numeric
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> One   { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Two   { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Three { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Four  { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Five  { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Six   { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Seven { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Eight { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Nine  { get; set; } = new();
    [XmlArrayItem(ElementName = "ListOfKeys")] 
    public virtual List<List<GenericKey>> Ten   { get; set; } = new();
    #endregion
}
