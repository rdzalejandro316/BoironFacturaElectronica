using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturasProvedores.Model
{
    public class Referencia : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        string _cod_ref = "";
        public string cod_ref { get { return _cod_ref; } set { _cod_ref = value; OnPropertyChanged("cod_ref"); } }

        string _descrip = "";
        public string descrip { get { return _descrip; } set { _descrip = value; OnPropertyChanged("descrip"); } }


        string _lote = "";
        public string lote { get { return _lote; } set { _lote = value; OnPropertyChanged("lote"); } }

        string _fec_ven_ref = DateTime.Now.ToString("dd/MM/yyyy");
        public string fec_ven_ref { get { return _fec_ven_ref; } set { _fec_ven_ref = value; OnPropertyChanged("fec_ven_ref"); } }



        decimal _cantidad;
        public decimal cantidad
        {
            get { return _cantidad; }
            set
            {
                _cantidad = value; OnPropertyChanged("cantidad");
                subtotal = _cantidad * _cos_uni;
                val_iva = ((subtotal * por_iva) / 100);
                total = subtotal + val_iva;
            }
        }



        decimal _cos_uni;
        public decimal cos_uni
        {
            get { return _cos_uni; }
            set
            {
                _cos_uni = value; OnPropertyChanged("cos_uni");
                subtotal = _cantidad * _cos_uni;
                val_iva = ((subtotal * por_iva) / 100);
                total = subtotal + val_iva;
            }
        }

        decimal _por_iva;
        public decimal por_iva
        {
            get { return _por_iva; }
            set
            {
                _por_iva = value; OnPropertyChanged("por_iva");
                subtotal = _cantidad * _cos_uni;
                val_iva = ((subtotal * por_iva) / 100);
                total = subtotal + val_iva;
            }
        }


        decimal _val_iva;
        public decimal val_iva
        {
            get { return _val_iva; }
            set { _val_iva = value; OnPropertyChanged("val_iva"); }
        }





        decimal _subtotal;
        public decimal subtotal
        {
            get { return _subtotal; }
            set
            {
                _subtotal = value; OnPropertyChanged("subtotal");
                val_iva = ((subtotal * por_iva) / 100);
                total = subtotal + val_iva;
            }
        }

        decimal _total;
        public decimal total { get { return _total; } set { _total = value; OnPropertyChanged("total"); } }


    }
}
