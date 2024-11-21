using System;
using System.Collections.Generic;
using System.Text;

namespace Biblioteca_OnBreak
{
    public class Cliente
    {

        public String Rut { get; set; }
        public String Razon_Social { get; set; }
        public String Nombre { get; set; }
        public String Correo { get; set; }
        public String Direccion { get; set; }
        public String Fono { get; set; }
        public ActividadEmpresa Actividad_Empresa { get; set; }
        public TipoEmpresa Tipo_Empresa { get; set; }


        public Cliente()
        {
            Init();
        }

        private void Init()
        {
            Rut = string.Empty;
            Razon_Social = string.Empty;
            Nombre = string.Empty;
            Correo = string.Empty;
            Direccion = string.Empty;
            Fono = string.Empty;
            Actividad_Empresa = ActividadEmpresa.Agropecuaria;
            Tipo_Empresa = TipoEmpresa.EIRL;
        }
    }
}
