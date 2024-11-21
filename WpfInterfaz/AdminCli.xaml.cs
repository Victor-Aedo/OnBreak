using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Behaviours;
using Biblioteca_OnBreak;

namespace WpfInterfaz
{
    /// <summary>
    /// Interaction logic for AdminCli.xaml
    /// </summary>
    public partial class AdminCli : MetroWindow
    {
        public AdminCli()
        {
            InitializeComponent();
            LimpiarControles();
            MostrarDatos();
            LimpiarLabels();
        }

        //private ClienteCollection clientes = new ClienteCollection();

        private void LimpiarControles() //Metodo que se encargará de Limpiar Todos los controles (TextBox,ComboBox,etc) y dejará valores por defecto.
        {
            txtRut.Text = string.Empty;
            txtdig.Text = string.Empty;
            txtRSocial.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtFono.Text = string.Empty;
            cboActividad.ItemsSource = Enum.GetValues(typeof(ActividadEmpresa));
            cboActividad.SelectedIndex = -1;
            cboTipo.ItemsSource = Enum.GetValues(typeof(TipoEmpresa));
            cboTipo.SelectedIndex = -1;
            cboFilActividad.ItemsSource = Enum.GetValues(typeof(ActividadEmpresa));
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.ItemsSource = Enum.GetValues(typeof(TipoEmpresa));
            cboFilTipoEmpresa.SelectedIndex = -1;

            dtgClientes.SelectedIndex = -1;
        }

        private void LimpiarLabels() //Metodo que se encargara de dejar los Labels vacios
        {   //estos lavels se llenarán con notificaciones de llenado de campo como "* ingrese información debidamente en el campo"
            //existe un label respectivo para cada campo. (ej: label rut para txtbox rut)
            lblRut.Content = string.Empty;
            lblRSocial.Content = string.Empty;
            lblDireccion.Content = string.Empty;
            lblNombre.Content = string.Empty;
            lblCorreo.Content = string.Empty;
            lblFono.Content = string.Empty;
            lblActividad.Content = string.Empty;
            lblTipo.Content = string.Empty;
        }

        private void MostrarDatos() //Metodo que se encargará de cargar el Data Grid con los datos de la colección "clientes" y luego refrescarlos.
        {
            dtgClientes.ItemsSource = MainWindow.listaclientes;
            dtgClientes.Items.Refresh();
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.SelectedIndex = -1;
            dtgClientes.SelectedIndex = -1; //Se especifica que por defecto ningún item se encuentre seleccionado en la tabla
        }

        private async void BtmBuscar_Click(object sender, RoutedEventArgs e) //Acción Click en botón buscar. Se encargará de buscar un cliente por el Rut.
        {
            LimpiarLabels();
            if (txtRut.Text != string.Empty && txtdig.Text != string.Empty) //si los campos rut y digito verificador no se encuentran vacios...
            {
                if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //Si se encuentra un cliente en la coleccion con el rut entonces...
                {
                    CargarCliente(txtRut.Text + "-" + txtdig.Text); //se llamará al metodo que cargará todos los datos del objeto cliente con el mismo rut en los campos respectivos.
                }
                else
                    await this.ShowMessageAsync("Ups...", "Cliente no encontrado."); //si no se encuentra una coincidencia de rut, se notificará al usuario.
            }
            else
            {
                lblRut.Content = "* Complete el Campo"; //si los campos rut y/o digito verificador están vaciós se pedirá llenarlos para poder realizar la busqueda.
            }
    }

