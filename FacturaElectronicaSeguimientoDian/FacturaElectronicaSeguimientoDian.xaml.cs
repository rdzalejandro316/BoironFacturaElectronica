using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using SrvEnvio = FacturaElectronicaSeguimientoDian.ServiceEnvio;
using SrvAjunto = FacturaElectronicaSeguimientoDian.ServiceAdjuntos;
using FacturaElectronicaSeguimientoDian.ServiceEnvio;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9557,"FacturaElectronicaSeguimientoDian");
    //Sia.TabU(9557);

    public partial class FacturaElectronicaSeguimientoDian : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        SrvEnvio.ServiceClient serviceClienteEnvio = new SrvEnvio.ServiceClient();
        SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();

        string tokenbussines = "";
        string tokenpassword = "";
        public FacturaElectronicaSeguimientoDian(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Factura Electronica";
            tabitem.MultiTab = false;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");

                TxFecIni.Text = DateTime.Now.ToString();
                TxFecFin.Text = DateTime.Now.ToString();


                string query = "select top 1 RTRIM(stockenemp_) AS stockenemp,RTRIM(stockenpas_) AS stockenpas,rtrim(surl_) as surl_,rtrim(surladj_) as surladj_ from In_confi";
                DataTable dt = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                if (dt.Rows.Count > 0)
                {

                    tokenbussines = dt.Rows[0]["stockenemp"].ToString().Trim();
                    tokenpassword = dt.Rows[0]["stockenpas"].ToString().Trim();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (CbTipo.SelectedIndex >= 0)
                {
                    string fec_ini = TxFecIni.Text;
                    string fec_fin = TxFecFin.Text;

                    string where = CbTipo.SelectedIndex == 0 ? " and cod_trn='005' " : " and (cod_trn='007' or cod_trn='008') ";

                    dataGridFE.ItemsSource = null;
                    sfBusyIndicator.IsBusy = true;

                    CancellationTokenSource source = new CancellationTokenSource();
                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(fec_ini, fec_fin, where), source.Token);
                    await slowTask;

                    if (((DataTable)slowTask.Result).Rows.Count > 0)
                    {
                        dataGridFE.ItemsSource = ((DataTable)slowTask.Result);
                        TxRegistr.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                    }
                    else
                    {
                        MessageBox.Show("no existen facturas en los filtros seleccionados", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        sfBusyIndicator.IsBusy = false;
                        dataGridFE.ItemsSource = null;
                        TxRegistr.Text = "0";
                    }

                    sfBusyIndicator.IsBusy = false;


                }
                else
                {
                    MessageBox.Show("seleccione el tipo de factura", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar");
            }
        }

        private DataTable LoadData(string fec_ini, string fec_fin, string where)
        {
            try
            {
                string query = "select * from incab_doc where  convert(date,incab_doc.fec_trn,103) between '" + fec_ini + "' and '" + fec_fin + "' " + where;
                DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                return tabla;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("error");
                return null;
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridFE.ExportToExcel(dataGridFE.View, options);
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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erorr al exportar:" + w);
            }
        }

        private void BtnEstado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridFE.SelectedIndex >= 0)
                {

                    DataRowView row = (DataRowView)dataGridFE.SelectedItems[0];
                    string num_trn = string.IsNullOrEmpty(row["fa_docelect"].ToString().Trim()) ?
                        row["num_trn"].ToString().Trim() : row["fa_docelect"].ToString().Trim();



                    DocumentStatusResponse response = serviceClienteEnvio.EstadoDocumento(tokenbussines, tokenpassword, num_trn);

                    StringBuilder st = new StringBuilder();

                    st.Append("ACEPTACION FISICA: " + response.aceptacionFisica.ToString() + Environment.NewLine);
                    st.Append("AMBIENTE: " + response.ambiente.ToString().Trim() + Environment.NewLine);
                    st.Append("CANDENA CODIGO QR: " + response.cadenaCodigoQR.ToString().Trim() + Environment.NewLine);
                    st.Append("CANDENA CODIGO CUFE: " + response.cadenaCufe.ToString().Trim() + Environment.NewLine);
                    st.Append("CODIGO: " + response.codigo.ToString().Trim() + Environment.NewLine);
                    st.Append("CONSECUTIVO: " + response.consecutivo.ToString().Trim() + Environment.NewLine);
                    st.Append("CUFE: " + response.cufe.ToString().Trim() + Environment.NewLine);
                    st.Append("DESCRIPCION DOC: " + response.descripcionDocumento.ToString().Trim() + Environment.NewLine);
                    st.Append("ESTADO DOC: " + response.descripcionEstatusDocumento.ToString().Trim() + Environment.NewLine);
                    st.Append("ENTREGA DIAN: " + response.entregaMetodoDIAN.ToString().Trim() + Environment.NewLine);
                    st.Append("ESTADO DOC2: " + response.estatusDocumento.ToString().Trim() + Environment.NewLine);
                    st.Append("DIAN VALID: " + response.esValidoDIAN.ToString().Trim() + Environment.NewLine);
                    st.Append("FECHA DOC: " + response.fechaDocumento.ToString().Trim() + Environment.NewLine);
                    st.Append("HISTORIAL ENTREGA: " + response.historialDeEntregas.ToString().Trim() + Environment.NewLine);
                    st.Append("MENSAJE: " + response.mensaje.ToString().Trim() + Environment.NewLine);
                    st.Append("MENSAJE DOC: " + response.mensajeDocumento.ToString().Trim() + Environment.NewLine);
                    st.Append("POSEE ADJ: " + response.poseeAdjuntos.ToString().Trim() + Environment.NewLine);
                    st.Append("POSEE GRAFIC: " + response.poseeRepresentacionGrafica.ToString().Trim() + Environment.NewLine);
                    st.Append("RESULTADO: " + response.resultado.ToString().Trim() + Environment.NewLine);
                    st.Append("TRACK ID: " + response.trackID.ToString().Trim() + Environment.NewLine);

                    TxResponse.Text = st.ToString();


                }
                else
                {
                    MessageBox.Show("selecione un documento para poder ver el estado", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    TxResponse.Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("el documento no se encontro", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                TxResponse.Text = "";
            }
        }



    }
}
