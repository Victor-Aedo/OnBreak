using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biblioteca_OnBreak
{
    public class ContratoCollection : List<Contrato>
    {
        public ContratoCollection()
        {

        }
        public bool Existe(string num_contrato)
        {
            return this.Exists(a => a.NumeroContrato == num_contrato);
        }

        public Contrato GetContrato(string Id)
        {
            try
            {
                return this.First(b => b.NumeroContrato == Id);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ClienteExiste(string rut)
        {
            try
            {
                return this.Exists(c => c.Cliente.Rut == rut);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
