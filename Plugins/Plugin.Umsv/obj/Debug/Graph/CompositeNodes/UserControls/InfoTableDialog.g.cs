﻿#pragma checksum "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AD72FEC784F8D6E3BFC1AD9CE0A83573"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// InfoTableDialog
    /// </summary>
    public partial class InfoTableDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button importButton;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button clearButton;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button okButton;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/graph/compositenodes/usercontrols/infotabledialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
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
            this.importButton = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
            this.importButton.Click += new System.Windows.RoutedEventHandler(this.importButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.clearButton = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
            this.clearButton.Click += new System.Windows.RoutedEventHandler(this.clearButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.okButton = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
            this.okButton.Click += new System.Windows.RoutedEventHandler(this.okButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.dataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            
            #line 72 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.importButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 79 "..\..\..\..\..\Graph\CompositeNodes\UserControls\InfoTableDialog.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.clearButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

