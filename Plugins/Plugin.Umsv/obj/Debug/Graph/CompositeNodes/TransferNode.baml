   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder  5	      ,   &Pendar.Ums.CompositeNodes.TransferNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5	      $��RightToLeft=�5      $��Tahoma)�5
        � 	Resources  5       	  Group � IsAsynchronous �� 
TargetType �� Property �� Value��%-  &	        '�� �    � 5   
    .  .��5      ��<NodeGroup Description="ترانسفر" Tag="Transfer" StartNode="DivertNodes[0]" xmlns="http://tempuri.org/VoiceGraph.xsd">
                    <DivertNode ID="0" ClearDigits="true" Timeout="30" TimeoutNode="DisconnectCall" ProxyMode="false" DeleteFromStart="0" DeleteFromEnd="0" ClearAllSource="false" RecordVoice="false" FailureNode="DisconnectCall" ForwardAnswer="true" />
                </NodeGroup>        5"      �� 5#   
   " ��6   .�5$      ��  � ��$	 0,3q�5%      5&      5'      .��5(      �� .+�5)   
   7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5*   
   .��6   شماره مقصد:6   �� 5+   
   -   5        $��LeftToRight=�5+      ��5,      �� *%#NodeData.DivertNodes[0].TargetPhone+ �� UpdateSourceTrigger$ PropertyChanged=�6l   �� 5-   
   .��6   #!تیم اپراتوری مقصد:6$   �� 5.   
   -   5        $��operatorsList��5.      $
��Left=�6(   $��RightToLeft=�6C   $	��195��6_    �� SelectedValuePath$ ID��6n    �� DisplayMemberPath$
 Name��6�    � DataContext 5/      �� *NodeData.DivertNodes[0]+	 �� SelectedValue	 50      �� *
TargetTeam+
 �� 	Converter
 $   Folder.Converter.GuidConverter  6W   �� 51   
   .��6   %#حداکثر زمان انتظار:6%      Folder.NumericUpDown  52   
   $
��Left=�53      $��70��54         Value 52      �� *!NodeData.DivertNodes[0].Timeout+54      �� 55   
   .��6   موزيک انتظار:6   :  4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  56   
      ShowOptions .     SelectedVoiceID 6   �� *#!NodeData.DivertNodes[0].WaitSound+57      �� 58   
   .��6   پيام انتظار:6     59   
    .   6   �� *%#NodeData.DivertNodes[0].WaitMessage+5:      �� 5;   
   .��6   CAحداکثر نفرات منتظر در صف (0 = نامحدود)68     5<   
   $
��Left=�5=      $��70��5>       5<      �� *#!NodeData.DivertNodes[0].QueueSize+5>      �� 5?   
   .��6   caضریب حداکثر نفرات منتظر به کاربران آنلاین (0 = نامحدود)6I     5@   
      DecimalPlaces$ 1��5A      $
��Left=�5B      $��70��5C       5@      �� *1/NodeData.DivertNodes[0].QueueSizePerOnlineUsers+5C      �� 5D   
   .��6   42گره خطای سپری شدن زمان مجاز:6.   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5E   
   $��0,0,0,5q�5F         SelectedTarget 5E      �� *%#NodeData.DivertNodes[0].TimeoutNode+5F      �� 5G   
   .��6   ,*گره عدم برقراری ارتباط:6)     5H   
   $��0,0,0,5q�5I       5H      �� *%#NodeData.DivertNodes[0].FailureNode+5I      9� 5J   
   $$��تغییر شماره مقصد��6   $��10q�6-   .��5K      �� .+�5L      �� .��6   پيشوند:6!   �� 5M      $��70��6   $
��Left=�5O      ��5N      �� *&$NodeData.DivertNodes[0].CalleePrefix+$ PropertyChanged=�5O      �� 5P      .��6   پسوند:6    �� 5Q      $��70��6   $
��Left=�5S      ��5R      �� *'%NodeData.DivertNodes[0].CalleePostfix+$ PropertyChanged=�5S      �� 5T      .��6   ><تعداد ارقام حذفی از ابتدای شماره:6;     5U      $
��Left=�5V      $��70��5W       5U   "   �� */-NodeData.DivertNodes[0].CalleeDeleteFromStart+5W      �� 5X      .��6   ><تعداد ارقام حذفی از انتهای شماره:6;     5Y      $
��Left=�5Z      $��70��5[       5Y   "   �� *-+NodeData.DivertNodes[0].CalleeDeleteFromEnd+5[      5\      5]      9� 5^   
   $&�� تغییر شماره مبداء��6   $��10q�6.   .��5_      �� .+�5`      ��  d� 	IsChecked 6   �� *(&NodeData.DivertNodes[0].ClearAllSource+.��6Y   '%پاک کردن شماره مبداء6o   �� 5a      .��6   پيشوند:6!   �� 5b      $��70��6   $
��Left=�5d      ��5c      �� *&$NodeData.DivertNodes[0].CallerPrefix+$ PropertyChanged=�5d      �� 5e      .��6   پسوند:6    �� 5f      $��70��6   $
��Left=�5h      ��5g      �� *'%NodeData.DivertNodes[0].CallerPostfix+$ PropertyChanged=�5h      �� 5i      .��6   ><تعداد ارقام حذفی از ابتدای شماره:6;     5j      $
��Left=�5k      $��70��5l       5j   "   �� */-NodeData.DivertNodes[0].CallerDeleteFromStart+5l      �� 5m      .��6   ><تعداد ارقام حذفی از انتهای شماره:6;     5n      $
��Left=�5o      $��70��5p       5n   "   �� *-+NodeData.DivertNodes[0].CallerDeleteFromEnd+5p      5q      5r      5s      5t      