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
using System.Globalization;

namespace WpfInterfaz
{
    /// <summary>
    /// Interaction logic for AdminContratos.xaml
    /// </summary>
    public partial class AdminContratos : MetroWindow
    {
        
        public AdminContratos()
        {
            InitializeComponent();
            LimpiarCampos(); //método que limpia los campos txt y cbo
            LimpiarLabels(); //método que limpia los labels
            MostrarDatos(); //método que se encarga de llenar los data grid con los datos de las colecciones

            this.dtpCreacion.Culture = new CultureInfo("es-CL", false); //seteamos el formato de fecha de los DateTimePicker
            this.dtpInicioEvento.Culture = new CultureInfo("es-CL", false);
            this.dtpFinEvento.Culture = new CultureInfo("es-CL", false);
        }

        public void LimpiarCampos() //Método que limpia todos los campos (txt box)
        {
            TxtNumContrato.Text = string.Empty;
            txtRut.Text = string.Empty;
            txtdig.Text = string.Empty;
            txtCliente.Text = string.Empty;
            dtpCreacion.SelectedDate = null;
            dtpTermino.SelectedDate = null;
            dtpInicioEvento.SelectedDate = null;
            dtpFinEvento.SelectedDate = null;
            TxtDireccion.Text = string.Empty;
            CboTipoEvento.SelectedIndex = -1;
            TxtObservacion.Text = string.Empty;
            dtgContratos.SelectedIndex = -1;
            dtgClientes.SelectedIndex = -1;
            txtAsistentes.Text = string.Empty;
            txtPersonal.Text = string.Empty;
            txtTotal.Text = string.Empty;
            txtValorBase.Text = string.Empty;
            txtNombreEvento.Text = string.Empty;
            txtFilRut.Text = string.Empty;
            cboFilActividad.ItemsSource = Enum.GetValues(typeof(ActividadEmpresa));
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.ItemsSource = Enum.GetValues(typeof(TipoEmpresa));
            cboFilTipoEmpresa.SelectedIndex = -1;
            txtFilRutCliente.Text = string.Empty;
            


            CboTipoEvento.DisplayMemberPath = "Nombre";
            cboFilTipoEvento.DisplayMemberPath = "Nombre";
            CboTipoEvento.SelectedValuePath = "Id";
            cboFilTipoEvento.SelectedValuePath = "Id";
            CboTipoEvento.ItemsSource = MainWindow.listaeventos;
            CboTipoEvento.SelectedIndex = -1;
            cboFilTipoEvento.ItemsSource = MainWindow.listaeventos;
            cboFilTipoEvento.SelectedIndex = -1;
            
        }

        public void LimpiarLabels() //Método que limpia los mensajes de los labels
        {
            lblRut.Content = string.Empty;
            lblF_Creacion.Content = string.Empty;
            lblF_Termino.Content = string.Empty;
            lblF_InicioEv.Content = string.Empty;
            lblF_FinEv.Content = string.Empty;
            lblDireccion.Content = string.Empty;
            lblTipoEvento.Content = string.Empty;
        } 

