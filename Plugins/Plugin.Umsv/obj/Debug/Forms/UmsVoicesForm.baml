   M S B A M L   `   `   `  ����L  HFolder.Audio, Version=3.11.0.17048, Culture=neutral, PublicKeyToken=nullA0clr-namespace:Folder.Audio;assembly=Folder.AudioFolder.Audio  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder 5       Plugin.Umsv   UMSV.UmsVoicesForm   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      >f0clr-namespace:Folder.Audio;assembly=Folder.Audio     5      9c+clr-namespace:Folder;assembly=Folder.Common     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      $��Tahoma)�5   D   $��RightToLeft=�5      .��5	      ]� .+�5
   
   9� -   5        $��
VoicePanel��5
   f   $��Right=�6   $��5q�6*   
�������$��5q�6H     9� 
IsExpanded  . ���#DDE2EB$y�	Collapsed=�6�   .��5      �� .+�5      � $��5q�6   .+�5      �� $��60��6   $��عنوان: ��6'   �� 5      -   5        $��SelectedVoiceName��5   3   $��2q�6   $	��150��6'   
�������6L   5         Folder.Audio.SoundControl  5      -   5          Name$ soundControl��5   I   $
��Left=�6!   $	��5,0q�6<   6_   5      5      9� 5   
   -   5        $��FilterPanel��5   e   $
��Left=�6   $��5q�6)   
�������$��5q�6G     . ���#DDE2EB.��5      �� $	��Top=�6   .+�5      � .+�5      �� $��70��6   $��
عنوان��6'   �� 5      -   5        $��Title��5   5   $��3q�6   $	��150��6)   65   5      � 5      .+�5      �� $��70��6   $��گروه��6'   �� 5      -   5        $��Group��5   6   $��3q�6   $	��150��6*    �� DisplayMemberPath$
 Name��6C    �� SelectedValuePath$ Value��6\   5      � 5      .+�5      �� $��70��6   $��ملاحظات��6'   �� 5      -   5        $��Comment��5   5   $��3q�6   $	��150��6)   65   5       �� 5!      -   5        $��
ViewButton��5!   s   $��0��6   $��2��6)   $��10��6=   $��95��6K   $��23��6V   $��مشاهده��6b   $
��Left=�6�   $��5q�6�   6�   5"      5#      &   System.Windows.Controls.DataGrid  5$   
   -	   5        $��dataGrid��5$      $	��300��6#      Columns 5%      0  *System.Windows.Controls.DataGridTextColumn  5&        Folder.UIElementExtension   VisibilityAccess$* $00000000-0000-0000-0000-000000000000��6<   ,  &System.Windows.Controls.DataGridColumn   Header$ 
شناسه��6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *ID+6�     5'      $ 
عنوان��6>    6%   �� *Name+6>     5(      $ ملاحظات��6E    6%   �� *Description+6E     5)      $ گروه��6D    6%   �� *
VoiceGroup+6D    � ContextMenu 5+      �� 5,      .@�5-      m� -
   5        $��
mitClearDB��5-      $M��Gپاک سازی پایگاه داده از صداهای غیر لازم��61   5.      50      52      53      54      