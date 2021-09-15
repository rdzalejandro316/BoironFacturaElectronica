using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using AnalisisDeVenta;
using System.Text;

namespace SiasoftAppExt
{
    /// Sia.PublicarPnt(9301,"AnalisisDeVenta");
    /// Sia.TabU(9301);
    public partial class AnalisisDeVenta : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        int moduloid = 0;
        //        string codbod = "";
        string cnEmp = "";
        string cod_empresa = "";

        public AnalisisDeVenta(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            //idemp = SiaWin._BusinessId;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;

            LoadConfig();
            //tabitem.VisibleButtonClose=false;

            // Border1.Height = Application.Current.MainWindow.ActualHeight-150;
            //this.Height = SiaWin.Height-5;

        }

        private void LoadConfig()
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Venta(" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                //GroupId = 0;
                //ProjectId = 0;
                //BusinessId = 0;
                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();

                TabControl1.SelectedIndex = 0;
                int grupo = SiaWin._UserGroup;
                string cod_grupo = "";

                DataTable dtGrupo = SiaWin.Func.SqlDT("select* from Seg_Group where GroupId = '" + grupo + "'", "Cuentas", 0);
                if (dtGrupo.Rows.Count > 0) cod_grupo = dtGrupo.Rows[0]["GroupCode"].ToString();

                if (!string.IsNullOrEmpty(cod_grupo))
                {
                    bool flag = false;
                    DataTable dtGrupoRango = SiaWin.Func.SqlDT(" select * from Seg_Group where GroupCode between '050' and '060'", "Cuentas", 0);
                    foreach (System.Data.DataRow dr in dtGrupoRango.Rows)
                    {
                        if (dr["GroupCode"].ToString().Trim() == cod_grupo) flag = true;
                    }

                    if (flag)
                        TextBoxVenI.IsEnabled = true;
                }


                string tag2 = SiaWin._UserTag2;
                if (!String.IsNullOrEmpty(tag2))
                {
                    DataTable dt = SiaWin.Func.SqlDT("select * from inmae_mer where cod_mer='" + tag2 + "'", "Cuentas", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        if (SiaWin._UserId != 21)
                        {
                            TextBoxVenI.Text = dt.Rows[0]["cod_mer"].ToString();
                            TextBoxVenI.IsEnabled = false;
                        }

                    }
                    else
                        TextBoxVenI.IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("aqui88");


            }
        }



        private string ArmaWhere()
        {
            string cadenawhere = null;
            string RefI = TextBoxRefI.Text.Trim();
            string RefF = TextBoxRefF.Text.Trim();
            string BodI = TextBoxBodI.Text.Trim();
            string BodF = TextBoxBodF.Text.Trim();
            string TerI = TextBoxTerI.Text.Trim();
            string VenI = TextBoxVenI.Text.Trim();
            string TipI = TextBoxTipI.Text.Trim();
            string TipF = TextBoxTipF.Text.Trim();
            string GruI = TextBoxGrpI.Text.Trim();
            string GruF = TextBoxGrpF.Text.Trim();

            //string ImpI = TextBoxImpI.Text.Trim();
            if (!string.IsNullOrEmpty(RefI) && !string.IsNullOrEmpty(RefF))
            {
                cadenawhere += " and  cue.cod_ref between '" + RefI + "' and '" + RefF + "'";
            }
            if (!string.IsNullOrEmpty(BodI) && !string.IsNullOrEmpty(BodF))
            {
                cadenawhere += " and  cue.cod_bod between '" + BodI + "' and '" + BodF + "'";
            }
            if (!string.IsNullOrEmpty(TerI))
            {
                cadenawhere += " and  cab.cod_cli='" + TerI + "'";
            }
            if (!string.IsNullOrEmpty(VenI))
            {
                cadenawhere += " and  cab.cod_Ven='" + VenI + "'";
            }
            if (!string.IsNullOrEmpty(TipI) && !string.IsNullOrEmpty(TipF))
            {
                cadenawhere += " and  ref.cod_tip between '" + TipI + "' and '" + TipF + "'";
            }
            if (!string.IsNullOrEmpty(GruI) && !string.IsNullOrEmpty(GruF))
            {
                cadenawhere += " and  ref.cod_gru between '" + GruI + "' and '" + GruF + "'";
            }

            //if (!string.IsNullOrEmpty(ImpI))
            //{
            //    cadenawhere += " and  ref.im='" + ImpI + "'";
            //}


            return cadenawhere;
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0.5;
            try
            {
                string where = ArmaWhere();

                if (string.IsNullOrEmpty(where)) where = " ";


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                VentasPorProducto.ItemsSource = null;
                VentaPorBodega.ItemsSource = null;
                VentasPorCliente.ItemsSource = null;
                VentasPorLinea.ItemsSource = null;
                VentasPorGrupo.ItemsSource = null;

                CharVentasBodega.DataContext = null;
                AreaSeriesVta.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;
                source.CancelAfter(TimeSpan.FromSeconds(1));
                //tabitem.Progreso(true);
                string ffi = FecIni.Text.ToString();
                string fff = FecFin.Text.ToString();

                

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff,  where, cod_empresa));
                await slowTask;
                //MessageBox.Show(slowTask.Result.ToString());
                BtnEjecutar.IsEnabled = true;
                //tabitem.Progreso(false);
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    VentasPorProducto.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    VentaPorBodega.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    Total2.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();

