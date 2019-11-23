using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{
    class Error
    {

         String level;
         String mensaje;
         String tipo;
         int linea;
         int columna;

        public Error(String le,String msj, String t, int lin, int col)
        {
            this.level = le;
            this.mensaje = msj;
            this.tipo = t;
            this.linea = lin;
            this.columna = col;
        }


        public String getLevel()
        {
            return level;
        }

        public String getMensaje()
        {
            return mensaje;
        }

        public String getTipo()
        {
            return tipo;
        }

        public int getLinea()
        {
            return linea;
        }

        public int getColumna()
        {
            return columna;
        }
    }
}
