﻿#pragma checksum "..\..\..\Forms\TeamsDashboardItem.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C3835BA72EEF17D194E8D7E3D9AAC4F6"
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
    /// TeamsDashboardItem
    /// </summary>
    public partial class TeamsDashboardItem : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\Forms\TeamsDashboardItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox Panel;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Forms\TeamsDashboardItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.SevenSegment OnlineUsers;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Forms\TeamsDashboardItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.SevenSegment QueuedCalls;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Forms\TeamsDashboardItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.SevenSegment TotalDialogs;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Forms\TeamsDashboardItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.SevenSegment DndUsers;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/forms/teamsdashboarditem.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\TeamsDashboardItem.xaml"
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
            
            #line 7 "..\..\..\Forms\TeamsDashboardItem.xaml"
            ((UMSV.TeamsDashboardItem)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Panel = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 3:
            this.OnlineUsers = ((Folder.SevenSegment)(target));
            return;
            case 4:
            this.QueuedCalls = ((Folder.SevenSegment)(target));
            return;
            case 5:
            this.TotalDialogs = ((Folder.SevenSegment)(target));
            return;
            case 6:
            this.DndUsers = ((Folder.SevenSegment)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

