﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AuroraDialogEnhancer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.6.0.0")]
    internal sealed partial class DefaultSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static DefaultSettings defaultInstance = ((DefaultSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DefaultSettings())));
        
        public static DefaultSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("--profile")]
        public string App_StartupArgument_Profile {
            get {
                return ((string)(this["App_StartupArgument_Profile"]));
            }
            set {
                this["App_StartupArgument_Profile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Segoe UI")]
        public string UI_FontStyle_FontFamily {
            get {
                return ((string)(this["UI_FontStyle_FontFamily"]));
            }
            set {
                this["UI_FontStyle_FontFamily"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public double UI_FontStyle_FontSize {
            get {
                return ((double)(this["UI_FontStyle_FontSize"]));
            }
            set {
                this["UI_FontStyle_FontSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#2cb065")]
        public string UI_ThemeInfo_AccentColor {
            get {
                return ((string)(this["UI_ThemeInfo_AccentColor"]));
            }
            set {
                this["UI_ThemeInfo_AccentColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://raw.githubusercontent.com/DigitalE4rth/AuroraDialogEnhancer/update-info/U" +
            "pdateInfo.xml")]
        public string Update_UpdateServerUri {
            get {
                return ((string)(this["Update_UpdateServerUri"]));
            }
            set {
                this["Update_UpdateServerUri"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Gecko/20100101 Firefox/110.0")]
        public string WebClient_UserAgent {
            get {
                return ((string)(this["WebClient_UserAgent"]));
            }
            set {
                this["WebClient_UserAgent"] = value;
            }
        }
    }
}
