using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Biblioteca_OnBreak
{
    public class TipoEventoCollection : List<TipoEvento>
    {
        public TipoEventoCollection()
        {

        }

        public TipoEvento GetTipoEvento(string Id)
        {
            try
            {
                return this.First(x => x.Id == Id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetIdEvento(int index)
        {
            try
            {
                return this[index].Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public TipoEvento GetEvento(int index)
        {
            try
            {
                return this[index];
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int GetIndex(string Id)
        {
            try
            {
                return this.FindIndex(x => x.Id == Id);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
