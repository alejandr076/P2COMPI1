using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{
    class Simbolo
    {

        String ambito;
        String nombre;
        String tipo;
        String visibilidad;
        Object valor;

        public Simbolo(String visi, String am, String nom, String tip, Object val)
        {
            this.visibilidad = visi;
            this.ambito = am;
            this.nombre = nom;
            this.tipo = tip;
            this.valor = val;
        }

        
        //GETTERS
        public String GetAmbito() {
            return ambito;
        }
        public String GetNombre() {
            return nombre;
        }
        public String GetTipo()
        {
            return tipo;
        }
        public Object GetValor()
        {
            return valor;
        }

        public String Getvis() {
            return visibilidad;
        }

        //SETTERS
        public void Setvis(String vis)
        {
            this.visibilidad = vis;
        }
        public void SetAmbito(String am)
        {
            this.ambito = am;
        }
        public void SetNombre(String nom)
        {
            this.nombre=nom;
        }
        public void SetTipo(String tip)
        {
            this.tipo = tip;
        }
        public void SetValor(Object val)
        {
            this.valor = val;
        }



    }
}
