   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder  5	      *   $Pendar.Ums.CompositeNodes.DivertNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5	      $��RightToLeft=�5      $��Tahoma)�5
        � 	Resources  5       	  Group � IsAsynchronous �� 
TargetType �� Property �� Value��%�  &	        '�� L    � 5   
    .  .��5      ��<NodeGroup Description="دایورت" Tag="Divert" StartNode="DivertNodes[0]" xmlns="http://tempuri.org/VoiceGraph.xsd">
                    <DivertNode ID="0" ClearDigits="true" Timeout="30" TimeoutNode="Bye" MaxTalkTimeNode="Bye" DeleteFromStart="0" DeleteFromEnd="0" ClearAllSource="false" TargetPhone="" ProxyMode="true" TargetNode="DisconnectCall" RecordVoice="false" BusyNode="Bye" SubscriberAbsentNode="Bye" UnallocatedNumberNode="Bye" FullQueueNode="Bye" FailureNode="Bye" ForwardAnswer="true" />
                </NodeGroup>        5)      �� 5*   
   " ��6   .�5+      ��  � ��$	 0,3q�5,      5-      5.      .��5/      �� .+�50   
   7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 51   
   .��6   شماره مقصد:6   �� 52   
   -   5        $��LeftToRight=�52      ��53      �� *,*NodeData.DivertNodes[0].DynamicTargetPhone+ �� UpdateSourceTrigger$ PropertyChanged=�6s   �� 54   
   .��6   '%شماره اپراتوری مقصد:6&   �� 55   
   -   5        $��LeftToRight=�55      ��56      �� *%#NodeData.DivertNodes[0].TargetPhone+$ PropertyChanged=�6l   �� 57   
   .��6   #!تیم اپراتوری مقصد:6$   �� 58   
   -   5        $��operatorsList��58      $
��Left=�6(   $��RightToLeft=�6C   $	��195��6_    �� SelectedValuePath$ ID��6n    �� DisplayMemberPath$
 Name��6�    � DataContext 59      �� *NodeData.DivertNodes[0]+	 �� SelectedValue	 5:      �� *
TargetTeam+
 �� 	Converter
 $   Folder.Converter.GuidConverter  6W   �� 5;   
   .��6   %#حداکثر زمان انتظار:6%      Folder.NumericUpDown  5<   
   $
��Left=�5=      $��70��5>         Value 5<      �� *!NodeData.DivertNodes[0].Timeout+5>      �� 5?   
   .��6   %#حداکثر زمان مکالمه:6%     5@   
   $
��Left=�5A      $��70��5B       5@      �� *%#NodeData.DivertNodes[0].MaxTalkTime+5B      �� 5C   
   .��6   موزيک انتظار:6   :  4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5D   
      ShowOptions .     SelectedVoiceID 6   �� *#!NodeData.DivertNodes[0].WaitSound+5E      �� 5F   
   .��6   پيام انتظار:6     5G   
    .   6   �� *%#NodeData.DivertNodes[0].WaitMessage+5H      9� 5I   
   $+��%عملیات بعد از دایورت��6   $��10q�61   .��5J      �� .+�5K      �� .��6   " گره بعد از مکالمه:6,   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5L      $��0,0,0,5q�5M         SelectedTarget 5L   $   �� *$"NodeData.DivertNodes[0].TargetNode+5M      �� 5N      .��6   <:گره سپری شدن حداکثر زمان مکالمه:6:     5O      $��0,0,0,5q�5P       5O   $   �� *)'NodeData.DivertNodes[0].MaxTalkTimeNode+5P      �� 5Q      .��6   42گره خطای سپری شدن زمان مجاز:66     5R      $��0,0,0,5q�5S       5R   $   �� *%#NodeData.DivertNodes[0].TimeoutNode+5S      �� 5T      .��6   ,*گره عدم برقراری ارتباط:61     5U      $��0,0,0,5q�5V       5U   $   �� *%#NodeData.DivertNodes[0].FailureNode+5V      �� 5W      .��6   -+گره در صورت اشغالی مقصد:62     5X      $��0,0,0,5q�5Y       5X   $   �� *" NodeData.DivertNodes[0].BusyNode+5Y      �� 5Z      .��6   31گره خاموش بودن موبایل مقصد:65     5[      $��0,0,0,5q�5\       5[   $   �� *.,NodeData.DivertNodes[0].SubscriberAbsentNode+5\      �� 5]      .��6   ,*گره شماره مقصد نامعتبر:61     5^      $��0,0,0,5q�5_       5^   $   �� */-NodeData.DivertNodes[0].UnallocatedNumberNode+5_      �� 5`      .��6   *(گره در صورت پر بودن صف:61     5a      $��0,0,0,5q�5b       5a   $   �� *'%NodeData.DivertNodes[0].FullQueueNode+5b      5c      5d      �� 5e   
   .��6   CAحداکثر نفرات منتظر در صف (0 = نامحدود)68     5f   
   $
