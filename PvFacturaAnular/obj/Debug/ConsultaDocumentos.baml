   M S B A M L   `   `   `  ����J  FPvFacturaAnular, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PvFacturaAnularPvFacturaAnular  5   	   (    "PvFacturaAnular.ConsultaDocumentos   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c891SiaSf!http://schemas.syncfusion.com/wpf    5   	   -localclr-namespace:PvFacturaAnular    5   	   -   5        $	��500��5   	   $	��850��6     � WindowStartupLocation$  CenterScreen=�69    � 	Resources 5
      3  -Syncfusion.UI.Xaml.Grid.GridHeaderCellControl �� 
TargetType   headerStyle �� Property �� Value��%�   &	        �� 5   
   "  6   .�5      ��  � ��$
 Bold"�6+   �� 5       � ��$ 11(�6)   �� 5       � ��
 �����6+   �� 5       � �� �#bdc3c76+   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��1* �6   �� 5      $��40 �6   .+�5   
   � .+�5      �� $��F5=Selecionar��6   $��Center=�6-   $��10 0 0 0q�6H   ���#2980b9$
��Bold"�6o   5      �� 5   
   $��1��6   
�������$
��Auto��61   
�������$��1,1,1,1q�6U   $��5q�6p   ._�5      �  5       �� 5!      $��1* �6$   .+�5#      >  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  -   5        $��sfBusyIndicator��5#   (   $��5,-99,4.8,5.4q�5$   %   $��Bottom=�5%   %   $��12(�6@      AnimationSpeed$	 100Q�6N      AnimationType$ Rainy=�5&   %      Header$! Cargando Datos - SiasoftApp��6;   $	��200��6`   	   IsBusy	 .  $��3��5'   4   (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5)      -   5        
  Name$
 DataGridDoc��5)   #   (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowResizingColumns .    AutoGenerateColumns .     AllowFiltering . $��5q�6�      HeaderStyle# [  6�      Columns 5+      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Tipo��6/      MappingName$ cod_trn��6A      Width$ 60Q�6W     5,      $ Numero��6/   $ num_trn��6C   $	 120Q�6Y     5-      $ Doc DIAN��6/   $ siasoft_num_trn��6E   $	 120Q�6c     5.      $ Fecha��6/   $ fec_trn��6B   $	 140Q�6X     5/      $ Cliente��6/   $ nom_ter��6D   $	 280Q�6Z     50      $ 70Q�6q   $ Cantidad��6|      TextAlignment$ Right=�6�   (  "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering .     DisplayBinding 6/   ��  �� Path$ cantidad� �� StringFormat$ {0:N2}��6�     51      $	 148Q�6p   $ Total��6|   $ Right=�6�    .   6/   �� $ tot_tot�$ {0:N2}��6�   53      54      55      � 57   
   $��2��6   ���#373737 � ColumnDefinitions 58      �� 59      $
��Auto �6#   �� 5:      $��1* �6#   �� 5;      $��1* �6#   .+�5>      �� $��Total : ��6   $��Center=�6(   $��Right=�6C   $��10 0 0 0q�6_   
�������$
��Bold"�6�   �� 5?      -   5        $��Total��5?      $��1��6%   
�������$��Center=�6H   $
��Left=�6c   �� 5A      -   5        $��BTNcons��5A      $��3��6$   $��2q�64   $��Selecionar (F5)��6?   ���#3498db
������� � Cursor$
 Hand��6�   ��6n   Z� 6�   5B      5C      5F      