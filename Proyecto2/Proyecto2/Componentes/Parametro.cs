using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{
    class Parametro
    {

        String nombre;
        String tipo;
        Object valor;

        public Parametro(String nom, String tip, Object val)
        {
            this.nombre = nom;
            this.tipo = tip;
            this.valor = val;
        }

        public String getnombre() {
            return nombre;
        }

        public void setNombre(String nom)
        {
            this.nombre = nom;
        }

        public String getTipo()
        {
            return tipo;
        }

        public void settipo(String tip)
        {
            this.tipo = tip;
        }

        public Object getval()
        {
            return valor;
        }

        public void setvalor(Object nom)
        {
            this.valor = nom;
        }



    }
}
