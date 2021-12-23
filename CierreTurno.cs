using SV_EDSON.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SV_EDSON.Logica;
namespace SV_EDSON.Presentacion.CAJA
{
    public partial class CierreTurno : Form
    {
        public CierreTurno()
        {
            InitializeComponent();
        }
        string correobase;
        string contraseña;
        string estado;
        double dinerocalculado;
        double resultado;
        string correoReceptor;
        int idusuario;
        int idcaja;
        string usuario;
        
        private void CierreTurno_Load(object sender, EventArgs e)
        {
            lblDeberiaHaber.Text = CIERRE_DE_CAJA.dineroencaja.ToString ();
            dinerocalculado = Convert.ToDouble ( lblDeberiaHaber.Text);
            mostrarCorreoBase();
            mostrarcorreodeEnvio();
            mostrarUsuarioSesion();
        }
        private void mostrarUsuarioSesion()
        {
            DataTable dt = new DataTable(); 
            Obtener_datos.mostrarUsuariosSesion(ref dt);//pasamos el dt como referencia
            foreach (DataRow row in dt.Rows ) //recorremos todas las filas para obttener los usuarios
            {
                usuario = row["Nombres_y_Apellidos"].ToString(); //pasamos el campo nombre y apellidos que queremos mostrar
            }
        }
        public void mostrarcorreodeEnvio() //proceso para mostar el correo con el cual se configuro la empresa
        {
            DataTable dt = new DataTable();
            Obtener_datos.mostrar_empresa(ref dt);
            foreach (DataRow row in dt.Rows)
            {
                correoReceptor = row["Correo_para_envio_de_reportes"].ToString();
                txtcorreo.Text = correoReceptor;
            }
        }
        //proceso con el cual configuraremos el correo base con el cual servira como emisor de los msj
       private void mostrarCorreoBase()
        {
            DataTable dt = new DataTable();
            Obtener_datos.mostrarCorreoBase(ref dt);
           foreach (DataRow row in dt.Rows ) //recorremos las filas del dt para capturar estos 3 datos
            {
                estado =Bases.Desencriptar(row["EstadoEnvio"].ToString());
                correobase= Bases.Desencriptar(row["Correo"].ToString());
                contraseña = Bases.Desencriptar(row["Password"].ToString());         
            }
           if (estado =="Sincronizado") //si se realizo correctamente la sincronizacion
            {
                checkCorreo.Checked = true; //rellenamos el chek
            }
           else
            {
                checkCorreo.Checked = false ; //sino fue asi seguira sin rellenar

            }
        }
        private void txthay_TextChanged(object sender, EventArgs e)
        {
            calcular(); //realizamos el proceso de calculo de los datos
           
        }
        //validaciones para determinar el cierre de caja si considio o no con el dinero en caja
        private void validacionesCalculo()
        {
            if (resultado==0) //si todo cuadra bien
            {
                lblanuncio.Text = "Genial, Todo esta perfecto";
                lblanuncio.ForeColor = Color.FromArgb(0, 166, 63);
                lbldiferencia.ForeColor = Color.FromArgb(0, 166, 63);
                lblanuncio.Visible = true;

            }
            //si hay menos dinero en caja que lo que muestra el cierre y si este es diferente de 0 informaremos ya que es perdida
            if (resultado < dinerocalculado & resultado !=0)
            {
                lblanuncio.Text = "La diferencia sera Registrada en su Turno y se enviara a Gerencia";
                lblanuncio.ForeColor = Color.FromArgb(231, 63, 67);
                lbldiferencia.ForeColor = Color.FromArgb(231, 63, 67);
                lblanuncio.Visible = true;

            }
            if(resultado > dinerocalculado) //si hay mas dinero en caja que lo calculado tambien se informara
            {
                lblanuncio.Text = "La diferencia sera Registrada en su Turno y se enviara a Gerencia";
                lblanuncio.ForeColor = Color.FromArgb(231, 63, 67);
                lbldiferencia.ForeColor = Color.FromArgb(231, 63, 67);
                lblanuncio.Visible = true;
            }
        }
        private void calcular() //proceso para calcular si el cieere de caja con lo que hay en caja considen 
        {
            try
            {

           
            double hay;
            hay = Convert.ToDouble(txthay.Text);
                if (string.IsNullOrEmpty( txthay.Text)) //si no hay dato en el txt entonces daremos un 0 por defecto
                {
                    hay = 0; //por degecto
                }        
            resultado = hay - dinerocalculado; //calcular la diferencia que hay si los cierres no coinciden
            lbldiferencia.Text = resultado.ToString (); //pasamos esta diferencia al lbl
            validacionesCalculo(); //realizamos validacion para saber si cuadra o no
            }
            catch (Exception)
            {

             
            }
           
        }

