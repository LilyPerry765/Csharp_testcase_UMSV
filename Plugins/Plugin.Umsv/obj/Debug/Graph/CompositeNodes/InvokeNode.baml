   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder  5      *   $Pendar.Ums.CompositeNodes.InvokeNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5      -   5        $��RightToLeft=�5
      $��Tahoma)�5        � 	Resources  5       	  Group � IsAsynchronous ! BooleanToVisibilityConverter1��%�  &	        &	 �    � 5   
    .  .��5      ��<NodeGroup Description="عملیات داخلی" StartNode="InvokeNodes[0]" LastNode="InvokeNodes[0]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="Invoke">
                    <InvokeNode ID="1" ClearDigits="true">
                        <NodeResult TargetNode="DisconnectCall" />
                    </InvokeNode>
                </NodeGroup>        5      �� 5   
   6'   5       .��5!      �� .+�5"   
   7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5#   
   .��6   گره پيشفرض:6   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5$   
      SelectedTarget 6   �� *20NodeData.InvokeNodes[0].NodeResult[0].TargetNode+�� 5%   
   .��6   نام عمليات:6   �� 5&   
   $��LeftToRight=�6   ��5'      �� *" NodeData.InvokeNodes[0].Function+ �� UpdateSourceTrigger$ PropertyChanged=��� 5(   
   .��6   پارامتر ها:6   &   System.Windows.Controls.DataGrid  5)   
   $��70��5*         CanUserResizeRows .     AutoGenerateColumns .     SelectionMode$ Single=�5-         VerticalGridLinesBrush
 �����   HorizontalGridLinesBrush
 �����	   CanUserSortColumns	 .  ��5)      �� *NodeData.InvokeNodes[0].Arg+
   Columns
 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn   Header$ نام��53   %      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 54   %   1  +System.Windows.Controls.DataGridBoundColumn   Binding 52   %   �� *Name+54   %     55      $ 
مقدار��56   %   $ * 57   %    55   %   �� *Value+57   %   59      �� 5:   
   -   5         ��Name$ keyCheckBox��5;      $	��0,3q�5:      .��5=   5   53گرفتن کد قبل از اجرای عمليات6S   �� 5>   
   -   5        $ recordCheckBox��5A      $	��0,3q�5>      .��5A   +   31ضبط صدا قبل از اجرای عمليات6H   �� 5B   
   -   5         ��Name$ recordStackmPanel��5B       � DataContext 5C      �� *NodeData.RecordNodes[0]+y�5D      ��  �� ElementName$ recordCheckBox�� �� Path$ 	IsChecked� �� 	Converter# [ .+�5E      �� .��6   %#حداکثر زمان انتظار:6)      Folder.NumericUpDown  5F      $
��Left=�6   $	��100��5H         Value 5G      �� *	Timeout+5H      �� 5I      .��6   کليد پايان:6!   �� 5J      $
��Left=�6   $	��100��5K       �� SelectedValuePath$ Content��5L       �� SelectedValue 5M      �� *	StopKey+.@�5N      �� $��#��6   �� 5O      $��*��6   �� 5P      $��0��6   �� 5Q      $��1��6   �� 5R      $��2��6   �� 5S      $��3��6   �� 5T      $��4��6   �� 5U      $��5��6   �� 5V      $��6��6   �� 5W      $��7��6   �� 5X      $��8��6   �� 5Y      $��9��6   5Z      5[      �� 5\   
   -   5        $ keyStackPanel��5\       5]      �� *NodeData.GetKeyNodes[0]+y�5^      �� $ keyCheckBox��$ 	IsChecked�# [ .+�5_      �� .��6   #!حداقل تعداد ارقام:6(     5`      $
��Left=�6   $	��100��5b       5a      �� *	MinDigits+5b      �� 5c      .��6   %#حداکثر تعداد ارقام:6)     5d      $
��Left=�6   $	��100��5f       5e      �� *	MaxDigits+5f      �� 5g      .��6   %#حداکثر زمان انتظار:6)     5h      $
��Left=�6   $	��100��5j       5i      �� *	Timeout+5j      �� 5k      .��6   کليد پايان:6!   �� 5l      $
��Left=�6   $	��100��5m      $ Content��5n       5o      �� *EndKey+.@�5p      �� .��6   #6"   �� 5q      .��6   *6"   5r      5s      5t      5u      