                    CharVentasBodega.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    AreaSeriesVta.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    VentasPorCliente.ItemsSource = ((DataSet)slowTask.Result).Tables[2];
                    Total3.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();

                    VentasPorVendedor.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    Total4.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();

                    VentasPorLinea.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    Total5.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();

                    VentasPorGrupo.ItemsSource = ((DataSet)slowTask.Result).Tables[5];
                    Total6.Text = ((DataSet)slowTask.Result).Tables[5].Rows.Count.ToString();

                    VentasPorFPago.ItemsSource = ((DataSet)slowTask.Result).Tables[6];
                    Total7.Text = ((DataSet)slowTask.Result).Tables[6].Rows.Count.ToString();

                    VentasPorClienteRef.ItemsSource = ((DataSet)slowTask.Result).Tables[7];
                    Total8.Text = ((DataSet)slowTask.Result).Tables[7].Rows.Count.ToString();


                    GridDocumen.ItemsSource = ((DataSet)slowTask.Result).Tables[8];
                    Total9.Text = ((DataSet)slowTask.Result).Tables[8].Rows.Count.ToString();


                    dataGridFP_detallado.ItemsSource = ((DataSet)slowTask.Result).Tables[9];


                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                    //TABLA 0
                    //double CantNeto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(neto)", "").ToString());
                    //double sub = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "").ToString());
                    //double descto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_des)", "").ToString());
                    //double iva = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_iva)", "").ToString());
                    //double total = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(total)", "").ToString());
                    //double costo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(costo)", "").ToString());


