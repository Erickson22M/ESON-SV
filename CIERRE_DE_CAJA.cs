using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Data.SqlClient;

using System.Globalization;
using System.Threading;
using SV_EDSON.Datos;
namespace SV_EDSON.Presentacion.CAJA
{
    public partial class CIERRE_DE_CAJA : Form
    {
        public CIERRE_DE_CAJA()
        {
            InitializeComponent();
        }
        public static double dineroencaja;
        int idcaja;
        DateTime fechaInicial;
        public static  double saldoInicial;
        DateTime fechafinal=DateTime.Now ;
        public static double ventasefectivo;
        public static double ingresosefectivo;
        public static double gastosefectivo;
        public static double ventascredito;
        public static double ventastarjeta;
        //---
        double efectivoEnCaja;
        public static  double ventastotales;
        double creditosPorPagar;
        double creditosPorCobrar;
        public static double ganancias;
        public static double Ingresos;
        public static double Egresos;
        public static double cobrosEfectivo;
        public static double cobrosTarjeta;
        public static double CobrosTotal;

        private void CIERRE_DE_CAJA_Load(object sender, EventArgs e)
        {
            Mostrar_cierres_de_caja_pendiente();
            lbldesdehasta.Text = "Corte de caja desde: " + fechaInicial + " Hasta: " + DateTime.Now;
            obtener_saldo_inicial();//calculo
            obtener_ventas_en_efectivo();//calculo
            obtener_gastos_por_turno();//calculo
            obtener_ingresos_por_turno();//calculo
            obtener_creditosPorPagar();//calculo
            mostrar_cobros_efectivo_por_turno();//calculo
            mostrar_cobros_tarjeta_por_turno(); //calculo
            sumar_CreditoPorCobrar();//calculo
            M_ventas_Tarjeta_por_turno();//calculo
            M_ventas_credito_por_turno();//calculo
            calcular(); //porceso para calcular diferentes datos que se mostraran al cerrar l caja
        }
        private void calcular() //proceso para calcular los diferentes calculos que se mostraran en el cierre de caja
        {
            CobrosTotal = cobrosEfectivo + cobrosTarjeta;
            efectivoEnCaja = saldoInicial + ventasefectivo + ingresosefectivo - gastosefectivo+cobrosEfectivo+cobrosTarjeta;
            ventastotales = ventasefectivo + ventascredito + ventastarjeta;
            //---Mostraremos en los labels correspondientes los datos o calculos realizados
            lblDineroEncaja.Text = efectivoEnCaja.ToString();
            lblVentasTotal.Text = ventastotales.ToString();
            lbltotalventas.Text = ventastotales.ToString();
            lbldineroTotalCaja.Text = efectivoEnCaja.ToString();
            Ingresos = saldoInicial + ventasefectivo + ingresosefectivo+ventastarjeta+cobrosTarjeta+cobrosEfectivo;
            Egresos = gastosefectivo;
            //lblgananciasVentas= ventastotales-
        }
        private void obtener_ingresos_por_turno()
        {
            //proceso para obtener ingresos por turno con el cual ocuparemos parametros como fi y ff
            Obtener_datos.sumar_ingresos_por_turno(idcaja, fechaInicial, fechafinal, ref ingresosefectivo);
            lblingresos.Text = ingresosefectivo.ToString ();
        }
        private void obtener_gastos_por_turno()
        {
            //obtenemos los gastos por turnos desde la apertura de una caja hasta su cierre
            Obtener_datos.sumar_gastos_por_turno(idcaja, fechaInicial, fechafinal, ref gastosefectivo);
            lblgastos.Text = gastosefectivo.ToString();
        }
        private void obtener_ventas_en_efectivo()
        {
            //proceso para calcular las ventas en efectivo con el cual pasamos como ref una variable para capturar el dato
            Obtener_datos.mostrar_ventas_en_efectivo_por_turno(idcaja, fechaInicial, fechafinal, ref ventasefectivo);
            lblventasefectivo.Text = ventasefectivo.ToString();//guardamos el dato capturado en la variable y lo mostramos en el label
            lblventasefectivoGeneral.Text = ventasefectivo.ToString(); //pasamos este mismo dato a otro label donde se calculan el total de ventas
        }
        private void obtener_saldo_inicial() //proceso para obtener el saldo inicial con el que se aperturo la caja
        {
            lblfondodeCaja.Text =saldoInicial.ToString();  //mostramos este dato en el label
        }
        private void obtener_creditosPorPagar()
        {
            //calculo de creditos por pagar, pasamos parametros y referencia a utilizar
            Obtener_datos.sumar_CreditoPorPagar(idcaja, fechaInicial, fechafinal, ref creditosPorPagar);
            lblPorpagar.Text = creditosPorPagar.ToString (); //mostramos el dato capturado en su label
        }
        //calculo de creditos por cobrar, pasamos parametros y referencia a utilizar
        private void sumar_CreditoPorCobrar()
        {
            Obtener_datos.sumar_CreditoPorCobrar(idcaja, fechaInicial, fechafinal, ref creditosPorCobrar);
            lblPorCobrar.Text = creditosPorCobrar.ToString();//mostramos el dato capturado en su label

        }
        private void Mostrar_cierres_de_caja_pendiente()
        {
            //proceso para mostrar todos los datos con los que se esta trabajando la caja que sigue aperturada
            //este proceso seguira hasta su respectivo cierre
            DataTable dt = new DataTable();
            Obtener_datos.mostrar_cierre_de_caja_pendiente(ref dt);
            foreach (DataRow dr in dt.Rows)
            {
                idcaja = Convert.ToInt32(dr["Id_caja"]);
                fechaInicial = Convert.ToDateTime(dr["fechainicio"]);
                saldoInicial = Convert.ToDouble(dr["SaldoInicial"]);
            }
        }

