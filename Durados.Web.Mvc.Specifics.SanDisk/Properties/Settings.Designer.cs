﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4961
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Durados.Web.Mvc.Specifics.SanDisk.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=durados.info\\sqlexpress;Initial Catalog=SanDisk_allegro_dev;Persist S" +
            "ecurity Info=True;User ID=sa;Password=sa2008")]
        public string SanDisk_allegroConnectionString {
            get {
                return ((string)(this["SanDisk_allegroConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=durados.info\\sqlexpress;Initial Catalog=Sandisk_Allegro_dev;Persist S" +
            "ecurity Info=True;User ID=sandisk;Password=sandisk")]
        public string Sandisk_AllegroConnectionString1 {
            get {
                return ((string)(this["Sandisk_AllegroConnectionString1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=durados.info\\sqlexpress;Initial Catalog=Sandisk_Allegro_dev;Persist S" +
            "ecurity Info=True;User ID=sandisk;Password=sandisk")]
        public string SanDisk_allegroConnectionString2 {
            get {
                return ((string)(this["SanDisk_allegroConnectionString2"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=DEVITOUTSRV1\\SQLEXPRESS;Initial Catalog=SanDisk_Allegro_RMA;Integrate" +
            "d Security=True")]
        public string SanDisk_Allegro_RMAConnectionString {
            get {
                return ((string)(this["SanDisk_Allegro_RMAConnectionString"]));
            }
        }
    }
}