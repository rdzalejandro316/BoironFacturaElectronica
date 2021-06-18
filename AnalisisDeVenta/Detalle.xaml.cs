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
using System.Windows.Shapes;

namespace AnalisisDeVenta
{
    public partial class Detalle : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cod_empresa = "";

        public string fecha_ini = "";
        public string fecha_fin = "";
        public string codigo = "";
        public string nombre = "";
        public string cnEmpExt = "";
        public string tagBTN = "";



        public Detalle()
        {
            InitializeComponent();
            pantalla();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }
        public void pantalla()
        {
            this.MinHeight = 600;
            this.MinWidth = 1000;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpConsultaInAnalisisDeVentasDetalle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", fecha_ini);
                cmd.Parameters.AddWithValue("@FechaFin", fecha_fin);
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);

                if (tagBTN == "1")
                {
                    tabItemExt1.Header = "Detalle de Producto " + nombre.Trim();
                    string cadena = "and cue.cod_ref='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "2")
                {
                    tabItemExt1.Header = "Detalle de Bodega " + nombre.Trim();
                    string cadena = "and cue.cod_bod='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "3")
                {
                    tabItemExt1.Header = "Detalle de Cliente " + nombre.Trim();
                    string cadena = "and cab.cod_cli='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "4")
                {
                    tabItemExt1.Header = "Detalle de Linea " + nombre.Trim();
                    string cadena = "and ref.cod_tip='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "5")
                {
                    tabItemExt1.Header = "Detalle del Grupo " + nombre.Trim();
                    string cadena = "and ref.cod_gru='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "6")
                {
                    tabItemExt1.Header = "Detalle de Forma de Pago " + nombre.Trim();
                    string cadena = "and cab.for_pag='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }
                if (tagBTN == "7")
                {
                    tabItemExt1.Header = "Detalle por Vendedor " + nombre.Trim();
                    string cadena = "and cab.cod_ven='" + codigo + "' ";
                    cmd.Parameters.AddWithValue("@Where", cadena);
                }


                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                dataGridCxC.ItemsSource = ds.Tables[0];
                Total.Text = ds.Tables[0].Rows.Count.ToString();


                double sub = Convert.ToDouble(ds.Tables[0].Compute("Sum(subtotal)", "").ToString());
                double descto = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_des)", "").ToString());
                double cantidad = Convert.ToDouble(ds.Tables[0].Compute("Sum(cantidad)", "").ToString());
                double iva = Convert.ToDouble(ds.Tables[0].Compute("Sum(val_iva)", "").ToString());
                double total = Convert.ToDouble(ds.Tables[0].Compute("Sum(total)", "").ToString());

                TextSubtotal.Text = sub.ToString("C");
                //TextDescuento.Text = descto.ToString("C");
                TextCantidad.Text = cantidad.ToString();
                TextIVA.Text = iva.ToString("C");
                TextTotal.Text = total.ToString("C");

            }
            catch (Exception w)
            {

                MessageBox.Show("error cargar:" + w);
            }

        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
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


        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cant = 0;
                double subtotalX = 0;
                double descuentoX = 0;
                double ivaX = 0;
                double totalX = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {
                    subtotalX += Convert.ToDouble(provider.GetValue(records[i].Data, "subtotal").ToString());
                    cant += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                    descuentoX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_des").ToString());
                    ivaX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_iva").ToString());
                    totalX += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                }

                Total.Text = dataGridCxC.View.Records.Count.ToString();
                TextSubtotal.Text = subtotalX.ToString("C");
                //TextDescuento.Text = descuentoX.ToString("C");
                TextCantidad.Text = cant.ToString();
                TextIVA.Text = ivaX.ToString("C");
                TextTotal.Text = totalX.ToString("C");

            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }

        }





    }
}
