   M S B A M L   `   `   `  ����L  HFolder.Audio, Version=3.11.0.17048, Culture=neutral, PublicKeyToken=nullA0clr-namespace:Folder.Audio;assembly=Folder.AudioFolder.Audio  5   	    Plugin.Mailbox!   Plugin.Mailbox.AnswerDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   >f0clr-namespace:Folder.Audio;assembly=Folder.Audio     5   	   -   5        $��Tahoma)�5   	   $��RightToLeft=�5   	     � Title$  پاسخ به پرسش��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5	   	    � SizeToContent$ WidthAndHeight=�6    � Topmost .  � ShowInTaskbar .  .��5      ]� .+�5   
   �� $��Bottom=�6   $��40��5      ���	#FFF5F5F5$��LeftToRight=�5       �� Orientation$ 
Horizontal=�5       � 	Resources 5       �� 
TargetType	 �� Property
 �� Value��%�   '��       �� 5      " ��6   .�5      �� 	 � ��$
 70��5      �� 5      	 � ��$
 21��5      �� 5      	 � ��$
 5,0,0,0q�5      5      5      .+�5      ��  �� IsCancel . .��6%   انصراف6-   �� 5      -   5         ��Name$ okButton��5       �� 	IsDefault . .��5   ,   
تاييد63   5      �� 5    
   .+�5!      � $��5q�6   .+�5"      �� .��6   #!شماره رهگیری پرسش:6,   �� 5#      -   5        $��FolloupCode��5#       �� 
IsReadOnly . $	��100��6?   6M   5$      � 5%      $��5q�6   .+�5&      �� .��6   پیغام صوتی:6%      Folder.Audio.SoundControl  5'      -   5          Name$ soundControl��5(      $
��Left=�5'   !   5(      5)      �� 5*      -   5        $��	RemoveAsk��5*   4   $��5q�6    d� 	IsChecked$
 TrueY�6#   .��6E   حذف پرسش6O   � 5+      $��5q�6   .+�5,      �� .��6   -+زمان اعتبار پیام (دقیقه)62   �� 5-      -   5        $��Expire��5-      $	��100��6(   66   5.      5/      50      51      