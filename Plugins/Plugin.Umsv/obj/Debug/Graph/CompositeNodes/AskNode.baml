   M S B A M L   `   `   `  ����K  GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=null6+clr-namespace:Folder;assembly=Folder.CommonFolder 5      J5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter 5	      '    !Pendar.Ums.CompositeNodes.AskNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      9f+clr-namespace:Folder;assembly=Folder.Common     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5	      $��RightToLeft=�5        � 	Resources  5       	  Group � IsAsynchronous��%+  &	        � 5   
    .  .��5      ��<NodeGroup Description="پرسش" Tag="Ask" StartNode="InvokeNodes[0]" LastNode="PlayNodes[2]" xmlns="http://tempuri.org/VoiceGraph.xsd">
                    <InvokeNode ID="1" ClearDigits="true" Function="AskCheckMailbox">
                        <Arg Name="boxNo" />
                        <NodeResult Value="true" TargetNode="6" />
                        <NodeResult Value="false" TargetNode="7" />
                    </InvokeNode>
                    <PlayNode ID="6" ClearDigits="true" TargetNode="5" IgnoreKeyPress="false">
                        <Voice Name="AskMessage" Type="0" />
                    </PlayNode>
                    <PlayNode ID="7" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="false">
                        <Voice Name="mailboxFull" Type="0" />
                    </PlayNode>
                    <PlayNode ID="2" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="false">
                        <Voice Name="YourFollowupCodeIs" Type="0" />
                        <Voice Name="[FollowUpCode]" Type="0" />
                        <Voice Name="mibashad" Type="0" />
                    </PlayNode>
                    <InvokeNode ID="3" ClearDigits="true" Function="RecordQuestion">
                        <Arg Name="boxNo" />
                        <NodeResult TargetNode="2" />
                    </InvokeNode>
                    <RecordNode ID="5" ClearDigits="true" Timeout="120" StopKey="*" TargetNode="3" CancelOnDisconnect="false" />
                </NodeGroup>        5F      5G      .��5H      �� .+�5I   
   7   1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5J   
   .��6   گره بعدی:6   ;   5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5K   
   $��0,0,0,5q�5L         SelectedTarget 5K      �� *" NodeData.PlayNodes[2].TargetNode+5L      :   4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5M   
      SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+<   6Pendar.Ums.CompositeNodes.UserControls.MailBoxInputBox  5N   
   -   5          Name$ boxNoInputBox��5N      $
��Left=�5O      $	��100��5P         Text 5Q      `� 5R       `� 	Converter 6   ,  &Folder.Converter.UniformMultiConverter  .1�5S      ��  �� Path$* $NodeData.InvokeNodes[0].Arg[0].Value�6    �� UpdateSourceTrigger$ PropertyChanged=�5T      �� 5U      $* $NodeData.InvokeNodes[1].Arg[0].Value�6   $ PropertyChanged=�5V      5W      5Y      �� 5Z   
   .��6   مدت ضبط پيام:6     Folder.NumericUpDown  5[   
   -   5        	  Name$	 durationTextBox��5[      $
��Left=�5]      $	��100��5^      
   Value
 5\      �� *!NodeData.RecordNodes[0].Timeout+$ PropertyChanged=�5^      5_      5`      