   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder  5      -   'Pendar.Ums.CompositeNodes.WebMethodNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5      -   5        $��RightToLeft=�5
      $��Tahoma)�5        � 	Resources  5       	  Group � IsAsynchronous ! BooleanToVisibilityConverter1��%�  &	        &	 �    � 5   
    .  .��5      ��<NodeGroup Description="فراخوانی Web Service" StartNode="InvokeNodes[0]" LastNode="InvokeNodes[0]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="WebMethod">
                    <InvokeNode ID="1" ClearDigits="true">
                        <NodeResult TargetNode="DisconnectCall" />
                    </InvokeNode>
                </NodeGroup>        5      �� 5   
   6'   5       .��5!      �� .+�5"   
   7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5#   
   .��6   گره پيشفرض:6   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5$   
      SelectedTarget 6   �� *20NodeData.InvokeNodes[0].NodeResult[0].TargetNode+�� 5%   
   .��6   آدرس Web Method6!   �� 5&   
   $��LeftToRight=�6   ��5'      �� *" NodeData.InvokeNodes[0].Function+ �� UpdateSourceTrigger$ PropertyChanged=��� 5(   
   .��6   پارامتر ها:6   &   System.Windows.Controls.DataGrid  5)   
   $��70��5*         CanUserResizeRows .     AutoGenerateColumns .     SelectionMode$ Single=�5-         VerticalGridLinesBrush
 �����   HorizontalGridLinesBrush
 �����	   CanUserSortColumns	 .  ��5)      �� *NodeData.InvokeNodes[0].Arg+
   Columns
 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn   Header$ نام��53   %      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 54   %   1  +System.Windows.Controls.DataGridBoundColumn   Binding 52   %   �� *Name+54   %     55      $ 
مقدار��56   %   $ * 57   %    55   %   �� *Value+57   %   59      �� 5:   
   -   5         ��Name$ keyCheckBox��5;      $	��0,3q�5:      .��5=   5   53گرفتن کد قبل از اجرای عمليات6S   �� 5>   
   -   5         ��Name$ recordStackmPanel��5>       � DataContext 5?      �� *NodeData.RecordNodes[0]+y�5@      ��  �� ElementName$ recordCheckBox�� �� Path$ 	IsChecked� �� 	Converter# [ .+�5A      �� .��6   %#حداکثر زمان انتظار:6)      Folder.NumericUpDown  5B      $
��Left=�6   $	��100��5D         Value 5C      �� *	Timeout+5D      �� 5E      .��6   کليد پايان:6!   �� 5F      $
��Left=�6   $	��100��5G       �� SelectedValuePath$ Content��5H       �� SelectedValue 5I      �� *	StopKey+.@�5J      �� $��#��6   �� 5K      $��*��6   �� 5L      $��0��6   �� 5M      $��1��6   �� 5N      $��2��6   �� 5O      $��3��6   �� 5P      $��4��6   �� 5Q      $��5��6   �� 5R      $��6��6   �� 5S      $��7��6   �� 5T      $��8��6   �� 5U      $��9��6   5V      5W      �� 5X   
   -   5        $ keyStackPanel��5X       5Y      �� *NodeData.GetKeyNodes[0]+y�5Z      �� $ keyCheckBox��$ 	IsChecked�# [ .+�5[      �� .��6   #!حداقل تعداد ارقام:6(     5\      $
��Left=�6   $	��100��5^       5]      �� *	MinDigits+5^      �� 5_      .��6   %#حداکثر تعداد ارقام:6)     5`      $
��Left=�6   $	��100��5b       5a      �� *	MaxDigits+5b      �� 5c      .��6   %#حداکثر زمان انتظار:6)     5d      $
��Left=�6   $	��100��5f       5e      �� *	Timeout+5f      �� 5g      .��6   کليد پايان:6!   �� 5h      $
��Left=�6   $	��100��5i      $ Content��5j       5k      �� *EndKey+.@�5l      �� .��6   #6"   �� 5m      .��6   *6"   5n      5o      5p      5q      