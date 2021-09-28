using PvFacturaAnular;
using System;
using System.Collections.Generic;
using System.Data;
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

    //Sia.PublicarPnt(9491,"PvFacturaAnular");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9491,"PvFacturaAnular");    
    //ww.bodega = "001";
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();        

    public partial class PvFacturaAnular : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public Boolean bandera = false;
        public string codbod = "";
        //public string cod_trn_an = "";
        public string[] ArrayReturn = null ;
        public string TipoTransaccion = "";
        public int idregcab = 0;
        public string bod_remision = "";

        //evalua si es una bodega de consignacion la factura de anulacion  
        public string bodega_cuerpo = "";

        //public string descrip = "";

        public PvFacturaAnular()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;//ddd          

        }
        public void pantalla()
        {
            this.MinHeight = 400;
            this.MaxHeight = 400;
            this.MinWidth = 500;
            this.MaxWidth = 500;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                //cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                //this.Title = "P.Venta Nota Credito Factura Anular:" + cod_empresa + "-" + nomempresa;
                this.Title = "Documentos:" + cod_empresa + "-" + nomempresa;


                FechaConsIni.Text = DateTime.Now.ToShortDateString();
                FechaConsFin.Text = DateTime.Now.ToShortDateString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //idemp = SiaWin._BusinessId;
            LoadConfig();
            pantalla();

            string cadena = "select cod_dev,descripcion from incon_dev order by cod_dev";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            CBXconcepto.ItemsSource = dt.DefaultView;
            CmbTipoDoc.Focus();

            DataTable dtbodegas = SiaWin.Func.SqlDT("select * from inmae_bod where cod_bod='"+ codbod + "'", "Clientes", idemp);
            if (dtbodegas.Rows.Count>0)
            {
                bod_remision = dtbodegas.Rows[0]["bod_cons"].ToString();
            }
        }

        private void BTNvalidar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(CmbTipoDoc.SelectedIndex<0)
                {
                    MessageBox.Show("Seleccione tipo de documento a validar");
                    CmbTipoDoc.Focus();
                    return;
                }
                string tipodoc = "005";
                if (CmbTipoDoc.SelectedIndex == 0) tipodoc = "004";

                if (BuscarFactura(TXfactura.Text, tipodoc) == false)
                {
                    MessageBox.Show("El documento Digitado no existe...");
                    return;
                }
                //if(TxtNota.Text.Trim()=="")
                //{
                //    MessageBox.Show("Digite Nota.... ");
                //    TxtNota.Focus();
                //    return;
                //}
                //if(TxtAutoriza.Text.Trim()=="")
                //{
                //    MessageBox.Show("Digite Autorizado...");
                //    TxtAutoriza.Focus();
                //    return;
                //}
                if(CBXconcepto.SelectedIndex<0)
                {
                    MessageBox.Show("Seleccione concepto de devolucion....");
                    CBXconcepto.Focus();
                    return;
                }
                if (CBXTipo.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione el tipo");
                    CBXTipo.Focus();
                    return;
                }


                string[] ValoresReturn = new string[5];
                ValoresReturn[0] = TXfactura.Text.Trim();
                ValoresReturn[1] = TxtNota.Text.Trim();
                ValoresReturn[2] = TxtAutoriza.Text.Trim();
                ValoresReturn[3] = CBXconcepto.SelectedValue.ToString();
                ValoresReturn[4] = CBXTipo.SelectionBoxItem.ToString();
                
                //ValoresReturn[4] = descrip;

                if (ValoresReturn.Length > 0) ArrayReturn = ValoresReturn;               

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void TXfactura_LostFocus(object sender, RoutedEventArgs e)
        {
            //return;
            if(CmbTipoDoc.SelectedIndex <0)
            {
                MessageBox.Show("Seleccione tipo de documento...");
                TXfactura.BorderBrush = Brushes.Red;                
                CmbTipoDoc.Focus();                
                return;
            }
            if (TXfactura.Text == "")
            {
                bandera = false;
                return;
            }
            string tipodoc = "005";
            if (CmbTipoDoc.SelectedIndex == 0) tipodoc = "004";

            if (BuscarFactura(TXfactura.Text,tipodoc) == false)
            {
                MessageBox.Show("La factura ingresada no existe");
                TXfactura.BorderBrush = Brushes.Red;
                bandera = false;
            }
            else
            {
                bandera = true;
                TXfactura.BorderBrush = Brushes.Gray;
            }
        }

        public Boolean BuscarFactura(string factura,string tipodoc)
        {
            try
            {
           
                string cadena = "select cabeza.cod_trn,cabeza.num_trn,cuerpo.cod_bod,cabeza.fec_trn,cabeza.idreg from InCab_doc as cabeza ";
                cadena = cadena + "inner join InCue_doc as cuerpo on cabeza.idreg = cuerpo.idregcab	";
                //cadena = cadena + "where cuerpo.cod_bod='"+codbod+ "' and cabeza.cod_trn='"+tipodoc+"' and cabeza.num_trn='"+factura.Trim()+"' ";                
                cadena = cadena + "where cabeza.cod_trn='" + tipodoc + "' and cabeza.num_trn='" + factura.Trim() + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Factura", idemp);
                if (dt.Rows.Count > 0)
                {
                    TipoTransaccion = dt.Rows[0]["cod_trn"].ToString().Trim();
                    idregcab = Convert.ToInt32(dt.Rows[0]["idreg"].ToString().Trim());
                    bodega_cuerpo = dt.Rows[0]["cod_bod"].ToString().Trim();
                    //descrip = dt.Rows[0]["descrip"].ToString().Trim();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar la consulta de la facura:" + w);
                return false;
            }
        }

        private void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            // validaciones
            int tipoDoc = CmbTipoDoc.SelectedIndex;
            string FechaConsultaIni = FechaConsIni.Text.Trim();
            string FechaConsultaFin = FechaConsFin.Text.Trim();

            if (string.IsNullOrEmpty(FechaConsultaIni) || string.IsNullOrEmpty(FechaConsultaFin))
            {
                MessageBox.Show("Falta Fecha de Consulta....");
                FechaConsIni.Focus();
                return;
            }
            if(tipoDoc<0)
            {
                MessageBox.Show("Seleccione un tipo de documento..");
                CmbTipoDoc.Focus();
                return;
            }

            if (FechaConsIni.Text != "" && FechaConsFin.Text != "")
            {
                string tipoTrn = "005";
                if (tipoDoc == 0) tipoTrn = "004";
                ConsultaDocumentos ventana = new ConsultaDocumentos();
                ventana.fechaini = FechaConsIni.Text;
                ventana.fechafin = FechaConsFin.Text;
                ventana.codbod = codbod;
                ventana.tipoTrn = tipoTrn;
                ventana.idemp = idemp;

                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();                
                
                if (ventana.Documento != "")
                {
                    TXfactura.Text = ventana.Documento;
                    TipoTransaccion = ventana.tipoTrn;
                    idregcab = ventana.idregcab;
                }
                ventana = null;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }




    }
}
