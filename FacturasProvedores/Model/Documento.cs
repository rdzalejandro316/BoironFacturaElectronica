using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturasProvedores.Model
{
    public class Documento : INotifyPropertyChanged
    {

        #region PropertyChanged


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private Ref RefgdcSource = new Ref();
        public Ref RefGDCSource
        {
            get { return RefgdcSource; }
            set { RefgdcSource = value; OnPropertyChanged("RefGDCSource"); }
        }

        #endregion

        string _fec_trn = DateTime.Now.ToString("dd/MM/yyyy");
        public string fec_trn { get { return _fec_trn; } set { _fec_trn = value; OnPropertyChanged("fec_trn"); } }


        string _cod_prv = "";
        public string cod_prv { get { return _cod_prv; } set { _cod_prv = value; OnPropertyChanged("cod_prv"); } }


        string _doc_ref = "";
        public string doc_ref { get { return _doc_ref; } set { _doc_ref = value; OnPropertyChanged("doc_ref"); } }


        string _des_mov = "";
        public string des_mov { get { return _des_mov; } set { _des_mov = value; OnPropertyChanged("des_mov"); } }



        string _fec_fact = DateTime.Now.ToString("dd/MM/yyyy");
        public string fec_fact { get { return _fec_fact; } set { _fec_fact = value; OnPropertyChanged("fec_fact"); } }


        double _dia_pla = 0;
        public double dia_pla { get { return _dia_pla; } set { _dia_pla = value; OnPropertyChanged("dia_pla"); } }


        string _fec_ven = DateTime.Now.ToString("dd/MM/yyyy");
        public string fec_ven { get { return _fec_ven; } set { _fec_ven = value; OnPropertyChanged("fec_ven"); } }


        int _tipo_pago = 0;
        public int tipo_pago { get { return _tipo_pago; } set { _tipo_pago = value; OnPropertyChanged("tipo_pago"); } }






        //------------- info documento

        string _nom_prv = "";
        public string nom_prv { get { return _nom_prv; } set { _nom_prv = value; OnPropertyChanged("nom_prv"); } }


        string _dir1 = "";
        public string dir1 { get { return _dir1; } set { _dir1 = value; OnPropertyChanged("dir1"); } }


        string _tel1 = "";
        public string tel1 { get { return _tel1; } set { _tel1 = value; OnPropertyChanged("tel1"); } }



        // -------------- totales

        decimal _tot_cnt = 0;
        public decimal tot_cnt { get { return _tot_cnt; } set { _tot_cnt = value; OnPropertyChanged("tot_cnt"); } }

        decimal _tot_cos_uni = 0;
        public decimal tot_cos_uni { get { return _tot_cos_uni; } set { _tot_cos_uni = value; OnPropertyChanged("tot_cos_uni"); } }

        decimal _tot_cos_tot = 0;
        public decimal tot_cos_tot { get { return _tot_cos_tot; } set { _tot_cos_tot = value; OnPropertyChanged("tot_cos_tot"); } }

        int _tot_reg = 0;
        public int tot_reg { get { return _tot_reg; } set { _tot_reg = value; OnPropertyChanged("tot_reg"); } }


        // -------------- metodos
        public void Clear()
        {
            this.fec_trn = DateTime.Now.ToString("dd/MM/yyyy"); 
            this.cod_prv = string.Empty;
            this.doc_ref = string.Empty;
            this.des_mov = string.Empty;
            this.des_mov = string.Empty;
            this.fec_fact = DateTime.Now.ToString("dd/MM/yyyy");
            this.dia_pla = 0;
            this.fec_ven = DateTime.Now.ToString("dd/MM/yyyy");
            this.tipo_pago = -1;

            this.nom_prv = "";
            this.dir1 = "";
            this.tel1 = "";

            RefGDCSource.Clear();
            tot_cnt = 0;
            tot_cos_uni = 0;
            tot_cos_tot = 0;
            tot_reg = 0;
        }

        public class Ref : ObservableCollection<Referencia>
        {
            public (decimal cnt, decimal cosunt, decimal sub, decimal valiva, decimal total) Total()
            {
                decimal _cnt = 0; decimal _cosunt = 0;
                decimal _sub = 0;
                decimal _valiva = 0;
                decimal _total = 0;

                foreach (var item in this)
                {
                    _cnt += item.cantidad;
                    _cosunt += item.cos_uni;
                    _sub += item.subtotal;
                    _valiva += item.val_iva;
                    _total += item.total;
                }
                return (cnt: _cnt, cosunt: _cosunt, sub: _sub, valiva: _valiva, total: _total);
            }
        }



    }


}