        //procesos de calculo de los diferentes datos que se mostraran en el cierre de caja
        private void M_ventas_Tarjeta_por_turno()
        {
            Obtener_datos.M_ventas_Tarjeta_por_turno(idcaja, fechaInicial, fechafinal, ref ventastarjeta);
            lblventas_Tarjeta.Text = ventastarjeta.ToString ();
        }
        private void mostrar_cobros_efectivo_por_turno()
        {
            Obtener_datos.mostrar_cobros_en_efectivo_por_turno(idcaja, fechaInicial, fechafinal, ref cobrosEfectivo);
            lblabonosEfectivo.Text = cobrosEfectivo.ToString();
        }
        private void mostrar_cobros_tarjeta_por_turno()
        {
            Obtener_datos.mostrar_cobros_tarjeta_por_turno(idcaja, fechaInicial, fechafinal, ref cobrosTarjeta);
            lblabonosTarjeta.Text = cobrosTarjeta.ToString();
        }
        private void M_ventas_credito_por_turno()
        {
            Obtener_datos.M_ventas_credito_por_turno(idcaja, fechaInicial, fechafinal, ref ventascredito);
            lblVentasAcredito.Text = ventascredito.ToString ();
        }

        private void Label20_Click(object sender, EventArgs e)
        {

        }

        private void BtnCerrar_turno_Click(object sender, EventArgs e)
        {          
            //proceso para cerrar el turno, llamamos al form cieereturno, pasamos el total de dinero en caja y cerramos el proceso
            CierreTurno frm = new CierreTurno();
            dineroencaja =Convert.ToDouble ( lblDineroEncaja.Text);
            frm.ShowDialog();
        }

        private void CIERRE_DE_CAJA_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void btnvolver_Click(object sender, EventArgs e)
        {
            Dispose(); //cerramos la pestana en la que estabamos y volvemos a la anterior 
            VENTAS_MENU_PRINCIPAL.VENTAS_MENU_PRINCIPALOK frm = new VENTAS_MENU_PRINCIPAL.VENTAS_MENU_PRINCIPALOK();
            frm.ShowDialog();
        }

        private void lblPAGOSEfectivo_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
