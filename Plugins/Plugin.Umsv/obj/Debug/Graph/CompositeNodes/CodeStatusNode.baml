   M S B A M L   `   `   `  ����K  GPlugin.Umsv, Version=13.1.4.39059, Culture=neutral, PublicKeyToken=null_4clr-namespace:Pendar.Ums.CompositeNodes.UserControls&Pendar.Ums.CompositeNodes.UserControls  5      M IFolder.Common, Version=3.11.0.38536, Culture=neutral, PublicKeyToken=nullJ5clr-namespace:Folder.Converter;assembly=Folder.CommonFolder.Converter 5      .    (Pendar.Ums.CompositeNodes.CodeStatusNode   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cuc4clr-namespace:Pendar.Ums.CompositeNodes.UserControls     5      Fconv5clr-namespace:Folder.Converter;assembly=Folder.Common     5      $��RightToLeft=�5      $��Tahoma)�5
        � 	Resources  5       	  Group � IsAsynchronous��%  &	        � 5   
    .  .��5      ��<NodeGroup Description="پیام مبتنی بر وضعیت" Tag="CodeStatus" StartNode="PlayNodes[0]" LastNode="PlayNodes[1]" xmlns="http://tempuri.org/VoiceGraph.xsd">
                    <PlayNode ID="1" ClearDigits="true" TargetNode="2" IgnoreKeyPress="false">
                        <Voice />
                    </PlayNode>
                    <PlayNode ID="4" ClearDigits="true" TargetNode="Bye" IgnoreKeyPress="true">
                        <Voice Name="[CurrentCodeStatusVoiceMessage]" Type="0" />
                    </PlayNode>
                    <PlayNode ID="5" ClearDigits="true" TargetNode="1" IgnoreKeyPress="false">
                        <Voice />
                    </PlayNode>
                    <GetKeyNode ID="2" ClearDigits="false" Timeout="4" TimeoutNode="3" EndKey="*" MinDigits="0" />
                    <InvokeNode ID="3" ClearDigits="true" Function="CheckCodeStatus">
                        <Arg Name="CodeStatusID" Value="" />
                        <Arg Name="SqlServer" Value="" />
                        <Arg Name="AuthenticationType" Value="0" />
                        <Arg Name="Username" Value="" />
                        <Arg Name="Password" Value="" />
                        <Arg Name="TableName" Value="" />
                        <Arg Name="SpName" Value="" />
                        <Arg Name="CodeField" Value="" />
                        <Arg Name="StatusField" Value="" />
                        <Arg Name="Catalog" Value="" />
                        <NodeResult TargetNode="4" />
                        <NodeResult Value="MaxTry" TargetNode="Bye" />
                        <NodeResult Value="InvalidCode" TargetNode="5" />
                    </InvokeNode>
                </NodeGroup>        5N      5O      .��5P      �� .+�5Q   
   7   1Pendar.Ums.CompositeNodes.UserControls.CommonArea  �� 5R   
   .��6   گره بعدی:6   ;   5Pendar.Ums.CompositeNodes.UserControls.TargetSelector  5S   
   $��0,0,0,5q�5T         SelectedTarget 5S      �� *" NodeData.PlayNodes[1].TargetNode+5T      �� 5U   
   .��6   CAگره بعداز حداکثر دفعات مجاز ورود کد:66     5V   
      IsAlwaysEnabled . $��0,0,0,5q�5X       5V      �� *20NodeData.InvokeNodes[0].NodeResult[1].TargetNode+5X      �� 5Y   
   .��6   " پيام وارد کردن کد:6$   :   4Pendar.Ums.CompositeNodes.UserControls.VoiceSelector  5Z   
      ShowOptions .     SelectedVoice 6   �� * NodeData.PlayNodes[0].Voice[0]+5[      �� 5\   
   .��6    پيام عدم وجود کد:6#     5]   
    .   6   �� * NodeData.PlayNodes[2].Voice[0]+5^      ?   9Pendar.Ums.CompositeNodes.UserControls.CodeStatusSelector  5_   
   $	��0,3q�6       
InvokeNode 5`       �� *NodeData.InvokeNodes[0]+5a      5b      