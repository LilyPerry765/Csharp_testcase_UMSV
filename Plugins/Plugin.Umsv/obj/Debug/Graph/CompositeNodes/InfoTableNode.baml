   M S B A M L   `   `   `  ����M  IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter  5      K GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls 5      -   'Pendar.Ums.CompositeNodes.InfoTableNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      -   5        $��RightToLeft=�5	      $��Tahoma)�5
        � 	Resources  5       	  Group � IsAsynchronous ! BooleanToVisibilityConverter1��%�  &	        &	 �    � 5   
    .  .��5      ��<NodeGroup Description="جدول اطلاعات گویا" StartNode="InvokeNodes[0]" LastNode="PlayNodes[2]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="InfoTable">
                    <InvokeNode ID="1" ClearDigits="true" Function="CheckAuthentication">
                        <Arg Name="AuthenticationType" Value="CallerID" />
                        <Arg Name="InfoTable" />
                        <Arg Name="Repeat" Value="False" />
                        <NodeResult Value="RecordFound" TargetNode="2" />
                        <NodeResult Value="RecordNotFound" TargetNode="3" />
                        <NodeResult Value="AskForCode" TargetNode="4" />
                    </InvokeNode>
                    <PlayNode ID="3" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="false">
                        <Voice Name="PlayRecordNotFound" Type="0" />
                    </PlayNode>
                    <PlayNode ID="4" ClearDigits="true" TargetNode="6" IgnoreKeyPress="false">
                        <Voice Name="EnterYourCode" Type="0" />
                    </PlayNode>
                    <GetKeyNode ID="6" ClearDigits="true" Timeout="4" TimeoutNode="5" MaxDigits="15" MaxDigitsNode="5" EndKey="#" MinDigits="5" />
                    <InvokeNode ID="5" ClearDigits="true" Function="CheckInfoTableCode">
                        <NodeResult Value="RecordFound" TargetNode="2" />
                        <NodeResult Value="RecordNotFound" TargetNode="3" />
                    </InvokeNode>
                    <PlayNode ID="2" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="false" />
                </NodeGroup>        5H      �� 5I   
   6'   5J      .��5K      ]� .+�5L   
   �� $	��Top=�6   .+�5M      7  1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5N      .��6   گره بعدی:6   ;  5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5O         SelectedTarget 6    �� *" NodeData.PlayNodes[2].TargetNode+>  8Pendar.Ums.CompositeNodes.UserControls.InfoTableSelector  5P      -   5          Name$ infoTableSelector��5P   #   $	��0,3q�6>      SelectedInfoTableID 5Q   #   �� *&$NodeData.InvokeNodes[0].Arg[1].Value+	� 5R      $	��0,3q�6    d� 	IsChecked 5S      �� *&$NodeData.InvokeNodes[0].Arg[0].Value+ �� 	Converter "   Folder.Converter.IfConverter   �� ConverterParameter +   %Folder.Converter.IfConverterParameter     	TrueValue$ CallerID��	   
FalseValue$	 ByCode��.��6�   (&شناسايی از طريق Caller ID6�   	� 5T      -   5        
 	�Name$
 codeRadioButton��5T      $	��0,3q�5V       5U      �� *&$NodeData.InvokeNodes[0].Arg[0].Value+      $ ByCode��$	 CallerID��.��5V   '   #!شناسايی از طريق کد6;   �� 5W      y�6   ��  �� ElementName$ codeRadioButton��# [  �� Path$ 	IsChecked�.+�5X      �� .��6   " پيام وارد کردن کد:6,   :  4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5Y         ShowOptions .     SelectedVoice 5Z   #   �� * NodeData.PlayNodes[1].Voice[0]+5[      �� 5\      .��6   &$پيام عدم وجود رکورد:6*     5]       .   5^      �� * NodeData.PlayNodes[0].Voice[0]+5_      �� 5`   
   $��پيام ها��6   .��5a      ]� .+�5b      �� $��Bottom=�6   $	��0,3q�5d       5c      �� *&$NodeData.InvokeNodes[0].Arg[2].Value+.��5d   (   )'تکرار پيام خوانده شده6?   6  0Pendar.Ums.CompositeNodes.UserControls.VoiceList  5e         IsForInfoTable . ��5f      �� *NodeData.PlayNodes[2].Voice+5g      5h      5y      5z      