   M S B A M L   `   `   `  ����K  GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls  5      3    -Pendar.Ums.CompositeNodes.RecordToMailBoxNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      $��RightToLeft=�5        � 	Resources  5       	  Group � IsAsynchronous��%�  &	        � 5   
    .  .��5      ��<NodeGroup Description="ضبط در صندوق عمومی" StartNode="PlayNodes[0]" LastNode="PlayNodes[1]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="RecordToMailbox">
                    <PlayNode ID="1" ClearDigits="true" TargetNode="4" IgnoreKeyPress="false">
                        <Voice Name="LeaveMessage" Type="0" />
                    </PlayNode>
                    <InvokeNode ID="3" ClearDigits="true" Function="RecordVoice">
                        <Arg Name="boxNo" />
                        <NodeResult TargetNode="2" />
                    </InvokeNode>
                    <PlayNode ID="2" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="false">
                        <Voice Name="YourMessageSaved" Type="0" />
                    </PlayNode>
                    <RecordNode ID="4" ClearDigits="true" Timeout="120" StopKey="*" TargetNode="3" CancelOnDisconnect="false" />
                </NodeGroup>        50      51      .��52      �� .+�53   
   7   1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 54   
   .��6   گره بعدی:6   ;   5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  55   
   $��0,0,0,5q�56         SelectedTarget 55      �� *" NodeData.PlayNodes[1].TargetNode+56      :   4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  57   
      SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+<   6Pendar.Ums.CompositeNodes.UserControls.MailBoxInputBox  58   
   -   5          Name$ boxNoInputBox��58      $
��Left=�59      $	��100��5:         Text 5;      �� *&$NodeData.InvokeNodes[0].Arg[0].Value+ �� UpdateSourceTrigger$ PropertyChanged=�5<      5=      