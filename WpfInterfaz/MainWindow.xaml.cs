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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Behaviours;
using Biblioteca_OnBreak;
using MahApps.Metro;

namespace WpfInterfaz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool tema = false;

        public static ClienteCollection listaclientes = new ClienteCollection();
        public static TipoEventoCollection listaeventos = new TipoEventoCollection();
        public static ContratoCollection listacontratos = new ContratoCollection();

        public MainWindow()
        {
            InitializeComponent();
            MostrarDatos();
            LlenarTipoEvento();
            InicializarCampos();
        }
        public void LlenarTipoEvento()
        {
            TipoEvento tipoEvento;
            tipoEvento = new TipoEvento { Id = "001", Nombre = "Día del Empleado", PersonalBase = 50, ValorBase = 20 };
            listaeventos.Add(tipoEvento);
            tipoEvento = new TipoEvento { Id = "002", Nombre = "Fiesta Familiar", PersonalBase = 50, ValorBase = 18 };
            listaeventos.Add(tipoEvento);
            tipoEvento = new TipoEvento { Id = "003", Nombre = "Aniversario Empresa", PersonalBase = 60, ValorBase = 30 };
            listaeventos.Add(tipoEvento);
        }

        public void InicializarCampos()
        {

            txtFilRut.Text = string.Empty;
            txtFilContrato.Text = string.Empty;
            txtFilRutCliente.Text = string.Empty;
            cboFilActividad.ItemsSource = Enum.GetValues(typeof(ActividadEmpresa));
            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.ItemsSource = Enum.GetValues(typeof(TipoEmpresa));
            cboFilTipoEmpresa.SelectedIndex = -1;

           
            cboFilTipoEvento.DisplayMemberPath = "Nombre";
            cboFilTipoEvento.SelectedValuePath = "Id";
            cboFilTipoEvento.ItemsSource = listaeventos;
            cboFilTipoEvento.SelectedIndex = -1;
        }

        public void LimpiarCampos()
        {
            txtFilRut.Text = string.Empty;
            txtFilContrato.Text = string.Empty;
            txtFilRutCliente.Text = string.Empty;   

            cboFilActividad.SelectedIndex = -1;
            cboFilTipoEmpresa.SelectedIndex = -1;
            cboFilTipoEvento.SelectedIndex = -1;
        }
        private void OnBreak_Click(object sender, RoutedEventArgs e)
        {
            Flyonebreak.IsOpen = true;
        }

        private void BtmAdmCon_Click(object sender, RoutedEventArgs e)
        {
            AdminContratos window = new AdminContratos();
            window.ShowDialog();
        }

        private void BtmAdmCli_Click(object sender, RoutedEventArgs e)
        {
            AdminCli adminwindow = new AdminCli();
            adminwindow.ShowDialog();
        }

        private void BtmListaCli_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            MostrarDatos(); //se refrescan los datos de la tabla
            FlyLista.IsOpen = true;   //se despliega un menu en donde se encuentra la tabla (función del Mah apps MetroWindows) 
        }

        private void MostrarDatos() //Metodo que se encargará de cargar el Data Grid con los datos de la colección "clientes" y luego refrescarlos.
        {
            dtgClientes.ItemsSource = listaclientes;
            dtgClientes.Items.Refresh();
            dtgClientes.SelectedIndex = -1; //Se especifica que por defecto ningún item se encuentre seleccionado en la tabla 

            dtgContratos.ItemsSource = (from x in MainWindow.listacontratos
                                        select new
                                        {
                                            NumeroContrato = x.NumeroContrato,
                                            RutCliente = x.Cliente.Rut,
                                            NombreCliente = x.Cliente.Nombre,
                                            FechaCreacion_Contrato = x.F_creacion.ToString("dd/MM/yyyy"),
                                            FechaTermino_Contrato = x.F_termino.ToString("dd/MM/yyyy"),
                                            FechaInicio_Evento = x.F_hora_inicio.ToString("dd/MM/yyyy HH:mm"),
                                            FechaFin_Evento = x.F_hora_fin.ToString("dd/MM/yyyy HH:mm"),
                                            Vigencia = x.Vigente,
                                            IDEvento = x.IdTipoEvento,
                                            TipoEvento = MainWindow.listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                            Observaciones = x.Observaciones
                                        }).ToList(); // linq

            dtgContratos.Items.Refresh();
            dtgClientes.SelectedIndex = -1;
            
        }

        private void BtmTema_Click(object sender, RoutedEventArgs e) //método que cambia el Tema de la Interfaz
        {
            if (tema == false)
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Yellow"),
                                     ThemeManager.GetAppTheme("BaseDark"));
                tema = true;
            }
            else
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Violet"),
                                     ThemeManager.GetAppTheme("BaseLight"));
                tema = false;
            }
        }

        private void BtmListaCon_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            MostrarDatos(); //se refrescan los datos de la tabla
            FlyLista_Contratos.IsOpen = true;   //se despliega un menu en donde se encuentra la tabla (función del Mah apps MetroWindows)
        }

        private void cboFilActividad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFilActividad.SelectedIndex != -1)
            {
                FiltrarListaClientes();
            }
        }

        private void txtFilRut_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarListaClientes();
        }

        private void cboFilTipoEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltrarListaClientes();
        }

        public void FiltrarListaClientes()
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

        private void BtmLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            FiltrarListaClientes();
            FiltrarListaContratos();
        }
        private void txtFilContrato_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarListaContratos();
        }

        private void txtFilRutCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarListaContratos();
        }

        private void cboFilTipoEvento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //string x = cboFilTipoEvento.SelectedValue.ToString();
            //this.ShowMessageAsync(x,"");

            FiltrarListaContratos();
        }

        public void FiltrarListaContratos()
        {
            if (txtFilContrato.Text != string.Empty && txtFilRutCliente.Text != string.Empty && cboFilTipoEvento.SelectedIndex != -1)
            {
                dtgContratos.ItemsSource = from x in listacontratos
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
                    dtgContratos.ItemsSource = from x in listacontratos
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
                            dtgContratos.ItemsSource = from x in listacontratos
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
                                                           TipoEvento = listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                           x.Observaciones
                                                       };
                        }
                        else
                        {
                            if (txtFilContrato.IsFocused)
                            {
                                dtgContratos.ItemsSource = from x in listacontratos
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
                                                               TipoEvento = listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                               x.Observaciones
                                                           };
                            }
                            else
                            {
                                if (txtFilRutCliente.IsFocused)
                                {
                                    dtgContratos.ItemsSource = from x in listacontratos
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
                                                                   TipoEvento = listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
                                                                   x.Observaciones
                                                               };
                                }
                                else
                                {
                                    if (cboFilTipoEvento.SelectedIndex != -1)
                                    {
                                        dtgContratos.ItemsSource = from x in listacontratos
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
                                                                       TipoEvento = listaeventos.GetTipoEvento(x.IdTipoEvento).Nombre,
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
        } 
    }
}
