using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2
{
    class Metodo
    {

        string nombre;
        List<Parametro> listaparametros;
        Boolean over = false;
        ParseTreeNode nodo;
        string tipo;
        string visi;

        public Metodo(String nom, ParseTreeNode no,string tip,string visi)
        {
            this.nombre = nom;
            this.nodo = no;
            this.tipo = tip;
            this.visi = visi;
        }
       
        public Metodo(String nom, List<Parametro> param,ParseTreeNode no, string tip, string visi) {
            this.nombre = nom;
            this.listaparametros = param;
            this.nodo = no;
            this.tipo = tip;
            this.visi = visi;
        }

        public Metodo(String nom, List<Parametro> param, Boolean ove, ParseTreeNode no, string tip, string visi)
        {
            this.nombre = nom;
            this.nodo = no;
            this.listaparametros = param;
            this.over = ove;
            this.visi = visi;
            this.tipo = tip;
        }

        public Metodo(String nom, Boolean ove,ParseTreeNode no,string tip, string visi)
        {
            this.nombre = nom;
            this.nodo = no;           
            this.over = ove;
            this.tipo = tip;
            this.visi = visi;
        }

        public String getNombre() {
            return nombre;
        }

        public void setNombre(String nom)
        {
            this.nombre = nom;
        }

        public List<Parametro> getParams() {
            return listaparametros;
        }

        public void setParams(List<Parametro> para) {
            this.listaparametros = para;
        }

        public Boolean getover() {
            return over;
        }

        public void setover(Boolean ov) {
            this.over = ov;
        }

        public ParseTreeNode getnodo() {
           return nodo;
        }

        public void setNodo(ParseTreeNode nodo)  {
            this.nodo = nodo;
        }

        public string getTipo() {
            return this.tipo;
        }

        public void setTipo(string tip) {
            this.tipo = tip;
        }

        public string getvisi()
        {
            return this.visi;
        }

        public void setvisi(string vis)
        {
            this.visi = vis;
        }

    }
}
