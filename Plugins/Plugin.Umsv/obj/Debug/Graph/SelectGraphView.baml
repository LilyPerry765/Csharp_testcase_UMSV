   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder  5   	    Plugin.Umsv   UMSV.SelectGraphView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   9f+clr-namespace:Folder;assembly=Folder.Common     5   	   $��RightToLeft=�5   	     � WindowStartupLocation$  CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	   $��Tahoma)�5	   	    � Title$@ :انتخاب درختواره برای ارسال پیام��5
   	   $	��300��5   	   $	��260��5   	      Folder.DialogResultHelper   DialogResult 5   	   �� *DialogResult+ � 	Resources 5       �� Source�� 5   
   $P Jpack://application:,,,/Folder.Common;Component/Resources/DefaultTheme.xaml?�6   %    .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   6.   �� 5      $��* �6   6+    � RowDefinitions 5   
   �� 5      $	��230 �6   6+   �� 5      6   .+�5   
   �� $
��Auto=�6   $
��Auto=�6:   $��RightToLeft=�5      $��7,7,7,7q�63   $��0��6D   $��1��6Q   .��5      �� -   5         ��Name$ GraphsListBox��5      	 �� DisplayMemberPath$
	 Name��5      
 �� SelectedValuePath$
 ID��5      ���	#01000000���	#01000000$��0q�5#       �� SelectedIndex$ 0��5$      ��5      �� *
GraphsList+ �� SelectedValue 5       �� *SelectedGraphIDForInforming+ �� UpdateSourceTrigger$ PropertyChanged=���5%      �� 5&       �� 
TargetType" ��6   .�5'      ��  �� Property � �� �� Value 5(      �� 5)   "   .R�5*   &   $� ���	#01000000 $� Orientation$ Vertical=�6T   5+   #   5-      �� 5.       � �� 5/      �� 50   "   " ��6(   .�51   &   ��  � ��$ 5q�6?   �� 52   &    � �� 53   *   �� 54   .    �� 
TargetType" ��6>   .R�55   2   �� $��0q�69   ���	#01000000._�56   6   	� |�.   J� IsHitTestVisible .  $��Tahoma)�6m    �� 
IsSelected d� 	IsChecked# z 6�   .��57   :   �� 58   7   59   3   5:   /   5<   '   5=   #   5?      �� 5@       � �� 5A      �� 5B   "   " ��62   .R�5C   &   �� ���#dbdbdb$	��0.5q�6C   ���	#01000000._�5D   *   �� $��Bd��6�   $��0q�61    �� Padding$ 0q�6E   ���	#01000000���	#01000000 J� SnapsToDevicePixels . ._�5E   .   �� # z 6=   5F   +   5G   '   5H   #   5J      5K      5M      5N      �� 5P   
   $��
تأیید��6   $��1��6!   $��2��6.   $��23��6B   $��80��6N   ��6Y   �� *CommitGraphCommand+5Q      5R      