   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder 5       Plugin.Umsv   UMSV.UserDataList   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4s&clr-namespace:System;assembly=mscorlib     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      9c+clr-namespace:Folder;assembly=Folder.Common     5      $��RightToLeft=�5	      $��Tahoma)�5
      .��5      ]� .+�5   
   9� -   5        $��FilterPanel��5   e   $
��Left=�6   $��5q�6)   
�������$��5q�6G     9� 
IsExpanded  . ���#DDE2EB.��5      �� $	��Top=�6   .+�5      � .+�5      �� $��90��6   $��شروع از��6'   $��3q�69     Folder.DateTimePicker  5      -   5          Name$ FromDate��5   2   $��3q�6'   $	��175��6D   5      � 5      .+�5      �� $��90��6   $��پایان تا��6'   $��3q�6:     5      -   5        $ ToDate��5   2   $��3q�6'   $	��175��6B   5      �� 5      -   5         ��Name$ 
ViewButton��5   V   $	��Top=�6   $��75��6.   $��23��69   $��مشاهده��6E   $
��Left=�6j   $��10q�6�   6�   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dataGrid��5      $
��Auto=�5         AutoGenerateColumns .     CanUserDeleteRows .     CanUserAddRows .     SelectionMode$ Extended=�5         SelectionUnit$ FullRow=�5      	   Columns	 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn
   
IsReadOnly
 . -   5        pollTitleColumn��   Header$ شماره تلفن��5#   %   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5"   %   ��  �� Path$ CallerID�5#   %     5$      
 . $ 
تاريخ��5%   %      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 5&   %    5$   7   �� *
CallTime++  %Folder.Converter.PersianDateConverter   Instance �� 	Converter# Z  �� ConverterParameter$ 
yyyy/MM/dd��5&   %     5'      
 . $ زمان��5(   %    5'   7   �� *
CallTime+ �� StringFormat$ HH:mm��5(   %     5)      
 . $ 
سرویس��5*   %   $	 100 5+   %    5)   7   �� *
Graph.Name+5+   %     5,      
 . $$ اطلاعات وارد شده��5-   %    5,   7   �� *Data+5-   %    � ContextMenu 5/      �� 50      .@�51      m� -   5        $��
ExportMenu��51      $+��%استخراج لیست آیتم ها��61   6O   52      54      55      56      