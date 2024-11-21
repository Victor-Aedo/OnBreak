using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Biblioteca_OnBreak
{
    public class Contrato
    {
       
        //Propiedades
        public String NumeroContrato { get; set; }
        public DateTime F_creacion { get; set; }
        public DateTime F_termino { get; set; }
        public DateTime F_hora_inicio { get; set; }
        public DateTime F_hora_fin { get; set; }
        public String Direccion { get; set; }
        public Boolean Vigente { get; set; }
        public String IdTipoEvento { get; set; }
        public String Observaciones { get; set; }
        public Cliente Cliente { get; set; }
      
        public Contrato()
        {
            Init();
        }

        private void Init()
        {
            NumeroContrato = string.Empty;
            F_creacion = DateTime.Now;
            F_termino = DateTime.Now;
            F_hora_inicio = DateTime.Now;
            F_hora_fin = DateTime.Now;
            Direccion = string.Empty;
            Vigente = false;
            IdTipoEvento = string.Empty;
            Observaciones = string.Empty;
            Cliente = new Cliente();

        }
    }
}