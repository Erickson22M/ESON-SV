using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using SV_EDSON.Logica;


namespace SV_EDSON

{
    public partial class usuariosok : Form
    {
        public usuariosok()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Cargar_estado_de_iconos()//proceso para cargar y mostrar los iconos para ususarios
        {
            try //proteccion del proceso
            {
                foreach (DataGridViewRow row in datalistado.Rows)//recorremos el dt para capturar los datos de las filas
                {

                    try
                    {

                        string Icono = Convert.ToString(row.Cells["Nombre_de_icono"].Value);  //capturamos columna nombre icono para utilizarlo

                        if (Icono == "1" ) //dependiiendo del valor que demos se nos mostraran cada icono
                        {
                            pictureBox3.Visible = false;
                        }
                        else if (Icono == "2")
                        {
                            pictureBox4.Visible = false;
                        }
                        else if (Icono == "3")
                        {
                            pictureBox5.Visible = false;
                        }
                        else if (Icono == "4")
                        {
                            pictureBox6.Visible = false;
                        }
                        else if (Icono =="5")
                        {
                            pictureBox7.Visible = false;
                        }
                        else if (Icono == "6")
                        {
                            pictureBox8.Visible = false;
                        }
                        else if (Icono == "7")
                        {
                            pictureBox9.Visible = false;
                        }
                        else if (Icono == "8")
                        {
                            pictureBox10.Visible = false;
                        }
                        //en total se mostraran 10 iconos para ser escogidos
                    }
                    catch (Exception ex)
                    {


                    }


                }
            }
            catch (Exception ex)
            {

            }
        }
       public bool validar_Mail(string sMail)//proceso para validar imail al crear ususario
        {
            //debe seguir la logica la cual es usuario@ejemplo.com
            return Regex.IsMatch(sMail, @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");

        }
        private void btnGuardar_Click(object sender, EventArgs e)//al dar clic en guardar se validara lo siguiente
        {
            if (validar_Mail(txtcorreo.Text) == false) //validamos correo si no es correcto mostramos msj
            {
                MessageBox.Show("Dirección de correo electronico no valida, el correo debe tener el formato: nombre@dominio.com, " + " por favor seleccione un correo valido", "Validación de correo electronico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtcorreo.Focus(); //barrita parpadiante en el txtcorreo
                txtcorreo.SelectAll();//seleccionamos el anterior para borrar
            }
            else
            {
                //si salio bien lo del correo validamos lo siguiente
                if (txtnombre.Text != "") //verificamos que no este vacio el campo nombre

                {
                    if (txtrol .Text != "") //validamos para que no quede vacio el campo rol
                    {

                        if (LblAnuncioIcono.Visible == false) //debe de escogerse un icono 

                        {
                            try //al realizar todas las validaciones capturamos los datos mostrados listos para ejecutar el proceso de insertar
                            {
                                SqlConnection con = new SqlConnection(); //creamos conexion
                                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion; //pasamos direccion
                                con.Open();//aperturamos conexion
                                SqlCommand cmd = new SqlCommand(); //inntacionamos el comando para usar proceso
                                cmd = new SqlCommand("insertar_usuario", con); //indicamos el proceso a utilizar
                                cmd.CommandType = CommandType.StoredProcedure;//interpretacion del proceso
                                //pasamos parametros necesarios
                                cmd.Parameters.AddWithValue("@nombres", txtnombre.Text);
                                cmd.Parameters.AddWithValue("@Login", txtlogin.Text);
                                cmd.Parameters.AddWithValue("@Password",Bases.Encriptar (txtPassword.Text));

                                cmd.Parameters.AddWithValue("@Correo", txtcorreo.Text);
                                cmd.Parameters.AddWithValue("@Rol", txtrol.Text);
                                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                                ICONO.Image.Save(ms, ICONO.Image.RawFormat);


                                cmd.Parameters.AddWithValue("@Icono", ms.GetBuffer());
                                cmd.Parameters.AddWithValue("@Nombre_de_icono", lblnumeroIcono.Text);
                                cmd.Parameters.AddWithValue("@Estado", "ACTIVO");

                                cmd.ExecuteNonQuery();//ejecutamos el proceso
                                con.Close();//cerramos conexion
                                mostrar();//mostramos los usuarios registrados ya con el nuevo
                                panelRegistros.Visible = false; //ocultamos panel registro
                                panelNuevo.Visible = true; //iniciamos el nuevo panel con los ususairos
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);//si sucedio un error lo indicaremos con un msj informatio
                            }


                        }
                        else //si no se escogio icono mandamos msj para que lo agregre
                        {
                            MessageBox.Show("Elija un Icono", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }



                    }
                    else //por si no escogio rol
                    {
                        MessageBox.Show("Elija un Rol", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                }
                else //por si lleno algunos de los campos de registro como el nombre
                {
                    MessageBox.Show("Asegúrese de haber llenado todos los campos para poder continuar", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
        }
        private void mostrar() //proceso para mostrar los usuarios registrados
        {
            try
            {
            DataTable dt = new DataTable(); //creamos dt para mostrar los datos 
            SqlDataAdapter da;//comandos para poder utilizar el dataset para capturar los datos de la base
            SqlConnection con = new SqlConnection(); //creamos conexion
            con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena de conexion
            con.Open();//aperturamos conexion
         
            da = new SqlDataAdapter("mostrar_usuario", con);//pasamos instancia del proceso a utilizar para mostrar los datos
             



                da.Fill(dt);//agrefamos los datos capturados en el dt
                datalistado.DataSource = dt;
                con.Close();
                //ocultamos columnas no necesarias de mostrar
                datalistado.Columns[1].Visible = false;
                datalistado.Columns[5].Visible = false;
                datalistado.Columns[6].Visible = false;
                datalistado.Columns[7].Visible = false;
                datalistado.Columns[8].Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            Bases.Multilinea(ref datalistado  ); //damos mejor diseno al dt para mostrar los ususarios

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox3.Image;
            lblnumeroIcono.Text = "1";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;

        }

        private void LblAnuncioIcono_Click(object sender, EventArgs e)
        {
            Cargar_estado_de_iconos();
            panelICONO.Visible = true;
            panelICONO.Dock = DockStyle.Fill;


        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox4.Image;
            lblnumeroIcono.Text = "2";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox5.Image;
            lblnumeroIcono.Text = "3";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox6.Image;
            lblnumeroIcono.Text = "4";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox7.Image;
            lblnumeroIcono.Text = "5";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox8.Image;
            lblnumeroIcono.Text = "6";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox9.Image;
            lblnumeroIcono.Text = "7";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            ICONO.Image = pictureBox10.Image;
            lblnumeroIcono.Text = "8";
            LblAnuncioIcono.Visible = false;
            panelICONO.Visible = false;
        }

        private void usuariosok_Load(object sender, EventArgs e)//iniciacion del formulario
        {
            panelRegistros.Visible = false;//ocultamos
            panelICONO.Visible = false;//ocultamos
            mostrar();//mostramos al iniciar formulario el dt con los usuarios disponibles
        }

        private void PictureBox2_Click(object sender, EventArgs e) //proceso para aggregar un nuevo usuario
        {
            panelRegistros.Visible = true; //mostramos panel de registro
            panelRegistros.Dock = DockStyle.Fill; //lo mostramos en toda la pantalla
            panelNuevo.Visible = false;
            LblAnuncioIcono.Visible = true;//mostramos panel para agregar icnono
            //dejamos en blanco todos los campos para que se llenen 
            txtnombre.Text = "";
            txtlogin.Text = "";
            txtPassword.Text = "";
               txtcorreo .Text = "";
            btnGuardar.Visible = true;
            btnGuardarCambios.Visible = false;
        }

        private void datalistado_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void datalistado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)//proceso para mostrar datos del usuario al dar click
        {
            //mostramos datos en sus respectivos campos
            lblId_usuario.Text = datalistado.SelectedCells[1].Value.ToString();
            txtnombre.Text = datalistado.SelectedCells[2].Value.ToString();
            txtlogin.Text = datalistado.SelectedCells[3].Value.ToString();

            txtPassword.Text = datalistado.SelectedCells[4].Value.ToString();
            //aqui agregye esto
            txtPassword.Text = Bases.Desencriptar(txtPassword.Text);//desencriptamos contrasena x si el usuario deseaa modificarla
           

            ICONO.BackgroundImage = null; //borramos imagen por defecto para poder agregar una nueva
            byte[] b = (Byte[])datalistado.SelectedCells[5].Value; //pasamos imagen escogida por el usuario
            MemoryStream ms = new MemoryStream(b);//secuencia en memoria para guardar archivo
            ICONO.Image = Image.FromStream(ms);//mostramos el archivo capturado
        
            LblAnuncioIcono.Visible = false;

            lblnumeroIcono .Text = datalistado.SelectedCells[6].Value.ToString();
            txtcorreo .Text = datalistado.SelectedCells[7].Value.ToString();
            txtrol .Text = datalistado.SelectedCells[8].Value.ToString();
            panelRegistros.Visible = true;
            panelRegistros.Dock = DockStyle.Fill;
            panelNuevo.Visible = false;
            btnGuardar.Visible = false;
            btnGuardarCambios.Visible = true;//hacemos visible el boton por si se actualiza algun dato
        }

        private void btnVolver_Click(object sender, EventArgs e)//proceso para cerrar el panel de registro al dar clic en el boton volver
        {
            panelRegistros.Visible = false;
            panelNuevo.Visible = true;
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)//proceso para modificar datos
        {
            string contrasena; //variable para capturar contrasena
            if (txtnombre.Text != "") //verificamos que haya escogido un usuario disponible
            {
                try
                {
                    SqlConnection con = new SqlConnection(); //creamos conexion
                    con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;//pasamos cadena de conexion
                    con.Open();//aperturamos
                    SqlCommand cmd = new SqlCommand(); //instancia para utilizar procesos de la base de datos
                    cmd = new SqlCommand("editar_usuario", con); //pasamos parametro a utilizar
                    cmd.CommandType = CommandType.StoredProcedure; //interpretamos proceso
                    //pasamos parametros ya modificados por el usuario en sus campos
                    cmd.Parameters.AddWithValue("@idUsuario", lblId_usuario .Text);
                    cmd.Parameters.AddWithValue("@nombres", txtnombre.Text);
                    cmd.Parameters.AddWithValue("@Login", txtlogin.Text);

                    //cmd.Parameters.AddWithValue("@Password",Bases.Desencriptar (txtPassword.Text));
                    cmd.Parameters.AddWithValue("@Password", Bases.Encriptar(txtPassword.Text));
                    contrasena =Convert.ToString (txtPassword);

                    cmd.Parameters.AddWithValue("@Correo", txtcorreo.Text);
                    cmd.Parameters.AddWithValue("@Rol", txtrol.Text);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();//aperturamos espacio en memoria para el icono
                    ICONO.Image.Save(ms, ICONO.Image.RawFormat);//guardamos imagent con el formato permitido


                    cmd.Parameters.AddWithValue("@Icono", ms.GetBuffer());//capturamos imagen y la guardamos en byte
                    cmd.Parameters.AddWithValue("@Nombre_de_icono", lblnumeroIcono.Text);
                    cmd.ExecuteNonQuery();//ejecutamos proceso
                    con.Close();//cerrar conexion
                    mostrar();//mostramos usuarios ya con el modificado
                    panelRegistros.Visible = false;//ocultamos panel de registro
                    panelNuevo.Visible = true;//mostramos panel actualizado con sus asuarios
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }


            }
        }

        private void ICONO_Click(object sender, EventArgs e)//si damos click en el botn de iconos
        {
            //cargamos los iconos disponibles para mostrarlos en el panelicono para ser escogido
            Cargar_estado_de_iconos();
            panelICONO.Visible = true;
            panelICONO.Dock = DockStyle.Fill; //mostramos el panel desplegado completamente
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void datalistado_CellClick(object sender, DataGridViewCellEventArgs e)//dar clic en columas del dt
        {

            if (e.ColumnIndex == this.datalistado.Columns["Eli"].Index)//si damos click en la columna eliminar
            {
                DialogResult result;//identificamos  la accion
                //mostramos msj informativo para confirmar o cancelar la accion de eliminar
                result = MessageBox.Show("¿Realmente desea eliminar este Usuario?", "Eliminando registros", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK) //si el resultado es ok
                {
                  
                    try
                    {
                        foreach (DataGridViewRow row in datalistado.SelectedRows)//recorremos dt para capturar id y login
                        {

                            int onekey = Convert.ToInt32(row.Cells["idUsuario"].Value);//capturamos datos
                            string usuario = Convert.ToString(row.Cells["Login"].Value);

                            try
                            {

                                try
                                {
                                    SqlCommand cmd;//instancias para proceso almacenado
                                    SqlConnection con = new SqlConnection();//nueva conexion
                                    con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion;
                                    con.Open();
                                    cmd = new SqlCommand("eliminar_usuario", con); //proceso a utilizar
                                    cmd.CommandType = CommandType.StoredProcedure;//interpretacion del proceos
                                    //pasamos parametros
                                    cmd.Parameters.AddWithValue("@idusuario", onekey);
                                    cmd.Parameters.AddWithValue("@login", usuario);
                                    cmd.ExecuteNonQuery(); //ejecumtamos proceso para eliminar el usuario
                                   
                                    con.Close();

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show(ex.Message);
                            }

                        }
                        mostrar(); //mostramos los usuarios ya sin el eliminado
                    }

                    catch (Exception ex)
                    {

                    }
                }
            }


            

            
        }

        private void pictureBox11_Click(object sender, EventArgs e)//dar clic al boton para cargar usuario desde la pc
        {
            dlg.InitialDirectory = "";//nombre del directoriio
            dlg.Filter = "Imagenes|*.jpg;*.png"; //filtro para solo aceptar este tipo de archivos
            dlg.FilterIndex = 2; //confirmamos que solo seran 2 tipos de archivos
            dlg.Title = "Cargador de Imagenes Edson SV"; //damos nombre a la ventana de carga
            if (dlg.ShowDialog() == DialogResult.OK) //si se escogio un archivo
            {
                ICONO.BackgroundImage = null; //eliminamos archivo por defecto
                ICONO.Image = new Bitmap(dlg.FileName);//asignamos el nuevo icono
                ICONO.SizeMode = PictureBoxSizeMode.Zoom; //damos medicion y diseno al icono
                lblnumeroIcono.Text = Path.GetFileName(dlg.FileName); //damos valor al icono para que se guaarde
                LblAnuncioIcono.Visible = false; //ocultamos panel
                panelICONO.Visible = false;//ocultamos panel
               // panel1.Visible = false;
              //  panel2.Visible = false;
            }
         }

        private void buscar_usuario()//ptoceso para buscar ususarios
        {
            try
            {
                DataTable dt = new DataTable(); //dt para capturar los datos
                SqlDataAdapter da; //instancia para poder capturar datos de la base con un proceso
                SqlConnection con = new SqlConnection(); //nnueva conexion
                con.ConnectionString = CONEXION.CONEXIONMAESTRA.conexion; //pasamos cadena
                con.Open();//aperturamos conexion

                da = new SqlDataAdapter("buscar_usuario", con);//pasamos proceso para instanciar
                da.SelectCommand.CommandType = CommandType.StoredProcedure;//capturaremos solo un dato
                da.SelectCommand.Parameters.AddWithValue("@letra", txtbuscar.Text);//capturamos solo un usuario
                da.Fill(dt);//pasamos los datos al dt
                datalistado.DataSource = dt; //pasamos los datos del dt al datalistado para mostrarlos
                con.Close();//cerramos coneion
                //ocultamos columnas no necesarias de mostrar
                datalistado.Columns[1].Visible = false;
                datalistado.Columns[5].Visible = false;
                datalistado.Columns[6].Visible = false;
                datalistado.Columns[7].Visible = false;
                datalistado.Columns[8].Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            Bases.Multilinea(ref datalistado);//mejor diseno para el dt

        }
        //proceso para solo utilizar numeros en la contrasena de usuario
        public void Numeros(System.Windows.Forms.TextBox CajaTexto, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                e.Handled = true;

            }


        }
        private void txtbuscar_TextChanged(object sender, EventArgs e)//ingresar dato en el txtbuscar
        {
            buscar_usuario(); //proceso para poder buscar usuarios

        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            Numeros(txtbuscar, e); //pasmos la contrasena para poder iniciar al ususario

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
