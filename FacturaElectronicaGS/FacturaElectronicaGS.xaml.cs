using FacturaElectronicaGS.ServiceEnvio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using SrvEnvio = FacturaElectronicaGS.ServiceEnvio;
using SrvAjunto = FacturaElectronicaGS.ServiceAdjuntos;
using FacturaElectronicaGS.ServiceAdjuntos;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9555,"FacturaElectronicaGS");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9555,"FacturaElectronicaGS");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog(); 

    public partial class FacturaElectronicaGS : Window
    {
        BasicHttpBinding port;
        SrvEnvio.ServiceClient serviceClienteEnvio = new SrvEnvio.ServiceClient();
        SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();

        dynamic SiaWin;
        public string tokenEmpresa = string.Empty;
        public string tokenAuthorizacion = string.Empty;
        public string Url = "";
        public int idrowcab = 0;
        public int cantidadAnexos_ = 1;
        public string NumRegCab = string.Empty;
        DataSet dsImprimir = new DataSet();
        DataSet dsAnulaFactura = new DataSet();
        public string NumDocElect = string.Empty;
        public string Codigo = string.Empty;
        public string Msg = string.Empty;
        public string FechaResp = string.Empty;
        public string Cufe = string.Empty;
        public int _ModuloId = 0;
        public int _EmpresaId = 0;
        public int _AccesoId = 0;

        public string Tipo_Documento = string.Empty;

        public string codpvt = string.Empty;
        public String cnEmp = string.Empty;
        public int idemp = 0;
        string cod_empresa = string.Empty;

        public FacturaElectronicaGS()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //MessageBox.Show("iniclass");
            if (idemp <= 0) idemp = SiaWin._BusinessId;
            //idemp = SiaWin._BusinessId; 


            this.tbxFechaEmision.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            BtnEnviar.Focus();
            //this.tbxFechaEmision.Text = "2019-12-01 07:36:01";           




        }

        //#region Construcción del Objeto Factura
        private FacturaGeneral BuildFactura()
        {
            try
            {

                //armo el objeto factura
                FacturaGeneral facturaDemo = new FacturaGeneral
                {
                    cantidadDecimales = "2"
                };

                facturaDemo.consecutivoDocumento = dsImprimir.Tables[0].Rows[0]["numtrn"].ToString().Trim();

                #region cliente
                Cliente cliente = new Cliente
                {
                    actividadEconomicaCIIU = "0010",

                    destinatario = new Destinatario[1]
                };
                Destinatario destinatario = new Destinatario
                {
                    canalDeEntrega = "0"
                };

                Destinatario destinatario1 = destinatario;

                string[] correoEntrega = new string[1];
                //correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim();
                correoEntrega[0] = tbxEmail.Text.Trim();

                destinatario1.email = correoEntrega;
                destinatario1.fechaProgramada = tbxFechaEmision.Text.Trim();

                destinatario1.nitProveedorReceptor = "1";
                destinatario1.telefono = dsImprimir.Tables[0].Rows[0]["tel1"].ToString().Trim();
                cliente.destinatario[0] = destinatario1;
                cliente.detallesTributarios = new Tributos[1];
                Tributos tributos1 = new Tributos
                {
                    codigoImpuesto = "01"
                };
                cliente.detallesTributarios[0] = tributos1;

                string codciu = dsImprimir.Tables[0].Rows[0]["cod_ciu"].ToString().Trim();
                string depart = "";
                if (codciu.Trim() != "" && codciu.Trim().Length > 3) depart = codciu.Substring(0, 2);
                //MessageBox.Show(codciu+"-"+depart);
                Direccion direccionFiscal = new Direccion
                {
                    ciudad = dsImprimir.Tables[0].Rows[0]["ciudad"].ToString().Trim(),
                    codigoDepartamento = depart,
                    departamento = dsImprimir.Tables[0].Rows[0]["nom_ciudane"].ToString().Trim(),
                    direccion = dsImprimir.Tables[0].Rows[0]["dir1"].ToString().Trim(),
                    lenguaje = "es",
                    municipio = codciu,
                    pais = "CO",
                    zonaPostal = ""
                };

                cliente.direccionFiscal = direccionFiscal;
                //cliente.email = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim();
                cliente.email = tbxEmail.Text.Trim();


                InformacionLegal informacionLegal = new InformacionLegal
                {
                    codigoEstablecimiento = "00001",
                    nombreRegistroRUT = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim(),
                    //numeroIdentificacion = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim(),
                    numeroIdentificacion = tbxnit.Text,
                    //numeroIdentificacionDV = dsImprimir.Tables[0].Rows[0]["dv"].ToString().Trim(),
                    numeroIdentificacionDV = tbxDV.Text.Trim(),
                    tipoIdentificacion = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim()
                };
                InformacionLegal informacionLegalCliente = informacionLegal;
                cliente.informacionLegalCliente = informacionLegalCliente;
                cliente.nombreRazonSocial = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                cliente.notificar = "SI";

                //cliente.numeroDocumento = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim();
                cliente.numeroDocumento = tbxnit.Text;
                //cliente.numeroIdentificacionDV = dsImprimir.Tables[0].Rows[0]["dv"].ToString().Trim();
                cliente.numeroIdentificacionDV = tbxDV.Text.Trim();
                cliente.responsabilidadesRut = new Obligaciones[1];

                string tip_prv = dsImprimir.Tables[0].Rows[0]["tip_prv"].ToString().Trim();
                string tdoc = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();
                string rango = "";
                switch (tip_prv)
                {
                    case "0": rango = "0-48"; break;
                    case "1": rango = "0-49"; break;
                    case "2": rango = "0-13"; break;
                    default: rango = "0-48"; break;
                }

                Obligaciones obligaciones1 = new Obligaciones
                {
                    //obligaciones = "O-14",
                    //regimen = "04"
                    obligaciones = rango,
                    regimen = tdoc == "13" ? "05" : "04"
                };

                cliente.responsabilidadesRut[0] = obligaciones1;

                cliente.tipoIdentificacion = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();
                cliente.tipoPersona = "1";

                facturaDemo.cliente = cliente;
                #endregion               

                #region detalleDeFactura
                int ItemsCue = dsImprimir.Tables[1].Rows.Count;
                facturaDemo.detalleDeFactura = new FacturaDetalle[ItemsCue];
                int item = 0;
                foreach (DataRow row in dsImprimir.Tables[1].Rows)
                {
                    FacturaDetalle producto1 = new FacturaDetalle
                    {
                        cantidadPorEmpaque = "1",
                        cantidadReal = "1.00",
                        cantidadRealUnidadMedida = "94",
                        cantidadUnidades = row["cantidad"].ToString().Trim(),
                        codigoProducto = row["cod_ref"].ToString().Trim(),
                        descripcion = row["nom_ref"].ToString().Trim(),
                        descripcionTecnica = row["nom_ref"].ToString().Trim(),
                        estandarCodigo = "999",
                        estandarCodigoProducto = row["cod_ref"].ToString().Trim(),
                        impuestosDetalles = new FacturaImpuestos[1],
                        cargosDescuentos = new CargosDescuentos[1],
                    };

                    if (row["muestra"].ToString().Trim() == "1")
                        producto1.codigoTipoPrecio = "01";



                    FacturaImpuestos impuesto1 = new FacturaImpuestos
                    {
                        baseImponibleTOTALImp = Convert.ToDecimal(row["base"]).ToString(),
                        codigoTOTALImp = "01",
                        controlInterno = "",
                        porcentajeTOTALImp = Convert.ToDecimal(row["por_iva"]).ToString(),
                        unidadMedida = "94",
                        unidadMedidaTributo = "",
                        valorTOTALImp = Convert.ToDecimal(row["val_iva"]).ToString(),
                        valorTributoUnidad = ""
                    };
                    if (Convert.ToDecimal(row["val_des"]) > 0)
                    {
                        CargosDescuentos cargoDescto = new CargosDescuentos
                        {
                            codigo = "07",
                            monto = Convert.ToDecimal(row["val_des"]).ToString(),
                            //montoBase = Convert.ToDecimal(row["val_uni"]).ToString(),
                            montoBase = Convert.ToDecimal(row["subtotal"]).ToString(),
                            porcentaje = Convert.ToDecimal(row["por_des"]).ToString(),
                            indicador = "0",
                            secuencia = "1",
                            descripcion = "Descuento de temporada"
                        };
                        producto1.cargosDescuentos[0] = cargoDescto;
                    }
                    producto1.impuestosDetalles[0] = impuesto1;

                    producto1.impuestosTotales = new ImpuestosTotales[1];
                    ImpuestosTotales impuestoTOTAL1 = new ImpuestosTotales
                    {
                        codigoTOTALImp = "01",
                        montoTotal = Convert.ToDecimal(row["val_iva"]).ToString()
                    };

                    //if (SiaWin._UserId == 21)
                    //{
                    //    double a = Convert.ToDouble(row["val_iva"]);
                    //    MessageBox.Show("mensage administrador:" + a);
                    //}

                    producto1.impuestosTotales[0] = impuestoTOTAL1;
                    producto1.marca = "HKA";

                    string muestra = row["muestra"].ToString().Trim();

                    if (muestra == "1")
                    {
                        decimal pres_default = 1000;
                        decimal pre_ref = Convert.ToDecimal(row["precio_refer"]);
                        producto1.precioReferencia = pre_ref > 0 ? pre_ref.ToString() : pres_default.ToString();
                    }

                    producto1.muestraGratis = muestra;
                    //producto1.precioTotal = Convert.ToDecimal(row["tot_tot"]).ToString();
                    producto1.precioTotal = muestra == "1" ? "0" : Convert.ToDecimal(row["tot_tot"]).ToString();

                    producto1.precioTotalSinImpuestos = muestra == "1" ? "0" : Convert.ToDecimal(row["base"]).ToString();

                    //producto1.precioTotalSinImpuestos = Convert.ToDecimal(row["base"]).ToString();

                    producto1.precioVentaUnitario = muestra == "1" ? Convert.ToDecimal(row["precio_refer"]).ToString() : Convert.ToDecimal(row["val_uni"]).ToString();


                    producto1.secuencia = Convert.ToDecimal(row["secuencia"]).ToString();
                    producto1.unidadMedida = "94";
                    facturaDemo.detalleDeFactura[item] = producto1;
                    item++;
                }
                #endregion

                #region DocumentosReferenciados
                //               String Tipo_Documento = "";
                if (Tipo_Documento == "Nota Credito" || Tipo_Documento == "Nota Debito")
                {
                    facturaDemo.documentosReferenciados = new DocumentoReferenciado[2];

                    #region DiscrepansyResponse
                    DocumentoReferenciado DocumentoReferenciado1 = new DocumentoReferenciado
                    {
                        codigoEstatusDocumento = "2",
                        codigoInterno = "4",
                        cufeDocReferenciado = dsImprimir.Tables[6].Rows[0]["facufe"].ToString().Trim()
                    };

                    string[] descripcion = new string[1];
                    descripcion[0] = "Nota";
                    DocumentoReferenciado1.descripcion = descripcion;
                    DocumentoReferenciado1.numeroDocumento = dsImprimir.Tables[6].Rows[0]["numerfactu"].ToString().Trim();
                    #endregion
                    facturaDemo.documentosReferenciados[0] = DocumentoReferenciado1;

                    #region BillingReference
                    DocumentoReferenciado DocumentoReferenciado2 = new DocumentoReferenciado
                    {
                        codigoInterno = "5",
                        cufeDocReferenciado = dsImprimir.Tables[6].Rows[0]["facufe"].ToString().Trim(),
                        fecha = Convert.ToDateTime(dsImprimir.Tables[6].Rows[0]["fechafactu"].ToString().Trim()).ToString("yyyy-MM-dd"),
                        numeroDocumento = dsImprimir.Tables[6].Rows[0]["numerfactu"].ToString().Trim()
                    };
                    #endregion
                    facturaDemo.documentosReferenciados[1] = DocumentoReferenciado2;
                }
                #endregion

                #region impuestosGenerales

                int reg = dsImprimir.Tables[2].Rows.Count;
                facturaDemo.impuestosGenerales = new FacturaImpuestos[reg];

                for (int j = 0; j < reg; j++)
                {
                    FacturaImpuestos impuestoGeneral1 = new FacturaImpuestos
                    {
                        baseImponibleTOTALImp = dsImprimir.Tables[2].Rows[j]["base"].ToString().Trim(),
                        codigoTOTALImp = "01",
                        porcentajeTOTALImp = dsImprimir.Tables[2].Rows[j]["por_iva"].ToString().Trim(),
                        unidadMedida = "94",
                        valorTOTALImp = dsImprimir.Tables[2].Rows[j]["val_iva"].ToString().Trim()
                    };
                    facturaDemo.impuestosGenerales[j] = impuestoGeneral1;
                }

                #endregion

                #region impuestosTotales
                DataRow[] porivas = dsImprimir.Tables[2].Select("por_iva>0");
                facturaDemo.impuestosTotales = new ImpuestosTotales[porivas.Length];

                int k = 0;
                foreach (DataRow rows in porivas)
                {
                    ImpuestosTotales impuestoGeneralTOTAL1 = new ImpuestosTotales
                    {
                        codigoTOTALImp = "01",
                        montoTotal = rows["val_iva"].ToString().Trim()
                    };
                    facturaDemo.impuestosTotales[k] = impuestoGeneralTOTAL1;
                    k++;
                }
                #endregion

                #region mediosDePago
                facturaDemo.mediosDePago = new MediosDePago[1];
                MediosDePago medioPago1 = new MediosDePago
                {
                    medioPago = "10",
                    metodoDePago = "2",
                    numeroDeReferencia = "01",
                    fechaDeVencimiento = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_ven"].ToString().Trim()).ToString("yyyy-MM-dd")
                };
                facturaDemo.mediosDePago[0] = medioPago1;
                #endregion

                #region rango numeracion


                facturaDemo.moneda = "COP";

                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracion_"].ToString().Trim();
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracionc_"].ToString().Trim();
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracionc_"].ToString().Trim();

                facturaDemo.redondeoAplicado = "0.00";

                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    facturaDemo.tipoDocumento = "01";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    facturaDemo.tipoDocumento = "91";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    facturaDemo.tipoDocumento = "91";
                #endregion

                decimal des_bonficado = Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["des_bonficado"]);
                decimal tot_tot = Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["tot_tot"]);
                decimal base_fac = Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["base"]);
                decimal monto = tot_tot - des_bonficado;

                int numitems = dsImprimir.Tables[1].Rows.Count;

                facturaDemo.tipoOperacion = "10";
                facturaDemo.totalProductos = numitems.ToString();


                facturaDemo.totalBaseImponible = dsImprimir.Tables[2].Compute("SUM(base)", "").ToString().Trim();
                facturaDemo.totalBrutoConImpuesto = dsImprimir.Tables[2].Compute("SUM(tot_tot)", "").ToString().Trim();
                facturaDemo.totalMonto = dsImprimir.Tables[2].Compute("SUM(tot_tot)", "").ToString().Trim();
                facturaDemo.totalSinImpuestos = dsImprimir.Tables[2].Compute("SUM(base)", "").ToString().Trim();



                return facturaDemo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error en la construccion de la factura:" + ex.Message, "BuildFactrua1");
                MessageBox.Show(ex.StackTrace.ToString(), "BuildFactrua2");
                return null;

            }
        }
        //#endregion


        #region Enviar (Web Service SOAP Emisión)
        //        private void BtnEnviar_Click(object sender, EventArgs e)
        private async void Enviando()
        {
            try
            {                
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    Tipo_Documento = "Factura";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    Tipo_Documento = "Nota Credito";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    Tipo_Documento = "Nota Credito";

                FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura

                if (factura == null)
                {
                    MessageBox.Show("Error en creacion de factura..", "BuildFactura");
                    return;
                }

                factura.fechaEmision = tbxFechaEmision.Text.Trim();
                factura.fechaVencimiento = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_ven"].ToString().Trim()).ToString("yyyy-MM-dd");

                string ArchivoRequest = "Tmp/" + factura.consecutivoDocumento.Trim() + ".txt";
                StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
                XmlSerializer Serializer1 = new XmlSerializer(typeof(FacturaGeneral));
                

                Serializer1.Serialize(MyFile, factura); // Objeto serializado
                MyFile.Close();
                Task<DocumentResponse> docRespuesta;
                rtxInformacion.Clear();

                rtxInformacion.Text = "Envio de Factura:" + Environment.NewLine;
                string cantidadAnexos = dsImprimir.Tables[4].Rows[0]["ad_junto"].ToString().Trim();                
                string trnenviar = dsImprimir.Tables[0].Rows[0]["cod_trn"].ToString();
                if (trnenviar == "007" || trnenviar == "008") cantidadAnexos = "0";


                if (MessageBox.Show("Confirmar envio ?", "Enviando documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    sfBusyIndicatorEstado.IsBusy = true;
                    GridMain.IsEnabled = false;
                    GridMain.Opacity = 0.5;

                    docRespuesta = serviceClienteEnvio.EnviarAsync(tokenEmpresa, tokenAuthorizacion, factura, "0");
                    await docRespuesta;


                    if (docRespuesta.IsCompleted)
                    {
                        sfBusyIndicatorEstado.IsBusy = false;
                        GridMain.IsEnabled = true;
                        GridMain.Opacity = 1;

                        StringBuilder msgError = new StringBuilder();
                        if (docRespuesta.Result.mensajesValidacion != null)
                        {
                            int nReturnMsg = docRespuesta.Result.mensajesValidacion.Count();
                            for (int i = 0; i < nReturnMsg; i++)
                            {
                                msgError.Append(docRespuesta.Result.mensajesValidacion[i].ToString() + Environment.NewLine);
                            }
                        }

                        if (docRespuesta.Result.codigo == 114)  //documento emitdo previa mente
                        {
                            DocumentStatusResponse resp = serviceClienteEnvio.EstadoDocumento(tokenEmpresa, tokenAuthorizacion, factura.consecutivoDocumento.ToString());
                            if (resp.codigo == 200)
                            {
                                rtxInformacion.Text = "ReEnvio de Factura emitido previa mente:" + docRespuesta.Result.codigo + Environment.NewLine;
                                ActualizaDocFacturaElectronicaRespuesta(resp);
                                rtxInformacion.Text += "Codigo: " + resp.codigo.ToString() + Environment.NewLine +
                               "Consecutivo Documento: " + resp.consecutivo + Environment.NewLine +
                               "Cufe: " + resp.cufe + Environment.NewLine +
                               "Mensaje: " + resp.mensaje + Environment.NewLine +
                               "Resultado: " + resp.resultado + Environment.NewLine + Environment.NewLine;

                                return;
                            }
                        }

                        //envio factura 
                        if (docRespuesta.Result.codigo == 200 || docRespuesta.Result.codigo == 201)
                        {
                            ActualizaDocFacturaElectronica(docRespuesta.Result);
                            this.rtxInformacion.Text += "Codigo: " + docRespuesta.Result.codigo.ToString() + Environment.NewLine +
                                 "Consecutivo Documento: " + docRespuesta.Result.consecutivoDocumento + Environment.NewLine +
                                 "Cufe: " + docRespuesta.Result.cufe + Environment.NewLine +
                                 "Mensaje: " + docRespuesta.Result.mensaje + Environment.NewLine +
                                 "Resultado: " + docRespuesta.Result.resultado + Environment.NewLine;
                            //this.Close();
                        }
                        else
                        {
                            StringBuilder response = new StringBuilder();
                            response.Append("x Codigo x:" + docRespuesta.Result.codigo.ToString() + Environment.NewLine);
                            response.Append("Consecutivo Documento :" + docRespuesta.Result.consecutivoDocumento + Environment.NewLine);
                            response.Append("Mensaje :" + docRespuesta.Result.mensaje + Environment.NewLine);
                            response.Append("Resultado :" + docRespuesta.Result.resultado + Environment.NewLine);
                            response.Append("Errores :" + msgError.ToString() + Environment.NewLine);


                            if (docRespuesta.Result.reglasValidacionDIAN != null)
                            {
                                for (int i = 0; i < docRespuesta.Result.reglasValidacionDIAN.Count(); i++)
                                {
                                    response.Append("DIAN:" + docRespuesta.Result.reglasValidacionDIAN[i].ToString() + Environment.NewLine);
                                }
                            }

                            rtxInformacion.Text += response.ToString();
                        }

                    }
                    else
                    {
                        rtxInformacion.Text = "Proceso cancelado";
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sfBusyIndicatorEstado.IsBusy = false;
                GridMain.IsEnabled = true;
                GridMain.Opacity = 1;
            }

        }
        #endregion

        #region CargarAdjuntos (Web Service SOAP Adjuntos)


        private int EnviarArchivosAdjuntos(int numeroDeArchivos, DocumentResponse docInfo)
        {

            //if (numeroDeArchivos <= 0) return 0;
            //SiaWin.Func.ImprimeLoteFacturas(dsImprimir.Tables[0], codpvt, codpvt, false, idemp, true);

            //string filepdf = docInfo.consecutivoDocumento.Trim() + ".PDF";
            //int procesados = 0;
            //for (int i = 0; i < numeroDeArchivos; i++)
            //{
            //    // archivo a trasnmitir es el archivo pdf creado 
            //    FileInfo file = new FileInfo(filepdf);
            //    if (file.Exists)
            //    {
            //        //MessageBox.Show("eXISTE");
            //        BinaryReader bReader = new BinaryReader(file.OpenRead());
            //        byte[] anexByte = bReader.ReadBytes((int)file.Length);
            //        //anexB64 = Convert.ToBase64String(anexByte);
            //        CargarAdjuntos uploadAttachment = new CargarAdjuntos
            //        {
            //            archivo = anexByte,
            //            numeroDocumento = docInfo.consecutivoDocumento
            //        };
            //        string[] correoEntrega = new string[1];
            //        correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim(); ;
            //        uploadAttachment.email = correoEntrega;
            //        //MessageBox.Show(file.Name.Substring(0, file.Name.Length - 4).ToString());
            //        uploadAttachment.nombre = file.Name.Substring(0, file.Name.Length - 4);

            //        //uploadAttachment.formato = file.Extension.Substring(1);
            //        uploadAttachment.formato = "pdf";
            //        uploadAttachment.numeroDocumento = docInfo.consecutivoDocumento.Trim();
            //        //MessageBox.Show(uploadAttachment.formato.ToString());
            //        //string ArchivoRequest = docInfo.consecutivoDocumento.Trim() + "Adjunto.txt";
            //        //StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
            //        //XmlSerializer Serializer1 = new XmlSerializer(typeof(CargarAdjuntos));
            //        //Serializer1.Serialize(MyFile, uploadAttachment); // Objeto serializado
            //        //MyFile.Close();


            //        uploadAttachment.tipo = "2";
            //        uploadAttachment.enviar = "1";
            //        //                    if (i + 1 == 1)
            //        //                  {
            //        //                    uploadAttachment.enviar = "1";
            //        //              }
            //        //            else
            //        //          {
            //        //            uploadAttachment.enviar = "0";
            //        //      }
            //        //    uploadAttachment.enviar = "1";


            //        StringBuilder msgError = new StringBuilder();
            //        UploadAttachmentResponse fileRespuesta = serviceClientAdjunto.CargarAdjuntos(tokenEmpresa, tokenAuthorizacion, uploadAttachment);
            //        if (fileRespuesta.mensajesValidacion != null)
            //        {
            //            //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
            //            int nReturnMsg = fileRespuesta.mensajesValidacion.Count();
            //            for (int ii = 0; ii < nReturnMsg; ii++)
            //            {
            //                //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
            //                msgError.Append(fileRespuesta.mensajesValidacion[ii].ToString() + Environment.NewLine);
            //            }
            //        }

            //        if (fileRespuesta.codigo == 200 || fileRespuesta.codigo == 201)
            //        {
            //            rtxInformacion.Text += "Archivo: " + file.Name + " procesado correctamente" + Environment.NewLine + msgError.ToString(); ;
            //            procesados++;
            //        }
            //        else
            //        {

            //            rtxInformacion.Text += "Archivo: " + file.Name + " - no fue transmitido" + Environment.NewLine+ fileRespuesta.mensaje.ToString();
            //        }
            //    }
            //    else
            //    {
            //        rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
            //        //no debería entrar a este ciclo
            //    }

            //}
            //return procesados;
            return 1;
        }
        private int EnviarArchivosAdjuntosRespuesta(int numeroDeArchivos, DocumentStatusResponse docInfo)
        {

            if (numeroDeArchivos <= 0) return 0;
            SiaWin.Func.ImprimeLoteFacturas(dsImprimir.Tables[0], codpvt, codpvt, false, idemp, true);
            //SiaWin.Func.ImprimeFacturaCredito(idrowcab, codpvt,codpvt, false, totalFac, tituloPie, idEmp, false);
            string filepdf = docInfo.consecutivo.Trim() + ".PDF";
            int procesados = 0;
            for (int i = 0; i < numeroDeArchivos; i++)
            {
                // archivo a trasnmitir es el archivo pdf creado 
                FileInfo file = new FileInfo(filepdf);
                if (file.Exists)
                {
                    //MessageBox.Show("eXISTE");
                    BinaryReader bReader = new BinaryReader(file.OpenRead());
                    byte[] anexByte = bReader.ReadBytes((int)file.Length);
                    //anexB64 = Convert.ToBase64String(anexByte);
                    CargarAdjuntos uploadAttachment = new CargarAdjuntos
                    {
                        archivo = anexByte,
                        numeroDocumento = docInfo.consecutivo
                    };
                    string[] correoEntrega = new string[1];
                    correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim(); ;
                    uploadAttachment.email = correoEntrega;
                    //MessageBox.Show(file.Name.Substring(0, file.Name.Length - 4).ToString());
                    uploadAttachment.nombre = file.Name.Substring(0, file.Name.Length - 4);

                    //uploadAttachment.formato = file.Extension.Substring(1);
                    uploadAttachment.formato = "pdf";
                    uploadAttachment.numeroDocumento = docInfo.consecutivo.Trim();
                    //MessageBox.Show(uploadAttachment.formato.ToString());
                    //string ArchivoRequest = docInfo.consecutivo.Trim() + "Adjunto.txt";
                    //StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
                    //XmlSerializer Serializer1 = new XmlSerializer(typeof(CargarAdjuntos));
                    //Serializer1.Serialize(MyFile, uploadAttachment); // Objeto serializado
                    //MyFile.Close();


                    uploadAttachment.tipo = "2";
                    uploadAttachment.enviar = "1";
                    //                    if (i + 1 == 1)
                    //                  {
                    //                    uploadAttachment.enviar = "1";
                    //              }
                    //            else
                    //          {
                    //            uploadAttachment.enviar = "0";
                    //      }
                    //    uploadAttachment.enviar = "1";


                    StringBuilder msgError = new StringBuilder();
                    UploadAttachmentResponse fileRespuesta = serviceClientAdjunto.CargarAdjuntos(tokenEmpresa, tokenAuthorizacion, uploadAttachment);
                    if (fileRespuesta.mensajesValidacion != null)
                    {
                        //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
                        int nReturnMsg = fileRespuesta.mensajesValidacion.Count();
                        for (int ii = 0; ii < nReturnMsg; ii++)
                        {
                            //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
                            msgError.Append(fileRespuesta.mensajesValidacion[ii].ToString() + Environment.NewLine);
                        }
                    }

                    if (fileRespuesta.codigo == 200 || fileRespuesta.codigo == 201)
                    {
                        rtxInformacion.Text += "Archivo: " + file.Name + " procesado correctamente" + Environment.NewLine + msgError.ToString(); ;
                        procesados++;
                    }
                    else
                    {

                        rtxInformacion.Text += "Archivo: " + file.Name + " - no fue transmitido" + Environment.NewLine + fileRespuesta.mensaje.ToString();
                    }
                }
                else
                {
                    rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                    //no debería entrar a este ciclo
                }

            }
            return procesados;
        }

        #endregion


        public bool LoadData(int idregdoc, string codpvta, string cn)
        {
            try
            {
                // retorna tablas 0 = cabeza factura y datos del cliente
                // 1 = cuerpo de factura y tarifas de iva
                // 2 = totales de factura factura y tarifas de iva
                // 3 = formas de pago
                // 4 = informacion del punto de venta
                // 5 = informacion config

                SqlConnection con = new SqlConnection(cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet dsImprimir = new DataSet();
                //PvFacturaElectronicaAnulacion
                cmd = new SqlCommand("_EmpPvFacturaElectronica", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@NumRegCab", idrowcab);//if you have parameters.
                cmd.Parameters.AddWithValue("@CodPvt", codpvt);//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);//if you have parameters.

                da = new SqlDataAdapter(cmd);
                dsImprimir.Clear();
                da.Fill(dsImprimir);
                tokenEmpresa = dsImprimir.Tables[5].Rows[0]["stockenemp_"].ToString().Trim();
                tokenAuthorizacion = dsImprimir.Tables[5].Rows[0]["stockenpas_"].ToString().Trim();


                if (string.IsNullOrEmpty(tokenEmpresa))
                {
                    System.Windows.MessageBox.Show("Token de empresa null o vacio");
                    return false;
                }
                if (string.IsNullOrEmpty(tokenAuthorizacion))
                {
                    System.Windows.MessageBox.Show("Token autorizacion  de empresa null o vacio");
                    return false;
                }

                int nItems = dsImprimir.Tables[0].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro en cabeza de documento..");
                    return false;
                }
                nItems = dsImprimir.Tables[1].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro en cuerpo de documento..");
                    return false;
                }
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005x")
                {
                    nItems = dsImprimir.Tables[3].Rows.Count;
                    if (nItems <= 0)
                    {
                        System.Windows.MessageBox.Show("No hay registro en formas de pago en documento..");
                        return false;
                    }
                }
                nItems = dsImprimir.Tables[4].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro informacion punto de venta...");
                    return false;
                }
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro informacion Config...");
                    return false;
                }
                this.tbxnit.Text = dsImprimir.Tables[0].Rows[0]["cod_cli"].ToString().Trim();
                this.tbxnitReal.Text = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim();
                this.tbxDV.Text = dsImprimir.Tables[0].Rows[0]["dv"].ToString().Trim();
                this.tbxnombre.Text = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                this.tbxEmail.Text = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim().ToUpper(); ;
                this.tbxFechaEmision.Text = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                this.txtNumFactura.Text = dsImprimir.Tables[0].Rows[0]["num_trn"].ToString().Trim();
                this.txtNumCiudad.Text = dsImprimir.Tables[0].Rows[0]["ciudad"].ToString().Trim();
                this.txtNumCodeCiudad.Text = dsImprimir.Tables[0].Rows[0]["cod_ciu"].ToString().Trim();
                this.txtDireccion.Text = dsImprimir.Tables[0].Rows[0]["dir1"].ToString().Trim();

                string tdoc = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();
                string nomtdo = dsImprimir.Tables[0].Rows[0]["nom_tdo"].ToString().Trim();
                tbxTdoc.Text = tdoc + "-" + nomtdo;


                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "LoadData");
            }
            return false;

        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Facturacion electronica SiaApp" + cod_empresa + "-" + nomempresa;

                string query = "select * from incab_doc where idreg='" + idrowcab + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "transacciones", idemp);
                if (dt.Rows.Count > 0)
                {
                    string cod_trn = dt.Rows[0]["ano_doc"].ToString();
                    string num_anu = dt.Rows[0]["num_anu"].ToString();
                    if (cod_trn == "005")
                    {
                        int año = Convert.ToInt32(dt.Rows[0]["ano_doc"]);
                        if (año <= 2019)
                        {
                            BtnEnviar.Visibility = Visibility.Collapsed;
                            BtnImprimir.Content = "IMPRIMIR FACTURA ANTIGUA";
                            BtnImprimir.Width = 200;
                        }
                    }
                    else
                    {
                        string query_nota = "select * from incab_doc where num_trn='" + num_anu + "' and cod_trn='005';";
                        DataTable dt_nota = SiaWin.Func.SqlDT(query_nota, "transacciones", idemp);
                        if (dt_nota.Rows.Count > 0)
                        {
                            int año = Convert.ToInt32(dt_nota.Rows[0]["ano_doc"]);
                            if (año <= 2019)
                            {
                                BtnEnviar.Visibility = Visibility.Collapsed;
                                BtnImprimir.Content = "IMPRIMIR NC FACTURA ANTIGUA";
                                BtnImprimir.Width = 200;
                            }
                        }
                    }
                }


                //MessageBox.Show(SiaWin._cn);
                //LoadData(idrowcab,codpvt, SiaWin._cn);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                if (string.IsNullOrEmpty(tbxEmail.Text))
                {
                    MessageBox.Show("el campo email debe de estar lleno", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(txtNumCiudad.Text))
                {
                    MessageBox.Show("el campo ciudad debe de estar lleno", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(txtNumCodeCiudad.Text))
                {
                    MessageBox.Show("el campo codigo de ciudad debe de estar lleno", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(txtDireccion.Text))
                {
                    MessageBox.Show("la direccion debe de estar llena", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion



                bool RedActiva = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                if (RedActiva == false)
                {
                    MessageBox.Show("conectese a internet para enviar la factura electronicamente");
                    return;
                }


                port = null;
                serviceClienteEnvio = null;
                port = new BasicHttpBinding();
                //SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();
                serviceClienteEnvio = new SrvEnvio.ServiceClient();

                //FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura
                //if(factura==null)
                //{
                //  MessageBox.Show("error en BuildFactura, retorno null");
                //}
                if (Validacion())
                    Enviando();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error en el envio:" + ex.Message);
                // MessageBox.Show(ex.StackTrace.ToString());

            }
        }
        private bool Validacion()
        {
            try
            {

                ///validar datos del cliente

                ///
                ///validar valores de factura
                ///

                ///// fddfsjsdfjsdsdjjssd
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;

        }

        public void ActualizaDocFacturaElectronica(DocumentResponse resp)
        {
            string numdocele = resp.consecutivoDocumento;
            string cufe = resp.cufe.Trim();
            string fecharesp = resp.fechaRespuesta.ToString();
            string msg = resp.mensaje;
            string code = resp.codigo.ToString();
            DateTime dtime = DateTime.Now;

            if (!string.IsNullOrEmpty(fecharesp))
            {
                dtime = Convert.ToDateTime(fecharesp);
            }
            /// envia a base de datos en cabeza de documento
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    string sqlcab = string.Empty;
                    if (!string.IsNullOrEmpty(fecharesp))
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe='" + cufe + "',fa_msg='" + msg + "',fa_fecharesp='" + dtime.ToString() + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    else
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe ='" + cufe + "',fa_msg='" + msg + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    this.Cufe = cufe;
                    this.Codigo = code;

                    connection.Close();

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

            }
        }
        public void ActualizaDocFacturaElectronicaRespuesta(DocumentStatusResponse resp)
        {
            string numdocele = resp.consecutivo;
            string cufe = resp.cufe.Trim();
            string fecharesp = resp.fechaDocumento.ToString();
            string msg = resp.mensaje;
            string code = resp.codigo.ToString();
            DateTime dtime = DateTime.Now;

            if (!string.IsNullOrEmpty(fecharesp))
            {
                dtime = Convert.ToDateTime(fecharesp);
            }
            /// envia a base de datos en cabeza de documento
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    string sqlcab = string.Empty;
                    if (!string.IsNullOrEmpty(fecharesp))
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe='" + cufe + "',fa_msg='" + msg + "',fa_fecharesp='" + dtime.ToString() + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    else
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe ='" + cufe + "',fa_msg='" + msg + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }

                    //MessageBox.Show("sqlcab:"+ sqlcab);

                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    this.Cufe = cufe;
                    this.Codigo = code;

                    connection.Close();

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                Load(idrowcab, codpvt, SiaWin._cn);
            }
            catch (Exception w)
            {
                MessageBox.Show("error loaded:" + w);
            }

            //if (!LoadData(idrowcab, codpvt, SiaWin._cn))
            //{
            //    MessageBox.Show("Error al cargar los datos del documento....");
            //    this.Close();
            //    return;
            //}

        }

        public void Load(int idrowcab, string codpvt, string cn)
        {
            try
            {
                if (!LoadData(idrowcab, codpvt, cn))
                {
                    MessageBox.Show("Error al cargar los datos del documento....");
                    this.Close();
                    return;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar");
            }
        }


        private Boolean ValidaEmail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            Cufe = "A";
            Codigo = "200";
            this.Close();
        }

        private void BtnTerceros_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(tbxnitReal.Text)) return;
                string[] strArrayParam = new string[] { tbxnitReal.Text, "", idemp.ToString(), "1" };   //ultimo parametro es modificiando 0=creando 1=modific
                SiaWin.Tab(9225, strArrayParam, null);
                //9231


                //Load(idrowcab, codpvt, SiaWin._cn);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la maestra de terceros:" + w);
            }
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Load(idrowcab, codpvt, SiaWin._cn);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al recargar:" + w);
            }
        }

        private async void BtnRenviarPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                rtxInformacion.Text = "Envio de Factura:" + Environment.NewLine;

                string ruta = AppDomain.CurrentDomain.BaseDirectory + "/Tmp";
                string numtrn = txtNumFactura.Text;

                sfBusyIndicatorEstado.IsBusy = true;
                sfBusyIndicatorEstado.Header = "Cargado Factura como Adjunto";
                GridMain.IsEnabled = false;
                GridMain.Opacity = 0.5;


                Task<SendEmailResponse> docRespuesta;
                docRespuesta = serviceClienteEnvio.EnvioCorreoAsync(tokenEmpresa, tokenAuthorizacion, numtrn, tbxEmail.Text, "0");
                await docRespuesta;

                if (docRespuesta.IsCompleted)
                {

                    StringBuilder response = new StringBuilder();
                    response.Append("x Codigo x:" + docRespuesta.Result.codigo.ToString() + Environment.NewLine);
                    response.Append("Mensaje :" + docRespuesta.Result.mensaje + Environment.NewLine);
                    response.Append("Resultado :" + docRespuesta.Result.resultado + Environment.NewLine);
                    rtxInformacion.Text += response.ToString();
                }

                sfBusyIndicatorEstado.IsBusy = false;
                sfBusyIndicatorEstado.Header = "Enviando .......";
                GridMain.IsEnabled = true;
                GridMain.Opacity = 1;


                rtxInformacion.Text += "** FIN ** " + Environment.NewLine;


            }
            catch (Exception w)
            {
                MessageBox.Show("error al reenviar adjunto:" + w);
                sfBusyIndicatorEstado.IsBusy = false;
                GridMain.IsEnabled = true;
                GridMain.Opacity = 1;
            }
        }



    }
}
