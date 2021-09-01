using FacturasProvedores;
using FacturasProvedores.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(10794,"FacturasProvedores");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(10794,"FacturasProvedores");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();  

    public partial class FacturasProvedores : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string nomempresa = "";

        public string idreg_editar = "";
        public string numtrn_editar = "";

        Documento Doc = new Documento();

        public FacturasProvedores()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = System.Windows.Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                LoadConfig();
                this.DataContext = Doc;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }
        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "FACTURAS DE PROVEDORES " + cod_empresa + "-" + nomempresa;

                Doc.fec_trn = DateTime.Now.ToString("dd/MM/yyyy");

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }



        private void Tx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8 || e.Key == Key.Enter)
                {

                    string cod = "", nom = "", id = "", tit = "";
                    string tbl = (sender as TextBox).Tag.ToString();

                    switch (tbl)
                    {
                        case "comae_ter":
                            cod = "cod_ter"; nom = "nom_ter"; id = "idrow"; tit = "Maestra de terceros";
                            break;
                    }

                    dynamic winb = SiaWin.WindowBuscar(tbl, cod, nom, cod, id, Title, cnEmp, false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    int idtbl = winb.IdRowReturn;
                    string codetbl = winb.Codigo;
                    string nomtbl = winb.Nombre;

                    if (idtbl > 0)
                    {
                        switch (tbl)
                        {
                            case "comae_ter":
                                Doc.cod_prv = codetbl; Doc.nom_prv = nomtbl;
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("el codigo que ingreso no existe en la " + tit, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        (sender as TextBox).Text = "";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        private void tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string tx = (sender as TextBox).Text;

                if (string.IsNullOrEmpty(tx)) return;

                string cod = "", nom = "", tit = "";
                string tbl = (sender as TextBox).Tag.ToString();

                switch (tbl)
                {
                    case "comae_ter":
                        cod = "cod_ter"; nom = "nom_ter"; tit = "Maestra de terceros";
                        break;
                }


                string query = "select * from " + tbl + " where  " + cod + "='" + tx + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    string code = dt.Rows[0][cod].ToString();
                    string name = dt.Rows[0][nom].ToString();

                    switch (tbl)
                    {
                        case "comae_ter":
                            Doc.cod_prv = code; Doc.nom_prv = name;
                            Doc.dir1 = dt.Rows[0]["dir1"].ToString().Trim();
                            Doc.tel1 = dt.Rows[0]["tel1"].ToString().Trim();
                            break;
                    }


                }
                else
                {
                    MessageBox.Show("el codigo que ingreso no existe en la " + tit, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    switch (tbl)
                    {
                        case "comae_ter":
                            Doc.cod_prv = "";
                            Doc.nom_prv = "";
                            Doc.dir1 = "";
                            Doc.tel1 = "";
                            break;
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }


        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string btn = (sender as Button).Content.ToString();

                if (btn == "Nuevo")
                {
                    (sender as Button).Content = "Guardar";
                    BtnSalir.Content = "Cancelar";
                    BtnEditar.IsEnabled = false;
                    PanelA.IsEnabled = true;
                    Doc.RefGDCSource.Add(new Referencia() { cod_ref = "" });
                    dataGrid.CommitEdit();
                    dataGrid.UpdateLayout();
                    dataGrid.SelectedIndex = 0;
                    TxCodCli.Focus();
                }
                else
                {
                    #region validaciones


                    if (string.IsNullOrEmpty(Doc.cod_prv))
                    {
                        MessageBox.Show("debe de llenar el campo de provedor", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }


                    if (string.IsNullOrEmpty(Doc.doc_ref))
                    {
                        MessageBox.Show("debe de llenar el campo de factura", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (Doc.tipo_pago < 0)
                    {
                        MessageBox.Show("debe de llenar el campo de tipo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (Doc.RefGDCSource.Count > 0)
                    {
                        bool fc = true;
                        bool fv = true;
                        bool fr = true;
                        foreach (var item in Doc.RefGDCSource)
                        {
                            if (item.cantidad == 0) fc = false;
                            if (item.cos_uni == 0) fv = false;
                            if (string.IsNullOrEmpty(item.cod_ref)) fr = false;
                        }

                        if (!fc)
                        {
                            MessageBox.Show("el documento no permite cantidades en 0", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        if (!fv)
                        {
                            MessageBox.Show("el documento no permite el costo unitario en 0", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }

                        if (!fr)
                        {
                            MessageBox.Show("el documento no permite que el campo referencia este vacio", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("debe de generar almenos un registro en el cuerpo del documento", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }



                    #endregion


                    int id = document(idreg_editar, numtrn_editar);
                    if (id > 0)
                    {
                        string message = string.IsNullOrEmpty(idreg_editar) ? "se genero el documento exitosamente" : "se edito el documento:" + numtrn_editar + " exitosamente";
                        MessageBox.Show(message, "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        Doc.Clear();
                        BtnNuevo.Content = "Nuevo";
                        BtnSalir.Content = "Salir";
                        BtnEditar.IsEnabled = true;
                        PanelA.IsEnabled = false;
                        idreg_editar = "";
                        numtrn_editar = "";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error BtnNuevo_Click:" + w);
            }

        }

        public int document(string idreg, string num_trn)
        {
            int bandera = -1;
            try
            {
                string message = string.IsNullOrEmpty(idreg) ? "Usted desea guardar la factura del proveedor..?" : "usted desea editar el documento:" + num_trn;
                string caption = string.IsNullOrEmpty(idreg) ? "Generar factura de proveedor" : "Editar factura de proveedo";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    if (!string.IsNullOrEmpty(idreg) || !string.IsNullOrEmpty(num_trn))
                    {
                        string delete = "delete incab_doc where idreg='" + idreg + "';";
                        delete += "delete incue_doc where idregcab='" + idreg + "';";

                        if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                        {
                            MessageBox.Show("documento eliminado exitosamente para volver a ser creado", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {
                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;


                        string cod_trn = "302";

                        string sqlcab = "";
                        string sqlcue = "";

                        string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                        sqlConsecutivo = sqlConsecutivo + "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @ini as char(4);declare @num as varchar(12); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                        sqlConsecutivo = sqlConsecutivo + "UPDATE inmae_trn SET num_act=ISNULL(num_act, 0) + 1 ; ";
                        sqlConsecutivo = sqlConsecutivo + "SELECT @iFolioHost = num_act,@ini=rtrim(inicial) FROM inmae_trn where cod_trn='" + cod_trn + "';";
                        sqlConsecutivo = sqlConsecutivo + "set @num=@iFolioHost; select @iConsecutivo=rtrim(@ini)+'-'+rtrim(convert(varchar,@num));";


                        string consecutivo = string.IsNullOrEmpty(idreg) ? "@iConsecutivo" : "'" + num_trn + "'";

                        string cod_prv = Doc.cod_prv.Trim();
                        string fec_trn = Doc.fec_trn.Trim();
                        string factura = Doc.doc_ref.Trim();
                        string nota = Doc.des_mov.Trim();
                        string fec_fact = Doc.fec_fact.Trim();
                        double dia_pla = Doc.dia_pla;
                        string fec_ven = Doc.fec_ven.Trim();
                        int tipo_pago = Doc.tipo_pago;



                        sqlcab = @"INSERT INTO incab_doc (cod_trn,num_trn,fec_trn,cod_prv,doc_ref,dia_pla,fec_ven,fec_fact,tipo_pago,des_mov) VALUES ";
                        sqlcab += $" ('{cod_trn}',{consecutivo},'{fec_trn}','{cod_prv}','{factura}',{dia_pla},'{fec_ven}','{fec_fact}',{tipo_pago},'{nota}');";
                        sqlcab += " DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY(); ";

                        foreach (var item in Doc.RefGDCSource)
                        {
                            if (item.cantidad > 0)
                            {

                                string cod_ref = item.cod_ref.Trim();
                                string descrip = item.descrip.Trim();
                                string lote = item.lote.Trim();
                                string fec_ven_ref = item.fec_ven_ref.Trim();
                                string cantidad = item.cantidad.ToString("F", CultureInfo.InvariantCulture);
                                string cos_uni = item.cos_uni.ToString("F", CultureInfo.InvariantCulture);
                                string val_iva = item.val_iva.ToString("F", CultureInfo.InvariantCulture);
                                string por_iva = item.por_iva.ToString("F", CultureInfo.InvariantCulture);
                                string subtotal = item.subtotal.ToString("F", CultureInfo.InvariantCulture);
                                string total = item.total.ToString("F", CultureInfo.InvariantCulture);

                                sqlcue += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,descrip,cod_bod,lote,fec_ven,cantidad,cos_uni,val_iva,por_iva,cos_tot) VALUES ";
                                sqlcue += $" (@NewID,'{cod_trn}',{consecutivo},'{cod_ref}','{descrip}','001','{lote}','{fec_ven_ref}',{cantidad},{cos_uni},{val_iva},{por_iva},{total}); ";
                            }
                        }



                        command.CommandText = sqlConsecutivo + sqlcab + sqlcue + @"select CAST(@NewId AS int);";

                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        bandera = Convert.ToInt32(r.ToString());
                    }
                }
                else
                {
                    bandera = -1;
                    dataGrid.Focus();
                }

                return bandera;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento de soporte:" + w);
                return bandera;
            }
        }


        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnEditar win = new BtnEditar();
                win.SiaWin = SiaWin;
                win.idemp = idemp;
                win.cnEmp = cnEmp;
                win.cod_empresa = cod_empresa;
                win.nomempresa = nomempresa;
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowDialog();

                if (win.flag)
                {
                    string idreg = win.idreg;
                    idreg_editar = win.idreg;
                    numtrn_editar = win.numtrn;

                    string cabselc = "select fec_trn,cod_prv,ter.nom_ter,ter.dir1,ter.tel1,doc_ref,dia_pla,fec_ven,fec_fact,tipo_pago,des_mov ";
                    cabselc += "from incab_doc as cab  ";
                    cabselc += "inner join comae_ter as ter on cab.cod_prv = ter.cod_ter ";
                    cabselc += "where cab.idreg='" + idreg + "';";

                    DataTable dtcab = SiaWin.Func.SqlDT(cabselc, "tabla", idemp);
                    if (dtcab.Rows.Count > 0)
                    {
                        Doc.Clear();

                        string fec_trn = dtcab.Rows[0]["fec_trn"].ToString().Trim();
                        string cod_prv = dtcab.Rows[0]["cod_prv"].ToString().Trim();
                        string nomter = dtcab.Rows[0]["nom_ter"].ToString().Trim();
                        string dir1 = dtcab.Rows[0]["dir1"].ToString().Trim();
                        string tel1 = dtcab.Rows[0]["tel1"].ToString().Trim();
                        string doc_ref = dtcab.Rows[0]["doc_ref"].ToString().Trim();
                        int dia_pla = Convert.ToInt32(dtcab.Rows[0]["dia_pla"]);
                        string fec_ven = dtcab.Rows[0]["fec_ven"].ToString().Trim();
                        string fec_fact = dtcab.Rows[0]["fec_fact"].ToString().Trim();
                        int tipo_pago = Convert.ToInt32(dtcab.Rows[0]["tipo_pago"]);
                        string des_mov = dtcab.Rows[0]["des_mov"].ToString().Trim();


                        Doc.fec_trn = fec_trn;
                        Doc.cod_prv = cod_prv;
                        Doc.nom_prv = nomter;
                        Doc.dir1 = dir1;
                        Doc.tel1 = tel1;
                        Doc.doc_ref = doc_ref;
                        Doc.dia_pla = dia_pla;
                        Doc.fec_ven = fec_ven;
                        Doc.fec_fact = fec_fact;
                        Doc.tipo_pago = tipo_pago;
                        Doc.des_mov = des_mov;

                        DataTable dtcue = SiaWin.Func.SqlDT("select * from incue_doc where idregcab='" + idreg + "'  ", "tabla", idemp);
                        if (dtcue.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow item in dtcue.Rows)
                            {
                                Doc.RefGDCSource.Add(new Referencia()
                                {
                                    cod_ref = item["cod_ref"].ToString().Trim(),
                                    descrip = item["descrip"].ToString().Trim(),
                                    lote = item["lote"].ToString().Trim(),
                                    fec_ven_ref = Convert.ToDateTime(item["fec_ven"]).ToString("dd/MM/yyyy"),
                                    cantidad = Convert.ToDecimal(item["cantidad"]),
                                    cos_uni = Convert.ToDecimal(item["cos_uni"]),
                                    por_iva = Convert.ToDecimal(item["por_iva"]),
                                    val_iva = Convert.ToDecimal(item["val_iva"]),
                                    subtotal = Convert.ToDecimal(item["subtotal"]),
                                    total = Convert.ToDecimal(item["cos_tot"])
                                });
                            }

                        }

                        BtnNuevo.Content = "Guardar";
                        BtnSalir.Content = "Cancelar";
                        PanelA.IsEnabled = true;
                        dataGrid.CommitEdit();
                        dataGrid.UpdateLayout();


                        #region totales

                        decimal _cnt = 0; decimal _cosunt = 0; decimal _sub = 0; decimal _valiva = 0; decimal _total = 0;
                        foreach (var item in Doc.RefGDCSource)
                        {
                            _cnt += item.cantidad;
                            _cosunt += item.cos_uni;
                            _sub += item.subtotal;
                            _valiva += item.val_iva;
                            _total += item.total;
                        }

                        Doc.tot_cnt = _cnt;
                        Doc.tot_cos_uni = _cosunt;
                        Doc.tot_cos_tot = _total;
                        Doc.tot_reg = Doc.RefGDCSource.Count;

                        #endregion

                        dataGrid.SelectedIndex = 0;
                        TxCodCli.Focus();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la pantalla de edicion:" + w);
            }
        }



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string btn = (sender as Button).Content.ToString();
                if (btn == "Cancelar")
                {
                    Doc.Clear();
                    (sender as Button).Content = "Salir";
                    BtnNuevo.Content = "Nuevo";
                    BtnEditar.IsEnabled = true;
                    idreg_editar = "";
                    numtrn_editar = "";
                    PanelA.IsEnabled = false;


                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Usted desea salir?", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al salir:" + w);
            }
        }



        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var data = ((DataGrid)sender).SelectedItem as Referencia;

                if (e.Column.Header.ToString() == "FechaLote")
                {

                    string fecha = data.fec_ven_ref;

                    if (string.IsNullOrEmpty(fecha)) return;


                    DateTime fs; string format = "dd/MM/yyyy";


                    if (DateTime.TryParseExact(fecha, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out fs) == false)
                    {
                        MessageBox.Show("lo que introdujo en el campo 'fecha de lote' no es una fecha por favor verifique el formato dela fecha es dd/mm/yyyy ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        data.fec_ven_ref = DateTime.Now.ToString("dd/MM/yyyy");

                    }
                }

                if (e.Column.Header.ToString() == "Referencia")
                {

                    if (data.cod_ref.Length > 15)
                    {
                        MessageBox.Show("el campo referencia no puede ser mayor a 15 caracteres ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        data.cod_ref = "";
                    }
                }

                if (e.Column.Header.ToString() == "descrip")
                {

                    if (data.cod_ref.Length > 200)
                    {
                        MessageBox.Show("el campo descripcion no puede ser mayor a 200 caracteres ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        data.descrip = "";
                    }
                }

                if (e.Column.Header.ToString() == "lote")
                {

                    if (data.cod_ref.Length > 20)
                    {
                        MessageBox.Show("el campo lote no puede ser mayor a 20 caracteres ", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        data.lote = "";
                    }
                }


                UpdateTot();

            }
            catch (Exception w)
            {
                MessageBox.Show("errro en dataGrid_CellEditEnding:" + w);
            }

        }

        public void UpdateTot()
        {
            try
            {

                var tt = Doc.RefGDCSource.Total();



                if (tt.sub > 0)
                {
                    Doc.tot_cnt = tt.cnt;
                    Doc.tot_cos_uni = tt.cosunt;
                    Doc.tot_cos_tot = tt.total;
                    Doc.tot_reg = Doc.RefGDCSource.Count;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("retenciones:" + w);
            }
        }


        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (dataGrid.IsReadOnly == true) return;
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    BtnNuevo.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }

                var data = ((DataGrid)sender).SelectedItem as Referencia;
                if (data == null) e.Handled = true;

                var uiElement = e.OriginalSource as UIElement;
                if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Right || e.Key == Key.Tab)) //&& ((DataGrid)sender).CurrentColumn.DisplayIndex == 0)
                {
                    if (string.IsNullOrEmpty(data.cod_ref))
                    {
                        MessageBox.Show("el campo referencia debe de estar lleno", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        e.Handled = true;
                        return;
                    }

                    int column = ((DataGrid)sender).CurrentColumn.DisplayIndex + 1;
                    int columntot = ((DataGrid)sender).Columns.Count;

                    int fila1 = ((DataGrid)sender).SelectedIndex;
                    int fila = ((DataGrid)sender).Items.IndexOf(((DataGrid)sender).SelectedItem);

                    if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab) && uiElement != null && (column < columntot))
                    {

                        if (!string.IsNullOrEmpty(data.cod_ref) && ((DataGrid)sender).CurrentColumn.DisplayIndex == columntot)
                        {
                            Int32 countref = Doc.RefGDCSource.Count;
                            if (countref == dataGrid.SelectedIndex + 1)
                            {
                                Doc.RefGDCSource.Add(new Referencia() { cod_ref = "" });
                                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                                dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                                dataGrid.CommitEdit();
                                dataGrid.UpdateLayout();
                            }
                        }

                        if (((DataGrid)sender).CurrentColumn.DisplayIndex >= 0)
                        {
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            e.Handled = true;
                            return;
                        }
                    }

                    if (e.Key == Key.Right && ((DataGrid)sender).CurrentColumn.DisplayIndex == 0 && !string.IsNullOrEmpty(data.cod_ref))
                    {
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                    }

                    if (e.Key == Key.Left && uiElement != null && (column > 1))
                    {
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                        e.Handled = true;
                    }

                    if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Right || e.Key == Key.Tab) && uiElement != null && (column == columntot))
                    {
                        dataGrid.CommitEdit();
                        dataGrid.UpdateLayout();

                        int add = 0;
                        if (fila + 1 == Doc.RefGDCSource.Count)
                        {
                            Doc.RefGDCSource.Add(new Referencia() { cod_ref = "" });
                            add = 1;
                        }

                        if (add > 0) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex + add], dataGrid.Columns[0]);
                        dataGrid.CommitEdit();
                        dataGrid.UpdateLayout();
                        dataGrid.SelectedIndex = dataGrid.SelectedIndex + add;
                        e.Handled = true;
                    }


                    if (e.Key == Key.Up && dataGrid.CurrentColumn.DisplayIndex == 0 && string.IsNullOrEmpty(data.cod_ref))
                    {
                        var selectedItem = dataGrid.SelectedItem as Referencia;
                        if (selectedItem != null)
                        {
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                            dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                            Doc.RefGDCSource.Remove(selectedItem);
                            dataGrid.UpdateLayout();
                            var selectedItemnew = dataGrid.SelectedItem as Referencia;
                            if (selectedItemnew.cantidad > 0)
                            {
                                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[2]);
                                dataGrid.CancelEdit();
                                dataGrid.UpdateLayout();
                            }
                            e.Handled = true;
                        }
                    }

                    if (e.Key == Key.Up)
                    {
                        var selectedItemnew = dataGrid.SelectedItem as Referencia;
                        if (selectedItemnew.cantidad > 0)
                        {
                            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[2]);
                            dataGrid.CancelEdit();
                            dataGrid.UpdateLayout();
                        }
                    }

                    if (e.Key == Key.F3)  //eliminar registro
                    {
                        if (((DataGrid)sender).SelectedIndex == 0 && Doc.RefGDCSource.Count == 1) return;
                        if (MessageBox.Show("Borrar Registro actual?", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            var selectedItem = dataGrid.SelectedItem as Referencia;
                            if (selectedItem != null)
                            {
                                int fila1x = ((DataGrid)sender).SelectedIndex;
                                Int32 countrefx = Doc.RefGDCSource.Count;
                                if (((DataGrid)sender).SelectedIndex == 0 && Doc.RefGDCSource.Count > 1)
                                {
                                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                                }
                                else
                                {
                                    if (((DataGrid)sender).SelectedIndex > 0 && Doc.RefGDCSource.Count > 1) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                                    if (((DataGrid)sender).SelectedIndex == Doc.RefGDCSource.Count - 1) uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                                }
                                Doc.RefGDCSource.Remove(selectedItem);
                            }
                            e.Handled = true;
                        }
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }
        }

        private void UpDown_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (Doc.dia_pla > 0)
                {
                    DateTime _fecdoc = Convert.ToDateTime(Doc.fec_fact);
                    int _diaplazo = Convert.ToInt32(Doc.dia_pla);
                    DateTime _fecven = _fecdoc.AddDays(_diaplazo);
                    Doc.fec_ven = _fecven.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en UpDown_LostFocus:" + w);
            }
        }

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Doc.fec_fact))
                {
                    DateTime _fecdoc = Convert.ToDateTime(Doc.fec_fact);
                    int _diaplazo = Convert.ToInt32(Doc.dia_pla);
                    DateTime _fecven = _fecdoc.AddDays(_diaplazo);
                    Doc.fec_ven = _fecven.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en UpDown_LostFocus:" + w);
            }
        }



    }
}