        public void BtmAgregarContrato_Click(object sender, RoutedEventArgs e) //Método que agrega un objeto Contrato a Contrato Collection
        {
            if (ComprobarCampos()) //si los campos se encunetran llenados correctamente
            {
                if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //si el rut existe
                {
                    Contrato con = new Contrato() //se crea un objeto del tipo contrato con los datos ingresados
                    {
                        NumeroContrato = dtpCreacion.SelectedDate.Value.ToString("yyyyMMddHHmm"), //Se crea un número de Contrato
                        F_creacion = dtpCreacion.SelectedDate.Value,
                        F_termino = dtpTermino.SelectedDate.Value,
                        F_hora_inicio = dtpInicioEvento.SelectedDate.Value,
                        F_hora_fin = dtpFinEvento.SelectedDate.Value,
                        Direccion = TxtDireccion.Text,
                        Vigente = Vigencia(),
                        IdTipoEvento = MainWindow.listaeventos.GetIdEvento(CboTipoEvento.SelectedIndex),
                        Observaciones = TxtObservacion.Text,
                        Cliente = MainWindow.listaclientes.GetCliente(txtRut.Text + '-' + txtdig.Text)
                    };

                    if (MainWindow.listacontratos.Existe(con.NumeroContrato)) //si el número de contrato ya existe en la colección contratos
                    {
                        this.ShowMessageAsync("Ups...", "Ye existe un contrato creado en la Fecha " + con.F_creacion.ToString("dd/MM/yyyy HH:mm"));
                    }
                    else //de lo contrario: 
                    {
                        MainWindow.listacontratos.Add(con); //se agrega el contrato a la colección.
                        this.ShowMessageAsync("Contrato Agregado", "El contrato fué agregado con éxito."); //mensaje que notifica que todo salió bien.
                        MostrarDatos(); //se actualizan los datos en el data grid.
                        LimpiarCampos(); //Metodo que limpiará todos los controles.
                        LimpiarLabels();
                    }
                }
                else
                {
                    this.ShowMessageAsync("Ups...", "El Rut no fué encontrado en la base de datos de clientes, antes debe ser registrado.");
                }
            }
        } 

