   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder  5      P LPlugin.SoftPhone, Version=13.1.4.39058, Culture=neutral, PublicKeyToken=nullclr-namespace:UMSVUMSV 5         UMSV.SoftPhoneForm   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      9c+clr-namespace:Folder;assembly=Folder.Common     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      !myclr-namespace:UMSV     5      $��LeftToRight=�6,   $��10q�6H   .��5      � -   5        $��	mainPanel��5        � RowDefinitions  5	   
   �� 5
      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
      Folder.JellyButton  -   5        $��UserLoginButton��5      $��1��5      $��Center=�5      $��Center=�5      $	��150��5      $	��150��5         Icon$G Apack://application:,,,/Plugin.UMSV;Component/Images/UserLogin.png��6%   5      (� 5   
   $��0��6   .��5      � -   5         �Name$ 
QueuePanel��5      $y�	Collapsed=�6,   5      5      (� 5   
   $��1��6   .��5       ]� -   5        $��
PhonePanel��5       $y�	Collapsed=�6*   $��Center=�6A   $��Center=�6^   .+�5!      � -   5         �Name$ DashboardGrid��5!      $��1��6.   $	��Top=�6>   $��RightToLeft=�6S   $��0,0,0,5q�6o      Folder.UIElementExtension   VisibilityAccess$* $FE8CDD32-915F-4A1A-8E8F-D24233272D0B��6�    5"      �� 5#      $
��Auto �6+   �� 5$      $��* �6+   �� 5%      $
��Auto �6+   �� 5&      $��* �6+   �� 5'      $
��Auto �6+   �� 5(      $��* �6+   �� 5)      $
��Auto �6+   �� 5*      $��* �6+     5-      �� 5.      .+�51      �� $��0��6   $O��Ipack://application:,,,/Plugin.SoftPhone;Component/Images/onlineUsers1.png��6)   $��32��6|   $��3,0,3,0q�6�    � ToolTip$! کاربران آنلاین��6�   �� 52      $��0��6   $��2��6)   $G��Apack://application:,,,/Plugin.SoftPhone;Component/Images/DND3.png��69   $��33��6�   $��8,0,3,0q�6�   $ DND کاربران��6�   �� 53      $��0��6   $��4��6)   $P��Jpack://application:,,,/Plugin.SoftPhone;Component/Images/currentCalls2.png��69   $��33��6�   $��10,0,3,0q�6�   $ تماس های جاری��6�   �� 54      $��0��6   $��6��6)   $I��Cpack://application:,,,/Plugin.SoftPhone;Component/Images/Queue1.png��69   $��33��6�   $��10,0,3,0q�6�   $ تماس های در صف��6�   �� 56      $��1��6   $
��Left=�6-   
������� �� Padding$	 1.5q�6_   
���   �$��1q�6�   $��60��6�   ._�57         Folder.SevenSegment  -   5        $��SevenSegmentOnlineUsers��57   B   $! کاربران آنلاین��6)   
�������$��1q�6t      DigitsCount$ 3��6�   	   OnColor	 �#FF3030
   OffColor
 �#650000���#3000006�   58      �� 5:      $��0��6   $��3��6*   $
��Left=�6:   
�������$	 1.5q�6l   
���   �$��1q�6�   $��60��6�   ._�5;        -   5        $��SevenSegmentDNDUsers��5;   ?   $ کاربران DND��6)   
�������$��1q�6n   $ 3��6�   	 �#FF3030
 �#650000���#3000006�   5<      �� 5>      $��0��6   $��5��6*   $
��Left=�6:   
�������$	 1.5q�6l   
���   �$��1q�6�   $��60��6�   ._�5?        -   5        $��SevenSegmentCalls��5?   A   $ تماس های جاری��6)   
�������$��1q�6m   $ 3��6�   	 �#FF3030
 �#650000���#3000006�   5@      �� 5B      $��0��6   $��7��6*   $
��Left=�6;   
�������$	 1.5q�6m   
���   �$��1q�6�   $��60��6�   ._�5C        -	   5        $��SevenSegmentQueue��5C   B   $ تماس های در صف��6)   
�������$��1q�6n   $ 3��6�   	 �#FF3030
 �#650000���#3000006�   5D      5F      �� 5G      -
   5        $��DisplayPanel��5G      $ 5q�6-   $	��Top=�69   $��60��6N    �� CornerRadius$ 10��6Z   $��2q�6l   
�������
���   �._�5H      �� $��Center=�6!   .+�5I      �� -   5        $��ActionLabel��5I       
�������63   �� 5J      -   5        $��KeysTextbox��5J   "   $��0q�65   
������ $��30(�6b   $��0,-7,0,0q�6p   $
��Bold"�6�   $��Arial)�6�   
�������6�   5K      5L      �� 5M      -   5        $��KeyPad��5M      $ 20��6'   $��2q�69   
�������
�������._�5N      A�  A� Columns$ 3��6"   $	��240��6.   $	��400��6:   .+�5O        -   5           
EditAccess$* $07898813-C0EF-41AD-A8EE-E91927E42620��5O   (   $H Bpack://application:,,,/Plugin.UMSV;Component/Images/AcceptCall.ico��6o   5P   $     5Q      -   5        $��
NullButton��5Q   (   {�.     DisableColor$ 
WhiteSmoke��6L     5R      -   5        $H Bpack://application:,,,/Plugin.UMSV;Component/Images/RejectCall.ico��5R   (   6r     5S      -   5           Text$ 7��5S   (   61     5T      -   5        $ 8��5T   (   61     5U      -   5        $ 9��5U   (   61     5V      -   5        $ 4��5V   (   61     5W      -   5        $ 5��5W   (   61     5X      -   5        $ 6��5X   (   61     5Y      -   5        $ 1��5Y   (   61     5Z      -   5        $ 2��5Z   (   61     5[      -   5        $ 3��5[   (   61     5\      -   5        $ *��5\   (   61     5]      -   5        $ 0��5]   (   61     5^      -   5        $ #��5^   (   61   5_      5`      �� 5a      $��Right=�6   .+�5c        UMSV.BlackButton  -   5          Name$ LoginButton��5c   %     UMSV.BlackButton   Click$ LogoffButton_Click��5e   !      Text$ Logoff��5d   !   5e   !     UMSV.BlackCheckBox  5f      -   5          Name$ 
HoldButton��5f   '     UMSV.BlackCheckBox   Changed$ HoldButton_Changed��6G      Text$
 HOLD��6;   6G     5g      -   5        $ TransferButton��5g   %   $ TransferButton_Click��6M   $ Transfer��6=   6M     5h      -    5        $ RedialButton��5h   %   $ RedialButton_Click��6I   $ Redial��6;   6I     5i      -!   5        $ AddToBlackListButton��5i   %   $  AddToBlackListButton_Click��6W   $ + Black List��6C   $* $5BE612DF-F546-445F-8484-5E131150BAB8��6z     5j      -"   5        $ 	DndButton��5j   '   $ DndButton_Changed��6E   $	 DND��6:   $* $F08317E2-895A-4363-93A5-D0ABF03F4BB6��6a   5m      5n      5o      5p      5q      