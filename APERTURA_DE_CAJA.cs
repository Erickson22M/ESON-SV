using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Management;
using System.Xml;
using SV_EDSON.Logica;
using SV_EDSON.Datos;
namespace SV_EDSON.Presentacion.CAJA
{
    public partial class APERTURA_DE_CAJA : Form
    {
        public APERTURA_DE_CAJA()
        {
            InitializeComponent();
        }
        int txtidcaja;
       

        private void ToolStripMenuItem2_Click(object sender, EventArgs e) //si damos click en iniciar debemos indicar antes el dinero en caja
        {
          bool  estado = Editar_datos.editar_dinero_caja_inicial(txtidcaja, Convert.ToDouble(txtmonto.Text));
            if (estado ==true ) //si el estado de la caja ya tiene capturado el dinero ingresado 
            {
                //pasamos al meno de ventas
                pasar_a_ventas();
            }       
        }

  
        private void APERTURA_DE_CAJA_Load(object sender, EventArgs e)
        {
            //cambiamos el idioma regional para trabajar con comas 
            Bases.Cambiar_idioma_regional();
            //odtenemos los datos de la caja con el id que se esta trabajando
            Obtener_datos.Obtener_id_caja_PorSerial(ref txtidcaja);
            //damos mejor aspecto al formulario centrando panel
            centrar_panel();
        }
        private void centrar_panel()
        {
            PanelCaja.Location = new Point((Width - PanelCaja.Width) / 2, (Height - PanelCaja.Height) / 2);
        }

        private void btnomitir_Click(object sender, EventArgs e)
        {
            //si damos omitir en vez de iniciar ingresaremos al form ventas sin dinero en caja
            pasar_a_ventas();
        }
        private void pasar_a_ventas() //proceso que nos enviara al formulario venras principal
        {
            Dispose();
            VENTAS_MENU_PRINCIPAL.VENTAS_MENU_PRINCIPALOK frm = new VENTAS_MENU_PRINCIPAL.VENTAS_MENU_PRINCIPALOK();
            frm.ShowDialog();
           
        }

        private void txtmonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Bases.Separador_de_Numeros(txtmonto, e); //proceso para verificar e capturar el monto que se de para la apertura
        }
    }
}
