﻿#pragma checksum "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6E9D0D0F05A64C380BF6B9EF79E06F24"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Folder;
using Folder.Converter;
using Pendar.Ums.CompositeNodes.UserControls;
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


namespace Pendar.Ums.CompositeNodes.UserControls {
    
    
    /// <summary>
    /// CodeStatusDialog
    /// </summary>
    public partial class CodeStatusDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 36 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button okButton;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid recordsDataGrid;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem importCsvMenuItem;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem deleteRecordsMenuItem;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid voiceDataGrid;
        
        #line default
        #line hidden
        
        
        #line 177 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem importVoiceMenuItem;
        
        #line default
        #line hidden
        
        
        #line 185 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem deleteVoiceMenuItem;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/graph/compositenodes/usercontrols/codestatusdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
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
            this.okButton = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.okButton.Click += new System.Windows.RoutedEventHandler(this.okButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.recordsDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 3:
            this.importCsvMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 60 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.importCsvMenuItem.Click += new System.Windows.RoutedEventHandler(this.importCsvMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.deleteRecordsMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 68 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.deleteRecordsMenuItem.Click += new System.Windows.RoutedEventHandler(this.deleteRecordsMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.voiceDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 151 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.voiceDataGrid.InitializingNewItem += new System.Windows.Controls.InitializingNewItemEventHandler(this.voiceDataGrid_InitializingNewItem);
            
            #line default
            #line hidden
            return;
            case 7:
            this.importVoiceMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 178 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.importVoiceMenuItem.Click += new System.Windows.RoutedEventHandler(this.importVoiceMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.deleteVoiceMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 186 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            this.deleteVoiceMenuItem.Click += new System.Windows.RoutedEventHandler(this.deleteVoiceMenuItem_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 6:
            
            #line 165 "..\..\..\..\..\Graph\CompositeNodes\UserControls\CodeStatusDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectVoice);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

