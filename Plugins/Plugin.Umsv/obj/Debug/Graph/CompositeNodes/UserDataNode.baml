   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      6+clr-namespace:Folder;assembly=Folder.CommonFolder  5      ,   &Pendar.Ums.CompositeNodes.UserDataNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5      $��RightToLeft=�5	      $��Tahoma)�5
        � 	Resources  5       	  Group � IsAsynchronous��%�  &	        � 5   
    .  .��5      ��<NodeGroup Description="دریافت و ثبت کد" StartNode="PlayNodes[0]" LastNode="InvokeNodes[1]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="UserData">
                    <PlayNode ID="2" ClearDigits="true" TargetNode="3" IgnoreKeyPress="false">
                        <Voice />
                    </PlayNode>
                    <GetKeyNode ID="3" ClearDigits="false" Timeout="15" TimeoutNode="7" MaxDigits="1" MaxDigitsNode="7" EndKey="*" MinDigits="1" />
                    <InvokeNode ID="7" ClearDigits="true" Function="SaveLineDigits">
                        <NodeResult TargetNode="4" />
                    </InvokeNode>
                    <PlayNode ID="4" ClearDigits="true" TargetNode="5" IgnoreKeyPress="true">
                        <Voice Name="Entered Code" Type="0" />
                        <Voice Name="[PlayCode]" Type="0" />
                        <Voice Name="mibashad" Type="0" />
                        <Voice Name="Gerefatn Tayeed" Type="0" />
                    </PlayNode>
                    <GetKeyNode ID="5" ClearDigits="true" MaxDigits="1" MinDigits="1">
                        <NodeResult Value="1" TargetNode="6" />
                        <NodeResult Value="2" TargetNode="2" />
                    </GetKeyNode>
                    <InvokeNode ID="6" ClearDigits="true" Function="SaveUserData">
                        <NodeResult TargetNode="Bye" />
                    </InvokeNode>
                </NodeGroup>        5G      5H      .��5I      �� .+�5J   
   7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5K   
   .��6   گره بعدی:6   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5L   
      SelectedTarget 6   �� *20NodeData.InvokeNodes[1].NodeResult[0].TargetNode+�� 5M   
   .��6   پيام صوتی:6   :  4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5N   
      ShowOptions .     SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+5O      �� 5P   
   -   5         ��Name$ keyStackPanel��5P       � DataContext 5Q      �� *NodeData.GetKeyNodes[0]+.+�5R      �� .��6   #!حداقل تعداد ارقام:6(      Folder.NumericUpDown  5S      $
��Left=�6   $	��100��5U         Value 5T      �� *	MinDigits+5U      �� 5V      .��6   %#حداکثر تعداد ارقام:6)     5W      $
��Left=�6   $	��100��5Y       5X      �� *	MaxDigits+5Y      �� 5Z      .��6   %#حداکثر زمان انتظار:6)     5[      $
��Left=�6   $	��100��5]       5\      �� *	Timeout+5]      �� 5^      .��6   کليد پايان:6!   �� 5_      $
��Left=�6   $	��100��5`       �� SelectedValuePath$ Content��5a      	 �� SelectedValue	 5b      �� *EndKey+.@�5c      �� .��6   #6"   �� 5d      .��6   *6"   5e      5f      5g      5h      