��Left=�5g      $��70��5h       5f      �� *#!NodeData.DivertNodes[0].QueueSize+5h      �� 5i   
   .��6   caضریب حداکثر نفرات منتظر به کاربران آنلاین (0 = نامحدود)6I     5j   
      DecimalPlaces$ 1��5k      $
��Left=�5l      $��70��5m       5j      �� *1/NodeData.DivertNodes[0].QueueSizePerOnlineUsers+5m      �� 5n   
    d� 	IsChecked 6   �� *%#NodeData.DivertNodes[0].RecordVoice+.��6N   ضبط مکالمه6Z   9� 5o   
   $$��تغییر شماره مقصد��6   $��10q�6-   .��5p      �� .+�5q      �� .��6   پيشوند:6!   �� 5r      $��70��6   $
��Left=�5t      ��5s      �� *&$NodeData.DivertNodes[0].CalleePrefix+$ PropertyChanged=�5t      �� 5u      .��6   پسوند:6    �� 5v      $��70��6   $
��Left=�5x      ��5w      �� *'%NodeData.DivertNodes[0].CalleePostfix+$ PropertyChanged=�5x      �� 5y      .��6   ><تعداد ارقام حذفی از ابتدای شماره:6;     5z      $
��Left=�5{      $��70��5|       5z   "   �� */-NodeData.DivertNodes[0].CalleeDeleteFromStart+5|      �� 5}      .��6   ><تعداد ارقام حذفی از انتهای شماره:6;     5~      $
��Left=�5      $��70��5�       5~   "   �� *-+NodeData.DivertNodes[0].CalleeDeleteFromEnd+5�      5�      5�      9� 5�   
   $&�� تغییر شماره مبداء��6   $��10q�6.   .��5�      �� .+�5�      ��  6   �� *(&NodeData.DivertNodes[0].ClearAllSource+.��6Y   '%پاک کردن شماره مبداء6o   �� 5�      .��6   پيشوند:6!   �� 5�      $��70��6   $
��Left=�5�      ��5�      �� *&$NodeData.DivertNodes[0].CallerPrefix+$ PropertyChanged=�5�      �� 5�      .��6   پسوند:6    �� 5�      $��70��6   $
��Left=�5�      ��5�      �� *'%NodeData.DivertNodes[0].CallerPostfix+$ PropertyChanged=�5�      �� 5�      .��6   ><تعداد ارقام حذفی از ابتدای شماره:6;     5�      $
��Left=�5�      $��70��5�       5�   "   �� */-NodeData.DivertNodes[0].CallerDeleteFromStart+5�      �� 5�      .��6   ><تعداد ارقام حذفی از انتهای شماره:6;     5�      $
��Left=�5�      $��70��5�       5�   "   �� *-+NodeData.DivertNodes[0].CallerDeleteFromEnd+5�      5�      5�      5�      5�      