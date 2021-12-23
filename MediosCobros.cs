using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SV_EDSON.Datos;
using SV_EDSON.Logica;
namespace SV_EDSON.Presentacion.Cobros
{
    public partial class MediosCobros : Form
    {
        public MediosCobros()
        {
            InitializeComponent();
        }
        double saldo;
        int idcliente;
        int idcaja;
        int idusuario;
        //variables para calculo
        double efectivo;
        double tarjeta;
        double vuelto;
        double restante;
        double efectivoCalculado;
        double montoabonado;
        private void MediosCobros_Load(object sender, EventArgs e)
        {
            saldo = CobrosForm.saldo; //utilizamos la variable del form cobros
            lbltotal.Text = saldo.ToString();
            idcliente = CobrosForm.idcliente;//utilizamos el idcliente del form cobros
            Obtener_datos.Obtener_id_caja_PorSerial(ref idcaja); //obtenemos el id de caja utilizada con la clase obtenerdatos
            Obtener_datos.mostrar_inicio_De_sesion(ref idusuario);//obtenemos el id del usuario que se esta utilizando
        }
        private void calcularRestante()  
        {
            try
            {
            efectivo = 0;
            tarjeta = 0;
            if (string.IsNullOrEmpty(txtefectivo2.Text))//si esta como vacio se guarda un 0 en la vaariable
            {
                efectivo = 0;
            }
            else
            {
                efectivo = Convert.ToDouble ( txtefectivo2.Text); //si se agrego un datos este se agregara a la variable efectivo

            }
            if (string.IsNullOrEmpty(txttarjeta2.Text)) //si el txt esta vacio igual tomara un valor 0
            {
                tarjeta = 0;
            }
            else
            {
                tarjeta = Convert.ToDouble(txttarjeta2.Text);//la variable targeta toma el valor que se agregue al txt
            }
            //calculo de vuelto 
            if(efectivo >saldo ) //si el monto dado en mayor al que se debe se realizara lo siguientte
            {
                vuelto = efectivo - saldo;   //sacamos el restante que sobra  
                efectivoCalculado = (efectivo - vuelto); // calculamos el total del monto a entrar en caja
                TXTVUELTO.Text = vuelto.ToString (); //le pasamos el vuelto al txtvuelvo para que se muestre
            }
            else
            {
                //si el pago es cabal, pasamos al efectivo calculado el monto total dado por el cliente 
                vuelto = 0;
                efectivoCalculado = efectivo; 
                TXTVUELTO.Text = vuelto.ToString();

                }

                //calculo del restante
                restante = saldo - efectivoCalculado - tarjeta; //calculamos si el cliente pago todo o le queda un restante en su saldo
                txtrestante.Text = restante.ToString(); //mostramos el total restante en el txtrestante
                
                if (restante <0)       //si el restante es menor a 0 es xk ya se pago toda la cuenta
                {
                    txtrestante.Visible = false; //ocultaremos el txtrestante
                    Label8.Visible = false; //ocultamos el label con el texto RESTANTE
                }
                else
                {
                    //SI ES MAYOR A 0 SI MOSTRAREMOS TANTO EL TXT CON EL RESTANTE COMO TAMBIEN EL LABEL
                    txtrestante.Visible = true;
                    Label8.Visible = true;
                }

                if (tarjeta ==saldo ) //si se realiza un pago mixto pero con la targeta se llega al saldo que se debe
                {
                    efectivo = 0; //igualaremos el pago a efectivo a 0 para cobrar todo en targeta
                    txtefectivo2.Text = efectivo.ToString ();// el txt efectivo negara el valor del pago efectivo dado y pondra un 0
                }
                if (tarjeta >saldo ) //el pago con targueta no puede pasarse del total del saldo 
                {
                    //si el valor dado es mayor reiniciamos el dato del txtargeta
                    MessageBox.Show("El pago con tarjeta no puede ser mayor que el saldo");
                    tarjeta = 0;
                    txttarjeta2.Text = tarjeta.ToString ();
                }

            }
            catch (Exception)
            {

            }                               
        }

        private void txtefectivo2_TextChanged(object sender, EventArgs e)
        {
            calcularRestante();
        }

        private void txttarjeta2_TextChanged(object sender, EventArgs e)
        {
            calcularRestante();

        }

        private void btncobrar_Click(object sender, EventArgs e)
        {
           montoabonado = efectivoCalculado + tarjeta;
            if(montoabonado>0) //si el monto abonado es mayor a 0 se procedera a los proceso de actualizacion de saldo
            {
                //procesos para disminuir las cuentas de los clientes cuando hagan pagos
            insertarControlCobro();
            disminuirSaldocliente();
            }
            else //sino es mayor a 0 le informamos al cliente
            {
                MessageBox.Show("Especifique un monto a abonar");
            }
        }
        private void insertarControlCobro() //proceso a utilizar cuando el usuario de click a abonar
        {
            //proceso para actualizar las cuentas de los clientes
            Lcontrolcobros parametros = new Lcontrolcobros(); //utilizaremos los parametros del lcontroncobros
            Insertar_datos funcion = new Insertar_datos(); //funcion insertarcontrolcobros de la clase insertar datos
            //pasamos los parametros que se piden en el lcontrolcobros
            parametros.Monto = efectivoCalculado + tarjeta;
            parametros.Fecha = DateTime.Now;
            parametros.Detalle = "Cobro a cliente";
            parametros.IdCliente = idcliente;
            parametros.IdUsuario = idusuario;
            parametros.IdCaja = idcaja;
            parametros.Comprobante = "-";
            parametros.efectivo = efectivoCalculado;
            parametros.tarjeta = tarjeta;
            if (funcion.Insertar_ControlCobros (parametros )==true) //si se ejecuto bien el proceso
            {
                Dispose(); //destruyimos el form para seguir con el sistema
            }
        }
        private void disminuirSaldocliente() //proceso para disminir las entradas que el cliente haga en sus cuentas
        {
            Lclientes parametros = new Lclientes();  //usamos la logica de lclientes
            Editar_datos funcion = new Editar_datos(); //funsion disminuirsaldoclientes de la clase editar datos
            parametros.idcliente = idcliente;//pasamos los parametros tanto el id como el monto a disminuir
            funcion.disminuirSaldocliente(parametros, montoabonado);

        }
    }
}
