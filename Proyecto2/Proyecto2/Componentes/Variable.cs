using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2
{

    class Variable
    {
        String nombre;
        String tipo;
        String visibilidad;

        String valorString;
        int valorint;
        double valordouble;
        char valorchar;
        Boolean valorbool;

        public Variable(String vis, String nom, String ti, String vs, int vi, double vd, char vc, Boolean vb) {
            this.visibilidad = vis;
            this.nombre = nom;
            this.tipo = ti;
            this.valorString = vs;
            this.valorint = vi;
            this.valordouble = vd;
            this.valorchar = vc;
            this.valorbool = vb;
        }

        int dimensiones;
        int dim1;
        int dim2;
        int dim3;

        public void setdim1(int d1)
        {
            this.dim1 = d1;
        }
        public int getdim1()
        {
            return this.dim1;
        }
        //2
        public void setdim2(int d2)
        {
            this.dim2 = d2;
        }
        public int getdim2()
        {
            return this.dim2;
        }
        //3
        public void setdim3(int d3)
        {
            this.dim3 = d3;
        }
        public int getdim3()
        {
            return this.dim3;
        }

        public Variable(string vis,string nom,string tip, int dim,int d1)
        {
            this.visibilidad = vis;
            this.nombre = nom;
            this.tipo = tip;
            this.dimensiones = dim;
            this.dim1 = d1;
        }
        public Variable(string vis, string nom, string tip, int dim, int d1,int d2)
        {
            this.visibilidad = vis;
            this.nombre = nom;
            this.tipo = tip;
            this.dimensiones = dim;
            this.dim1 = d1;
            this.dim2 = d2;
        }
        public Variable(string vis, string nom, string tip, int dim, int d1, int d2, int d3)
        {
            this.visibilidad = vis;
            this.nombre = nom;
            this.tipo = tip;
            this.dimensiones = dim;
            this.dim1 = d1;
            this.dim2 = d2;
            this.dim3 = d3;
        }

        public int getDimen()
        {
            return this.dimensiones;
        }

        public void setDimen(int num)
        {
            this.dimensiones = num;
        }

        public string getnombre()
        {
            return nombre;
        }
        public string gettipo()
        {
            return tipo;
        }

        public string getvs()
        {
            return valorString;
        }

        public int getvi()
        {
            return valorint;
        }

        public double getvd()
        {
            return valordouble;
        }

        public Boolean getvb()
        {
            return valorbool;
        }

        public char getvc()
        {
            return valorchar;
        }

        public String getVisi()
        {
            return visibilidad;
        }

        

        public void setnombre(String nom)
        {
            this.nombre = nom;
        }
        public void settipo(String tip)
        {
            this.tipo = tip;
        }

        public void setvs(String vs)
        {
            this.valorString = vs;
        }

        public void setvi(int vi)
        {
            this.valorint = vi;
        }

        public void setvd(double vd)
        {
            this.valordouble = vd;
        }

        public void setvb(Boolean vb)
        {
            this.valorbool = vb;
        }

        public void setvc(Char vc)
        {
            this.valorchar = vc;
        }

    }
}
