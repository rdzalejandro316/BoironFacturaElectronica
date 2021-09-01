using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace FacturasProvedores
{
    public partial class BtnEditar : Window
    {
        public int idemp = 0;
        public string cnEmp = "";
        public string cod_empresa = "";
        public string nomempresa = "";
        public dynamic SiaWin;

        public bool flag = false;
        public string idreg = "";
        public string numtrn = "";

        public BtnEditar()
        {
            InitializeComponent();
            TxFecIni.Text = DateTime.Now.ToString();
            TxFecFin.Text = DateTime.Now.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = System.Windows.Application.Current.MainWindow;
            Title = "Edicion de Documento proveedores :" + cod_empresa + " - " + nomempresa;
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"select *,iif(tipo_pago = 0,'Pendiente','Pagado') as tipo from incab_doc where convert(date,fec_trn,103) between '{TxFecIni.Text}' and '{ TxFecFin.Text }' and cod_trn='302'; ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    dataGrid.ItemsSource = dt.DefaultView;
                    TxTotal.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("no existen documentos de proveedores con ese rango de fecha", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    dataGrid.ItemsSource = null;
                    TxTotal.Text = "0";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error BtnConsultar_Click:" + w);
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                string id = row["idreg"].ToString().Trim();
                numtrn = row["num_trn"].ToString().Trim();
                if (MessageBox.Show("desea editar el documento:" + numtrn + " ?", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    flag = true;
                    idreg = id;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("debe de seleccionar el documento que desea editar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                flag = false;
            }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    string id = row["idreg"].ToString().Trim();
                    string num_trn = row["num_trn"].ToString().Trim();

                    if (MessageBox.Show("desea editar el documento:" + numtrn + " para cambiarlo a estado pagado ?", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string query = $"update incab_doc set tipo_pago=1 where idreg='{id}';";

                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("actualizacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnConsultar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("debe de seleccionar el documento para actualizar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cambiar el estado de pago:" + w);
            }
        }



    }
}
