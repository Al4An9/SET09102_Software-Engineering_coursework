   M S B A M L   `   `   `  ����@  <NBMFS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:NBMFS.ViewsNBMFS.Views  5   	   +    %NBMFS.Views.ShowAllMessagesViewModels   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:NBMFS.Views     5   	     � Title$  ShowAllMessagesViewModels��5   	   $	��450��6+   $	��800��68   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��* �6   �� 5      $��5* �6   .+�5   
   ]� $��0��6   $	��5 5q�6$   .+�5      �� .+�5      �� $��2 2 2 10q�6o   ��6   �� *ShowSmsButtonText+��6?   �� *ShowSmsMessageButtonCommand+6o   �� 5      $��2 2 2 10q�6w   ��6   �� *ShowTwitterButtonText+��6C   �� *!ShowTwitterMessageButtonCommand+6w   �� 5      $��2 2 2 10q�6s   ��6   �� *ShowEmailButtonText+��6A   �� *ShowEmailMessageButtonCommand+6s   �� 5      $��2 2 2 10q�6o   ��6   �� *ShowSirButtonText+��6?   �� *ShowSirMessageButtonCommand+6o   5      5      &   System.Windows.Controls.DataGrid  5   
   $��1��6   $
��Left=�6#   $	��284��6>   $��	92,35,0,0q�6K   $	��Top=�6^   $	��440��6v      
IsReadOnly .    AutoGenerateColumns .  ��5   %   ��  �� Path$ MessageList� �� Mode$ OneWay=� �� NotifyOnSourceUpdated .  �� NotifyOnTargetUpdated .    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Type��6%   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 63   �� $ MType�  5      $	 Header��6%   
 65   �� $ Header�  5      $	 Subject��6%   
 66   �� $ Subject�  5       $	 Sender��6%   
 65   �� $ Sender�  5!      $
	 Body��6%   
 63   �� $
 Body�5#      5%      5&      