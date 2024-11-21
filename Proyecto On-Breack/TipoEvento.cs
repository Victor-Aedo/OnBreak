using System;
using System.Collections.Generic;
using System.Text;

namespace Biblioteca_OnBreak
{
    public class TipoEvento
    {
        public String Id { get; set; }
        public String Nombre { get; set; }
        public int ValorBase { get; set; }
        public int PersonalBase { get; set; }

        public TipoEvento()
        {
            Init();
        }

        private void Init()
        {
            Id = "0";
            Nombre = string.Empty;
            ValorBase = 0;
            PersonalBase = 0;
        }
    }
}