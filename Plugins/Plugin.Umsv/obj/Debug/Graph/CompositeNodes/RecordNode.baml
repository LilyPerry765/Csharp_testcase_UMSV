   M S B A M L   `   `   `  ����K  GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder 5      *    $Pendar.Ums.CompositeNodes.RecordNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9e+clr-namespace:Folder;assembly=Folder.Common     5      $��RightToLeft=�5        � 	Resources  5       	  Group � IsAsynchronous��%�  &	        � 5   
    .  .��5      ��<NodeGroup Description="ضبط پیام" StartNode="PlayNodes[0]" LastNode="PlayNodes[1]" xmlns="http://tempuri.org/VoiceGraph.xsd" Tag="Record">
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
                </NodeGroup>        51      52      .��53      �� .+�54   
   7   1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 55   
   .��6   گره بعدی:6   ;   5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  56   
   $��0,0,0,5q�57         SelectedTarget 56      �� *" NodeData.PlayNodes[1].TargetNode+57      :   4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  58   
      SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+<   6Pendar.Ums.CompositeNodes.UserControls.MailBoxInputBox  59   
   -   5          Name$ boxNoInputBox��59      $
��Left=�5:      $	��100��5;         Text 5<      �� *&$NodeData.InvokeNodes[0].Arg[0].Value+ �� UpdateSourceTrigger$ PropertyChanged=��� 5=   
   .��6   مدت ضبط پيام:6     Folder.NumericUpDown  5>   
   -   5          Name$ durationTextBox��5>      $
��Left=�5@      $	��100��5A         Value 5?      �� *!NodeData.RecordNodes[0].Timeout+$ PropertyChanged=�5A      5B      5C      