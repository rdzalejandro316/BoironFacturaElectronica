   M S B A M L   `   `   `  ����O  KPvReimprimeFacturaNC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5      (    "SiasoftAppExt.PvReimprimeFacturaNC   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -localclr-namespace:SiasoftAppExt     5      a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c891SiaSf!http://schemas.syncfusion.com/wpf    5      -   5        $	��600��5	      $
��1100��6     � 	Resources  5      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value  BtnStyle��%  &	        &	 �     �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 11(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#3737376+   5      �� 5   
   " ��6   .�5      ��  � ��
 ����6+   �� 5       � ��
 ����6,   �� 5       � ��
 �����6+   �� 5       � ��$	 100��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   �� 5       �@�Cursor$
 Hand��6'   5      5      .��5      � .+�5   
   �  � RowDefinitions 5      �� 5       $��45 �6    �� 5!      $��* �6    �� 5"      $��40 �6    .+�5%      � $��0��6   
���   � � ColumnDefinitions 5&      �� 5'      $��70 �6'   �� 5(      $	��100 �6'   �� 5)      $	��100 �6'   �� 5*      $��80 �6'   �� 5+      $	��160 �6'   �� 5,      $��60 �6'   �� 5-      $	��120 �6'   �� 5.      $��1* �6'   .+�51      �� $��Fecha:  ��6   $��0��6-   $��0��6:   
�������$��Center=�6]   $��Right=�6x   $��13(�6�   $
��Bold"�6�   (  "System.Windows.Controls.DatePicker  52      -   5          Name$ FechaIni��52      $��0��6/   $��1��6<   $��Center=�6L   6j     53      -   5        $ FechaFin��53      $��0��6/   $��2��6<   $��Center=�6L   6j   �� 55      $��
Tipo Doc: ��6    � ToolTip$ Tipo de documento��6.   $��0��6K   $��3��6X   
�������$��Center=�6{   $��Right=�6�   $��13(�6�   $
��Bold"�6�   �� 57      -   5         ��Name$ 
CmbTipoDoc��57      $��2,5,2,5q�60   $��0��6A   $��4��6O   $	 �� IsSynchronizedWithCurrentItem$
	 TrueY�6�   $��Center=�6�   $��30��6�   
 �� SelectedItem
 6`   �� *SelectedItem+.@�58      �� $��Factura POS��6$   �� 59      $��Factura Credito��6#   �� 5:      $��Nota Credito Total��6#   �� 5;      $��Nota Credito Devolucion��6#   �� 5<      $��
Cotizacion��6#   �� 5=      $��Pedidos��6#   5>      �� 5@      $��# Doc: ��6   $��5��6+   
�������$��Center=�6N   $��Right=�6i   $��13(�6�   $
��Bold"�6�   �� 5A      -   5         ��Name$ 	TxtNumDoc��5A   '   $��0��6   $��25��6:   $��6��6F   $��Center=�6V   $��Center=�6q    �� 	MaxLength$ 12��6�   
���  ��$
��Bold"�6�   �� 5E       �� Orientation$ 
Horizontal=�6   $��7��66   $��10,0,0,0q�6F   .+�5F      �� -   5         ��Name$ Ejecutar��5F      $��Ejecutar��6/   #��[ 6Z   �� 5G      -   5        $ 
ReImprimir��5G      $��Re-Imprimir��61   #��[ 6`   �� 5H      -   5        $ 
BtnRenviar��5H      $��	FE renvio��61   $! FACTURA ELECTRONICA REENVIO��6E   #��[ 6�   5J      5M      � 5O      $��1��6   .+�5P      >  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  -	   5        $��sfBusyIndicator��5P   (   $��5q�6?   $��Center=�6J   $��12(�6e      AnimationSpeed$	 100Q�6s      AnimationType$ Rainy=�6�      Header$! Cargando Datos - SiasoftApp��6�   $	��200��6�      IsBusy .  6�   (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5Q      -
   5          Name$ 
dataGridSF��5Q   $   $��2,2,2,2q�69      EnableDataVirtualization . (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ShowGroupDropArea . $��11(�6�      AllowFiltering . 
���   �   AllowResizingColumns .    HeaderStyle# [  6�      Columns 5S      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$ IdRegCab��6/      MappingName$ idreg��6E      Width$ 50Q�6Y     5T      $
 Tipo��6/   $ cod_trn��6A   $ 40Q�6W     5U      $ 	Documento��6/   $ num_trn��6F   $	 100Q�6\     5V      $ Doc DIAN��6/   $ siasoft_num_trn��6E   $	 100Q�6c   0  *Syncfusion.UI.Xaml.Grid.GridDateTimeColumn  5W         CustomPattern$ dd-MM-yyyy hh:mm:ss��63      Pattern$ CustomPattern=�6W   $ Fecha��6o   $ fec_trn��6�   $	 120Q�6�     5X      $ Nit/cc��6/   $ cod_cli��6C   $	 120Q�6Y     5Y      $ Nombre Cliente��6/   $ nom_cli��6K   $	 350Q�6a   /	  )Syncfusion.UI.Xaml.Grid.GridNumericColumn	  5Z      $ Cantidad��62   $ cantidad��6H   $ 70Q�6_     5[      $ Total��6p       TextAlignment$  Right=�6�   $	 100Q�6�   !   DisplayBinding! 6/   �� " �� Path$" tot_tot�# �� StringFormat$# {0:c}��6�     5^      $ cod_ven��6/   $ cod_ven��6D   $ 50Q�6Z     5_      $ vendedor��6/   $ nom_mer��6E   $	 150Q�6[     5a      $ TrnAnu��6/   $ trn_anu��6C   $ 40Q�6Y     5b      $ NumAnu��6/   $ num_anu��6C   $	 100Q�6Y     5c      $
 Cufe��6/   $ fa_cufe��6A   $	 150Q�6W     5d      $ DocumentoElect��6/   $ fa_docelect��6K   $	 100Q�6e     5f      $ Codigo��6/   $ 	fa_codigo��6C   $ 50Q�6[     5g      $ FechaRespuesta��6/   $ fa_fecharesp��6K   $	 150Q�6f     5h      $	 Msg��6/   $ fa_msg��6@   $	 250Q�6U   5j      5k      � 5l      $��2��6   ���#373737 5m      �� 5n      $
��Auto �6'   �� 5o      $��1* �6'   .+�5r      �� $��Total Documentos : ��6   $��0��67   $
��Left=�6G   $��Center=�6b   $��10,0,0,0q�6}   
�������$
��Bold"�6�   �� 5s      -   5        $ ��Name$$ TextTotalEntradas��5s      $��1��68   $��0��6H   $
��Left=�6Q   $��Center=�6l   
�������6�   5u      5v      5y      5|      