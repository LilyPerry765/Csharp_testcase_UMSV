   M S B A M L   `   `   `  ����K  GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter 5      ,    &Pendar.Ums.CompositeNodes.FollowUpNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      $��RightToLeft=�5        � 	Resources  5       	  Group � IsAsynchronous��%�  &	        � 5   
    .  .��5      ��<NodeGroup Description="پیگیری" StartNode="PlayNodes[0]" LastNode="JumpNodes[0]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="FollowUp">
                    <InvokeNode ID="3" ClearDigits="true" Function="CheckFollowupCode">
                        <Arg Name="boxNo" />
                        <NodeResult Value="Answered" TargetNode="5" />
                        <NodeResult Value="InvalidCode" TargetNode="6" />
                        <NodeResult Value="NoAnswerYet" TargetNode="7" />
                        <NodeResult Value="MaxTry" TargetNode="4" />
                    </InvokeNode>
                    <PlayNode ID="1" ClearDigits="true" TargetNode="2" IgnoreKeyPress="false">
                        <Voice Name="GiveYourFollowupCode" Type="0" />
                    </PlayNode>
                    <PlayNode ID="6" ClearDigits="true" TargetNode="1" IgnoreKeyPress="false">
                        <Voice Name="InvalidFollowupCode" Type="0" />
                    </PlayNode>
                    <JumpNode ID="4" ClearDigits="true" TargetNode="Bye" />
                    <PlayNode ID="7" ClearDigits="true" TargetNode="4" IgnoreKeyPress="false">
                        <Voice Name="NoAnswerYet" Type="0" />
                    </PlayNode>
                    <PlayNode ID="5" ClearDigits="true" TargetNode="4" IgnoreKeyPress="false">
                        <Voice Name="[PlayAnswerMessage]" Type="0" />
                    </PlayNode>
                    <GetKeyNode ID="2" ClearDigits="false" Timeout="3" TimeoutNode="3" EndKey="*" MinDigits="0" />
                </NodeGroup>        5I      5J      .��5K      �� .+�5L   
   7   1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5M   
   .��6   گره بعدی:6   ;   5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5N   
   $��0,0,0,5q�5O         SelectedTarget 5N      �� *" NodeData.JumpNodes[0].TargetNode+5O      :   4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5P   
      SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+<   6Pendar.Ums.CompositeNodes.UserControls.MailBoxInputBox  5Q   
   -   5          Name$ boxNoInputBox��5Q      $
��Left=�5R      $	��100��5S         Text 5T      �� *&$NodeData.InvokeNodes[0].Arg[0].Value+ �� UpdateSourceTrigger$ PropertyChanged=�5U      5V      