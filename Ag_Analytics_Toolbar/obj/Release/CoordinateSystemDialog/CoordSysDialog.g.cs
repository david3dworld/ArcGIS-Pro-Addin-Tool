﻿#pragma checksum "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F30B170F7D5E54F344813063680402674BCE40D41592FEA351130188948EF325"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ag_Analytics_Toolbar.Common;
using Ag_Analytics_Toolbar.CoordinateSystemDialog;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework.Controls;
using ArcGIS.Desktop.Mapping.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Ag_Analytics_Toolbar.CoordinateSystemDialog {
    
    
    /// <summary>
    /// CoordSysDialog
    /// </summary>
    public partial class CoordSysDialog : ArcGIS.Desktop.Framework.Controls.ProWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 47 "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ShowVCS;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ArcGIS.Desktop.Mapping.Controls.CoordinateSystemsControl CoordinateSystemsControl;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OK;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Ag_Analytics_Toolbar;component/coordinatesystemdialog/coordsysdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ShowVCS = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 2:
            this.CoordinateSystemsControl = ((ArcGIS.Desktop.Mapping.Controls.CoordinateSystemsControl)(target));
            return;
            case 3:
            this.OK = ((System.Windows.Controls.Button)(target));
            
            #line 62 "..\..\..\CoordinateSystemDialog\CoordSysDialog.xaml"
            this.OK.Click += new System.Windows.RoutedEventHandler(this.Close_OnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

