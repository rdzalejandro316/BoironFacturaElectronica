using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SiasoftAppExt
{

    public partial class PvReimprimeFacturaNC : Window
    {

        // original 
        //Sia.PublicarPnt(9306,"PvReimprimeFacturaNC");

        // prueba 
        //Sia.PublicarPnt(9558,"PvReimprimeFacturaNC");

        dynamic SiaWin;
        public int idEmp = 0;
        string codbod = "";
        public string codpvta = "";
        string nompvta = "";
        string cnEmp = "";
        int moduloid = 0;
        //        DataTable dtBod = new DataTable();
        DataTable dt = new DataTable();
        public string codtrn = string.Empty;
        public int idrowcab = 0;
        public string cufe = string.Empty;
        public string fechaentr = string.Empty;
        public string MSG = string.Empty;
        public string codigo = string.Empty;
        public string trnAnu = string.Empty;
        public string numAnu = string.Empty;
        public double totalFactura = 0;
        public PvReimprimeFacturaNC()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;
            //codpvta = SiaWin._UserTag;
            this.DataContext = this;
            FechaIni.Text = DateTime.Now.ToShortDateString();
            FechaFin.Text = DateTime.Now.ToShortDateString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }
        public void LoadInfo()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string nomemp = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Empresa:" + nomemp.Trim() + "(" + idEmp.ToString() + ")";
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                //        _usercontrol.Seg.Auditor(0,_usercontrol.ProjectId,idUser,_usercontrol.GroupId,idEmp,_usercontrol.ModuleId,_usercontrol.AccesoId,0,"Ingreso a: Punto de venta"+" - " +_titulo,"");
                if (codpvta == string.Empty)
                {
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    this.IsEnabled = true;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idEmp);
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idEmp);
                    //MessageBox.Show(nompvta + "-" + codbod);
                    if (string.IsNullOrEmpty(codbod.Trim()))
                    {
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.Title = this.Title + " PuntoVenta:" + nompvta.Trim() + " Bodega:" + codbod;
                    }
                }
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idEmp, 5, 42, 0, "Ingreso a:ReimprimirFacturasyNotasCredito Empresa:" + nomemp, "");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void Ejecutar_Click(object sender, RoutedEventArgs e)
        {

            //LoadData(codtrn);
            try
            {

                int _TipoDoc = CmbTipoDoc.SelectedIndex;
                if (_TipoDoc < 0)
                {
                    MessageBox.Show("Seleccione un Tipo de Documento..");
                    CmbTipoDoc.Focus();
                    CmbTipoDoc.IsDropDownOpen = true;
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                dataGridSF.ItemsSource = null;


                string codtrn = "005";
                if (_TipoDoc == 0) codtrn = "004";  // pos
                if (_TipoDoc == 1) codtrn = "005";  // factura
                if (_TipoDoc == 2) codtrn = "007";  //anulacion factura
                if (_TipoDoc == 3) codtrn = "008"; // devolucion aplicada
                if (_TipoDoc == 4) codtrn = "011"; // coditazion 
                if (_TipoDoc == 5) codtrn = "505"; // pedidos

                string FecIni = FechaIni.Text;
                string FecFin = FechaFin.Text;
                string bodega = codbod.Trim();

                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(FecIni, FecFin, bodega, codtrn, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridSF.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TextTotalEntradas.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                this.Opacity = 1;
                //MessageBox.Show("aqui 2.1" + ex);

            }
        }

        private DataSet SlowDude(string FecIni, string FecFin, string bodega, string cod_trn, CancellationToken cancellationToken)
        {
            try
            {
                DataSet jj = LoadData(FecIni, FecFin, bodega, cod_trn, cancellationToken);
                return jj;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        private DataSet LoadData(string FI, string FF, string bodega, string cod_trn, CancellationToken cancellationToken)
        {
            try
            {

                string tiponum = cod_trn == "005" ? "siasoft_num_trn" : "num_trn";

                string query = "select cab.idreg,cab.cod_trn,cab." + tiponum + " as num_trn,cab.fec_trn,cab.cod_cli,ter.cod_ven,vend.nom_mer,rtrim(ter.nom_ter) as nom_cli,cue.cod_bod,sum(cue.cantidad) as cantidad,sum(isnull(cue.subtotal+cue.val_iva-cue.val_des-cue.val_ret-cue.val_ica-cue.val_riva,0)) as tot_tot,max(trn.tip_trn) as tip_trn,cab.fa_cufe,cab.fa_fecharesp,cab.fa_codigo,cab.fa_msg,cab.fa_docelect,cab.trn_anu,cab.num_anu from incue_doc as cue ";
                query += " inner join incab_doc as cab on cab.idreg = cue.idregcab and cab.cod_trn='" + cod_trn + "'  inner join inmae_bod as bod on bod.cod_bod = cue.cod_bod ";
                query += " inner join comae_ter as ter on cab.cod_cli = ter.cod_ter ";
                query += " left join InMae_mer as vend on  ter.cod_ven = vend.cod_mer ";
                query += " inner join inmae_trn as trn on trn.cod_trn=cab.cod_trn   where convert(date,cab.fec_trn) between '" + FI + "' and '" + FF + " 23:59:59' ";
                query += " and cue.cod_bod = '" + bodega + "' group by cab.idreg,cab.cod_trn,cab." + tiponum + ",cab.fec_trn,cab.cod_cli,ter.nom_ter,cue.cod_bod,fa_cufe,cab.fa_fecharesp,cab.fa_codigo,cab.fa_msg ,fa_docelect,cab.trn_anu,cab.num_anu,ter.cod_ven,vend.nom_mer order by cab.cod_trn,cab.fec_trn";

                DataSet ds = new DataSet();
                if (string.IsNullOrEmpty(cod_trn)) return null;
                dt.Clear();

                DataTable tabla = SiaWin.Func.SqlDT(query, "Tabla", idEmp);
                ds.Tables.Add(tabla);
                return ds;
            }
            catch (Exception e)
            {
                //MessageBox.Show("aqui 44:"+e);
                return null;
            }
        }



        private void ReImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridSF.SelectedIndex >= 0)
                {
                    MessageBoxResult result = MessageBox.Show("USTED DESEA REIMPRIMIR EL DOCUMENTO SELECCIONADO...?", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                    DataRowView row = (DataRowView)dataGridSF.SelectedItems[0];
                    if (row == null)
                    {
                        MessageBox.Show("Registro sin datos");
                        return;
                    }
                    string numtrn = row["idreg"].ToString();
                    string cod_trn = row["cod_trn"].ToString().Trim();
                    codtrn = cod_trn;
                    idrowcab = Convert.ToInt32(numtrn);
                    cufe = row["fa_cufe"].ToString();
                    codigo = row["fa_codigo"].ToString();
                    trnAnu = row["trn_anu"].ToString();
                    numAnu = row["num_anu"].ToString();

                    string numero_tran = row["num_trn"].ToString();


                    string tipo = ((ComboBoxItem)CmbTipoDoc.SelectedItem).Content.ToString().Trim();

                    SiaWin.seguridad.Auditor(
                        0,
                        SiaWin._ProyectId,
                        SiaWin._UserId,
                        SiaWin._UserGroup,
                        SiaWin._BusinessId,
                        moduloid,
                        -1,
                        -9,
                         "PUNTO DE VENTA - " + codpvta + " REIMPRIMIO DOCUMENTO " + tipo.ToUpper() + " - " + numero_tran,
                        "REIMPRESION"
                        );

                    totalFactura = row["tot_tot"] == null ? 0 : Convert.ToDouble(row["tot_tot"].ToString());
                    this.Close();
                }
                else
                {
                    MessageBox.Show("seleccione el documento que quire imprimir");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la pantalla externa de imprimir:" + w);
            }

        }

        private void BtnRenviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dataGridSF.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridSF.SelectedItems[0];
                    int id = Convert.ToInt32(row["idreg"]);
                    string num_trn = row["num_trn"].ToString().Trim();
                    string cod_trn = row["cod_trn"].ToString().Trim();
                    string trn_anu = row["trn_anu"].ToString().Trim();
                    string fa_cufe = row["fa_cufe"].ToString().Trim();

                    if (cod_trn == "005" || trn_anu == "005")
                    {
                        if (!string.IsNullOrWhiteSpace(fa_cufe))
                        {
                            MessageBoxResult mess = MessageBox.Show("el documento: " + num_trn + " ya fue enviado electronicamente ustede desea reenviarlo?", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                            if (mess == MessageBoxResult.Yes)
                            {
                                this.idrowcab = id;
                                this.codtrn = cod_trn;
                                this.cufe = "";
                                this.Close();
                            }
                        }

                    }

                }
                else
                {
                    MessageBox.Show("debe de seleccionar un items de la consulta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("error al abrir reenvio");
            }
        }


    }

}
