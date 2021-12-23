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
using System.IO;
using System.Threading;
using System.Globalization;
using SV_EDSON.Logica;
using SV_EDSON.Presentacion.VENTAS_MENU_PRINCIPAL;
using SV_EDSON.Datos;
using System.Security.Cryptography.X509Certificates;
using SV_EDSON.Presentacion.Admin_nivel_dios;
using System.IO.Ports;
using System.Drawing.Imaging;

namespace SV_EDSON.Presentacion.VENTAS_MENU_PRINCIPAL
{
    public partial class VENTAS_MENU_PRINCIPALOK : Form
    {
        public VENTAS_MENU_PRINCIPALOK()
        {
            InitializeComponent();
        }

        int contador_stock_detalle_de_venta;
        int idproducto;
        int idClienteEstandar;
        public static    int idusuario_que_inicio_sesion;
        public static    int idVenta;
        int iddetalleventa;
        int Contador;
        public static  double txtpantalla;
        double lblStock_de_Productos;
        public static double total;
        public static   int Id_caja;
        string SerialPC;
        string sevendePor;
         public static  string txtventagenerada;
        double txtprecio_unitario;
        string usainventarios;
        string ResultadoLicencia;
        string FechaFinal;
        double cantidad;
        string Tema;
        int contadorVentasEspera;
        string Ip;
        bool EstadoCobrar = false;
        public static bool EstadoMediosPago = false; //podra ser utilizada en otros formularios
        Panel panel_mostrador_de_productos = new Panel();
    
        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void VENTAS_MENU_PRINCIPALOK_Load(object sender, EventArgs e)
        {
            //validarLicencia();

            //aperturamos procesos para obtener datos necesarios para la funcionalidad del formulario
            Bases.Cambiar_idioma_regional();
            Bases.Obtener_serialPC(ref SerialPC);
            Obtener_datos.Obtener_id_caja_PorSerial(ref Id_caja); //id de la caja para capturar todas laas acciones en ella
            
            Obtener_id_de_cliente_estandar();
            Obtener_datos.mostrar_inicio_De_sesion(ref idusuario_que_inicio_sesion);

            ValidarTiposBusqueda(); //validamos si se trabaja con teclado o con scanner
            ValidarTemaCaja(); //
            Limpiar_para_venta_nueva();//limpiamos los campos cada vez que reiniciemos para una venta nueva
            ObtenerIpLocal();    //optenemos id para mostrarlo en el sistema si es necesario
        }

        private void ValidarTiposBusqueda()
        {
            MOSTRAR_TIPO_DE_BUSQUEDA();
            //validamos los 2 tipos de busqueda de productos disponibles
            if (Tipo_de_busqueda == "TECLADO") //si el tipo de busqueda es por teclado
            {
                //modificamos los siguientes campos
                lbltipodebusqueda2.Text = "Buscar con TECLADO";
                BTNLECTORA.BackColor = Color.WhiteSmoke;
                BTNTECLADO.BackColor = Color.PaleTurquoise;
                txtbuscar.Clear();
                txtbuscar.Focus();
            }
            else
            {
                //si es por lector de barrar modificamos estos otros
                lbltipodebusqueda2.Text = "Buscar con LECTORA de Codigos de Barras";
                BTNLECTORA.BackColor = Color.PaleTurquoise;
                BTNTECLADO.BackColor = Color.WhiteSmoke;
                txtbuscar.Clear();
                txtbuscar.Focus();
            }
        }

        private void ObtenerIpLocal() //obtenemos el idiplocal para mostrarlo para uso ssi es necesario
        {
            this.Text = Bases.ObtenerIp(ref Ip);
        }

        private void ContarVentasEspera()
        {
            Obtener_datos.contarVentasEspera(ref contadorVentasEspera);
            if (contadorVentasEspera == 0)
            {
                panelNotificacionEspera.Visible = false; //si no hay ventas en espera ocultamos el panel 
            }
            else //si hay ventas pues lo mostramos y con la cantidad en el label
            {
                panelNotificacionEspera.Visible = true;
                lblContadorEspera.Text = contadorVentasEspera.ToString();
            }
        }
        private void ValidarTemaCaja() //validamos el color del formulario si es oscuro o claro
        {
            Obtener_datos.mostrarTemaCaja(ref Tema);
            if(Tema =="Redentor") //si el tema es igual al estado redentor se mostrara el tema claro
            {
                TemaClaro();
                IndicadorTema.Checked = false;
            }
            else
            {
                //si no el tema oscuro y se activara el checked
                TemaOscuro();
                IndicadorTema.Checked = true;

            }
        }
      /*  private void validarLicencia()
        {
            DLicencias funcion = new DLicencias();
            funcion.ValidarLicencias(ref ResultadoLicencia, ref FechaFinal);           
            if (ResultadoLicencia == "VENCIDA")
            {
                funcion.EditarMarcanVencidas();
                Dispose();
                LICENCIAS_MENBRESIAS.MembresiasNuevo frm = new LICENCIAS_MENBRESIAS.MembresiasNuevo();
                frm.ShowDialog();
            }



        }*/
        private void Limpiar_para_venta_nueva()
        {
            idVenta = 0; //ponemos por default un 0 al crear la venta se creara uno nuevo
            Listarproductosagregados(); //proceso para mostrar en el dt los procesos que se van agregando a la venta
            txtventagenerada = "VENTA NUEVA"; //cambuaos el estado de la venta para comenzar una nueva
            sumar();
            //ocultamos paneles no necesarios
            PanelEnespera.Visible = false;
            panelBienvenida.Visible = true;
            PanelOperaciones.Visible = false;
            ContarVentasEspera();
            EstadoMediosPago = false; //volvemos a cancelar el estado confirmado
        }


       private void sumar() //proceso que permite sumar los totales de las ventas a realizar
        {
            try
            {

                int x;
                x = datalistadoDetalleVenta.Rows.Count;
                if(x==0) //si el detalle de venta no tiene datos es xk no hay productos por vender
                {
                    txt_total_suma.Text = "0.00"; //daremos como dato un 0 para que lo aloje 
                }
                  
                double totalpagar;
                totalpagar = 0; //variable para capturar el total de la venta
                foreach (DataGridViewRow fila in datalistadoDetalleVenta.Rows ) //recorremos fila del detalleventa para sumar estos datos
                {
                    //el total a pagar sera la suma de los importes de esta venta
                    totalpagar += Convert.ToDouble  (fila.Cells["Importe"].Value);
                    //alojamos el total a pagar en el txttotalsuma para poder mostrarlo
                    txt_total_suma.Text =Convert.ToString ( totalpagar);
                    lblsubtotal.Text = Convert.ToString(totalpagar);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);//protegemos el codigo con un try cash si el proceso no sale bien mostraremos un msj informtativo
            }
        }

      
        private void LISTAR_PRODUCTOS_Abuscador() //proceso para encontrar los productos disponibles mas facilmente
        {
            try
            {
                DataTable dt = new DataTable(); //creamos dt para alojar los datos
                SqlDataAdapter da; //creamos una conexion para poder utilizar el dt con la base de datos  
                CONEXION.CONEXIONMAESTRA.abrir(); //aperturamos la conexion
                da = new SqlDataAdapter("BUSCAR_PRODUCTOS_oka", CONEXION.CONEXIONMAESTRA.conectar );//inicializamos proceso
                da.SelectCommand.CommandType = CommandType.StoredProcedure; //comando para indicar el uso del proceso
                da.SelectCommand.Parameters.AddWithValue("@letrab", txtbuscar.Text); //pasamos parametros que indica el proceso
                da.Fill(dt);//agregamos los datos al dt
                dgProductos.DataSource = dt; //pasamos los datos alojados en el dt al datagrip para poder mostrarlo
                CONEXION.CONEXIONMAESTRA.cerrar(); //cerramos conexion
                //oculltamos filas no nesesarias de mostrar
                dgProductos.Columns[0].Visible = false;
                dgProductos.Columns[1].Visible = false;
                dgProductos.Columns[2].Width  = 600;
                dgProductos.Columns[3].Visible = false;
                dgProductos.Columns[4].Visible = false;
                dgProductos.Columns[5].Visible = false;
                dgProductos.Columns[6].Visible = false;
                dgProductos.Columns[7].Visible = false;
                dgProductos.Columns[8].Visible = false;
                dgProductos.Columns[9].Visible = false;
                dgProductos.Columns[10].Visible = false;
            }
            catch (Exception ex)
            {
                CONEXION.CONEXIONMAESTRA.cerrar();
                MessageBox.Show(ex.StackTrace);
            }
        }

     
      
