﻿#pragma checksum "..\..\..\Forms\TotalCallReport.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F99F4C2DE1B3E971BA0EC8438B96ED06"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Enterprise.Charting.Charting;
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


namespace UMSV.Forms {
    
    
    /// <summary>
    /// TotalCallReport
    /// </summary>
    public partial class TotalCallReport : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander FilterPanel;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.DateTimePicker FromDate;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.DateTimePicker ToDate;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ViewButton;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid infoGrid;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TotalCall;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TotalAnswerd;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DisconnectedCall;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock WaitedCall;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AvgWaitedCall;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MaxWaitedCall;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AVGAnswerdCall;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\Forms\TotalCallReport.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MaxCallTime;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Umsv;component/forms/totalcallreport.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\TotalCallReport.xaml"
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
            this.FilterPanel = ((System.Windows.Controls.Expander)(target));
            return;
            case 2:
            this.FromDate = ((Folder.DateTimePicker)(target));
            return;
            case 3:
            this.ToDate = ((Folder.DateTimePicker)(target));
            return;
            case 4:
            this.ViewButton = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\Forms\TotalCallReport.xaml"
            this.ViewButton.Click += new System.Windows.RoutedEventHandler(this.ViewButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.infoGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.TotalCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.TotalAnswerd = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.DisconnectedCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.WaitedCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.AvgWaitedCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.MaxWaitedCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.AVGAnswerdCall = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.MaxCallTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

