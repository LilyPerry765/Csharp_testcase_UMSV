   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder  5   	   K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=nullclr-namespace:UMSVUMSV 5   	      UMSV.PhoneCallView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   9f+clr-namespace:Folder;assembly=Folder.Common     5   	    cclr-namespace:UMSV     5   	   $��RightToLeft=�5   	     � WindowStartupLocation$  CenterScreen=�5   	   $��Tahoma)�5   	    � Title$$ ارسال پيام تلفنی��5	   	   $	��460��5   	   $	��816��5   	      Folder.DialogResultHelper   DialogResult 5
   	   �� *DialogResult+ � 	Resources 5       �� Source�� 5   
   $P Jpack://application:,,,/Folder.Common;Component/Resources/DefaultTheme.xaml?�6   %    .��5      ]� .+�5   
   ]� $	��Top=�6   $
��5,10q�5      .+�5      �� $
��Left=�6   $��3q�5      عنوان:6+   �� 5      $��3q�6   ��5      �� *Informing.Subject+5      �� 5   
    �� Orientation$ 
Horizontal=�6   $	��Top=�6.   .+�5      �� .��6   دفعات تلاش6       Folder.NumericUpDown  5         Value 6   �� *Informing.RetryCount+5      ]� 5   
   $��Bottom=�6   $��5q�5      $��27��5       5       �� 
TargetType �� Property	 �� Value��%�   '��       �� 5       " ��6   .�5!      ��  � ��$	 Right=�5"      �� 5#       � ��$	 3q�5$      �� 5%       � ��$	 75��5&      5'      5(      .+�5)      �� ��6   �� *SaveCommand+.��66   
اعمال6=   �� 5*      
 �� IsCancel
 . .��6%   انصراف6-   �� 5+       �� 	IsDefault . ��6   �� *	OKCommand+.��5,   &   
تاييد6-   �� 5-      $��Center=�6   $
��10,5q�5/       d� 	IsChecked 5.      �� *Informing.Enabled+.��5/   %   
فعال6+   50      �� 51   
   -   5         ��Name$ InformingTabControl��52      $��5q�51       �� SelectedIndex 53      �� *SelectedTabIndex+.@�54      �� $ ��شماره های تماس��6   .��55        UMSV.InformingRecordsGrid  -   5          Name$ InformingPanel��55   7   $��1��6*   67   56      �� 57      $��زمان بندی��6   .��58        UMSV.SchedulePanel  59      5:      5;      5<      