        string Tipo_de_busqueda;
        private void MOSTRAR_TIPO_DE_BUSQUEDA()
        {
            //proceso con el cual capturamos el tipo de busqueda que se configuro al crear la empresa
            SqlConnection con = new SqlConnection();
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
            SqlCommand com = new SqlCommand("Select Modo_de_busqueda  from EMPRESA", con);

            try
            {
                con.Open();//abrimos conexion
                Tipo_de_busqueda = Convert.ToString (com.ExecuteScalar());//capturamos el tipo de busqueda
                con.Close(); //cerramos conexion
            }
            catch (Exception)
            {
               // MessageBox.Show(ex.StackTrace);
            }
        }


        private void btnTecladoVirtual_Click(object sender, EventArgs e)
        {

        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
           
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
           
        }

       

        private void MenuStrip9_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

       

        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            //configuramos los 2 tipos de busqueda que se han configurado
            if (Tipo_de_busqueda =="LECTORA")
            {
                ValidarVentasNuevas();
                lbltipodebusqueda2.Visible = false; //ocultamos el lbl de la busqueda por teclado
                TimerBUSCADORcodigodebarras.Start();
            }
            else if (Tipo_de_busqueda=="TECLADO")//si el tipo de busqueda esta por teclado
            {
                if (txtbuscar.Text =="")
                {
                    ocultar_mostrar_productos(); //ocultamos produtos hasta que el txt tenga tento
                }
                else if  (txtbuscar.Text != "")
                {
                    mostrar_productos(); //si ya hay alguna letra se mostraran los productos
                }
                LISTAR_PRODUCTOS_Abuscador();

            }
            
        }
        private void mostrar_productos() //proceso para mostrar los productos disponibles
        {
            //damos tamano, color, locacion y lo hacemos visibel el panel
            panel_mostrador_de_productos.Size =new System.Drawing.Size(600, 186);
            panel_mostrador_de_productos.BackColor = Color.White;
            panel_mostrador_de_productos.Location = new Point(panelReferenciaProductos.Location.X, panelReferenciaProductos.Location.Y);
            panel_mostrador_de_productos.Visible = true;
            //hacemos visible de igual manera el dg y lo heredamos al panel para que se muestre alli
            dgProductos.Visible = true;
            dgProductos.Dock = DockStyle.Fill;
            dgProductos.BackgroundColor = Color.White;
            lbltipodebusqueda2.Visible = false;
            panel_mostrador_de_productos.Controls.Add(dgProductos);
            this.Controls.Add(panel_mostrador_de_productos);//damos acceso al panel mostrador para controlar sus acciones
            panel_mostrador_de_productos.BringToFront();//mostramos delante de todo el panel mostrador
        }
        private void ocultar_mostrar_productos()//proceso para ocultar el panel que muestra los productos en el dg
        {
            panel_mostrador_de_productos.Visible = false;
            dgProductos.Visible = false;
            lbltipodebusqueda2.Visible = true;
        }
        private void dgProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgProductos_CellClick(object sender, DataGridViewCellEventArgs e)//si damos click a uno de los productos aperturamos el vender por teclado
        {
            
            vender_por_teclado();

        }
        public void ValidarVentasNuevas() //proceso para iniciar una venta desde cero
        {
           if (datalistadoDetalleVenta.RowCount ==0) //cada vez que el detalle de venta no tenga dato
            {
                Limpiar_para_venta_nueva(); //limpiaremos los espacios de los txt para una nueva venta
            }
        }
        private void vender_por_teclado()
        {
            //validamos las ventas nuevas que se hagan
            ValidarVentasNuevas();
            txtbuscar.Text = dgProductos.SelectedCells[10].Value.ToString();
            idproducto = Convert.ToInt32(dgProductos.SelectedCells[1].Value.ToString());

            // mostramos los registros del producto en el detalle de venta
            mostrar_stock_de_detalle_de_ventas();
            contar_stock_detalle_ventas();
        
            if(contador_stock_detalle_de_venta == 0)
            {
                // Si es producto no esta agregado a las ventas se tomara el Stock de la tabla Productos
                lblStock_de_Productos = Convert.ToDouble ( dgProductos.SelectedCells[4].Value.ToString());     
            }
            else
            {
                 //en caso que el producto ya este agregado al detalle de venta se va a extraer el Stock de la tabla Detalle_de_venta
                lblStock_de_Productos = Convert.ToDouble(datalistado_stock_detalle_venta.SelectedCells[1].Value.ToString());
            }
            //Extraemos los datos del producto de la tabla Productos directamente
            usainventarios = dgProductos.SelectedCells[3].Value.ToString();
            lbldescripcion.Text = dgProductos.SelectedCells[9].Value.ToString();
            lblcodigo.Text = dgProductos.SelectedCells[10].Value.ToString();
            lblcosto.Text = dgProductos.SelectedCells[5].Value.ToString();
            sevendePor = dgProductos.SelectedCells[8].Value.ToString();
            txtprecio_unitario = Convert.ToDouble(dgProductos.SelectedCells[6].Value.ToString());
            //Preguntamos que tipo de producto sera el que se agrege al detalle de venta
            if (sevendePor == "Granel")
            {
                vender_a_granel();
            }
            else if (sevendePor == "Unidad")
            {
                txtpantalla =1;
                vender_por_unidad();
            }

        }
        private void vender_a_granel() //procesp para la venta de producto por libras o partes
        {
          
            CANTIDAD_A_GRANEL frm = new CANTIDAD_A_GRANEL();
            frm.preciounitario = txtprecio_unitario;
            frm.FormClosing += Frm_FormClosing;         
            frm.ShowDialog();
          

        }
       
        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ejecutar_ventas_a_granel(); //proceso para ejecutar el tipo de venta por parte
        }

        public  void ejecutar_ventas_a_granel()//proceso para ejecutar el tipo de venta por parte
        {
            
            ejecutar_insertar_ventas();//realizamos el proceso de venta
            if (txtventagenerada == "VENTA GENERADA") //si el estado se actualiza a esto
            {
                insertar_detalle_venta(); //insertamos detalle de venta al form para mostrarlo
                Listarproductosagregados(); //agregamos el producto que vamos a vender
                //limpiamos el txt para agregar un nuevo producto
                txtbuscar.Text = "";
                txtbuscar.Focus();

            }
        
        }
        private void Obtener_id_de_cliente_estandar() //proceso para obtener cliente estandar para ventas mas rapidas
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
            SqlCommand com = new SqlCommand("select idclientev  from clientes where Estado=0", con); //llamamos el dato con el id 0
            try
            {
                con.Open();
                idClienteEstandar = Convert.ToInt32(com.ExecuteScalar());//obtrnemos id necesario en el proceso
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }
    
        private void Obtener_id_venta_recien_Creada()//obtenemos id de las ventas que vallams realizando
        {
            SqlConnection con = new SqlConnection();//realizamos una conexion abierta 
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena de conexion
            //llamamos proceso a utilizar
            SqlCommand com = new SqlCommand("mostrar_id_venta_por_Id_caja", con);
            com.CommandType = CommandType.StoredProcedure;//confirmamos que trabajaremos con sp
            com.Parameters.AddWithValue("@Id_caja", Id_caja); //pasamos refercia de donde se capturara el id
            try
            {
                con.Open();
                idVenta = Convert.ToInt32(com.ExecuteScalar());
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("mostrar_id_venta_por_Id_caja");
            }
        }
        private void vender_por_unidad()//proceso de venta por unidad diferete al tipo por granel
        {
            try
            {
               if (txtbuscar.Text == dgProductos.SelectedCells[10].Value .ToString ())
                {
                    dgProductos.Visible = true; //mostramos el dg con los productos disponibles
                    ejecutar_insertar_ventas(); //insertamos datos que queremos vender
                 if (txtventagenerada =="VENTA GENERADA") //si la capruta salio bien
                    {
                        insertar_detalle_venta(); //insertamos los detalles de la venta
                        Listarproductosagregados();//mostramos los productos que se agregarom
                        //limpiamos el txt para agregar uno nuevo si se desea
                        txtbuscar.Text = "";
                        txtbuscar.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
        private void ejecutar_insertar_ventas()//proceso para la realizacion de una nueva venta
        {
            if (txtventagenerada == "VENTA NUEVA")//si el estado esta en venta nueva realizamos el proceso de insertar venta
            {
                try
                {
                    //protegemos con try y cash para pasar parametros y variables que se necesiten para mostrar datos
                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("insertar_venta", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idcliente", idClienteEstandar);
                    cmd.Parameters.AddWithValue("@fecha_venta", DateTime.Today);
                    cmd.Parameters.AddWithValue("@nume_documento", 0);
                    cmd.Parameters.AddWithValue("@montototal", 0);
                    cmd.Parameters.AddWithValue("@Tipo_de_pago", 0);
                    cmd.Parameters.AddWithValue("@estado", "EN ESPERA");
                    cmd.Parameters.AddWithValue("@IGV", 0);
                    cmd.Parameters.AddWithValue("@Comprobante", 0);
                    cmd.Parameters.AddWithValue("@id_usuario", idusuario_que_inicio_sesion);
                    cmd.Parameters.AddWithValue("@Fecha_de_pago", DateTime.Today);
                    cmd.Parameters.AddWithValue("@ACCION", "VENTA");
                    cmd.Parameters.AddWithValue("@Saldo", 0);
                    cmd.Parameters.AddWithValue("@Pago_con", 0);
                    cmd.Parameters.AddWithValue("@Porcentaje_IGV", 0);
                    cmd.Parameters.AddWithValue("@Id_caja", Id_caja);
                    cmd.Parameters.AddWithValue("@Referencia_tarjeta", 0);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    Obtener_id_venta_recien_Creada();
                    txtventagenerada = "VENTA GENERADA";//cambuiamos el esado para ya mostrarla como una venta
                    mostrar_panel_de_Cobro();//mostramos panel para el cobto de la venta

                }
                catch (Exception ex)
                {
                    MessageBox.Show("insertar_venta");
                }

            }
        }
       private void mostrar_panel_de_Cobro()
        {
            panelBienvenida.Visible = false;
            PanelOperaciones.Visible = true;//panel para seguir con las ventas
        }
        private void Listarproductosagregados() //procesp para listar todos los procesos agregados a una venta
        {
            try
            {
                DataTable dt = new DataTable();//creamos dt para pasarlo datos 
                SqlDataAdapter da; //utilizacion de comando y generador para trabajas en sql con sp
                SqlConnection con = new SqlConnection(); //creamos conexion
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                con.Open();//la aperturamos
                da = new SqlDataAdapter("mostrar_productos_agregados_a_venta", con);//utilizacion de sp
                da.SelectCommand.CommandType = CommandType.StoredProcedure;//obtenemos las referencias a utilizar
                da.SelectCommand.Parameters.AddWithValue("@idventa",idVenta ); //pasamos parametro
                da.Fill(dt);
                datalistadoDetalleVenta.DataSource = dt;
                con.Close();
                datalistadoDetalleVenta.Columns[0].Width = 50;
                datalistadoDetalleVenta.Columns[1].Width = 50;
                datalistadoDetalleVenta.Columns[2].Width = 50;
                datalistadoDetalleVenta.Columns[3].Visible = false;
                datalistadoDetalleVenta.Columns[4].Width = 250;
                datalistadoDetalleVenta.Columns[5].Width = 100;
                datalistadoDetalleVenta.Columns[6].Width = 100;
                datalistadoDetalleVenta.Columns[7].Width = 100;
                datalistadoDetalleVenta.Columns[8].Visible = false;
                datalistadoDetalleVenta.Columns[9].Visible = false;
                datalistadoDetalleVenta.Columns[10].Visible = false;
                datalistadoDetalleVenta.Columns[11].Width = datalistadoDetalleVenta.Width - (datalistadoDetalleVenta.Columns[0].Width- datalistadoDetalleVenta.Columns[1].Width- datalistadoDetalleVenta.Columns[2].Width-
                datalistadoDetalleVenta.Columns[4].Width- datalistadoDetalleVenta.Columns[5].Width- datalistadoDetalleVenta.Columns[6].Width- datalistadoDetalleVenta.Columns[7].Width);
                datalistadoDetalleVenta.Columns[12].Visible = false;
                datalistadoDetalleVenta.Columns[13].Visible = false;
                datalistadoDetalleVenta.Columns[14].Visible = false;
                datalistadoDetalleVenta.Columns[15].Visible = false;
                datalistadoDetalleVenta.Columns[16].Visible = false;
                datalistadoDetalleVenta.Columns[17].Visible = false;
                datalistadoDetalleVenta.Columns[18].Visible = false;
                //datalistadoDetalleVenta.Columns[4].ReadOnly = false;

                if (Tema=="Redentor")
                {
                Bases.Multilinea(ref datalistadoDetalleVenta);//damos estilos al dt claro
                }
                else
                {
                  
                Bases.MultilineaTemaOscuro(ref datalistadoDetalleVenta);//damos estilo al dt pscuro
                }
                
                sumar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
        private void insertar_detalle_venta() //proceso para insertar productos a una ventaa
        {
            try
            {
                if (usainventarios =="SI") //VERIFICAMOS SI USA INVENTARIO PARA REALIZAR EL CONTROL DEL MISMO SI ES ASI
                {
                    if ( lblStock_de_Productos >= txtpantalla)  //verificamos si el stok disponible es mayor a la venta
                    {
                        insertar_detalle_venta_Validado(); //si lo anterior se confirma realizamos el proceso para agregar la venta
                    }
                    else
                    {
                        TimerLABEL_STOCK.Start(); //si no es asi informamos sobre que el stock es menor
                    }
                }

           else if  (usainventarios =="NO") //si no usa inventario pasamos directamente a realizar la venta
                {
                    insertar_detalle_venta_SIN_VALIDAR();//proceso donde no se valida el inventario
                }
        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
          
        }
        private void insertar_detalle_venta_Validado() //proceso para las ventas que si usan inventario
        {
            try
            {
                //pasamos o capturamos los datos en su respectiva variable
                SqlConnection con = new SqlConnection();
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("insertar_detalle_venta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idventa", idVenta);
                cmd.Parameters.AddWithValue("@Id_presentacionfraccionada", idproducto);
                cmd.Parameters.AddWithValue("@cantidad", txtpantalla);
                cmd.Parameters.AddWithValue("@preciounitario", txtprecio_unitario);
                cmd.Parameters.AddWithValue("@moneda", 0);
                cmd.Parameters.AddWithValue("@unidades", "Unidad");
                cmd.Parameters.AddWithValue("@Cantidad_mostrada", txtpantalla);
                cmd.Parameters.AddWithValue("@Estado", "EN ESPERA");
                cmd.Parameters.AddWithValue("@Descripcion", lbldescripcion.Text);
                cmd.Parameters.AddWithValue("@Codigo", lblcodigo.Text);
                cmd.Parameters.AddWithValue("@Stock", lblStock_de_Productos);
                cmd.Parameters.AddWithValue("@Se_vende_a", sevendePor);
                cmd.Parameters.AddWithValue("@Usa_inventarios", usainventarios);
                cmd.Parameters.AddWithValue("@Costo", lblcosto.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                disminuir_stock_en_detalle_de_venta(); //proceso para disminuir el stok de la venta  a realizar
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message); //si no sale bien el proceso nos mostrara un msj informandonos del error
            }
        }
   
        private void insertar_detalle_venta_SIN_VALIDAR()//proceso para las ventas que no usan inventario
        {
            try
            {
                //pasamos los parametros sin validar el stok sino para realizar la venta normalmente
                SqlConnection con = new SqlConnection();
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("insertar_detalle_venta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idventa", idVenta);
                cmd.Parameters.AddWithValue("@Id_presentacionfraccionada", idproducto);
                cmd.Parameters.AddWithValue("@cantidad", txtpantalla);
                cmd.Parameters.AddWithValue("@preciounitario", txtprecio_unitario);
                cmd.Parameters.AddWithValue("@moneda", 0);
                cmd.Parameters.AddWithValue("@unidades", "Unidad");
                cmd.Parameters.AddWithValue("@Cantidad_mostrada", txtpantalla);
                cmd.Parameters.AddWithValue("@Estado", "EN ESPERA");
                cmd.Parameters.AddWithValue("@Descripcion", lbldescripcion.Text);
                cmd.Parameters.AddWithValue("@Codigo", lblcodigo.Text);
                cmd.Parameters.AddWithValue("@Stock", lblStock_de_Productos);
                cmd.Parameters.AddWithValue("@Se_vende_a", sevendePor);
                cmd.Parameters.AddWithValue("@Usa_inventarios", usainventarios);
                cmd.Parameters.AddWithValue("@Costo", lblcosto.Text);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
        }

        private void contar_stock_detalle_ventas() //proceso para capturar el total de producto disponible
        {
            int x;
            x = datalistado_stock_detalle_venta.Rows.Count; //pasamos a x el total de filas que hay en las columnas
            contador_stock_detalle_de_venta = (x); //pasamos la captura del dato al contador
        }
        private void mostrar_stock_de_detalle_de_ventas()//mostrar el producto disponible para ser vendido
        {
             try
            {
                DataTable dt = new DataTable(); //creamos dt para agregr los datos
                SqlDataAdapter da;//indicamos que usaremos datos de una bd
                SqlConnection con = new SqlConnection(); //creamos conexion
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena de conexion
                con.Open(); //abricos conexion
                da = new SqlDataAdapter("mostrar_stock_de_detalle_de_ventas", con); //pasamos parametro a utilzar
                da.SelectCommand.CommandType = CommandType.StoredProcedure; //inicializamos el proceso
                da.SelectCommand.Parameters.AddWithValue("@Id_producto", idproducto); //pasamos parametro necesario
                da.Fill(dt); //pasamos los datos capturados al dt
                datalistado_stock_detalle_venta.DataSource = dt;//pasamos los datos del dt al datalistado para ser mostrados
                con.Close();//cerramos conexion
              

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message );
            }
        }

        private void ejecutar_editar_detalle_venta_sumar() //procecso poder sumar productos en una venta
        {
            try
            {
            SqlCommand cmd; //ejecutar base de datos
            SqlConnection con = new SqlConnection();//creamos nuava conexion
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion; //pasamos cadena
            con.Open();//aperturamos
            cmd = new SqlCommand("editar_detalle_venta_sumar", con);//instanciamos proceso
            cmd.CommandType = CommandType.StoredProcedure;//interpretamos proceso
             //pasamos parametros
            cmd.Parameters.AddWithValue("@Id_producto", idproducto);
            cmd.Parameters.AddWithValue("@cantidad", txtpantalla);
            cmd.Parameters.AddWithValue("@Cantidad_mostrada", txtpantalla);
            cmd.Parameters.AddWithValue("@Id_venta", idVenta);
            cmd.ExecuteNonQuery();//ejecutamos proceso
            con.Close();//cerramos conexion
            }
            catch (Exception)
            {

               
            }
           
        }
        private void disminuir_stock_en_detalle_de_venta()//proceso para poder restar productos en una venta
        {
            try
            {
                CONEXION.CONEXIONMAESTRA.abrir();//aperturamos conexion
                SqlCommand cmd = new SqlCommand("disminuir_stock_en_detalle_de_venta", CONEXION.CONEXIONMAESTRA.conectar);//instanciamso proceso
                cmd.CommandType = CommandType.StoredProcedure; //interpretamos proceso
                //pasamos parametros o referencias necesarias
                cmd.Parameters.AddWithValue("@Id_Producto1", idproducto);
                cmd.Parameters.AddWithValue("@cantidad", txtpantalla );
                cmd.ExecuteNonQuery();//ejecutamos proceso
                CONEXION.CONEXIONMAESTRA.cerrar();//cerramos conexion
            }
            catch (Exception)
            {

             
            }
        }
        private void Obtener_datos_del_detalle_de_venta()//proceso para capturar los datos de una venta a realizar
        {
            
            try
            {
                //creamos variables para poder capturar los datos desde el datalistado
                iddetalleventa = Convert.ToInt32 ( datalistadoDetalleVenta.SelectedCells[9].Value.ToString());
                idproducto = Convert.ToInt32(datalistadoDetalleVenta.SelectedCells[8].Value.ToString());
                sevendePor = datalistadoDetalleVenta.SelectedCells[17].Value.ToString();
                usainventarios = datalistadoDetalleVenta.SelectedCells[16].Value.ToString();
                cantidad=Convert.ToDouble( datalistadoDetalleVenta.SelectedCells[5].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void editar_detalle_venta_sumar() //proceso para realizar venta ya con todas las configuraciones
        {
    
            //verificamos si el producto trabaja con inventario
             if (usainventarios=="SI")
                 {
                   lblStock_de_Productos = Convert.ToDouble ( datalistadoDetalleVenta.SelectedCells[15].Value.ToString()); //capturamos el total disponible
                
                if (lblStock_de_Productos >0) //configuramos para poder agregar o disminuir producto siempe que el stock seam mayor a 0
                {
                    //si todo salio bien podremos hacer uso de estos 2
                ejecutar_editar_detalle_venta_sumar();
                disminuir_stock_en_detalle_de_venta();
                }
                else
                    {
                        TimerLABEL_STOCK.Start(); //si no es asi mandamos msj informando 
                    }
            
                }
             else
                {
                 ejecutar_editar_detalle_venta_sumar();//si no es mayor a 0 solo habilitamos el de aumentar producto
                }
                Listarproductosagregados(); //si no trabaja con inventario se aggregaran los productos sin problema
           
          }
        private void editar_detalle_venta_restar() //restar producto en el detalle de venta
        {
           
            if (usainventarios == "SI") //si usa inventarios
            {
                //si se trabaja con inventario y se resta producto este mismo se aumentara en el stock
                ejecutar_editar_detalle_venta_restar();
                aumentar_stock_en_detalle_de_venta();
            }
            else
            {
                ejecutar_editar_detalle_venta_restar();//si no trabaja con inventario solo ejecutamos el de restar
            }
            Listarproductosagregados();//listamos productos en la venta
        }
        private void aumentar_stock_en_detalle_de_venta()//proceso para umentar producto a una venta
        {
            try
            {
                CONEXION.CONEXIONMAESTRA.abrir(); //abrimos conexion
                //instanciamos proceso a utilizar
                SqlCommand cmd = new SqlCommand("aumentar_stock_en_detalle_de_venta", CONEXION.CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;//interpretamos parametro para pasar parametros
                //pasamos parametros
                cmd.Parameters.AddWithValue("@Id_Producto1", idproducto);
                cmd.Parameters.AddWithValue("@cantidad", txtpantalla);
                cmd.ExecuteNonQuery(); //ejecutamos proceso
                CONEXION.CONEXIONMAESTRA.cerrar();//cerramos conexion
            }
            catch (Exception)
            {

            }
        }
        private void ejecutar_editar_detalle_venta_restar()//proceso para realizar disminucion de producto en una venta
        {
            try
            {
            SqlCommand cmd;//ejecutaremos base de datos
            SqlConnection con = new SqlConnection(); //creamos conecion
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena
            con.Open();//aperturamos conexion
            cmd = new SqlCommand("editar_detalle_venta_restar", con);//instanciamos proceso
            cmd.CommandType = CommandType.StoredProcedure;//interpretamos proceso
                //pasamos parametros
            cmd.Parameters.AddWithValue("@iddetalle_venta", iddetalleventa);
            cmd.Parameters.AddWithValue("cantidad", txtpantalla );
            cmd.Parameters.AddWithValue("@Cantidad_mostrada", txtpantalla);
            cmd.Parameters.AddWithValue("@Id_producto", idproducto);
            cmd.Parameters.AddWithValue("@Id_venta", idVenta);
            cmd.ExecuteNonQuery(); //ejecutamos proceso
            con.Close(); //cerramos la conexion
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void datalistadoDetalleVenta_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            Obtener_datos_del_detalle_de_venta(); //mostramos los productos agregaods a venta


            if (e.ColumnIndex == this.datalistadoDetalleVenta.Columns["S"].Index) //si damos clic en la columna s
            {
                txtpantalla = 1; //pasamos un 1 al txtpantalla
                editar_detalle_venta_sumar(); //ejecutamos proceso para ir sumando de un en uno
            }
            if (e.ColumnIndex== this .datalistadoDetalleVenta.Columns ["R"].Index ) //si da clic en r restaremos
            {
                txtpantalla = 1; //pasamos 1 para que nos deje realizar el proceso
                editar_detalle_venta_restar(); //restaremos de 1 en uno
                EliminarVentas(); //si llega a 0 eliminara la venta
            }
            

            if (e.ColumnIndex == this.datalistadoDetalleVenta.Columns["EL"].Index) //si damos clic a la colum elim
            {

                int iddetalle_venta =Convert.ToInt32 ( datalistadoDetalleVenta.SelectedCells[9].Value); //capturamos id para eliminar
                    try
                    {
                        SqlCommand cmd;//indicamos que daremos intruccion a base de datos
                        SqlConnection con = new SqlConnection(); //creamos conexion
                        con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena
                        con.Open();//abrimos conex
                        cmd = new SqlCommand("eliminar_detalle_venta", con);//instanciamos el proceso
                        cmd.CommandType = CommandType.StoredProcedure;//interpretamos proceso
                        //pasamos parametros a utilizar
                        cmd.Parameters.AddWithValue("@iddetalleventa", iddetalle_venta);
                        cmd.ExecuteNonQuery();//ejecutamos proceso
                        con.Close();//cerramos conexion
                        txtpantalla = Convert.ToDouble(datalistadoDetalleVenta.SelectedCells[5].Value);//actualizamos datos en el txt
                        aumentar_stock_en_detalle_de_venta(); //aumentamos al stock el producto de la venta eliminada
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }      
                Listarproductosagregados();//mostramos los productos disponibles
                EliminarVentas();//proceso para eliminar la venta
            }
        }
        private void EliminarVentas() //proceso para eliminar ventas hechas
        {
            contar_tablas_ventas(); //contamos el total de ventas en 
            if (Contador == 0) //si en las filas no hay ninguna venta agregada
            {
                eliminar_venta_al_agregar_productos();//eliminaremos venta siempre y cuando haya producto en ella
                Limpiar_para_venta_nueva(); //limmpiamos campos para una nueva venta
            }
        }
        private void eliminar_venta_al_agregar_productos() //eliminaremos venta siempre y cuando tenga productos en ella
        {
            try
            {
                SqlCommand cmd;//indicamos que daremos instruccio a base de datps
                SqlConnection con = new SqlConnection(); //creamos conexion
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion; //pasamos cadena 
                con.Open();//abrimos
                cmd = new SqlCommand("eliminar_venta", con);//instanciamos proceso
                cmd.CommandType = CommandType.StoredProcedure;//interpteta proceso
                //pasamos parametro necesario
                cmd.Parameters.AddWithValue("@idventa", idVenta);
                cmd.ExecuteNonQuery();//ejecutamos proceso
                con.Close();//cerramos conexion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void contar_tablas_ventas() //contar el total de ventas en la tablas del datalistadoventas
        {
            int x;
            x = datalistadoDetalleVenta.Rows.Count; //pasamos el total de filas
            Contador = (x); //pasamos dato al contador
        }


        private void datalistadoDetalleVenta_KeyPress(object sender, KeyPressEventArgs e)//proceso para utilizar telcas
        {
            if (datalistadoDetalleVenta.RowCount > 0) //si el detalle de venta es mayor a 0 en productos
            {
                Obtener_datos_del_detalle_de_venta(); //obttenemos este dato
                if (e.KeyChar == Convert.ToChar("+")) //si es mayor podremos usar la tecla mas para aunemtar en 1
                {
                    editar_detalle_venta_sumar(); //al dar al mas aumentara el producto de 1  en uno
                }
                if (e.KeyChar == Convert.ToChar("-")) //disminuira el producto de uno en uno hasta llegar a 0 se podra
                {
                    editar_detalle_venta_restar();
                    contar_tablas_ventas();
                    if (Contador == 0) //si llefa a 0 eliminara la venta 
                    {
                        eliminar_venta_al_agregar_productos();
                        txtventagenerada = "VENTA NUEVA";//cambiamos el estado para una nueva venta
                    }
                }
            }
        }
        //damos valor a los btn al dar click en ellos 
        private void btn1_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "1";
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "2";

        }

        private void btn3_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "3";
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "4";
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "5";
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "6";
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "7";
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "8";
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "9";
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            txtmonto.Text = txtmonto.Text + "0";
        }
        bool SECUENCIA = true; //sirver para poder usar el . solo una vez como separador de decimal
        //aqui termine de dar valos a los botones de los numeros para ser utilizados

        private void btnSeparador_Click(object sender, EventArgs e)
        {
            if (SECUENCIA == true) //reiniciamos para usar el separador
            {
                txtmonto.Text = txtmonto.Text + ".";
                SECUENCIA = false; //cuando este se use una vez cambiamos el estado hasta una nueva venta
            }
            else
            {
                return; 
            }
        }

        private void txtmonto_TextChanged(object sender, EventArgs e)
        {
            //if (SECUENCIA == true)
            //{
            //    txtmonto.Text = txtmonto.Text + ".";
            //    SECUENCIA = false;
            //}
            //else
            //{
            //    return;
            //}
        }
        private void txtmonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Bases.Separador_de_Numeros(txtmonto, e);
        }

        private void btnborrartodo_Click(object sender, EventArgs e)
        {
            txtmonto.Clear();
            SECUENCIA = true;
        }

        private void TimerBUSCADORcodigodebarras_Tick(object sender, EventArgs e)
        {
            TimerBUSCADORcodigodebarras.Stop();
            vender_por_lectora_de_barras();
        }
        private void vender_por_lectora_de_barras()
        {
            try
            {
                 if (txtbuscar.Text =="")
            {
                dgProductos.Visible = false;
                lbltipodebusqueda2.Visible = true;
            }
            if(txtbuscar.Text !="")
            {
                dgProductos.Visible = true;
                lbltipodebusqueda2.Visible = false;
                LISTAR_PRODUCTOS_Abuscador();
           
                idproducto =Convert.ToInt32 ( dgProductos.SelectedCells[1].Value.ToString());
                mostrar_stock_de_detalle_de_ventas();
                contar_stock_detalle_ventas();

                if (contador_stock_detalle_de_venta  ==0)
                {
                    lblStock_de_Productos = Convert.ToDouble(dgProductos.SelectedCells[4].Value.ToString());
                }
                else
                {
                    lblStock_de_Productos = Convert.ToDouble(datalistado_stock_detalle_venta.SelectedCells[1].Value.ToString());
                }
                usainventarios = dgProductos.SelectedCells[3].Value.ToString();
                lbldescripcion.Text = dgProductos.SelectedCells[9].Value.ToString();
                lblcodigo.Text = dgProductos.SelectedCells[10].Value.ToString();
                lblcosto.Text = dgProductos.SelectedCells[5].Value.ToString();
                txtprecio_unitario =Convert.ToDouble ( dgProductos.SelectedCells[6].Value.ToString());
            sevendePor = dgProductos.SelectedCells[8].Value.ToString();
                if (sevendePor =="Unidad")
                {
                    txtpantalla =1;
                    vender_por_unidad();
                }

            }
            }
            catch (Exception)
            {

              
            }
        
        }
        private void lbltipodebusqueda2_Click(object sender, EventArgs e)
        {

        }
        private void editar_detalle_venta_CANTIDAD()
        {
            try
            {
                SqlCommand cmd;
                SqlConnection con = new SqlConnection();
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                con.Open();
                cmd = new SqlCommand("editar_detalle_venta_CANTIDAD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id_producto", idproducto);
                cmd.Parameters.AddWithValue("@cantidad", txtmonto.Text);
                cmd.Parameters.AddWithValue("@Cantidad_mostrada", txtmonto.Text);
                cmd.Parameters.AddWithValue("@Id_venta", idVenta);
                cmd.ExecuteNonQuery();
                con.Close();
                Listarproductosagregados();
                txtmonto.Clear();
                txtmonto.Focus();
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button21_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtmonto.Text ))
            {
           if (datalistadoDetalleVenta.RowCount >0 )
            {      

            if (sevendePor =="Unidad")

            {
                string cadena = txtmonto.Text;
                if (cadena.Contains ("."))
                {
                    MessageBox.Show("Este Producto no acepta decimales ya que esta configurado para ser vendido por UNIDAD", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                        BotonCantidad();


                }
             }
            else if (sevendePor == "Granel")
                {
                    BotonCantidad();
                }
           }
           else
            {
                txtmonto.Clear();
                txtmonto.Focus();
            }
            }
         
        }
        private void BotonCantidad() //proceso con el cual modificaremos la cantidad de los productos en una venta
        {

            double MontoaIngresar;
            MontoaIngresar = Convert.ToDouble(txtmonto.Text);//pasamos el dato del txtmonto a la variablle
            double Cantidad;
            Cantidad = Convert.ToDouble(datalistadoDetalleVenta.SelectedCells[5].Value);//capturamos la cantidad 

            double stock ;
            double condicional ;
            string ControlStock ;
            ControlStock = datalistadoDetalleVenta.SelectedCells[16].Value.ToString();//capturamos si trabaja con stoxk o no
            if (ControlStock =="SI") //si trabaja con stock
            {
                stock= Convert.ToDouble(datalistadoDetalleVenta.SelectedCells[11].Value);//capturamos el stoxk total
                condicional = Cantidad + stock; //sumamos a los productos a comprar la cantidad que se ingreso
                if (condicional>= MontoaIngresar) //si la cantidad es menos al monto que hay
                {
                    BotonCantidadEjecuta(); //se ejecutara el proceso de cambio de cantidad de los productos a vedner
                }
                else
                {
                    TimerLABEL_STOCK.Start();//si no es asi paramos el proceso
                }
            }
            else
            {
                BotonCantidadEjecuta(); //si no trabaka con stoxk seguira normal el proceso
            }
          

        }
        private void BotonCantidadEjecuta() //proceso para modificar el total de procutos para una venta en un solo paso
        {
            double MontoaIngresar; //variable para guaradar el monto
            MontoaIngresar = Convert.ToDouble(txtmonto.Text);//canturamos en la variable lo que hay en el txt
            double Cantidad;
            Cantidad = Convert.ToDouble(datalistadoDetalleVenta.SelectedCells[5].Value);

            if (MontoaIngresar > Cantidad)
            {
                txtpantalla = MontoaIngresar - Cantidad; //mostraremos el 
                editar_detalle_venta_sumar();//suamos el total de la venta
            }
            else if (MontoaIngresar < Cantidad)
            {
                txtpantalla = Cantidad - MontoaIngresar;
                editar_detalle_venta_restar();
            }
        }

        //procesos para llamado de direcciones de los botonoes
        private void frm_FormClosed (Object sender, FormClosedEventArgs e)
        {
            if (EstadoMediosPago == true)
            {
                Limpiar_para_venta_nueva(); //al cerrarse se limpiaran los datos de venta para una nueva
            }
            
        }

        private void Panel17_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void TimerLABEL_STOCK_Tick(object sender, EventArgs e)
        {
            if (ProgressBarETIQUETA_STOCK.Value <100)
            {
                ProgressBarETIQUETA_STOCK.Value = ProgressBarETIQUETA_STOCK.Value + 10;
                LABEL_STOCK.Visible = true;
                LABEL_STOCK.Dock = DockStyle.Fill;
            }
            else
            {
                LABEL_STOCK.Visible = false;
                LABEL_STOCK.Dock = DockStyle.None;
                ProgressBarETIQUETA_STOCK.Value = 0;
                TimerLABEL_STOCK.Stop();
            }
        }

        private void befectivo_Click_1(object sender, EventArgs e)
        {
            cobrar(); //proceso para realizar el pago al dar click en cobrar
        }
        private void cobrar()
        {
            if (datalistadoDetalleVenta.RowCount > 0)
            {
                total = Convert.ToDouble(txt_total_suma.Text);
                MEDIOS_DE_PAGO frm = new MEDIOS_DE_PAGO();
                frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                frm.ShowDialog();
            }
        }
      
        //llamamos formulario de ventas en espera
        private void btnrestaurar_Click_1(object sender, EventArgs e)
        {
            Ventas_en_espera frm = new Ventas_en_espera();
            frm.FormClosing += Frm_FormClosing1;
            frm.ShowDialog();
        }

        private void Frm_FormClosing1(object sender, FormClosingEventArgs e)
        {
            //al cerrar el ventas en espera volvemos a mostrar los productosu ya actualizados 
            Listarproductosagregados();
            mostrar_panel_de_Cobro();
        }

        private void btneliminar_Click(object sender, EventArgs e) //proceso para eliminar una venta
        {
            if (datalistadoDetalleVenta.RowCount >0) //verificamos que hayan datos en el dt
            {
                //preguntamos para confirmar la eliminacion
            DialogResult pregunta = MessageBox.Show("¿Realmente desea eliminar esta Venta?", "Eliminando registros", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (pregunta==DialogResult.OK ) //si da ok es  xk se confirma
            {
            Eliminar_datos.eliminar_venta(idVenta); //eliminamos venta de acuerdo al idventa
            Limpiar_para_venta_nueva();//limpiamos campos para una venta nueva
            }
            }
           
           
        }

        //procesos para la configuracion de las ventas por espera
        private void btnespera_Click(object sender, EventArgs e)
        {
            if (datalistadoDetalleVenta.RowCount>0)
            {
            PanelEnespera.Visible = true;
            PanelEnespera.BringToFront();
            PanelEnespera.Dock = DockStyle.Fill;
            txtnombre.Clear();
            }
          

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ocularPanelenEspera();
        }

        private void ocularPanelenEspera()
        {
            PanelEnespera.Visible = false;
            PanelEnespera.Dock = DockStyle.None;
        }
        private void btnGuardarEspera_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty (txtnombre.Text )) //si en el txt hay algo diferente a null procedemos a editar venta
            {
                editarVentaEspera();
            }
            else
            {
                MessageBox.Show("Ingrese una referencia");
            }
            
        }
        private void editarVentaEspera()//proceso para editar ventas que tengamos en espera

        {
            Editar_datos.ingresar_nombre_a_venta_en_espera(idVenta, txtnombre.Text);//proceson con el cual obtenemos el id y el nombre de la venta
            Limpiar_para_venta_nueva(); //limpiamos los datos paraa una venta nueva
            ocularPanelenEspera(); //ocultamos el panel de ventas en espera
        }
        private void btnAutomaticoEspera_Click(object sender, EventArgs e)
        {
            //damos nombre por defecto y le adjuntamos el id
            txtnombre.Text = "Ticket" + idVenta;
            editarVentaEspera();
        }

        

        private void BTNLECTORA_Click_1(object sender, EventArgs e)
        {
            ModoLectora();   
        }

        private void ModoLectora() //poceso para escoger el tipo de busqueda por lector
        {
            ocultar_mostrar_productos();
            lbltipodebusqueda2.Text = "Buscar con LECTORA de Codigos de Barras";
            Tipo_de_busqueda = "LECTORA";
            BTNLECTORA.BackColor = Color.PaleTurquoise;
            BTNTECLADO.BackColor = Color.WhiteSmoke;
            txtbuscar.Clear();
            txtbuscar.Focus();

        }

        private void BTNTECLADO_Click_1(object sender, EventArgs e)
        {
            ModoTeclado();
        }
        private void ModoTeclado() //proceso para escoger el tipo de busqueda por teclado
        {
            ocultar_mostrar_productos();
            lbltipodebusqueda2.Text = "Buscar con  TECLADO";
            Tipo_de_busqueda = "TECLADO";
            BTNTECLADO.BackColor = Color.PaleTurquoise;
            BTNLECTORA.BackColor = Color.WhiteSmoke;
            txtbuscar.Clear();
            txtbuscar.Focus();

        }
        //manejo de los botones y las direccions donde enviaran al usuario

        private void btnGastos_Click(object sender, EventArgs e)
        {
            Gastos_varios.Gastos frm = new Gastos_varios.Gastos();
            frm.ShowDialog();
        }

        private void btnIngresosCaja_Click(object sender, EventArgs e)
        {
            Ingresos_varios.IngresosVarios frm = new Ingresos_varios.IngresosVarios();
            frm.ShowDialog();

        }

        private void btnCreditoPagar_Click(object sender, EventArgs e)
        {
            Apertura_de_credito.PorPagar frm = new Apertura_de_credito.PorPagar();
            frm.ShowDialog();
        }

        private void btnCreditoCobrar_Click(object sender, EventArgs e)
        {
            Apertura_de_credito.PorCobrarOk frm = new Apertura_de_credito.PorCobrarOk();
            frm.ShowDialog();
        }

        private void btnadmin_Click(object sender, EventArgs e)
        {
            Dispose();
            //DASHBOARD_PRINCIPAL frm = new DASHBOARD_PRINCIPAL();
            LOGIN frm = new LOGIN();
            frm.ShowDialog();

        }

        private void VENTAS_MENU_PRINCIPALOK_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dlgRes = MessageBox.Show("¿Realmente desea Cerrar el Sistema?", "Cerrando", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgRes == DialogResult.Yes)
            {
                Dispose();
                CopiasBd.GeneradorAutomatico frm = new CopiasBd.GeneradorAutomatico();
                frm.ShowDialog();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void VENTAS_MENU_PRINCIPALOK_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void btnCobros_Click(object sender, EventArgs e)
        {
            //mostraremos form con los cobros que se han realizado
            Cobros.CobrosForm frm = new Cobros.CobrosForm();
            frm.ShowDialog();
        }

        private void btnMayoreo_Click(object sender, EventArgs e)
        {
            aplicar_precio_mayoreo();
        }
        private void aplicar_precio_mayoreo()
            //proceso para aplicar precio de mayoreo a productos sin importar la cantidad que se lleve
        {
            if (datalistadoDetalleVenta.Rows.Count >0) //si el detalle de venta es mayor a 0 aplicaremos precio de mayoreo a todos los prodcutos que trabajen con ello
            {
            LdetalleVenta parametros = new LdetalleVenta();
            Editar_datos funcion = new Editar_datos();
            parametros.Id_producto = idproducto;
            parametros.iddetalle_venta  =iddetalleventa;
            if (funcion.aplicar_precio_mayoreo (parametros)==true)
            {
                Listarproductosagregados(); //al momento de aplicarse volveremos a cargar los productos ya actualizados
            }
            }
            
        }

        private void datalistadoDetalleVenta_Click(object sender, EventArgs e)
        {

        }

        private void btnprecio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtmonto.Text))//si el txtmnto es diferente a null o vacio se procedera al proceso
            {
                LdetalleVenta parametros = new LdetalleVenta();//parametros de la clase ldetalleventa
                Editar_datos funcion = new Editar_datos();//la funcion a realizar sera editarprecioventa
                //pasamos los parametros 
                parametros.iddetalle_venta = iddetalleventa;
                parametros.preciounitario = Convert.ToDouble(txtmonto.Text);
                if (funcion.editarPrecioVenta(parametros) == true)//si los parametros fueron correctos e igual la funcion se realizara el nuevo proceso
                {
                    Listarproductosagregados(); //actualizamos los productos
                }
            }
        }

        private void btndevoluciones_Click(object sender, EventArgs e)
        {
            HistorialVentas.HistorialVentasForm frm = new HistorialVentas.HistorialVentasForm();
            frm.ShowDialog();
        }

        private void IndicadorTema_CheckedChanged(object sender, EventArgs e)
        {
            if (IndicadorTema.Checked==true) //si el cambio de tema esta encendido daremos un tema oscuro
            {
                Tema = "Oscuro";
                EditarTemaCaja();
                TemaOscuro();
                Listarproductosagregados();
            }
            else //si esta apagado volvera a los colores normales
            {
                Tema = "Redentor";
                EditarTemaCaja();
                TemaClaro();
                Listarproductosagregados();

            }
        }
        private void EditarTemaCaja() //configurar el color del form para caja
        {
            Lcaja parametros = new Lcaja();
            Editar_datos funcion = new Editar_datos();
            parametros.Tema = Tema;
            funcion.EditarTemaCaja(parametros);
           
        }
        private void TemaOscuro()
        {
            //PanelC1 Encabezado
            PanelC1.BackColor = Color.FromArgb(35, 35, 35);
            lblNombreSoftware.ForeColor = Color.White;
            txtbuscar.BackColor = Color.FromArgb(20, 20, 20);
            txtbuscar.ForeColor = Color.White;
            lbltipodebusqueda2.BackColor = Color.FromArgb(20, 20, 20);
            //PanelC2 Intermedio
            panelC2.BackColor = Color.FromArgb(35, 35, 35);
            btnCobros.BackColor = Color.FromArgb(45, 45, 45);
            btnCobros.ForeColor = Color.White;
            btnverMovimientosCaja.BackColor = Color.FromArgb(45, 45, 45);
            btnverMovimientosCaja.ForeColor = Color.White;

            btnadmin.BackColor = Color.FromArgb(45, 45, 45);
            btnadmin.ForeColor = Color.White;
            BtnCerrar_turno.BackColor = Color.FromArgb(45, 45, 45);
            BtnCerrar_turno.ForeColor = Color.White;

            btnCreditoCobrar.BackColor = Color.FromArgb(45, 45, 45);
            btnCreditoCobrar.ForeColor = Color.White;
            btnCreditoPagar.BackColor = Color.FromArgb(45, 45, 45);
            btnCreditoPagar.ForeColor = Color.White;

            //PanelC3
            PanelC3.BackColor = Color.FromArgb(35, 35, 35);
            btnMayoreo.BackColor = Color.FromArgb(45, 45, 45);
            btnMayoreo.ForeColor = Color.White;
            btnIngresosCaja.BackColor = Color.FromArgb(45, 45, 45);
            btnIngresosCaja.ForeColor = Color.White;
            btnGastos.BackColor = Color.FromArgb(45, 45, 45);
            btnGastos.ForeColor = Color.White;
            btnPagos.BackColor = Color.FromArgb(45, 45, 45);
            btnPagos.ForeColor = Color.White;
            BtnTecladoV.BackColor = Color.FromArgb(45, 45, 45);
            BtnTecladoV.ForeColor = Color.White;
            //PanelC4 Pie de pagina
            panelC4.BackColor = Color.FromArgb(20, 20, 20);
            btnespera.BackColor = Color.FromArgb(20, 20, 20);
            btnespera.ForeColor = Color.White;
            btnrestaurar.BackColor = Color.FromArgb(20, 20, 20);
            btnrestaurar.ForeColor = Color.White;
            btneliminar.BackColor = Color.FromArgb(20, 20, 20);
            btneliminar.ForeColor = Color.White;
            btndevoluciones.BackColor = Color.FromArgb(20, 20, 20);
            btndevoluciones.ForeColor = Color.White;
            //PanelOperaciones
            PanelOperaciones.BackColor = Color.FromArgb(28, 28, 28);
            txt_total_suma.ForeColor = Color.WhiteSmoke;
            //PanelBienvenida
            panelBienvenida.BackColor= Color.FromArgb(35, 35, 35);
            label8.ForeColor = Color.WhiteSmoke;
            Listarproductosagregados();
            


        }
        private void TemaClaro()
        {
            //PanelC1 encabezado
            PanelC1.BackColor = Color.White;
            lblNombreSoftware.ForeColor = Color.Black;
            txtbuscar.BackColor = Color.White;
            txtbuscar.ForeColor = Color.Black;
            lbltipodebusqueda2.BackColor = Color.White;

            //PanelC2 intermedio
            panelC2.BackColor = Color.White;
            btnCobros.BackColor = Color.WhiteSmoke;
            btnCobros.ForeColor = Color.Black;
            btnverMovimientosCaja.BackColor = Color.WhiteSmoke;
            btnverMovimientosCaja.ForeColor = Color.Black;

            btnadmin.ForeColor = Color.Black;
            btnadmin.BackColor = Color.WhiteSmoke;
            BtnCerrar_turno.ForeColor = Color.Black;
            BtnCerrar_turno.BackColor = Color.WhiteSmoke;

            btnCreditoCobrar.BackColor = Color.WhiteSmoke;
            btnCreditoCobrar.ForeColor = Color.Black;
            btnCreditoPagar.BackColor = Color.WhiteSmoke;
            btnCreditoPagar.ForeColor = Color.Black;

            //PanelC3
            PanelC3.BackColor = Color.White;
            btnMayoreo.BackColor = Color.WhiteSmoke;
            btnMayoreo.ForeColor = Color.Black;
            btnIngresosCaja.BackColor = Color.WhiteSmoke;
            btnIngresosCaja.ForeColor = Color.Black;
            btnGastos.BackColor = Color.WhiteSmoke;
            btnGastos.ForeColor = Color.Black;
            btnPagos.BackColor = Color.WhiteSmoke;
            btnPagos.ForeColor = Color.Black;
            BtnTecladoV.BackColor = Color.WhiteSmoke;
            BtnTecladoV.ForeColor = Color.Black;
            //PanelC4 pie de pagina
            panelC4.BackColor = Color.Gainsboro;
            btnespera.BackColor = Color.Gainsboro;
            btnespera.ForeColor = Color.Black;
            btnrestaurar.BackColor = Color.Gainsboro;
            btnrestaurar.ForeColor = Color.Black;
            btneliminar.BackColor = Color.Gainsboro;
            btneliminar.ForeColor = Color.Black;
            btndevoluciones.BackColor = Color.Gainsboro;
            btndevoluciones.ForeColor = Color.Black;
            //PanelOperaciones
            PanelOperaciones.BackColor = Color.FromArgb(242, 243, 245);
            txt_total_suma.ForeColor = Color.Black;
            //PanelBienvenida
            panelBienvenida.BackColor = Color.White;
            label8.ForeColor = Color.FromArgb(64, 64, 64);
            Listarproductosagregados();


        }

        private void txtbuscar_KeyDown(object sender, KeyEventArgs e)
        {
            EventosTipoBusqueda(e);
            EventosNavegarDgProductos(e); //e es el evento que controla el keydowm
            EventosNavegarDgDetalleVenta(e);
        }

        private void EventosNavegarDgDetalleVenta(KeyEventArgs e)
        {
            if (dgProductos.Visible == false) //si el dgproductos nose muestra nos permitira utilizar el arriba y abajo
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) //si presiono la tecla arriba o la tecla abajo 
                {
                    datalistadoDetalleVenta.Focus(); //entonces el focus se posicionara en el dgproductos
                }
                
            }
        }

        private void EventosNavegarDgProductos(KeyEventArgs e)
        {
            if(dgProductos.Visible==true) //si el dgproductos esta visible es xk se esta haciendo busqueda
            {
                EstadoCobrar = true;

                if (e.KeyCode== Keys.Enter)//si preciono enter se registrara a la venta el producto escogido
                {
                    EstadoCobrar = false;
                    vender_por_teclado();
                }

                if(e.KeyCode==Keys.Up || e.KeyCode == Keys.Down) //si presiono la tecla arriba o la tecla abajo 
                {
                    dgProductos.Focus(); //entonces el focus se posicionara en el dgproductos
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter && EstadoCobrar == true)//si preciono enter pero el estado no se a hecho true no se ejeutara para vender
                {
                    cobrar();
                }
            }
        }

        private void datalistadoDetalleVenta_KeyDown(object sender, KeyEventArgs e)
        {
            EventosTipoBusqueda(e);
            EventoCobros(e);
        }

        private void EventoCobros(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgProductos.Visible == false)
            {
                cobrar();
            }
        }

        private void EventosTipoBusqueda(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1) //e = eventos, si preciono de las teclas el f1 se activara el modo lectora
            {
                ModoLectora();
            }
            if (e.KeyCode == Keys.F2) //e = eventos, si preciono de las teclas el f2 se activara el modo teclado
            {
                ModoTeclado();
            }

            if (e.KeyCode == Keys.Escape) //e = eventos, si preciono de las teclas el f2 se activara el modo teclado
            {
                ValidarTiposBusqueda(); //CON ESCAPE SE VOLVERA AL TIPO DE BUSQUEDA CONFIGURADO POR DEFECTO 
            }
        }

        private void dgProductos_KeyDown(object sender, KeyEventArgs e)
        {
            EventosNavegarDgProductos(e);
            EventosTipoBusqueda(e);
        }

        private void btnMovimientosCaja_Click(object sender, EventArgs e)
        {
            //mostramos los datos de gastos e ingresos dunranre la caja esta aperturada
            CAJA.Listado_gastos_ingresos frm = new CAJA.Listado_gastos_ingresos();
            frm.ShowDialog();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Dispose(); //cerramos form anterior y aperturamos el de cieere de caja
            CAJA.CIERRE_DE_CAJA frm = new CAJA.CIERRE_DE_CAJA();
            frm.ShowDialog();
        }

        private void MenuStrip21_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void BtnTecladoV_Click(object sender, EventArgs e)
        {
           // txtbuscar.Clear();
           // txtbuscar.Focus();
        }

    }
}   
