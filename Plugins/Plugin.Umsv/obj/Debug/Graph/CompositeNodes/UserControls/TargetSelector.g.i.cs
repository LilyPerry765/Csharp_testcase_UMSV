﻿#pragma checksum "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0968B9D1136644B34E5E319C19599196"
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
    /// TargetSelector
    /// </summary>
    public partial class TargetSelector : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBoxItem noneComboBoxItem;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBoxItem pickedNodeComboBoxItem;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBoxItem selectorComboBoxItem;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/graph/compositenodes/usercontrols/targetselector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
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
            
            #line 5 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
            ((Pendar.Ums.CompositeNodes.UserControls.TargetSelector)(target)).DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.UserControl_DataContextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.comboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
            this.comboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.comboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.noneComboBoxItem = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 4:
            this.pickedNodeComboBoxItem = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 5:
            this.selectorComboBoxItem = ((System.Windows.Controls.ComboBoxItem)(target));
            
            #line 61 "..\..\..\..\..\Graph\CompositeNodes\UserControls\TargetSelector.xaml"
            this.selectorComboBoxItem.Selected += new System.Windows.RoutedEventHandler(this.selectorComboBoxItem_Selected);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

