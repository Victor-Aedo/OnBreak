using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Biblioteca_OnBreak
{
    public class ClienteCollection:List<Cliente>
    {
        public ClienteCollection()
        {

        }
        public bool Existe(string rut)
        {
            return this.Exists(a => a.Rut == rut);
        }

        public Cliente GetCliente (string rut)
        {
            try
            {
                return this.First(b => b.Rut == rut);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
