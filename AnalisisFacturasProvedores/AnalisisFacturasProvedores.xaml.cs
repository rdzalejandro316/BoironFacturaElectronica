using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    /// Sia.PublicarPnt(10795,"AnalisisFacturasProvedores");
    /// Sia.TabU(10795);
    public partial class AnalisisFacturasProvedores : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";



        public AnalisisFacturasProvedores(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Facturas (" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();

                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();

            }
            catch (Exception e)
            {
                MessageBox.Show("error loafcaonfig:" + e);
            }
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    if (string.IsNullOrEmpty(tag)) return;


                    string cmpcodigo = "";
                    string cmpnombre = "";
                    string cmporden = "";
                    string cmpidrow = "";
                    string cmptitulo = "";
                    bool mostrartodo = true;
                    string cmpwhere = "";

                    switch (tag)
                    {
                        case "comae_ter":
                            cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros"; mostrartodo = false;
                            break;
                    }

                    int idr = 0; string code = "";
                    dynamic winb = SiaWin.WindowBuscar(tag, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idemp);
                    winb.Height = 500;
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    winb = null;
                    if (idr > 0)
                    {
                        foreach (FrameworkElement item in GridFilter.Children)
                        {

                            if (item is TextBox)
                            {
                                string tagfor = item.Tag.ToString();
                                if (tagfor == tag) ((TextBox)item).Text = code;
                            }
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error en TextBox_PreviewKeyDown:" + ex.Message.ToString());
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                string tag = ((TextBox)sender).Tag.ToString();
                if (string.IsNullOrEmpty(tag)) return;

                string cmpcodigo = "";

                switch (tag)
                {
                    case "comae_ter": cmpcodigo = "cod_ter"; break;
                }

                DataTable dt = SiaWin.Func.SqlDT("select " + cmpcodigo + " from " + tag + " where " + cmpcodigo + "='" + (sender as TextBox).Text + "';", "temp", idemp);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("el codigo " + (sender as TextBox).Text + " que ingreso no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    (sender as TextBox).Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:" + w);
            }
        }



        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                if (string.IsNullOrEmpty(FecIni.Text))
                {
                    MessageBox.Show("seleccione la fecha inicial a consultar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(FecFin.Text))
                {
                    MessageBox.Show("seleccione la fecha final a consultar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                GridFilter.IsEnabled = false;
                GridFilter.Opacity = 0.5;
                sfBusyIndicator.IsBusy = true;

                #region limpiesa de grillas
                GridFacturas.ItemsSource = null;
                GridFacturas.ClearFilters();
                #endregion

                string ffi = FecIni.Text.ToString();
                string fff = FecFin.Text.ToString();
                string cod_ter = TextBoxTerI.Text.ToString();
                int tipo = CbTipo.SelectedIndex;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, cod_ter, tipo, cod_empresa));
                await slowTask;



                if (slowTask.IsCompleted)
                {
                    if (slowTask.Result != null)
                    {
                        DataTable dt = ((DataSet)slowTask.Result).Tables[0];

                        GridFacturas.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                        GridDetalle.ItemsSource = ((DataSet)slowTask.Result).Tables[1].DefaultView;

                        TabControl1.SelectedIndex = 1;


                        double cantidad = Convert.ToDouble(dt.Compute("Sum(cantidad)", "") == DBNull.Value ? 0 : dt.Compute("Sum(cantidad)", ""));
                        double subtotal = Convert.ToDouble(dt.Compute("Sum(subtotal)", "") == DBNull.Value ? 0 : dt.Compute("Sum(subtotal)", ""));
                        double cos_uni = Convert.ToDouble(dt.Compute("Sum(cos_uni)", "") == DBNull.Value ? 0 : dt.Compute("Sum(cos_uni)", ""));
                        double val_iva = Convert.ToDouble(dt.Compute("Sum(val_iva)", "") == DBNull.Value ? 0 : dt.Compute("Sum(val_iva)", ""));
                        double total = Convert.ToDouble(dt.Compute("Sum(cos_tot)", "") == DBNull.Value ? 0 : dt.Compute("Sum(cos_tot)", ""));
                        llenarTotales(cantidad, subtotal, cos_uni, val_iva, total, ((DataSet)slowTask.Result));


                    }
                }

                sfBusyIndicator.IsBusy = false;
                GridFilter.IsEnabled = true;
                GridFilter.Opacity = 1;
            }
            catch (Exception ex)
            {
                sfBusyIndicator.IsBusy = false;
                GridFilter.IsEnabled = true;
                GridFilter.Opacity = 1;
                MessageBox.Show("errror en la consulta" + ex);
            }
        }

        public void llenarTotales(double cantidad, double subtotal, double cosuni, double iva, double total, DataSet data)
        {
            try
            {

                int n_item = TabControlInfo.Items.Count;

                for (int i = 1; i <= n_item; i++)
                {
                    var Total = (TextBlock)this.FindName("Total" + i);
                    if (Total != null) Total.Text = data.Tables[i - 1].Rows.Count.ToString();

                    var TextCantidad = (TextBlock)this.FindName("TextCantidad" + i);
                    if (TextCantidad != null) TextCantidad.Text = cantidad.ToString("N");

                    var TextSubtotal = (TextBlock)this.FindName("TextSubtotal" + i);
                    if (TextSubtotal != null) TextSubtotal.Text = subtotal.ToString("C");

                    var TextDescuento = (TextBlock)this.FindName("TextCostUnit" + i);
                    if (TextDescuento != null) TextDescuento.Text = cosuni.ToString("C");

                    var TextIva = (TextBlock)this.FindName("TextIva" + i);
                    if (TextIva != null) TextIva.Text = iva.ToString("C");

                    var TextTotal = (TextBlock)this.FindName("TextTotal" + i);
                    if (TextTotal != null) TextTotal.Text = total.ToString("C");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al llenar totales:" + w);
            }


        }


        private DataSet LoadData(string Fi, string Ff, string codter, int tipo, string cod_empresa)
        {

            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpAnalisisFacturasProvedores", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", Fi);
                cmd.Parameters.AddWithValue("@FechaFin", Ff);
                cmd.Parameters.AddWithValue("@codter", codter);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show("error loadata:" + e.Message);
                return null;
            }
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void dataGrid_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            try
            {
                string tag = ((SfDataGrid)sender).Tag.ToString();

                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cantidadX = 0;
                double subtotalX = 0;
                double cosuniX = 0;
                double ivaX = 0;
                double totalX = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {
                    if (provider.GetValue(records[i].Data, "cantidad") != null)
                        cantidadX += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());

                    if (provider.GetValue(records[i].Data, "cos_uni") != null)
                        cosuniX += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_uni").ToString());

                    if (provider.GetValue(records[i].Data, "subtotal") != null)
                        subtotalX += Convert.ToDouble(provider.GetValue(records[i].Data, "subtotal").ToString());


                    if (provider.GetValue(records[i].Data, "val_iva") != null)
                        ivaX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_iva").ToString());

                    if (provider.GetValue(records[i].Data, "cos_tot") != null)
                        totalX += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_tot").ToString());
                }


                var Total = (TextBlock)this.FindName("Total" + tag);
                if (Total != null) Total.Text = (sender as SfDataGrid).View.Records.Count.ToString();

                var TextCantidad = (TextBlock)this.FindName("TextCantidad" + tag);
                if (TextCantidad != null) TextCantidad.Text = cantidadX.ToString("N");

                var TextCosUnit = (TextBlock)this.FindName("TextCostUnit" + tag);
                if (TextCosUnit != null) TextCosUnit.Text = cosuniX.ToString("C");

                var TextSubtotal = (TextBlock)this.FindName("TextSubtotal" + tag);
                if (TextSubtotal != null) TextSubtotal.Text = subtotalX.ToString("C");

                var TextIva = (TextBlock)this.FindName("TextIva" + tag);
                if (TextIva != null) TextIva.Text = ivaX.ToString("C");

                var TextTotal = (TextBlock)this.FindName("TextTotal" + tag);
                if (TextTotal != null) TextTotal.Text = totalX.ToString("C");


            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;

            if (e.ColumnName == "cantidad" || e.ColumnName == "val_iva" || e.ColumnName == "subtotal" || e.ColumnName == "cos_uni" || e.ColumnName == "cos_tot")
            {
                double value = 0;
                if (!string.IsNullOrEmpty(e.CellValue.ToString()))
                {
                    if (double.TryParse(e.CellValue.ToString(), out value))
                    {
                        e.Range.Number = value;
                    }
                    e.Handled = true;
                }
            }


            if (e.ColumnName == "cod_ref" || e.ColumnName == "cod_prv" || e.ColumnName == "cod_trn" || e.ColumnName == "num_trn")
            {
                string value = e.CellValue.ToString();

                e.Range.Text = value;
                e.Handled = true;
            }

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string tag = ((Button)sender).Tag.ToString();
                SfDataGrid sfdg = (SfDataGrid)this.FindName(tag);

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                options.ExcludeColumns.Add("Detalle");

                var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}
