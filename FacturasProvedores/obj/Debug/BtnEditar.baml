   M S B A M L   `   `   `  ����M  IFacturasProvedores, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:FacturasProvedoresFacturasProvedores  5   	   "    FacturasProvedores.BtnEditar   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:FacturasProvedores     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfGridConverter.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c896
syncfusion!http://schemas.syncfusion.com/wpf    5   	   -   5          � Title$  
Documentos��5   	   $	��400��6   $	��700��6)    � 	Resources 5
      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value��%�   &	        �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 10(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#3737376+   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��50 �6   �� 5      �� 5      $��40 �6   .+�5   
   � 
���   � � ColumnDefinitions 5      �� 5      $��70 �6#   �� 5      $	��100 �6#   �� 5       $��70 �6#   �� 5!      $	��100 �6#   �� 5"      .+�5&      �� $��
Fecha Ini:��6   $��Center=�6*   $��Right=�6E   
�������$
��Bold"�6t   (  "System.Windows.Controls.DatePicker  5'      -   5        $��TxFecIni��5'      $��1��6)   $��Center=�69   �� 5)      $��2��6   $��
Fecha Fin:��6(   $��Center=�6:   $��Right=�6U   
�������$
��Bold"�6�     5*      -   5        $��TxFecFin��5*      $��3��6)   $��Center=�69   �� 5,      $��4��6    �� Orientation$ 
Horizontal=�6)   $��10,0,0,0q�6B   .+�5-      �� -   5        $��BtnConsultar��5-      $��	Consultar��6-   $	��100��6A   $��30��6M   $��5q�6Y   $
��Left=�6d   ���#3498db���#3498db
�������6�   �� 5.      -   5        $��	BtnChange��5.      $��Cambiar a estado pago��6*    � ToolTip$4 .opcion para cambiar la factura a estado pagado��6J   $	��150��6�   $��30��6�   $��5q�6�   $
��Left=�6�   ���#3498db���#3498db
�������6�   5/      52      � 54   
   $��1��6   .+�55      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5        $��dataGrid��55   $   $��1��64   $��2q�6A   	   EnableDataVirtualization	 . (  "Syncfusion.UI.Xaml.Grid.SfGridBase
   AutoGenerateColumns
 .     ShowGroupDropArea .  $��10(�6�      AllowFiltering .    SelectionMode$ Single=�6�      HeaderStyle# [  6�      Columns 57      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   MappingName$ idreg��60      IsHidden . 6D   0  *Syncfusion.UI.Xaml.Grid.GridDateTimeColumn  58         
HeaderText$ Fecha��64   $ fecha��6I      Width$	 100Q�6]     59      $ 	Documento��60   $ num_trn��6G   $	 100Q�6]     5:      $ Factura��60   $ doc_ref��6E   $	 100Q�6[     5;      $ Nit/CC��60   $ cod_prv��6D   $	 120Q�6Z     5<      $
 Tipo��60   $
 tipo��6B   $	 120Q�6U     5=      $
 Nota��60   $ des_mov��6B   (  "Syncfusion.UI.Xaml.Grid.GridColumn   ColumnSizer$ AutoLastColumnFill=�6X   5?      5@      � 5B   
   $��2��6   
���   �.+�5C      �� $ 
Horizontal=�6   $��Center=�62   $��5q�6M   .+�5D      �� $��Registros :��6   
�������$
��Bold"�6B   �� 5E      -   5        $��TxTotal��5E      
�������$��-��6>   $��2,0,0,0q�6G   5F      �� 5H      $��Right=�6   $ 
Horizontal=�65   .+�5I      �� -   5        $��	BtnSelect��5I      $��Seleccionar��6*   $	��100��6@   $��4��6L   $��30��6\   $��5q�6h   ���#3498db���#3498db
�������6�   5J      5K      5M      5N      