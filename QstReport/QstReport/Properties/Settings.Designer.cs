﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QstReport.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("L:\\ST\\02-COLLABORATIF\\TSV\\Domaine QST-DO\\_Diffusion interne\\0 - Preparation CR")]
        public string DefaultSavePath {
            get {
                return ((string)(this["DefaultSavePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("QST")]
        public string SIAM_UserName {
            get {
                return ((string)(this["SIAM_UserName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("qstdo")]
        public string SIAM_Password {
            get {
                return ((string)(this["SIAM_Password"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("siam.tech.cana.ri")]
        public string SIAM_HostName {
            get {
                return ((string)(this["SIAM_HostName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("QST")]
        public string EPEIRES_UserName {
            get {
                return ((string)(this["EPEIRES_UserName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("epeires")]
        public string EPEIRES_Password {
            get {
                return ((string)(this["EPEIRES_Password"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("epeires.cdg-lb.aviation")]
        public string EPEIRES_HostName {
            get {
                return ((string)(this["EPEIRES_HostName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("L:\\ST\\02-COLLABORATIF\\TSV\\Domaine QST-DO\\0 - Modèles de références\\Date - Réunion" +
            " Coordination Opérationnelle - base.xls")]
        public string RCO_Model_File {
            get {
                return ((string)(this["RCO_Model_File"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("L:\\ST\\02-COLLABORATIF\\TSV\\Domaine QST-DO\\0 - Modèles de références\\Date - Réunion" +
            " GSST- base.xls")]
        public string GSST_Model_File {
            get {
                return ((string)(this["GSST_Model_File"]));
            }
        }
    }
}