        private async void BtmAgregar_Click(object sender, RoutedEventArgs e) //Acción del Botón Agregar (agregará un objeto del tipo cliente a la colección)
        {
            if (!MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //metodo de la colección que verificará si el rut ya existe en la memoria.
            {
                if (ConfirmarCampos()) //se llamará al metodo encargado de confirmar que todos los campos necesarios se encuentren llenados correctamente
                {                      //y se notificará los que no lo estén.
                    Cliente cli = new Cliente() //Se creara un objeto del tipo Cliente y luego se guardarán todos los campos en las propiedades respectivas.
                    {
                        Rut = txtRut.Text + "-" + txtdig.Text,
                        Razon_Social = txtRSocial.Text,
                        Direccion = txtDireccion.Text,
                        Nombre = txtNombre.Text,
                        Correo = txtCorreo.Text,
                        Fono = txtFono.Text,
                        Actividad_Empresa = (ActividadEmpresa)cboActividad.SelectedValue,
                        Tipo_Empresa = (TipoEmpresa)cboTipo.SelectedValue
                    };
                    MainWindow.listaclientes.Add(cli); //se agrega el cliente a la colección.
                    await this.ShowMessageAsync("Cliente Agregado", "El cliente fué agregado con éxito."); //mensaje que notifica que todo salió bien.
                    MostrarDatos(); //se actualizan los datos en el data grid.
                    LimpiarControles(); //Metodo que limpiará todos los controles.
                }
            }
            else
            {
                await this.ShowMessageAsync("Ups", "Ya existe un cliente con el Rut: " + txtRut.Text + "-" + txtdig.Text);  //si el rut ya se encontraba anteriormente en la colección
            }                                                                                                         //se notificará.
        }

        private async void BtmActualizar_Click(object sender, RoutedEventArgs e) //Acción del botón Actualizar (actualiza los datos de un objeto en la colección)
        {
            LimpiarLabels(); //método que limpiara los labels.
            try
            {
                if (txtRut.Text != string.Empty && txtdig.Text != string.Empty) //si los campos de rut y digito verificador no se encuentran vacios entonces...
                {
                    if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //Si se encuentra un cliente en la coleccion con el rut entonces...
                    {
                        if (ConfirmarCampos())  //se llamará al metodo que verificará que todos los campos hayan sido llenados y de forma correcta.
                        {
                            Cliente cliente = MainWindow.listaclientes.GetCliente(txtRut.Text + "-" + txtdig.Text); //se crea un objeto cliente con las propiedades
                                                                                                    //respectivas del objeto en la colección que posee el mismo rut
                            cliente.Rut = txtRut.Text + "-" + txtdig.Text;                          //del cual se verificó su existencia anteriormente.
                            cliente.Razon_Social = txtRSocial.Text;
                            cliente.Nombre = txtNombre.Text;
                            cliente.Correo = txtCorreo.Text;
                            cliente.Direccion = txtDireccion.Text;
                            cliente.Fono = txtFono.Text;
                            cliente.Actividad_Empresa = (ActividadEmpresa)cboActividad.SelectedValue;
                            cliente.Tipo_Empresa = (TipoEmpresa)cboTipo.SelectedValue;

                            MostrarDatos(); //se actualizan los datos de la tabla cliente
                            LimpiarControles(); //método que limpia todos los controles.
                            await this.ShowMessageAsync("Cliente Actualizado", "El cliente ha sido actualizado con éxito."); //mensaje que notifica que todo salión bien.
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Ups...", "El cliente que desea actualizar no existe."); //si el rut ingresado no existe en la colección se notificará al usuario.
                    }
                }
                else
                {
                    lblRut.Content = "* Complete el Campo"; //si los campos de rut y digito verificador se encuentran vacios entonces...
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void BtmBorrar_Click(object sender, RoutedEventArgs e) //Acción del Botón Borrar
        {
            var mySettings = new MetroDialogSettings() //se crea un objeto del tipo MetroDialogSettings (dialogo del tipo yes,no)
            {
                AffirmativeButtonText = "Aceptar",     //se especifica el contenido de los botones "afirmativo y negativo"/yes,no)
                NegativeButtonText = "Cancelar",
            };

            LimpiarLabels(); //metodo que limpia los labels
            if (txtRut.Text != string.Empty && txtdig.Text != string.Empty) //si los campos de rut y digito verificador no se encuentran vacios entonces...
            {
                if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //Si se el rut existe en un objeto de la coleccion clientes entonces...
                {
                    if(!MainWindow.listacontratos.ClienteExiste(txtRut.Text + "-" + txtdig.Text)) //Si el rut se encuentra registrado en la coleccion de contratos significa que existe un contrato asociado al rut.
                    {
                        CargarCliente(txtRut.Text + "-" + txtdig.Text); //se llamará al metodo que cargará todos los datos del objeto cliente en los campos respectivos. (algo estético)
                        MessageDialogResult result = await this.ShowMessageAsync("¿Está Seguro?", "¿Está seguro de eliminar este cliente?", MessageDialogStyle.AffirmativeAndNegative, mySettings); //Mensaje Dialogo que solicita al usuario confirmar la acción de eliminar.
                        if (result == MessageDialogResult.Affirmative) //si la respuesta es afirmativa(yes)...
                        {
                            Cliente cliente = MainWindow.listaclientes.GetCliente(txtRut.Text + "-" + txtdig.Text); //se busca en la coleccion un onjeto que coincida con el rut. Luego se cargan sus datos en un nuevo obeto.
                            MainWindow.listaclientes.Remove(cliente); //se borra de la colección el cliente que contenga los mismos datos del nuevo objeto cliente.
                            dtgClientes.Items.Refresh(); //Se actualiza la tabla de clientes. 
                            LimpiarControles(); //metodo que limpia todos los campos.
                            await this.ShowMessageAsync("Cliente Eliminado", "El cliente ha sido eliminado con éxito."); //mensaje que notifica al usuario que se eliminó correctamente al cliente.
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Ups...", "El cliente no puede ser eliminado debido a que este posee uno o más contratos registrados.");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("Ups...", "El cliente que desea eliminar no existe."); //si el rut no se encuentra en un objeto de la coleccion cliente se notificará al usuario.
                }
            }
            else
            {
                lblRut.Content = "* Complete el Campo"; //si los campos rut y codigo verificador no se encuentran llenados se notificará al usuario.
            }
        }

        private void BtmListaCli_Click(object sender, RoutedEventArgs e) //accion de seleccionar un item en la tabla de clientes (data grid) 
        {
            txtFilRut.Text = string.Empty;
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.SelectedIndex = -1;
            MostrarDatos();
            FlyLista.IsOpen = true;   //se despliega un menu en donde se encuentra la tabla (función del Mah apps MetroWindows)
             
        }

        private bool ConfirmarCampos() //Método que se encarga de confirmar que todos los campos se encuentren llenados y de forma correcta.
        {                              //retorna un booleano true or false.
            LimpiarLabels(); //metodo que limpia los Labels.

            string mensaje = "* Complete el Campo";  //mensaje por defecto si el contador x es distinto de 0
            int x = 0; //por defecto es 0, si el valor permanecé de esta forma el retorno será true.

            if (txtRut.Text == string.Empty || txtdig.Text == string.Empty)
            {
                lblRut.Content = mensaje;
                x = 1;
            }
            else
            {
                if (txtRut.Text.Length < 7) //si el campo rut posee menos de 7 carácteres se notificará una notificación de formato invalido.
                {
                    lblRut.Content = "* Rut inválido(min. 7 dig.)";
                    x = 1;
                }
            }
            if (txtRSocial.Text == string.Empty)
            {
                lblRSocial.Content = mensaje;
                x = 1;
            }
            if (txtNombre.Text == string.Empty)
            {
                lblNombre.Content = mensaje;
                x = 1;
            }
            if (txtCorreo.Text == string.Empty)
            {
                lblCorreo.Content = mensaje;
                x = 1;
            }
            if (txtDireccion.Text == string.Empty)
            {
                lblDireccion.Content = mensaje;
                x = 1;
            }
            if (txtFono.Text == string.Empty)
            {
                lblFono.Content = mensaje;
                x = 1;
            }
            if (cboActividad.SelectedIndex == -1)
            {
                lblActividad.Content = mensaje;
                x = 1;
            }
            if (cboTipo.SelectedIndex == -1)
            {
                lblTipo.Content = mensaje;
                x = 1;
            }

            switch (x)
            {
                case 0:
                    return true;
                default:
                    return false;
            }

        }

        private void CargarCliente(string rut) //método encargado de llenar los campos con los datos de un objeto de la coleccion en el que coincida el rut.
        {                                      //a este método se le necesita entregar un rut.
            Cliente cliente = MainWindow.listaclientes.GetCliente(rut);
            int rutlenght;
            rutlenght = cliente.Rut.Length - 2;

            txtRut.Text = cliente.Rut.Substring(0, rutlenght);
            txtdig.Text = cliente.Rut.Substring(cliente.Rut.IndexOf('-') + 1);
            txtRSocial.Text = cliente.Razon_Social;
            txtDireccion.Text = cliente.Direccion;
            txtNombre.Text = cliente.Nombre;
            txtCorreo.Text = cliente.Correo;
            txtFono.Text = cliente.Fono;
            cboActividad.SelectedValue = cliente.Actividad_Empresa;
            cboTipo.SelectedValue = cliente.Tipo_Empresa;
        }

        private void CargarDesdeGrilla(string rut) //Método que llena los campos con la información de un objeto seleccionado en la tabla 
        {
            try
            {
                Cliente cliente = MainWindow.listaclientes.GetCliente(rut); //se crea un objeto del tipo cliente en donde se guardan los datos de un ojeto de la colección con el mismo ínidice.
                int rutlenght; //variable que guarda el largo del rut
                rutlenght = cliente.Rut.Length - 2; //el valor de la variable es el largo del rut del objeto cliente - 2 (osea rut 12345678-2 =  12345678)

                txtRut.Text = cliente.Rut.Substring(0, rutlenght);  //se cargan los datos del objeto cliente en los campos.
                txtdig.Text = cliente.Rut.Substring(cliente.Rut.IndexOf('-') + 1);
                txtRSocial.Text = cliente.Razon_Social;
                txtDireccion.Text = cliente.Direccion;
                txtNombre.Text = cliente.Nombre;
                txtCorreo.Text = cliente.Correo;
                txtFono.Text = cliente.Fono;
                cboActividad.SelectedValue = cliente.Actividad_Empresa;
                cboTipo.SelectedValue = cliente.Tipo_Empresa;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dtgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e) //acción de seleccionar un objeto en la data grid (tabla)
        {
            try
            {
                //verifico si seleccionó algo de la lista, por defecto -1 significa que nada está seleccionado
                if (dtgClientes.SelectedIndex != -1)
                {
                    //CargarDesdeGrilla(dtgClientes.SelectedIndex); //si el index es diferente de -1 se llama al metodo encargado de llenar los campos.
                    CargarDesdeGrilla(dtgClientes.SelectedItem.GetType().GetProperty("Rut").GetValue(dtgClientes.SelectedItem, null).ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void txtFono_PreviewTextInput_1(object sender, TextCompositionEventArgs e) //metodo que no permite ingresar caracteres exepto números y "+" en el textbox Fono
        {
            if ((e.Text != "+") && IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }
        }

        private void txtRut_PreviewTextInput(object sender, TextCompositionEventArgs e) //metodo que no permite ingresar caracteres exepto números en el textbox Rut
        {
            if (IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }
        }

        private void txtdig_PreviewTextInput(object sender, TextCompositionEventArgs e) //metodo que no permite ingresar caracteres exepto números y "K" en el textbox digito verificador
        {
            if ((e.Text.ToUpper() != "K") && IsNumber(e.Text)== false)
            {
                e.Handled = true;
            }
        }

        private bool IsNumber(string Text) //metodo que verifica que el caracter ingresado se un número, retorna un booleano
        {
            int output;
            return int.TryParse(Text, out output);
        }

        private void cboFilActividad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFilActividad.SelectedIndex != -1)
            {
                filtrar();
            }
        }

        private void txtFilRut_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrar();
        }

        private void cboFilTipoEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtrar();
        }

        public void filtrar()
        {
            if (txtFilRut.Text != string.Empty && cboFilActividad.SelectedIndex != -1 && cboFilTipoEmpresa.SelectedIndex != -1)
            {
                dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Rut.StartsWith(txtFilRut.Text) && x.Actividad_Empresa.ToString() == cboFilActividad.SelectedItem.ToString() && x.Tipo_Empresa.ToString() == cboFilTipoEmpresa.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
            }
            else
            {
                if (txtFilRut.Text != string.Empty && cboFilActividad.SelectedIndex != -1)
                {
                    dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Rut.StartsWith(txtFilRut.Text) && x.Actividad_Empresa.ToString() == cboFilActividad.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                }
                else
                {
                    if (txtFilRut.Text != string.Empty && cboFilTipoEmpresa.SelectedIndex != -1)
                    {
                        dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Rut.StartsWith(txtFilRut.Text) && x.Tipo_Empresa.ToString() == cboFilTipoEmpresa.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                    }
                    else
                    {
                        if (cboFilActividad.SelectedIndex != -1 && cboFilTipoEmpresa.SelectedIndex != -1)
                        {
                            dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Actividad_Empresa.ToString() == cboFilActividad.SelectedItem.ToString() && x.Tipo_Empresa.ToString() == cboFilTipoEmpresa.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                        }
                        else
                        {
                            if (txtFilRut.IsFocused)
                            {
                                dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Rut.StartsWith(txtFilRut.Text)) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                            }
                            else
                            {
                                if (cboFilActividad.SelectedIndex != -1)
                                {
                                    dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Actividad_Empresa.ToString() == cboFilActividad.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                                }
                                else
                                {
                                    if (cboFilTipoEmpresa.SelectedIndex != -1)
                                    {
                                        dtgClientes.ItemsSource = from x in MainWindow.listaclientes where (x.Tipo_Empresa.ToString() == cboFilTipoEmpresa.SelectedItem.ToString()) select new { x.Rut, x.Razon_Social, x.Nombre, x.Correo, x.Direccion, x.Fono, x.Actividad_Empresa, x.Tipo_Empresa };
                                    }
                                    else
                                    {
                                        MostrarDatos();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BtmLimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {
            txtFilRut.Text = string.Empty;
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.SelectedIndex = -1;
            filtrar();
        }

        private void BtmSalir_Click(object sender, RoutedEventArgs e) //acción del botón salir.
        {
            this.Close();
        }

        
    }
}
