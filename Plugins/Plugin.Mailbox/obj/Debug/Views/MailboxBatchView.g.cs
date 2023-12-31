﻿#pragma checksum "..\..\..\Views\MailboxBatchView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D5DC7541C685F01678E0183C032CFADE"
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
using Plugin.Mailbox.Assets;
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


namespace Plugin.Mailbox.Views {
    
    
    /// <summary>
    /// MailboxBatchView
    /// </summary>
    public partial class MailboxBatchView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid1;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.PersianDateBox activationDateDatePicker;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.PersianDateBox expirationDateDatePicker;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.NumericUpDown maxArchiveMessageTextBox;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.NumericUpDown maxNewMessageTextBox;
        
        #line default
        #line hidden
        
        
        #line 160 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.NumericUpDown messageAskExpirePeriodTextBox;
        
        #line default
        #line hidden
        
        
        #line 176 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.NumericUpDown messageAnswerExpirePeriodTextBox;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.NumericUpDown messageNewExpirePeriodTextBox;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\..\Views\MailboxBatchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox autoDequeueFullMailboxCheckBox;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Mailbox;component/views/mailboxbatchview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MailboxBatchView.xaml"
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
            this.grid1 = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.activationDateDatePicker = ((Folder.PersianDateBox)(target));
            return;
            case 3:
            this.expirationDateDatePicker = ((Folder.PersianDateBox)(target));
            return;
            case 4:
            this.maxArchiveMessageTextBox = ((Folder.NumericUpDown)(target));
            return;
            case 5:
            this.maxNewMessageTextBox = ((Folder.NumericUpDown)(target));
            return;
            case 6:
            this.messageAskExpirePeriodTextBox = ((Folder.NumericUpDown)(target));
            return;
            case 7:
            this.messageAnswerExpirePeriodTextBox = ((Folder.NumericUpDown)(target));
            return;
            case 8:
            this.messageNewExpirePeriodTextBox = ((Folder.NumericUpDown)(target));
            return;
            case 9:
            this.autoDequeueFullMailboxCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