        private async void BtmTerminarContrato_Click(object sender, RoutedEventArgs e) //Método que actualiza la vigencia y fecha de termino de un contrato.
        {
            try
            {
                if (TxtNumContrato.Text != string.Empty) //si el campo numero de contrato no está vacio...
                {
                    if (MainWindow.listacontratos.Existe(TxtNumContrato.Text)) // si el numero de contrato existe en la coleccion...
                    {
                        if (MainWindow.listacontratos.GetContrato(TxtNumContrato.Text).Vigente) //si el contrato se encuentra vigente actualmente. 
                        {
                            var mySettings = new MetroDialogSettings() //se crea un objeto del tipo MetroDialogSettings (dialogo del tipo yes,no)
                            {
                                AffirmativeButtonText = "Aceptar",     //se especifica el contenido de los botones "afirmativo y negativo"/yes,no)
                                NegativeButtonText = "Cancelar",
                            };

                            MessageDialogResult result = await this.ShowMessageAsync("¿Está Seguro?", "¿Está seguro de terminar este contrato?", MessageDialogStyle.AffirmativeAndNegative, mySettings); //Mensaje Dialogo que solicita al usuario confirmar la acción.

                            if (result == MessageDialogResult.Affirmative) //si la respuesta es afirmativa(yes)... 
                            {
                                Contrato contrato = MainWindow.listacontratos.GetContrato(TxtNumContrato.Text); //se crea un objeto contrato con las propiedades de un objeto en la coleccion con el mismo numero de contrato

                                contrato.F_termino = DateTime.Now; //se actualiza la fecha de termino a la fecha y hora actual
                                contrato.Vigente = false; //se actualiza el estado del contrato (propiedad vigencia)

                                MostrarDatos(); //se Actualizan los DataGrid con la nueva información
                                CargarContrato(contrato.NumeroContrato); //se cargan los datos del contrato finalizado en los campos.
                                await this.ShowMessageAsync("Contrato Terminado", "El contrato se ha actualizado con éxito."); //mensaje que notifica que todo salión bien.
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("Contrato no Vigente", "El Contrato no se encuentra vigente actualmente.");
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Contrato no Existe", "El Número de Contrato ingresado no existe.");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("Contrato no seleccionado", "Seleccione antes un contrato de la Lista");
                }
            }
            catch (Exception)
            {

                throw;
            }
        } 

        public void btmBuscarRut_Click(object sender, RoutedEventArgs e) //Método que busca un objeto contrato y cliente by rut del cliente y luego carga los datos en los campos.
        {
            lblRut.Content = string.Empty; // se limpia el contenido del label rut. 

            if (txtRut.Text != string.Empty && txtdig.Text != string.Empty) //si los campos rut y digito verificador no se encuentran vacios...
            {
                if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text)) //Si se encuentra un cliente en la coleccion con el rut entonces...
                {
                    CargarCliente(txtRut.Text + "-" + txtdig.Text); //se llamará al metodo que cargará los datos del objeto cliente con el mismo rut en los campos respectivos.
                }
                else
                    this.ShowMessageAsync("Ups...", "Cliente no encontrado."); //si no se encuentra una coincidencia de rut, se notificará al usuario.
            }
            else
            {
                lblRut.Content = "* Complete el Campo"; //si los campos rut y/o digito verificador están vaciós se pedirá llenarlos para poder realizar la busqueda.
            }
        }   

        private void BtmListaClientes_Click(object sender, RoutedEventArgs e) // Metodo acción del botón Lista de CLientes, muestra y actualiza el data grid de Clientes.
        {
            txtFilRut.Text = string.Empty;
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.SelectedIndex = -1;
            MostrarDatos(); //se refrescan los datos de la tabla
            FlyLista_Clientes.IsOpen = true;   //se despliega un menu en donde se encuentra la tabla (función del Mah apps MetroWindows)
        } 

        private async void btmBuscarContrato_Click(object sender, RoutedEventArgs e) //Metodo que busca un contrato por el número de este
        {
            var result = await this.ShowInputAsync("Ingrese Número de Contrato", ""); //se despliega un mensaje en el que se debe ingresar el número de contrato
            if (result != null) //si la variable result no tiene un valor nulo:
            {
                if (MainWindow.listacontratos.Existe(result))//si el número de contrato existe en la colección de contratos...
                {
                    Contrato contrato = MainWindow.listacontratos.GetContrato(result); //se crea un objeto del tipo contrato con los datos de un contrato en la colección con el mismo número de contrato
                    CargarContrato(contrato.NumeroContrato); //método que carga los datos de un objeto contrato en los campos correspondientes de la interfaz.
                }
                else
                {
                    await this.ShowMessageAsync("Ups...", "Número de Contrato: " + result + " no existe.");
                }
            }
        }

        private void BtmListaContratos_Click(object sender, RoutedEventArgs e) // Método acción del botón Lista Contratos, Muestra y actualiza el DataGrid con los datos de Contrato Collection.
        {
            txtFilContrato.Text = string.Empty;
            txtFilRutCliente.Text = string.Empty;
            cboFilTipoEvento.SelectedIndex = -1;
            MostrarDatos();
            FlyLista_Contratos.IsOpen = true;   //se despliega un menu en donde se encuentra la tabla (función del Mah apps MetroWindows)
        } 

        private void BtmLimpiar_Click(object sender, RoutedEventArgs e) //Metodo acción del botón Limpiar, limpia todos los campos y labels de la interfaz.
        {
            LimpiarCampos();
            LimpiarLabels();
            BloquearControles(false);
        }

        private void BtmCalcular_Click(object sender, RoutedEventArgs e) // Método acción del boton calcular, despliega un Flyout en donde se encuentra la interfaz de calcular.
        {
            FlyCalcularTotal.IsOpen = true;
        }

        public bool ComprobarCampos()//Método encargado de comprobar que todos los datos se encuentren llenados de forma correcta. Retorna un booleano.
        {
            LimpiarLabels(); //metodo que limpia los Labels.

            string mensaje = "* Complete el Campo";  //mensaje por defecto si el contador x es distinto de 0
            int x = 0; //por defecto es 0, si el valor permanecé de esta forma el retorno será True,.

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

            if (dtpCreacion.SelectedDate == null)
            {
                x = 1;
                lblF_Creacion.Content = mensaje;
            }

            if (dtpTermino.SelectedDate == null)
            {
                x = 1;
                lblF_Termino.Content = mensaje;
            }
            else
            {
                if (dtpTermino.SelectedDate <= dtpCreacion.SelectedDate)
                {
                    x = 1;
                    lblF_Termino.Content = "* Error";
                    this.ShowMessageAsync("Ups...", "La fecha de termino del contrato no puede ser igual o anterior a la fecha de creación.");
                }
            }

            if (dtpInicioEvento.SelectedDate == null)
            {
                x = 1;
                lblF_InicioEv.Content = mensaje;
            }
            else
            {
                if (dtpInicioEvento.SelectedDate < dtpCreacion.SelectedDate)
                {
                    x = 1;
                    lblF_InicioEv.Content = "* Error";
                    this.ShowMessageAsync("Ups...", "La fecha del evento no puede ser anterior a la fecha de creación del contrato.");
                }else
                {
                    if(dtpInicioEvento.SelectedDate > dtpTermino.SelectedDate)
                    {
                        x = 1;
                        lblF_InicioEv.Content = "* Error";
                        this.ShowMessageAsync("Ups...", "La fecha del evento no puede ser posterior a la fecha en la que este termina.");
                    }
                }
            }

            if (dtpFinEvento.SelectedDate == null)
            {
                x = 1;
                lblF_FinEv.Content = mensaje;
            }
            else
            {
                if (dtpFinEvento.SelectedDate < dtpCreacion.SelectedDate)
                {
                    x = 1;
                    lblF_FinEv.Content = "* Error";
                    this.ShowMessageAsync("Ups...", "El evento no puede terminar antes de la fecha de creación del mismo.");
                }
                else
                {
                    if(dtpFinEvento.SelectedDate <= dtpInicioEvento.SelectedDate)
                    {
                        x = 1;
                        lblF_FinEv.Content = "* Error";
                        this.ShowMessageAsync("Ups...", "El evento no puede terminar antes, o a la misma hora de la fecha en la que inicia.");
                    }
                    else
                    {
                        if(dtpFinEvento.SelectedDate > dtpTermino.SelectedDate)
                        {
                            x = 1;
                            lblF_FinEv.Content = "* Error";
                            this.ShowMessageAsync("Ups...", "El evento no puede terminar luego de que el contrato haya finalizado.");
                        }
                    }
                }
            }

            if (TxtDireccion.Text == string.Empty)
            {
                x = 1;
                lblDireccion.Content = mensaje;
            }

            if (CboTipoEvento.SelectedIndex == -1)
            {
                x = 1;
                lblTipoEvento.Content = mensaje;
            }
            switch (x)
            {
                case 0:
                    return true;
                default:
                    return false;
            }
        } 

        private void MostrarDatos() //Metodo que se encargará de cargar los Data Grid con los datos de las colecciónes "contratos" y "clietnes" y luego refrescarlos.
        {

            // Dos sintaxis
            //dtgContratos.ItemsSource = MainWindow.listacontratos.Select(x => new { NumeroContrato = x.NumeroContrato, RutCliente = x.Cliente.Rut}).ToList(); // lambda
            dtgContratos.ItemsSource = (from x in MainWindow.listacontratos select new { NumeroContrato = x.NumeroContrato, RutCliente = x.Cliente.Rut, NombreCliente = x.Cliente.Nombre,
                FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"), FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"), FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"), FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                Vigencia = x.Vigente, IDEvento = x.IdTipoEvento, TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre, Observaciones = x.Observaciones }).ToList(); // linq

            dtgContratos.Items.Refresh();
            dtgClientes.ItemsSource = MainWindow.listaclientes;
            dtgClientes.Items.Refresh();
            dtgClientes.SelectedIndex = -1;
            dtgContratos.SelectedIndex = -1;

            //filtros de tabla cliente
            cboFilActividad.ItemsSource = Enum.GetValues(typeof(ActividadEmpresa));
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.ItemsSource = Enum.GetValues(typeof(TipoEmpresa));
            cboFilTipoEmpresa.SelectedIndex = -1;
        }

        private void dtgContratos_SelectionChanged(object sender, SelectionChangedEventArgs e) //acción de seleccionar un objeto en la data grid de Contratos (tabla)
        {
            try
            {
                //verifico si seleccionó algo de la lista, por defecto -1 significa que nada está seleccionado
                if (dtgContratos.SelectedIndex != -1)
                {
                    //Contrato contrato = MainWindow.listacontratos[dtgContratos.SelectedIndex]; //se crea un objeto del tipo contrato en donde se guardan los datos de un ojeto de la colección con el mismo ínidice.
                    //CargarContrato(contrato); //

                    //CargarDesdeGrillaClientes(dtgClientes.SelectedItem.GetType().GetProperty("Rut").GetValue(dtgClientes.SelectedItem, null).ToString());
                    CargarContrato(dtgContratos.SelectedItem.GetType().GetProperty("NumeroContrato").GetValue(dtgContratos.SelectedItem, null).ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dtgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e) //acción de seleccionar un objeto en la data grid de Clientes
        {
            try
            {
                //verifico si seleccionó algo de la lista, por defecto -1 significa que nada está seleccionado
                if (dtgClientes.SelectedIndex != -1)
                {
                    CargarDesdeGrillaClientes(dtgClientes.SelectedItem.GetType().GetProperty("Rut").GetValue(dtgClientes.SelectedItem, null).ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CargarContrato(string numcontrato) //Método que llena los campos con la información de un objeto seleccionado en la tabla 
        {
            try
            {
                Contrato contrato = MainWindow.listacontratos.GetContrato(numcontrato);

                int rutlenght; //variable que guarda el largo del rut
                rutlenght = contrato.Cliente.Rut.Length - 2; //el valor de la variable es el largo del rut del objeto cliente - 2 (osea rut 12345678-2 =  12345678)

                TxtNumContrato.Text = contrato.NumeroContrato;
                txtRut.Text = contrato.Cliente.Rut.Substring(0, rutlenght);  //se cargan los datos del objeto contrato en los campos.
                txtdig.Text = contrato.Cliente.Rut.Substring(contrato.Cliente.Rut.IndexOf('-') + 1);
                txtCliente.Text = contrato.Cliente.Nombre;
                dtpCreacion.SelectedDate = contrato.F_creacion;
                dtpTermino.SelectedDate = contrato.F_termino;
                dtpInicioEvento.SelectedDate = contrato.F_hora_inicio;
                dtpFinEvento.SelectedDate = contrato.F_hora_fin;
                TxtDireccion.Text = contrato.Direccion;
                CboTipoEvento.SelectedIndex = MainWindow.listaeventos.GetIndex(contrato.IdTipoEvento);
                TxtObservacion.Text = contrato.Observaciones;

                BloquearControles(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CargarDesdeGrillaClientes(string rut) //Método que llena los campos con la información de un objeto seleccionado en la tabla 
        {
            try
            {
                Cliente cliente = MainWindow.listaclientes.GetCliente(rut); //se crea un objeto del tipo cliente en donde se guardan los datos de un ojeto de la colección con el mismo ínidice.
                int rutlenght; //variable que guarda el largo del rut
                rutlenght = cliente.Rut.Length - 2; //el valor de la variable es el largo del rut del objeto cliente - 2 (osea rut 12345678-2 =  12345678)

                txtRut.Text = cliente.Rut.Substring(0, rutlenght);  //se cargan los datos del objeto cliente en los campos.
                txtdig.Text = cliente.Rut.Substring(cliente.Rut.IndexOf('-') + 1);
                txtCliente.Text = cliente.Nombre;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void BloquearControles(Boolean bloqueo) //Bloquea o desbloquea los campos de texto y Cbo
        {
            if (bloqueo)
            {
                txtRut.IsEnabled = false;
                txtdig.IsEnabled = false;
                dtpCreacion.IsEnabled = false;
                dtpTermino.IsEnabled = false;
                dtpInicioEvento.IsEnabled = false;
                dtpFinEvento.IsEnabled = false;
                TxtDireccion.IsEnabled = false;
                CboTipoEvento.IsEnabled = false;
                TxtObservacion.IsEnabled = false;
            }
            else
            {
                txtRut.IsEnabled = true;
                txtdig.IsEnabled = true;
                dtpCreacion.IsEnabled = true;
                dtpTermino.IsEnabled = true;
                dtpInicioEvento.IsEnabled = true;
                dtpFinEvento.IsEnabled = true;
                TxtDireccion.IsEnabled = true;
                CboTipoEvento.IsEnabled = true;
                TxtObservacion.IsEnabled = true;
            }

        }

        public Boolean Vigencia()//Método que retorna el valor de vigencia segun el contrato se encuentre vigente.
        {
            if (DateTime.Now > dtpTermino.SelectedDate.Value)
            {
                return false;
            }
            else
            {
                return true;
            }

        } 

        private void CargarCliente(string rut) //método encargado de llenar los campos con los datos de un objeto de la coleccion en el que coincida el rut.
        {                                      //a este método se le necesita entregar un rut.
            Cliente cliente = MainWindow.listaclientes.GetCliente(rut);
            int rutlenght;
            rutlenght = cliente.Rut.Length - 2;

            txtRut.Text = cliente.Rut.Substring(0, rutlenght);
            txtdig.Text = cliente.Rut.Substring(cliente.Rut.IndexOf('-') + 1);
            txtCliente.Text = cliente.Nombre;
        }

        private void Rut_TextChanged(object sender, TextChangedEventArgs e) //método acción de escribir en txtRut, está constantemente comprobando si el rut existe en la colección Cliente, de ser así llena el campo Nombre con el Cliente Match.
        {
            if (MainWindow.listaclientes.Existe(txtRut.Text + "-" + txtdig.Text))
            {
                txtCliente.Text = MainWindow.listaclientes.GetCliente(txtRut.Text + "-" + txtdig.Text).Nombre;
            }
            else
            {
                txtCliente.Text = string.Empty;
            }
        }

        private void SoloNumeros(object sender, TextCompositionEventArgs e)//metodo que no permite ingresar texto en un TextBox en caso de no ser númerp
        {
            if (IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }
        }

        private bool IsNumber(string Text) //metodo que verifica que el caracter ingresado sea un número, retorna un booleano
        {
            int output;
            return int.TryParse(Text, out output);
        }

        private void txtdig_PreviewTextInput(object sender, TextCompositionEventArgs e) //metodo que no permite ingresar caracteres o numeros diferentes a "K" o números en el txtBox digito verificador
        {
            if ((e.Text.ToUpper() != "K") && IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }

        }

        private void CboTipoEvento_SelectionChanged(object sender, SelectionChangedEventArgs e) //si se escoge un item en el cbo tipo de evento se llenará el campo del valor base.
        {
            if (CboTipoEvento.SelectedIndex != -1)
            {
                txtValorBase.Text = MainWindow.listaeventos.GetEvento(CboTipoEvento.SelectedIndex).ValorBase.ToString() + " UF";
                txtNombreEvento.Text = MainWindow.listaeventos.GetEvento(CboTipoEvento.SelectedIndex).Nombre;
            }
        } 

        private void txtValorBase_TextChanged(object sender, TextChangedEventArgs e) // Si el campo del texto valor base cambia, se calcula el total y se muestra en el txtbox final
        {
                calcularTotal();
        }

        private void txtAsistentes_TextChanged(object sender, TextChangedEventArgs e) // Si el campo del texto asistentes cambia, se calcula el total y se muestra en el txtbox final
        {
            calcularTotal();
        }

        private void txtPersonal_TextChanged(object sender, TextChangedEventArgs e) // Si el campo del texto personal cambia, se calcula el total y se muestra en el txtbox final
        {
            calcularTotal();
        }

        public void calcularTotal() //metodo en el que se calcula el total del costo de eventos , asistentes y personal extra
        {
            int valorBase = 0;
            double valorAsistentes;
            double valorPersonalExtra;

            if (CboTipoEvento.SelectedIndex != -1)
            {
                valorBase = MainWindow.listaeventos.GetEvento(CboTipoEvento.SelectedIndex).ValorBase;
            }

            if (txtAsistentes.Text != string.Empty)
            {
                if (Int32.Parse(txtAsistentes.Text) > 0 && Int32.Parse(txtAsistentes.Text) <= 20)
                {
                    valorAsistentes = 3;
                }
                else
                {
                    if (Int32.Parse(txtAsistentes.Text) > 20 && Int32.Parse(txtAsistentes.Text) <= 50)
                    {
                        valorAsistentes = 5;
                    }
                    else
                    {
                        valorAsistentes = Math.Round(Convert.ToDouble(txtAsistentes.Text) / 20) * 2;
                    }
                }
            }
            else
            {
                valorAsistentes = 0;
            }

            if (txtPersonal.Text != string.Empty)
            {
                if (Int32.Parse(txtPersonal.Text) == 2)
                {
                    valorPersonalExtra = 2;
                }
                else
                {
                    if (Int32.Parse(txtPersonal.Text) == 3)
                    {
                        valorPersonalExtra = 3;
                    }
                    else
                    {
                        if (Int32.Parse(txtPersonal.Text) == 4)
                        {
                            valorPersonalExtra = 3.5;
                        }
                        else
                        {
                            if (Int32.Parse(txtPersonal.Text) > 4)
                            {
                                valorPersonalExtra = (Int32.Parse(txtPersonal.Text) * 0.5) + 3.5;
                            }
                            else
                            {
                                valorPersonalExtra = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                valorPersonalExtra = 0;
            }
            
            txtTotal.Text = (valorBase + valorAsistentes + valorPersonalExtra).ToString() + " UF";
        }

        private void cboFilActividad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFilActividad.SelectedIndex != -1)
            {
                filtrarListaClientes();
            }
        }

        private void txtFilRut_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrarListaClientes();
        }

        private void cboFilTipoEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtrarListaClientes();
        }

        public void filtrarListaClientes()
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
            filtrarListaClientes();

            txtFilContrato.Text = string.Empty;
            txtFilRutCliente.Text = string.Empty;
            cboFilTipoEvento.SelectedIndex = -1;
            filtrarListaContratos();
        }

        private void BtmSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } //acción del boton salir

        private void txtFilContrato_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrarListaContratos();
        }

        private void txtFilRutCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrarListaContratos();
        }

        private void cboFilTipoEvento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //string x = cboFilTipoEvento.SelectedValue.ToString();
            //this.ShowMessageAsync(x,"");

            filtrarListaContratos();
        }

        public void filtrarListaContratos()
        {
            if (txtFilContrato.Text != string.Empty && txtFilRutCliente.Text != string.Empty && cboFilTipoEvento.SelectedIndex != -1)
            {
                dtgContratos.ItemsSource = from x in MainWindow.listacontratos 
                                           where x.NumeroContrato.StartsWith(txtFilContrato.Text) && 
                                                 x.Cliente.Rut.StartsWith(txtFilRutCliente.Text) && 
                                                 x.IdTipoEvento == cboFilTipoEvento.SelectedValue.ToString()
                                           select new
                                           {
                                               x.NumeroContrato,
                                               RutCliente = x.Cliente.Rut,
                                               NombreCliente = x.Cliente.Nombre,
                                               FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                               FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                               FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                               FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                               Vigencia = x.Vigente,
                                               IDEvento = x.IdTipoEvento,
                                               TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                               x.Observaciones
                                           };
            }
            else
            {
                if (txtFilContrato.Text != string.Empty && txtFilRutCliente.Text != string.Empty)
                {
                    dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                               where x.NumeroContrato.StartsWith(txtFilContrato.Text) &&
                                                     x.Cliente.Rut.StartsWith(txtFilRutCliente.Text)                                                  
                                               select new
                                               {
                                                   x.NumeroContrato,
                                                   RutCliente = x.Cliente.Rut,
                                                   NombreCliente = x.Cliente.Nombre,
                                                   FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                   FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                   FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                   FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                   Vigencia = x.Vigente,
                                                   IDEvento = x.IdTipoEvento,
                                                   TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                   x.Observaciones
                                               };
                }
                else
                {
                    if (txtFilContrato.Text != string.Empty && cboFilTipoEvento.SelectedIndex != -1)
                    {
                        dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                                   where x.NumeroContrato.StartsWith(txtFilContrato.Text) &&                                                         
                                                         x.IdTipoEvento == cboFilTipoEvento.SelectedValue.ToString()
                                                   select new
                                                   {
                                                       x.NumeroContrato,
                                                       RutCliente = x.Cliente.Rut,
                                                       NombreCliente = x.Cliente.Nombre,
                                                       FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                       FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                       FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                       FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                       Vigencia = x.Vigente,
                                                       IDEvento = x.IdTipoEvento,
                                                       TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                       x.Observaciones
                                                   };
                    }
                    else
                    {
                        if (txtFilRutCliente.Text != string.Empty && cboFilTipoEvento.SelectedIndex != -1)
                        {
                            dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                                       where x.Cliente.Rut.StartsWith(txtFilRutCliente.Text) &&
                                                             x.IdTipoEvento == cboFilTipoEvento.SelectedValue.ToString()
                                                       select new
                                                       {
                                                           x.NumeroContrato,
                                                           RutCliente = x.Cliente.Rut,
                                                           NombreCliente = x.Cliente.Nombre,
                                                           FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                           FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                           FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                           FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                           Vigencia = x.Vigente,
                                                           IDEvento = x.IdTipoEvento,
                                                           TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                           x.Observaciones
                                                       };
                        }
                        else
                        {
                            if (txtFilContrato.IsFocused)
                            {
                                dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                                           where x.NumeroContrato.StartsWith(txtFilContrato.Text)
                                                           select new
                                                           {
                                                               x.NumeroContrato,
                                                               RutCliente = x.Cliente.Rut,
                                                               NombreCliente = x.Cliente.Nombre,
                                                               FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                               FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                               FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                               FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                               Vigencia = x.Vigente,
                                                               IDEvento = x.IdTipoEvento,
                                                               TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                               x.Observaciones
                                                           };
                            }
                            else
                            {
                                if (txtFilRutCliente.IsFocused)
                                {
                                    dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                                               where x.Cliente.Rut.StartsWith(txtFilRutCliente.Text)                                                                    
                                                               select new
                                                               {
                                                                   x.NumeroContrato,
                                                                   RutCliente = x.Cliente.Rut,
                                                                   NombreCliente = x.Cliente.Nombre,
                                                                   FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                                   FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                                   FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                                   FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                                   Vigencia = x.Vigente,
                                                                   IDEvento = x.IdTipoEvento,
                                                                   TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                                   x.Observaciones
                                                               };
                                }
                                else
                                {
                                    if (cboFilTipoEvento.SelectedIndex != -1)
                                    {
                                        dtgContratos.ItemsSource = from x in MainWindow.listacontratos
                                                                   where x.IdTipoEvento == cboFilTipoEvento.SelectedValue.ToString()
                                                                   select new
                                                                   {
                                                                       x.NumeroContrato,
                                                                       RutCliente = x.Cliente.Rut,
                                                                       NombreCliente = x.Cliente.Nombre,
                                                                       FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                                                       FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                                                       FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                                                       FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                                                       Vigencia = x.Vigente,
                                                                       IDEvento = x.IdTipoEvento,
                                                                       TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                                       x.Observaciones
                                                                   };
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
        } //metodo que se encarga de filtrar los contratos segun numero de contrato, rut y tipo de evento

    }
}