        private void checkCorreo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkCorreo_Click(object sender, EventArgs e)
        {
            if(estado !="Sincronizado") //si el correo no esta sincronizado mostraremos el pagel de  conf correo para ingresar unp
            {
                Presentacion.CorreoBase.ConfigurarCorreo frm = new Presentacion.CorreoBase.ConfigurarCorreo();
                frm.FormClosing += Frm_FormClosing;
                frm.ShowDialog();//alcerrar el formulario ingresaremos de nuevo al mostrar correo
            }
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mostrarCorreoBase(); //mostramos el correo base con el que se trabajara
        }

        private void BtnCerrar_turno_Click(object sender, EventArgs e)
        {
            cerrarCaja();//caprturamos los datos y si todo salioi bien realizara el cierre de la caja

        }

        private void cerrarCaja()
        {
            //obtenemos el id de usuario y el idcaja a travez de los procesos de datos
            Obtener_datos.mostrar_inicio_De_sesion(ref idusuario);
            Obtener_datos.Obtener_id_caja_PorSerial(ref idcaja);

            Lmcaja parametros = new Lmcaja();
            Editar_datos funcion = new Editar_datos();
            parametros.fechafin = DateTime.Now;
            parametros.fechacierre = DateTime.Now;
            parametros.ingresos = CIERRE_DE_CAJA.Ingresos;
            parametros.egresos = CIERRE_DE_CAJA.Egresos;
            parametros.Saldo_queda_en_caja = 0;
            parametros.Id_usuario = idusuario;
            parametros.Total_calculado = dinerocalculado;
            parametros.Total_real = Convert.ToDouble (txthay.Text);//total que hay en caja
            parametros.Estado = "CAJA CERRADA"; //damos la caja como estado cerrado
            parametros.Diferencia = resultado; //pasamos el resultado del total que se espera menos lo que hay
            parametros.Id_caja = idcaja;
            //realizamos la funcion y comprobamos si se realizo y si no
           if (funcion.cerrarCaja(parametros)==true)
            {
                enviarcorreo();             
            }
        }
        
        private void enviarcorreo() //proceso para verificar si se enviara por correo el cierre o no
        {
            if (checkCorreo.Checked==true) //si esta el check relleno es porque se congiguro  ya el correo
            {
                ReemplazarHtml(); //reempralamos los datos del cieere al html
                bool estado;
                estado = Bases.enviarCorreo(correobase, contraseña, htmldeEnvio.Text, "Cierre de caja Edson SV", txtcorreo.Text, "");
                if (estado == true) //si el estado esta como true es xk se realizo correctamente la implementacion del correo
                {
                    MessageBox.Show("Reporte de cierre de caja enviado"); //mensaje ingormatico
                    generarCopiaBd(); //generamos las copias de seguirdad mostrando su form por si se desesa cancelar

                }
                else //si el estado no esta activo mandamos msj informando del error del envio y generamos la copia siempre
                {
                    MessageBox.Show("Error de envio al correo");
                    generarCopiaBd();
                }
                
            }
            else //si no esta seleccionado el chek del correo pues solamente se generara la cpoia
            {
                generarCopiaBd();
            }
               
        }
        private void generarCopiaBd()
        {
                Dispose();//finalizamos form anterios y iniciamos el form de copias
                CopiasBd.GeneradorAutomatico frm = new CopiasBd.GeneradorAutomatico();
                frm.ShowDialog();
        }
        //nos servira para la edicion y modificacion de los datos que se mostraran en el cierre de caja que se enviara por correo
        public void ReemplazarHtml()
        {
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@ventas_totales", CIERRE_DE_CAJA.ventastotales.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@Ganancias", CIERRE_DE_CAJA.ganancias.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@fecha", DateTime.Now.ToString () );
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@usuario_nombre", usuario);
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@fondo_caja", CIERRE_DE_CAJA.saldoInicial.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@ventas_efectivo", CIERRE_DE_CAJA.ventasefectivo.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@pagos", "0");
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@cobros", "0");
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@ingresosvarios", CIERRE_DE_CAJA.ingresosefectivo.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@gastosvarios", CIERRE_DE_CAJA.gastosefectivo.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@esperado", lblDeberiaHaber.Text);
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@vefectivo", CIERRE_DE_CAJA.ventasefectivo.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@vtarjeta", CIERRE_DE_CAJA.ventastarjeta.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@vcredito", CIERRE_DE_CAJA.ventascredito.ToString());
            htmldeEnvio.Text = htmldeEnvio.Text.Replace("@Tventas", CIERRE_DE_CAJA.ventastotales.ToString());

        }

       
    }
}