                    llenarTotales(((DataSet)slowTask.Result));


                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.Opacity = 1;
                MessageBox.Show("aqui 2.1" + ex);

            }
        }

        public void llenarTotales(DataSet data)
        {


            try
            {
                int n_item = TabControlInfo.Items.Count;

                for (int i = 1; i <= n_item; i++)
                {
                    var Total = (TextBlock)this.FindName("Total" + i);
                    if (Total != null) Total.Text = data.Tables[i - 1].Rows.Count.ToString();


                    double CantNeto = Convert.ToDouble(data.Tables[i - 1].Rows.Count > 0 ? data.Tables[i - 1].Compute("Sum(neto)", "") : 0);
                    var TextCantidad = (TextBlock)this.FindName("TextCantidad" + i);
                    if (TextCantidad != null) TextCantidad.Text = CantNeto.ToString("N");

                    double sub = Convert.ToDouble(data.Tables[i - 1].Rows.Count > 0 ? data.Tables[i - 1].Compute("Sum(subtotal)", "") : 0);
                    var TextSubtotal = (TextBlock)this.FindName("TextSubtotal" + i);
                    if (TextSubtotal != null) TextSubtotal.Text = sub.ToString("C");

                    double descto = Convert.ToDouble(data.Tables[i - 1].Rows.Count > 0 ? data.Tables[i - 1].Compute("Sum(val_des)", "") : 0);
                    var TextDescuento = (TextBlock)this.FindName("TextDescuento" + i);
                    if (TextDescuento != null) TextDescuento.Text = descto.ToString("C");

                    double iva = Convert.ToDouble(data.Tables[i - 1].Rows.Count > 0 ? data.Tables[i - 1].Compute("Sum(val_iva)", "") : 0);
                    var TextIva = (TextBlock)this.FindName("TextIva" + i);
                    if (TextIva != null) TextIva.Text = iva.ToString("C");

                    double total = Convert.ToDouble(data.Tables[i - 1].Rows.Count > 0 ? data.Tables[i - 1].Compute("Sum(total)", "") : 0);
                    var TextTotal = (TextBlock)this.FindName("TextTotal" + i);
                    if (TextTotal != null) TextTotal.Text = total.ToString("C");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al llenar totales:" + w);
            }
        }


        private DataSet LoadData(string Fi, string Ff,string where, string cod_empresa)
        {

            try
            {


                SqlConnection con1 = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                //cmd = new SqlCommand("SpConsultaInAnalisisDeVentas", con);
                cmd = new SqlCommand("_EmpSpConsultaInAnalisisDeVentas", con1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", Fi);
                cmd.Parameters.AddWithValue("@FechaFin", Ff);
                cmd.Parameters.AddWithValue("@Where", where);                
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con1.Close();

                return ds;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                /// MessageBox.Show("aqui 44");
                return null;
            }
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;

            if
            (
                e.ColumnName == "cantidad" || e.ColumnName == "can_dev" || e.ColumnName == "neto" ||
                e.ColumnName == "subtotal" || e.ColumnName == "val_des" || e.ColumnName == "val_iva" ||
                e.ColumnName == "total"

                )
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


            if (
                e.ColumnName == "cod_bod" || e.ColumnName == "cod_sub" || e.ColumnName == "cod_ref" || e.ColumnName == "per_doc"
                || e.ColumnName == "cod_ven" || e.ColumnName == "cod_pag" || e.ColumnName == "cod_cli" || e.ColumnName == "cod_prv"
                || e.ColumnName == "cod_tip" || e.ColumnName == "cod_ven" || e.ColumnName == "cod_trn" || e.ColumnName == "cod_mar"
                || e.ColumnName == "cod_gru" || e.ColumnName == "cod_tall" || e.ColumnName == "cod_col" || e.ColumnName == "num_trn"
                )
            {
                string value = e.CellValue.ToString();

                e.Range.Text = value;
                e.Handled = true;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;

                SfDataGrid sfdg = new SfDataGrid();
                if (((Button)sender).Tag.ToString() == "1") sfdg = VentasPorProducto;
                if (((Button)sender).Tag.ToString() == "2") sfdg = VentaPorBodega;
                if (((Button)sender).Tag.ToString() == "3") sfdg = VentasPorCliente;
                if (((Button)sender).Tag.ToString() == "4") sfdg = VentasPorVendedor;
                if (((Button)sender).Tag.ToString() == "5") sfdg = VentasPorLinea;
                if (((Button)sender).Tag.ToString() == "6") sfdg = VentasPorGrupo;
                if (((Button)sender).Tag.ToString() == "7") sfdg = VentasPorFPago;
                if (((Button)sender).Tag.ToString() == "8") sfdg = VentasPorClienteRef;
                if (((Button)sender).Tag.ToString() == "9") sfdg = GridDocumen;
                //string nameFileXLS = "Kardex" + DateTime.Now.ToLocalTime().ToString();
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


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();

                    if (string.IsNullOrEmpty(tag)) return;
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (tag == "inmae_ref")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ref"; cmpnombre = "nom_ref"; cmporden = "nom_ref"; cmpidrow = "idrow"; cmptitulo = "Maestra de productos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "estado=1";
                    }
                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "idrow"; cmptitulo = "Maestra de bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "estado=1 and ind_vta=1";
                    }
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "inmae_mer")
                    {
                        cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_tip")
                    {
                        cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "idrow"; cmptitulo = "Maestra de lineas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_gru")
                    {
                        cmptabla = tag; cmpcodigo = "cod_gru"; cmpnombre = "nom_gru"; cmporden = "cod_gru"; cmpidrow = "idrow"; cmptitulo = "Maestra de grupo"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }

                    //MessageBox.Show(cmptabla + "-" + cmpcodigo + "-" + cmpnombre + "-" + cmporden + "-" + cmpidrow + "-" + cmptitulo + "-" + cmpconexion + "-" + cmpwhere);
                    int idr = 0; string code = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    winb = null;
                    if (idr > 0)
                    {
                        ((TextBox)sender).Text = code;
                        if (tag == "inmae_ref") TextBoxRefF.Text = code;
                        if (tag == "inmae_bod") TextBoxBodF.Text = code;
                        if (tag == "inmae_tip") TextBoxTipF.Text = code;
                        if (tag == "inmae_gru") TextBoxGrpF.Text = code;
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
                MessageBox.Show(ex.Message.ToString());
                MessageBox.Show("aqui45");
            }

        }

        private void TextBoxRefI_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.Key.ToString());
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            tabitem.Cerrar(0);
        }


        //*****************************************************************



        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = ((Button)sender).Tag.ToString();
                Detalle Windows_Detalle = new Detalle();

                if (tag == "1")
                {
                    DataRowView row = (DataRowView)VentasPorProducto.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_ref"].ToString();
                    Windows_Detalle.nombre = row["nom_ref"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "2")
                {
                    DataRowView row = (DataRowView)VentaPorBodega.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_bod"].ToString();
                    Windows_Detalle.nombre = row["nom_bod"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "3")
                {
                    DataRowView row = (DataRowView)VentasPorCliente.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_cli"].ToString();
                    Windows_Detalle.nombre = row["nom_cli"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;

                }
                if (tag == "4")
                {
                    DataRowView row = (DataRowView)VentasPorLinea.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_tip"].ToString();
                    Windows_Detalle.nombre = row["nom_tip"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "5")
                {
                    DataRowView row = (DataRowView)VentasPorGrupo.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_gru"].ToString();
                    Windows_Detalle.nombre = row["nom_gru"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "6")
                {
                    DataRowView row = (DataRowView)VentasPorFPago.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_fpag"].ToString();
                    Windows_Detalle.nombre = row["nom_pag"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }
                if (tag == "7")
                {
                    DataRowView row = (DataRowView)VentasPorVendedor.SelectedItems[0];
                    Windows_Detalle.fecha_ini = FecIni.Text;
                    Windows_Detalle.fecha_fin = FecFin.Text;
                    Windows_Detalle.codigo = row["cod_ven"].ToString();
                    Windows_Detalle.nombre = row["nom_ven"].ToString();
                    Windows_Detalle.cnEmpExt = cnEmp;
                }


                Windows_Detalle.tagBTN = tag;
                Windows_Detalle.ShowInTaskbar = false;
                Windows_Detalle.Owner = Application.Current.MainWindow;
                Windows_Detalle.ShowDialog();

            }
            catch (Exception)
            {
                MessageBox.Show("Selecione una casilla del Grid");
            }
        }



        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                string tag = ((SfDataGrid)sender).Tag.ToString();

                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cantidadX = 0;
                double subtotalX = 0;
                double descuentoX = 0;
                double ivaX = 0;
                double totalX = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {

                    cantidadX += Convert.ToDouble(provider.GetValue(records[i].Data, "neto").ToString());
                    subtotalX += Convert.ToDouble(provider.GetValue(records[i].Data, "subtotal").ToString());
                    descuentoX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_des").ToString());
                    ivaX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_iva").ToString());
                    totalX += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                }

                if (tag == "1")
                {
                    TextCantidad1.Text = cantidadX.ToString();
                    TextSubtotal1.Text = subtotalX.ToString("C");
                    TextDescuento1.Text = descuentoX.ToString("C");
                    TextIva1.Text = ivaX.ToString("C");
                    TextTotal1.Text = totalX.ToString("C");
                    Total1.Text = VentasPorProducto.View.Records.Count.ToString();
                }
                if (tag == "2")
                {
                    TextCantidad2.Text = cantidadX.ToString();
                    TextSubtotal2.Text = subtotalX.ToString("C");
                    TextDescuento2.Text = descuentoX.ToString("C");
                    TextIva2.Text = ivaX.ToString("C");
                    TextTotal2.Text = totalX.ToString("C");
                    Total2.Text = VentaPorBodega.View.Records.Count.ToString();
                }
                if (tag == "3")
                {
                    TextCantidad3.Text = cantidadX.ToString();
                    TextSubtotal3.Text = subtotalX.ToString("C");
                    TextDescuento3.Text = descuentoX.ToString("C");
                    TextIva3.Text = ivaX.ToString("C");
                    TextTotal3.Text = totalX.ToString("C");
                    Total3.Text = VentasPorCliente.View.Records.Count.ToString();
                }
                if (tag == "4")
                {
                    TextCantidad4.Text = cantidadX.ToString();
                    TextSubtotal4.Text = subtotalX.ToString("C");
                    TextDescuento4.Text = descuentoX.ToString("C");
                    TextIva4.Text = ivaX.ToString("C");
                    TextTotal4.Text = totalX.ToString("C");
                    Total4.Text = VentasPorVendedor.View.Records.Count.ToString();
                }
                if (tag == "5")
                {
                    TextCantidad5.Text = cantidadX.ToString();
                    TextSubtotal5.Text = subtotalX.ToString("C");
                    TextDescuento5.Text = descuentoX.ToString("C");
                    TextIva5.Text = ivaX.ToString("C");
                    TextTotal5.Text = totalX.ToString("C");
                    Total5.Text = VentasPorLinea.View.Records.Count.ToString();
                }
                if (tag == "6")
                {
                    TextCantidad6.Text = cantidadX.ToString();
                    TextSubtotal6.Text = subtotalX.ToString("C");
                    TextDescuento6.Text = descuentoX.ToString("C");
                    TextIva6.Text = ivaX.ToString("C");
                    TextTotal6.Text = totalX.ToString("C");
                    Total6.Text = VentasPorGrupo.View.Records.Count.ToString();
                }
                if (tag == "7")
                {
                    TextCantidad7.Text = cantidadX.ToString();
                    TextSubtotal7.Text = subtotalX.ToString("C");
                    TextDescuento7.Text = descuentoX.ToString("C");
                    TextIva7.Text = ivaX.ToString("C");
                    TextTotal7.Text = totalX.ToString("C");
                    Total7.Text = VentasPorFPago.View.Records.Count.ToString();
                }
                if (tag == "8")
                {
                    TextCantidad8.Text = cantidadX.ToString();
                    TextSubtotal8.Text = subtotalX.ToString("C");
                    TextDescuento8.Text = descuentoX.ToString("C");
                    TextIva8.Text = ivaX.ToString("C");
                    TextTotal8.Text = totalX.ToString("C");
                    Total8.Text = VentasPorClienteRef.View.Records.Count.ToString();
                }
                if (tag == "9")
                {
                    TextCantidad9.Text = cantidadX.ToString();
                    TextSubtotal9.Text = subtotalX.ToString("C");
                    TextDescuento9.Text = descuentoX.ToString("C");
                    TextIva9.Text = ivaX.ToString("C");
                    TextTotal9.Text = totalX.ToString("C");
                    Total9.Text = GridDocumen.View.Records.Count.ToString();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }


            //TextSubtotal.Text = subtotalX.ToString("C");
            //TextDescuento.Text = descuentoX.ToString("C");
            //TextIva.Text = ivaX.ToString("C");
            //TextTotal.Text = totalX.ToString("C");

        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void BtnDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridDocumen.SelectedItems[0];
                string num_trn = row["num_trn"].ToString().Trim();
                string cod_trn = row["cod_trn"].ToString().Trim();
                int id = idreg(cod_trn, num_trn);
                if (id > 0)
                {
                    SiaWin.TabTrn(1, idemp, true, id, moduloid, WinModal: true);
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir documento:" + w);
            }
        }

        public int idreg(string cod_trn, string num_trn)
        {
            int id = 0;
            DataTable tabla = SiaWin.Func.SqlDT("select * from incab_doc where cod_trn='" + cod_trn + "' and num_trn='" + num_trn + "' ", "doc", idemp);
            if (tabla.Rows.Count > 0) id = Convert.ToInt32(tabla.Rows[0]["idreg"]);
            return id;
        }

        private void BtnCruzarcoin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("declare @fechaini as date = '" + FecIni.Text.ToString() + "';declare @fechafin as date = '" + FecFin.Text.ToString() + "' ;");
                sbsql.Append("declare @vtain as table (cod_trn char(3),num_trn char(12),cod_ter char(15),subtotal decimal(18, 2));");
                sbsql.Append("declare @vtaco as table (cod_trn char(3),num_trn char(12),cod_ter char(15),subtotal decimal(18, 2));");
                sbsql.Append("declare @error as table (modulo char(2),error varchar(500));");
                sbsql.Append("insert into @vtain select  cab.cod_trn,cab.num_trn,rtrim(cab.cod_cli) as cod_ter,sum(subtotal) as subtotal from incue_doc as cue inner join incab_doc as cab on ");
                sbsql.Append(" cab.idreg = cue.idregcab and cab.cod_trn = cue.cod_trn and cab.num_trn = cue.num_trn ");
                sbsql.Append("inner join inmae_trn as trn on trn.cod_trn = cab.cod_trn ");
                sbsql.Append("inner join InMae_ref as ref on ref.cod_ref = cue.cod_ref and ref.cod_tip <> '000' ");
                sbsql.Append("where convert(date, fec_trn) between @fechaini and @fechafin  and cab.cod_trn in('004', '005', '007', '008') ");
                sbsql.Append("group by cab.cod_trn,trn.cod_tdo,cab.num_trn,cab.cod_cli order by cod_trn, num_trn;");
                sbsql.Append(" insert into @vtaco select cab.cod_trn,cab.num_trn,rtrim(cue.cod_ter) as cod_ter,sum(iif(substring(cod_cta, 1, 4) = '4135', iif(cab.cod_trn = '04', cre_mov, deb_mov), iif(substring(cod_cta, 1, 4) = '4175', iif(cab.cod_trn = '08', deb_mov, 0), 0))) as subtotal ");
                sbsql.Append("from cocab_doc as cab inner join cocue_doc as cue on cab.cod_trn = cue.cod_trn and cab.num_trn = cue.num_trn ");
                sbsql.Append(" where convert(date, fec_trn) between @fechaini and @fechafin and(cab.cod_trn = '04' or cab.cod_trn = '08') and substring(cue.cod_cta,1,4)<> '2495' ");
                sbsql.Append("  group by cab.cod_trn,cab.num_trn,cue.cod_ter order by cod_trn, num_trn ;");
                sbsql.Append("declare @regin as int = 0;declare @regco as int = 0;");
                sbsql.Append("select @regin = count(*) from @vtain;  select @regco = count(*) from @vtaco;");
                //sbsql.Append("--1 - valida cantidad de registros en in vs co, deben de ser iguales en los 2 modulos;");
                sbsql.Append("if (@regin <> @regco)");
                sbsql.Append(" begin ");
                sbsql.Append("insert into @error(modulo, error) values('--', 'numero de registros diferentes: registros inventarios=' + convert(char(10), @regin) + ' registros contabilidad:' + convert(char(10), @regco)) ");
                sbsql.Append("if (@regin > @regco) insert into @error(modulo, error) select 'in','Documento de inventarios:' + cod_trn + '-' + num_trn + ' no existe en contabilidad.' from @vtain as vtain where not exists(select * from @vtaco as vtaco where vtaco.num_trn = vtain.num_trn) ");
                sbsql.Append("if (@regco > @regin) insert into @error(modulo, error) select 'co','Documento de inventarios:' + cod_trn + '-' + num_trn + ' no existe en contabilidad.'  from @vtaco as vtaco where not exists(select * from @vtain as vtain where vtaco.num_trn = vtain.num_trn) ");
                sbsql.Append("end");
                //sbsql.Append("--2 - valida valores de in a co;");
                sbsql.Append(" insert into @error(modulo, error)  select 'in','Documento de In con diferencias -' + vtain.cod_trn + '-' + vtain.num_trn + 'SubIn=' + FORMAT(vtain.subtotal, 'N', 'en-us') + ' - SubCo=' + FORMAT(vtaco.subtotal, 'N', 'en-us') + ' - Diferencia=' + FORMAT(vtain.subtotal - vtaco.subtotal, 'N', 'en-us') from @vtain as vtain ");
                sbsql.Append(" inner join @vtaco as vtaco on vtain.num_trn = vtaco.num_trn ");
                sbsql.Append(" where vtain.subtotal<> vtaco.subtotal order by vtain.num_trn");
                //sbsql.Append(" --2 - valida valores de in a co ;");
                sbsql.Append(" insert into @error(modulo, error) select 'co','Documento de Co con diferencias -' + vtaco.cod_trn + '-' + vtaco.num_trn + 'SubCo=' + FORMAT(vtaco.subtotal, 'N', 'en-us') + ' - SubIn=' + FORMAT(vtain.subtotal, 'N', 'en-us') + ' - Diferencia=' + FORMAT(vtaco.subtotal - vtain.subtotal, 'N', 'en-us') from @vtaco as vtaco ");
                sbsql.Append(" inner join @vtain as vtain on vtaco.num_trn = vtain.num_trn ");
                sbsql.Append(" where vtaco.subtotal<> vtain.subtotal order by vtaco.num_trn");
                //sbsql.Append(" --2 - valida NIT de in a co ;");
                sbsql.Append(" insert into @error(modulo, error) select 'co','Documento de IN con diferencias - en nit ' + vtaco.cod_trn + '-' + vtaco.num_trn + 'NitCo=' + vtaco.cod_ter + ' - NitIn=' + vtain.cod_ter   from @vtaco as vtaco ");
                sbsql.Append(" inner join @vtain as vtain on vtaco.num_trn = vtain.num_trn ");
                sbsql.Append(" where vtaco.cod_ter<> vtain.cod_ter order by vtaco.num_trn");

                sbsql.Append(" declare @nErrores as int = 0; select @nErrores = count(*) from @error; ");
                sbsql.Append(" if (@nerrores > 0) ");
                sbsql.Append(" begin");
                sbsql.Append(" select* from @error");
                sbsql.Append(" end");
                DataTable tabla = SiaWin.Func.SqlDT(sbsql.ToString(), "doc", idemp);
                //MessageBox.Show(sbsql.ToString());
                if (tabla.Rows.Count > 0)
                {
                    SiaWin.Browse(tabla, true);

                }
                else
                {
                    MessageBox.Show("No existen Errores ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }



        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }


}

