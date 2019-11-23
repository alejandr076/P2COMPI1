using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{
    class Clase
    {

        String nombre;
        List<Variable> listavar;
        List<Metodo> listamet;

        public Clase(String nom)
        {
            this.nombre = nom;
        }

        public Clase(String nom, List<Variable> vars)
        {
            this.nombre = nom;
            this.listavar = vars;
        }

        public Clase(String nom, List<Variable> lv, List<Metodo> lm)
        {
            this.nombre = nom;
            this.listamet = lm;
            this.listavar = lv;
        }

        public String getNombre()
        {
            return nombre;
        }

        public List<Variable> GetVariables()
        {
            return listavar;
        }


        public List<Metodo> GetMetodos()
        {
            return listamet;
        }

        public void addVariable(Variable var)
        {
            listavar.Add(var);
        }

        public void addMetodo(Metodo metodo)
        {
            listamet.Add(metodo);
        }

    }
}

