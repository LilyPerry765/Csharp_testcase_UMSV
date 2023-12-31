﻿#pragma checksum "..\..\..\Forms\MailboxMessagesForm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6E9A31710195BC779A2D04E11C791493"
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
using UMSV;


namespace Plugin.Mailbox {
    
    
    /// <summary>
    /// MailboxMessagesForm
    /// </summary>
    public partial class MailboxMessagesForm : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander FilterPanel;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox TypeComboxBox;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox BoxNoTextbox;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SenderTextbox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FollowupCodeTextbox;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.DateTimePicker FromDate;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Folder.DateTimePicker ToDate;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CommentTextbox;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ExpiredMessage;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ViewButton;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn boxNoColumn;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn senderColumn;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn followupCodeColumn;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn commentColumn;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ImportMenu;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ImportAnswerMenuItem;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem AddMenu;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem EditMenu;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ConvertToPublic;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ConvertToNew;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem AnswerMenu;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ArchiveMessage;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Forms\MailboxMessagesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ExtractMessage;
        
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
            System.Uri resourceLocater = new System.Uri("/Plugin.Mailbox;component/forms/mailboxmessagesform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\MailboxMessagesForm.xaml"
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
            this.TypeComboxBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.BoxNoTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.SenderTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.FollowupCodeTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.FromDate = ((Folder.DateTimePicker)(target));
            return;
            case 7:
            this.ToDate = ((Folder.DateTimePicker)(target));
            return;
            case 8:
            this.CommentTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.ExpiredMessage = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.ViewButton = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ViewButton.Click += new System.Windows.RoutedEventHandler(this.ViewButton_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.dataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 50 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.dataGrid.ContextMenuOpening += new System.Windows.Controls.ContextMenuEventHandler(this.dataGrid_ContextMenuOpening);
            
            #line default
            #line hidden
            
            #line 55 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.dataGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.dataGrid_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 56 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.dataGrid.InitializingNewItem += new System.Windows.Controls.InitializingNewItemEventHandler(this.dataGrid_InitializingNewItem);
            
            #line default
            #line hidden
            return;
            case 12:
            this.boxNoColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 13:
            this.senderColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 14:
            this.followupCodeColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 15:
            this.commentColumn = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 16:
            this.ImportMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 87 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ImportMenu.Click += new System.Windows.RoutedEventHandler(this.ImportMenu_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            this.ImportAnswerMenuItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 88 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ImportAnswerMenuItem.Click += new System.Windows.RoutedEventHandler(this.ImportAnswerMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            this.AddMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 89 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.AddMenu.Click += new System.Windows.RoutedEventHandler(this.AddItem_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.EditMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 91 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.EditMenu.Click += new System.Windows.RoutedEventHandler(this.EditItem_Click);
            
            #line default
            #line hidden
            return;
            case 20:
            this.ConvertToPublic = ((System.Windows.Controls.MenuItem)(target));
            
            #line 92 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ConvertToPublic.Click += new System.Windows.RoutedEventHandler(this.ConvertToPublic_Click);
            
            #line default
            #line hidden
            return;
            case 21:
            this.ConvertToNew = ((System.Windows.Controls.MenuItem)(target));
            
            #line 93 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ConvertToNew.Click += new System.Windows.RoutedEventHandler(this.ConvertToNew_Click);
            
            #line default
            #line hidden
            return;
            case 22:
            this.AnswerMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 94 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.AnswerMenu.Click += new System.Windows.RoutedEventHandler(this.AnswerItem_Click);
            
            #line default
            #line hidden
            return;
            case 23:
            this.ArchiveMessage = ((System.Windows.Controls.MenuItem)(target));
            
            #line 95 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ArchiveMessage.Click += new System.Windows.RoutedEventHandler(this.ArchiveMessage_Click);
            
            #line default
            #line hidden
            return;
            case 24:
            this.ExtractMessage = ((System.Windows.Controls.MenuItem)(target));
            
            #line 97 "..\..\..\Forms\MailboxMessagesForm.xaml"
            this.ExtractMessage.Click += new System.Windows.RoutedEventHandler(this.ExtractMessage_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

