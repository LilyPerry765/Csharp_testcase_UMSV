﻿#pragma checksum "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C9996F5E158B63D279919377E1CFC072"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Folder.Converter;
using Pendar.Ums.Model.Converters;
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
    /// VoiceList
    /// </summary>
    public partial class VoiceList : System.Windows.Controls.ListView, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 96 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem addMenuItem;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem editMenuItem;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem deleteMenuItem;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/graph/compositenodes/usercontrols/voicelist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
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
            
            #line 10 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            ((Pendar.Ums.CompositeNodes.UserControls.VoiceList)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.ListView_Unloaded);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            ((Pendar.Ums.CompositeNodes.UserControls.VoiceList)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.ListView_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.addMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 99 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            this.addMenuItem.Click += new System.Windows.RoutedEventHandler(this.addMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.editMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 104 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            this.editMenuItem.Click += new System.Windows.RoutedEventHandler(this.editMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.deleteMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 109 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            this.deleteMenuItem.Click += new System.Windows.RoutedEventHandler(this.deleteMenuItem_Click);
            
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
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 2:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewMouseDownEvent;
            
            #line 22 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.ListViewItem_PreviewMouseDown);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseEnterEvent;
            
            #line 24 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseEventHandler(this.ListViewItem_MouseEnter);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseMoveEvent;
            
            #line 26 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseEventHandler(this.ListViewItem_MouseMove);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Control.MouseDoubleClickEvent;
            
            #line 28 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.ListViewItem_MouseDoubleClick);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 3:
            
            #line 45 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            ((System.Windows.Controls.ToolBar)(target)).Loaded += new System.Windows.RoutedEventHandler(this.ToolBar_Loaded);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 63 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.addMenuItem_Click);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 70 "..\..\..\..\..\Graph\CompositeNodes\UserControls\VoiceList.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.playButton_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
