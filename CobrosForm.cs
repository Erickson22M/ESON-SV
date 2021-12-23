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
    public partial class CobrosForm : Form
    {
        public CobrosForm()
        {
            InitializeComponent();
        }
        public static  int idcliente;
        public static    double saldo;
        private void Label21_Click(object sender, EventArgs e)
        {

        }

        private void txtclientesolicitante_TextChanged(object sender, EventArgs e)
        {
            buscar();
        }
        private void buscar()  //proceso para la busqueda de los clientes
        {
            DataTable dt = new DataTable();
            Obtener_datos.buscar_clientes(ref dt, txtclientesolicitante.Text); //pasamos el dt para mostrarse y el txtcliente donde se buscara
            datalistadoClientes.DataSource = dt;//pasamos los datos solicitados
            //ocultamos columnas con datos que no necesitamos solo mostraremos el nombre del cliente
            datalistadoClientes.Columns[0].Visible = false;
            datalistadoClientes.Columns[1].Visible = false; //id
            datalistadoClientes.Columns[3].Visible = false;//direccion
            datalistadoClientes.Columns[4].Visible = false;//iden.fiscal
            datalistadoClientes.Columns[5].Visible = false;//celular
            datalistadoClientes.Columns[6].Visible = false;//estado
            datalistadoClientes.Columns[7].Visible = false;//saldo
            datalistadoClientes.Columns[2].Width = datalistadoClientes.Width; //tamano de la columna igual al del datagrid
            datalistadoClientes.BringToFront(); //enfrente de todo
            datalistadoClientes.Visible = true;
            //locacion del dlclientes tomara como referencia el panelregistro tanto en x como en y
            datalistadoClientes.Location = new Point(panelRegistros.Location.X, panelRegistros.Location.Y);
            datalistadoClientes.Size= new Size (538, 220); //tamano
            panelRegistros.Visible = false;//el panel registro se ocultara hasta al hacer click en un cliente
        }

        private void datalistadoClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //cuando ya damos click a un cliente se ocultara el listado de clientes y se mostrara el panel de registro
            idcliente =(int)datalistadoClientes.SelectedCells[1].Value; //el idcliente captura el id del cliente que hay en la columna1
            txtclientesolicitante.Text = datalistadoClientes.SelectedCells[2].Value.ToString();//el txtclienteS capturara el dato de la columna2
            obtenerSaldo();
            datalistadoClientes.Visible = false;
            panelRegistros.Visible = true;
            mostrarEstadosCuentaCliente();

        }
        private void obtenerSaldo()
        {
            txttotal_saldo.Text= datalistadoClientes.SelectedCells[7].Value.ToString();
            saldo = Convert.ToDouble ( datalistadoClientes.SelectedCells[7].Value);
        }
            
         
       
        private void mostrarEstadosCuentaCliente() //proceso para mostrar el estado de cuenta de los clientes guardados
        {
            DataTable dt = new DataTable();
            Obtener_datos.mostrarEstadosCuentaCliente(ref dt, idcliente); //pasamos la referencia y el id que capturamos en el proceso alm
            datalistadoHistorial.DataSource = dt; //pasamos los datos capturados al datagistorial
            Bases estilo = new Bases(); //usamos el estilo 
            estilo.MultilineaCobros (ref datalistadoHistorial); //utilizamos el estilo que configuramos en la clase base
            panelH.Visible = true;
            panelM.Visible = false;
            panelHistorial.Visible = true;
            panelHistorial.Dock = DockStyle.Fill;
            panelMovimientos.Visible = false;
            panelMovimientos.Dock = DockStyle.None;
        }
        private void CobrosForm_Load(object sender, EventArgs e)
        {
            centrarPanel();
        }
        private void centrarPanel()
        {
            //centramos el paner contenedor con la referencia del panel principal menos la del panel contenedor
            PanelContenedor.Location = new Point((Width - PanelContenedor.Width) / 2, (Height - PanelContenedor.Height) / 2);
        }
        private void btnMovimientos_Click(object sender, EventArgs e)
        {
            mostrarControlCobros();
        }
        private void mostrarControlCobros()
        {
            DataTable dt = new DataTable();
            Obtener_datos.mostrar_ControlCobros(ref dt); //usamos proceso mostrar control con su referencia dt
            datalistadoMovimientos.DataSource = dt; //pasamos los datos al dtalistado
            Bases estilo = new Bases();
            estilo.MultilineaCobros(ref datalistadoMovimientos);//usamos el estilo de la clase base para los datagrid de cobros
            //ocultamos columnas no necesarias que se visualizen
            datalistadoMovimientos.Columns[1].Visible = false;//idcontrolcobro
            datalistadoMovimientos.Columns[5].Visible = false;//idcliente
            datalistadoMovimientos.Columns[6].Visible = false;//idusuario
            datalistadoMovimientos.Columns[7].Visible = false;//idcaja

            panelH.Visible = false; //panel historial se ocultara
            panelM.Visible = true;//panel movimientos visible
            panelHistorial.Visible = false;
            panelMovimientos.Visible = true;
            panelMovimientos.Dock = DockStyle.Fill;//expandemos completamente
            panelHistorial.Dock = DockStyle.None; 
        }

        private void btnhistorial_Click(object sender, EventArgs e)
        {
            mostrarEstadosCuentaCliente();
        }

        private void btnabonar_Click(object sender, EventArgs e)
        {
            if (saldo >0 ) //mostraremos el form medios de cobro siempre y cuando el saldo del cliente sea mayor a 0
            {
            MediosCobros frm = new MediosCobros();
            frm.FormClosing += Frm_FormClosing;//cuando se cieere el form medios cobros, lo volveremos a mostrar pero ya con los datos actualizados
            frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("El saldo del cliente actual es 0");
            }
           
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            buscar(); //pasamos este proceso para que si queremos ver los datos actualizados reinicimos la busqueda del cliente
            obtenerSaldo();
            mostrarControlCobros();
        }

        private void txtclientesolicitante_Click(object sender, EventArgs e)
        {
            txtclientesolicitante.SelectAll(); //cuando demos click en la barra se seleccionar todo el texto
        }

        private void datalistadoMovimientos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == datalistadoMovimientos.Columns ["Eli"].Index ) //si doy click a la columna movimientos es para eliminar un abono
            {
                //si es preguntamos al cliente para confirmar la eliminacion
                DialogResult result= MessageBox.Show("¿Realmente desea eliminar esta Abono?", "Eliminando registros", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
              
                //si responde con un click en ok realizamos la eliminacion del abono y volvemos aumentar el monto al saldo del cliente
                if (result == DialogResult.OK )
                {
                    aumentarSaldo();
                }
            }
        }
        private void aumentarSaldo()
        {
            double monto;
            monto = Convert.ToDouble(datalistadoMovimientos.SelectedCells[2].Value); //agregamos el monto que se eliminara para aumentrlo el cual esta en la colum 2 del datalistado
            Lclientes parametros = new Lclientes(); //logica de los parametros
            Editar_datos funcion = new Editar_datos(); //funcion aumentarsaldoclientes de la clase editardatos
            parametros.idcliente = idcliente;
          if (  funcion.aumentarSaldocliente(parametros, monto)==true) //si se realizo correctamente la funcion
            {
                eliminarControlCobros();
            }
            
        }
        private void eliminarControlCobros() //eliminamos tipo de pago que realizo el cliente
        {
            Lcontrolcobros parametros = new Lcontrolcobros();
            Eliminar_datos funcion = new Eliminar_datos();//parametro eliminarcontrolcobro
            parametros.IdcontrolCobro = Convert.ToInt32(datalistadoMovimientos.SelectedCells[1].Value);//capturamos el iddel control de cobro para eliminar 
           if ( funcion.eliminarControlCobro (parametros )==true ) //si se pasaron los pametros correctamente se realizara el proceso
            {
                buscar(); //volvemoss a llamar el proceso buscar para que se puedan ver los datos actualizados al mostrarse
            }
        }
    }
}
