﻿#pragma checksum "..\..\..\Forms\UmsVoicesForm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EFDD27584DFB3C3940B41EE71EF17E38"
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
using Folder.Audio;
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


namespace UMSV {
    
    
    /// <summary>
    /// UmsVoicesForm
    /// </summary>
    public partial class UmsVoicesForm : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander VoicePanel;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label SelectedVoiceName;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.Audio.SoundControl soundControl;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander FilterPanel;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Title;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Group;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Comment;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ViewButton;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Forms\UmsVoicesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mitClearDB;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/forms/umsvoicesform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\UmsVoicesForm.xaml"
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
            this.VoicePanel = ((System.Windows.Controls.Expander)(target));
            return;
            case 2:
            this.SelectedVoiceName = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.soundControl = ((Folder.Audio.SoundControl)(target));
            
            #line 16 "..\..\..\Forms\UmsVoicesForm.xaml"
            this.soundControl.VoiceChanged += new System.EventHandler<Folder.Audio.VoiceChangedEventArgs>(this.soundControl_VoiceChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.FilterPanel = ((System.Windows.Controls.Expander)(target));
            return;
            case 5:
            this.Title = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.Group = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.Comment = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.ViewButton = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\Forms\UmsVoicesForm.xaml"
            this.ViewButton.Click += new System.Windows.RoutedEventHandler(this.ViewButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.dataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 36 "..\..\..\Forms\UmsVoicesForm.xaml"
            this.dataGrid.SelectedCellsChanged += new System.Windows.Controls.SelectedCellsChangedEventHandler(this.dataGrid_SelectedCellsChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.mitClearDB = ((System.Windows.Controls.MenuItem)(target));
            
            #line 46 "..\..\..\Forms\UmsVoicesForm.xaml"
            this.mitClearDB.Click += new System.Windows.RoutedEventHandler(this.mitClearDB_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

