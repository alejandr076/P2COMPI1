using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using System.Windows.Forms;

namespace Proyecto2
{
    class Acciones
    {

        public static List<Simbolo> extra = new List<Simbolo>();

        public static List<Simbolo> listasimbolos = new List<Simbolo>();//LISTA DE SIMBOLOS
        public static List<Error> listaerrores = new List<Error>();//LISTA DE ERRORES
        public static List<Variable> listavar;//LISTA DE VARIABLES
        public static List<Metodo> listametodos;//LISTA DE METODOS
        public static List<Clase> listaclases = new List<Clase>();//LISTA DE CLASES

        public static String resultado = "";// STRING A MOSTRAR EN LA SALIDA

        public static Clase claseactual;//CLASE QUE SE ANALIZA
        public static String nombreclaseactual = "";//NOMBRE DE LA CLASE QUE SE ANALIZA
        public static string ambito = "global";//AMBITO QUE SE MANEJA

        static int valor = 0;
        static double valord = 0.0;
        static string operador = "";
        static int valorf = 0;
        static string cambio = "";
        public static int nummain = 0;
        public static string ambito2 = "";
        static bool secumple = false;
        public static string ambitoaux = "";
        static string id = "";

        static int metodosmainxclase = 0;

        public static void compilar(ParseTreeNode root)//PRIMERA PASADA: CLASES Y SUS METODOS
        {
            switch (root.Term.Name.ToString())
            {
                case "INICIO":
                    {
                        compilar(root.ChildNodes[0]);
                        break;
                    }
                case "LISTACLASES":
                    {
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            compilar(root.ChildNodes[i]);
                        }
                        break;
                    }
                case "CLASE":
                    {
                        nombreclaseactual = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        agregarSimbolo("publico", ambito, nombreclaseactual, "clase", "----");
                        ambito = nombreclaseactual;
                        listavar = new List<Variable>();
                        listametodos = new List<Metodo>();
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            compilar(root.ChildNodes[i]);
                        }
                        claseactual = new Clase(nombreclaseactual, listavar, listametodos);
                        listaclases.Add(claseactual);
                        metodosmainxclase = 0;
                        ambito = "global";
                        break;
                    }
                case "LISTAINSTRUCCIONES":
                    {
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            compilar(root.ChildNodes[i]);
                        }
                        break;
                    }
                case "DECLARACION":
                    {
                        Declarar(root, ambito);
                        break;
                    }
                case "DARREGLO":
                    {
                        DeclararArreglo(root, ambito);
                        break;
                    }
                case "ASIGNACION":
                    {
                        String iden = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        Object valor = Operacion(root.ChildNodes[1], ambito);
                        Type vty = valor.GetType();
                        foreach (Simbolo sym in listasimbolos)
                        {
                            if (sym.GetNombre() == iden && sym.GetAmbito() == ambito)
                            {
                                sym.SetValor(valor);
                                //Console.WriteLine(valor);
                            }
                        }
                        foreach (Clase clase in listaclases)
                        {
                            if (clase.getNombre() == ambito2)
                            {
                                foreach (Variable var in clase.GetVariables())
                                {
                                    if (var.getnombre() == iden)
                                    {
                                        string tip = var.gettipo();
                                        if (tip.Equals("int") && vty.Equals(typeof(int)))
                                        {
                                            int nuevo = int.Parse(valor.ToString());
                                            var.setvi(nuevo);
                                        }
                                        else if (tip.Equals("string") && vty.Equals(typeof(string)))
                                        {
                                            var.setvs(valor.ToString());
                                        }
                                        else if (tip.Equals("char") && vty.Equals(typeof(char)))
                                        {
                                            char val = char.Parse(valor.ToString());
                                            var.setvc(val);
                                        }
                                        else if (tip.Equals("double") && vty.Equals(typeof(double)))
                                        {
                                            double num = double.Parse(valor.ToString());
                                            var.setvd(num);
                                        }
                                        else if (tip.Equals("bool") && vty.Equals(typeof(Boolean)))
                                        {
                                            Boolean vari = Boolean.Parse(valor.ToString());
                                            var.setvb(vari);
                                        }
                                        else
                                        {
                                            String lexema = "Asignacion";
                                            String mensaje = "No se puede asignar un valor si el tipo no coincide o no se ha declarado previamente";
                                            String tipoerror = "Semantico";
                                            int lin = 0;
                                            int col = 0;
                                            Error nuevo = new Error(lexema, mensaje, tipoerror, lin, col);
                                            listaerrores.Add(nuevo);
                                        }
                                    }
                                }
                            }

                        }

                        break;
                    }
                case "LISTAMETODOS":
                    {
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            compilar(root.ChildNodes[i]);
                        }
                        break;
                    }
                case "METODOVOID":
                    {
                        Metodo nuevo;
                        String visi = "publico";
                        String nombre;
                        String tipo = "void";
                        String valor = "-----";
                        int numhijos = root.ChildNodes.Count;

                        switch (numhijos)
                        {
                            case 2:
                                if (root.ChildNodes[0].ToString().Contains("(Keyword)"))
                                {
                                    metodosmainxclase++;
                                    if (metodosmainxclase == 1)
                                    {
                                        nuevo = new Metodo("main", root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, "main", tipo, valor);

                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR SEMANTICO DE SOLO UN MAIN POR CLASE");
                                    }

                                }
                                else
                                {
                                    nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    nuevo = new Metodo(nombre, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                }
                                break;
                            case 3:
                                if (root.ChildNodes[1].ToString().Equals("LISTAPARAM"))
                                {
                                    nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    List<Parametro> parames = ObtenerListaParametros(root.ChildNodes[1], nombre);
                                    nuevo = new Metodo(nombre, parames, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);

                                }
                                else if (root.ChildNodes[1].ToString().Contains("(id)"))
                                {
                                    visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                    nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    nuevo = new Metodo(nombre, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                }
                                else if (root.ChildNodes[1].ToString().Contains("override"))
                                {
                                    nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    nuevo = new Metodo(nombre, true, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                }
                                break;
                            case 4:
                                if (root.ChildNodes[0].ToString().Equals("VISIBILIDAD"))
                                {
                                    visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                    nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    if (root.ChildNodes[2].ToString().Contains("override"))
                                    {
                                        nuevo = new Metodo(nombre, true, root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    }
                                    else if (root.ChildNodes[2].ToString().Equals("LISTAPARAM"))
                                    {
                                        List<Parametro> listaparams = ObtenerListaParametros(root.ChildNodes[2], nombre);
                                        nuevo = new Metodo(nombre, listaparams, root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    }
                                }
                                else
                                {
                                    nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    List<Parametro> lpa = ObtenerListaParametros(root.ChildNodes[2], nombre);
                                    nuevo = new Metodo(nombre, lpa, true, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                }
                                break;
                            case 5:
                                visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[3], nombre);
                                nuevo = new Metodo(nombre, lp, true, root, tipo, visi);
                                listametodos.Add(nuevo);
                                agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                break;
                        }
                        break;
                    }
                case "METODORETURN":
                    {
                        Metodo nuevo;
                        String visi = "publico";
                        String nombre;
                        String tipo = "metodo/";
                        String valor = "-----";
                        int numhijos = root.ChildNodes.Count;
                        switch (numhijos)
                        {
                            case 1:
                                {
                                    switch (root.ChildNodes[0].ChildNodes.Count)
                                    {
                                        case 4://ID TIPO DIM LI
                                            {
                                                nombre = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                                tipo += root.ChildNodes[0].ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                                int numdim = root.ChildNodes[0].ChildNodes[2].ChildNodes.Count;
                                                tipo += "/" + numdim;
                                                nuevo = new Metodo(nombre, root, tipo, visi);
                                                listametodos.Add(nuevo);
                                                if (numdim == 1)
                                                {
                                                    int primera = 0;

                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    if (t.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());

                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]", "----");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }
                                                }
                                                else if (numdim == 2)
                                                {

                                                    int primera = 0;
                                                    int segunda = 0;
                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    object dim2 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[1], ambito);
                                                    Type t2 = dim2.GetType();
                                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());
                                                        segunda = int.Parse(dim2.ToString());

                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            for (int j = 0; j < segunda; j++)
                                                            {
                                                                agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]", "----");
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }

                                                }
                                                else if (numdim == 3)
                                                {
                                                    int primera = 0;
                                                    int segunda = 0;
                                                    int tercera = 0;
                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    object dim2 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[1], ambito);
                                                    Type t2 = dim2.GetType();
                                                    object dim3 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[2], ambito);
                                                    Type t3 = dim3.GetType();
                                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());
                                                        segunda = int.Parse(dim2.ToString());
                                                        tercera = int.Parse(dim3.ToString());
                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            for (int j = 0; j < segunda; j++)
                                                            {
                                                                for (int k = 0; k < tercera; k++)
                                                                {
                                                                    agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                                }
                                                            }
                                                        }


                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }
                                                }
                                                break;
                                            }
                                        case 5://DOS OPCIONES
                                            {
                                                //VISIBILIDAD ID TIPO DIM LI
                                                if (root.ChildNodes[0].ChildNodes[0].ToString().Equals("VISIBILIDAD"))
                                                {
                                                    visi = root.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)","");
                                                    nombre = root.ChildNodes[0].ChildNodes[1].ToString().Replace(" (id)","");
                                                    tipo += root.ChildNodes[0].ChildNodes[2].ToString().Replace(" (Keyword)","");
                                                    int numdim = root.ChildNodes[0].ChildNodes[3].ChildNodes.Count;
                                                    tipo += "/"+numdim;
                                                    nuevo = new Metodo(nombre,root,tipo,visi);
                                                    listametodos.Add(nuevo);
                                                    if (numdim == 1)
                                                    {
                                                        int primera = 0;

                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        if (t.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());

                                                            for (int i = 0; i < primera; i++)
                                                            {
                                                                agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]", "----");
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }
                                                    }
                                                    else if (numdim == 2)
                                                    {
                                               
                                                        int primera = 0;
                                                        int segunda = 0;
                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        object dim2 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[1], ambito);
                                                        Type t2 = dim2.GetType();
                                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());
                                                            segunda = int.Parse(dim2.ToString());

                                                                for (int i = 0; i < primera; i++)
                                                                {
                                                                    for (int j = 0; j < segunda; j++)
                                                                    {
                                                                        agregarSimbolo(visi, ambito, nombre, nombre +"[" + i + "]" + "[" + j + "]", "----");
                                                                    }
                                                                }
                                                            
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }

                                                    }
                                                    else if (numdim == 3)
                                                    {
                                                        int primera = 0;
                                                        int segunda = 0;
                                                        int tercera = 0;
                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        object dim2 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[1], ambito);
                                                        Type t2 = dim2.GetType();
                                                        object dim3 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[2], ambito);
                                                        Type t3 = dim3.GetType();
                                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());
                                                            segunda = int.Parse(dim2.ToString());
                                                            tercera = int.Parse(dim3.ToString());
                                                                for (int i = 0; i < primera; i++)
                                                                {
                                                                    for (int j = 0; j < segunda; j++)
                                                                    {
                                                                        for (int k = 0; k < tercera; k++)
                                                                        {
                                                                            agregarSimbolo(visi, ambito, nombre, nombre+ "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                                        }
                                                                    }
                                                                }
                                                            

                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }
                                                    }

                                                }
                                                else//ID0 TIPO1 DIM2 LP3 LI4
                                                {
                                                    nombre = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                                    tipo += root.ChildNodes[0].ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                                    int numdim = root.ChildNodes[0].ChildNodes[2].ChildNodes.Count;
                                                    tipo += "/" + numdim;
                                                    List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[0].ChildNodes[3],nombre);
                                                    nuevo = new Metodo(nombre,lp, root, tipo, visi);
                                                    listametodos.Add(nuevo);
                                                    if (numdim == 1)
                                                    {
                                                        int primera = 0;

                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        if (t.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());

                                                            for (int i = 0; i < primera; i++)
                                                            {
                                                                agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]", "----");
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }
                                                    }
                                                    else if (numdim == 2)
                                                    {

                                                        int primera = 0;
                                                        int segunda = 0;
                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        object dim2 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[1], ambito);
                                                        Type t2 = dim2.GetType();
                                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());
                                                            segunda = int.Parse(dim2.ToString());

                                                            for (int i = 0; i < primera; i++)
                                                            {
                                                                for (int j = 0; j < segunda; j++)
                                                                {
                                                                    agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]", "----");
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }

                                                    }
                                                    else if (numdim == 3)
                                                    {
                                                        int primera = 0;
                                                        int segunda = 0;
                                                        int tercera = 0;
                                                        object dim1 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[0], ambito);
                                                        Type t = dim1.GetType();
                                                        object dim2 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[1], ambito);
                                                        Type t2 = dim2.GetType();
                                                        object dim3 = Operacion(root.ChildNodes[0].ChildNodes[2].ChildNodes[2], ambito);
                                                        Type t3 = dim3.GetType();
                                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                                        {
                                                            primera = int.Parse(dim1.ToString());
                                                            segunda = int.Parse(dim2.ToString());
                                                            tercera = int.Parse(dim3.ToString());
                                                            for (int i = 0; i < primera; i++)
                                                            {
                                                                for (int j = 0; j < segunda; j++)
                                                                {
                                                                    for (int k = 0; k < tercera; k++)
                                                                    {
                                                                        agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                                    }
                                                                }
                                                            }


                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 6:// VISI0 ID1 TIPO2 DIM3 LP4 LI5
                                            {
                                                visi = root.ChildNodes[0].ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                                nombre = root.ChildNodes[0].ChildNodes[1].ToString().Replace(" (id)", "");
                                                tipo += root.ChildNodes[0].ChildNodes[2].ToString().Replace(" (Keyword)", "");
                                                int numdim = root.ChildNodes[0].ChildNodes[3].ChildNodes.Count;
                                                tipo += "/" + numdim;
                                                List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[0].ChildNodes[4],nombre);
                                                nuevo = new Metodo(nombre,lp, root, tipo, visi);
                                                listametodos.Add(nuevo);
                                                if (numdim == 1)
                                                {
                                                    int primera = 0;

                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    if (t.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());

                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]", "----");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }
                                                }
                                                else if (numdim == 2)
                                                {

                                                    int primera = 0;
                                                    int segunda = 0;
                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    object dim2 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[1], ambito);
                                                    Type t2 = dim2.GetType();
                                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());
                                                        segunda = int.Parse(dim2.ToString());

                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            for (int j = 0; j < segunda; j++)
                                                            {
                                                                agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]", "----");
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }

                                                }
                                                else if (numdim == 3)
                                                {
                                                    int primera = 0;
                                                    int segunda = 0;
                                                    int tercera = 0;
                                                    object dim1 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[0], ambito);
                                                    Type t = dim1.GetType();
                                                    object dim2 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[1], ambito);
                                                    Type t2 = dim2.GetType();
                                                    object dim3 = Operacion(root.ChildNodes[0].ChildNodes[3].ChildNodes[2], ambito);
                                                    Type t3 = dim3.GetType();
                                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                                    {
                                                        primera = int.Parse(dim1.ToString());
                                                        segunda = int.Parse(dim2.ToString());
                                                        tercera = int.Parse(dim3.ToString());
                                                        for (int i = 0; i < primera; i++)
                                                        {
                                                            for (int j = 0; j < segunda; j++)
                                                            {
                                                                for (int k = 0; k < tercera; k++)
                                                                {
                                                                    agregarSimbolo(visi, ambito, nombre, nombre + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                                }
                                                            }
                                                        }


                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                                    }
                                                }
                                                break;
                                            }
                                       
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    if (root.ChildNodes[1].ToString().Contains("id"))
                                    {
                                        tipo += root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    }
                                    else
                                    {
                                        tipo += root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                    }
                                    nuevo = new Metodo(nombre, root, tipo, visi);
                                    listametodos.Add(nuevo);
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    break;
                                }
                            case 4:
                                {
                                    if (root.ChildNodes[0].ToString().Equals("VISIBILIDAD"))
                                    {
                                        if (root.ChildNodes[2].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (Keyword)", "");
                                        }
                                        nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        nuevo = new Metodo(nombre, root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    }
                                    else if (root.ChildNodes[2].ToString().Contains("override"))
                                    {
                                        nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                        if (root.ChildNodes[1].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                        }
                                        nuevo = new Metodo(nombre, true, root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    }
                                    else if (root.ChildNodes[2].ToString().Equals("LISTAPARAM"))
                                    {
                                        nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                        if (root.ChildNodes[1].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                        }
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                        List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[2], nombre);
                                        nuevo = new Metodo(nombre, lp, root, tipo, visi);
                                        listametodos.Add(nuevo);

                                    }
                                    break;
                                }
                            case 5:
                                {
                                    if (root.ChildNodes[0].ToString().Contains(" (id)"))
                                    {
                                        nombre = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                        if (root.ChildNodes[1].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                                        }
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                        List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[3], nombre);
                                        nuevo = new Metodo(nombre, lp, true, root, tipo, visi);
                                        listametodos.Add(nuevo);

                                    }
                                    else if (root.ChildNodes[3].ToString().Equals("LISTAPARAM"))
                                    {
                                        visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                        nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        if (root.ChildNodes[2].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (Keyword)", "");
                                        }
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                        List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[3], nombre);
                                        nuevo = new Metodo(nombre, lp, root, tipo, visi);
                                        listametodos.Add(nuevo);

                                    }
                                    else
                                    {
                                        visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                        nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        if (root.ChildNodes[2].ToString().Contains("id"))
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (id)", "");
                                        }
                                        else
                                        {
                                            tipo += root.ChildNodes[2].ToString().Replace(" (Keyword)", "");
                                        }
                                        nuevo = new Metodo(nombre, true, root, tipo, visi);
                                        listametodos.Add(nuevo);
                                        agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    visi = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                    nombre = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    if (root.ChildNodes[2].ToString().Contains("id"))
                                    {
                                        tipo += root.ChildNodes[2].ToString().Replace(" (id)", "");
                                    }
                                    else
                                    {
                                        tipo += root.ChildNodes[2].ToString().Replace(" (Keyword)", "");
                                    }
                                    agregarSimbolo(visi, ambito, nombre, tipo, valor);
                                    List<Parametro> lp = ObtenerListaParametros(root.ChildNodes[4], nombre);
                                    nuevo = new Metodo(nombre, lp, true, root, tipo, visi);
                                    listametodos.Add(nuevo);

                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
            }
        }

        //METODO PARA OBTENER LA LISTA DE PARAMETROS DE CADA METODO
        private static List<Parametro> ObtenerListaParametros(ParseTreeNode raiz, string nm)
        {
            List<Parametro> enviar = new List<Parametro>();
            foreach (ParseTreeNode item in raiz.ChildNodes)
            {
                string tipo = item.ChildNodes[0].ToString().Replace(" (Keyword)", "");
                string nombre = item.ChildNodes[1].ChildNodes[0].ToString().Replace(" (id)", "");
                Parametro nuevo = new Parametro(nombre, tipo, valor);
                string ambit = ambito + "/" + nm;//cambio
                string tip = tipo + "/parametro";
                agregarSimbolo("publico", ambit, nombre, tip, "----");
                enviar.Add(nuevo);
            }
            return enviar;
        }

        //METODO PARA REALIZAR LAS OPERACIONES
        public static object Operacion(ParseTreeNode raizop, string bam)
        {
            switch (raizop.ChildNodes.Count)
            {
                case 1:
                    {
                        String valor = raizop.ChildNodes[0].ToString();
                        if (valor.Contains("(int)"))
                        {
                            String valfinal = valor.Replace(" (int)", "");
                            return Convert.ToInt32(valfinal);
                        }
                        else if (valor.Contains("(string)"))
                        {
                            return valor.Replace(" (string)", "");
                        }
                        else if (valor.Contains("(double)"))
                        {
                            String valfinal = valor.Replace(" (double)", "");
                            return Convert.ToDouble(valfinal);
                        }
                        else if (valor.Contains("(char)"))
                        {
                            String valfinal = valor.Replace(" (char)", "");
                            return Convert.ToChar(valfinal);
                        }
                        else if (valor.Contains("(bool)"))
                        {
                            String valfinal = valor.Replace(" (bool)", "");
                            if (valfinal.Equals("true"))
                            {
                                return true;
                            }
                            else if (valfinal.Equals("false"))
                            {
                                return false;
                            }
                            else if (valfinal.Equals("verdadero"))
                            {
                                return true;
                            }
                            else if (valfinal.Equals("falso"))
                            {
                                return false;
                            }

                        }
                        else if (valor.Contains("(id)"))
                        {
                            String identificador = valor.Replace(" (id)", "");
                            bool existe = false;
                            bool ambitobien = false;
                            foreach (Simbolo item in listasimbolos)
                            {
                                if (identificador == item.GetNombre())
                                {
                                    existe = true;
                                    string[] lista = bam.Split('/');
                                    if (item.GetAmbito() == bam || item.GetAmbito() == ambitoaux || item.GetAmbito() == lista[0])
                                    {
                                        ambitobien = true;
                                        return item.GetValor();
                                    }

                                }
                            }
                            if (existe == false)
                            {
                                Console.WriteLine("MARCAR ERROR LA VARIABLE NO EXISTE");
                            }
                            if (ambitobien == false)
                            {
                                Console.WriteLine("MARCAR ERROR SE INTENTO ACCEDER A UNA VARIABLE DESDE UN AMBITO INCORRECTO");
                            }
                        }
                        else if (valor.Equals("CALL"))
                        {
                            bool existe = false;
                            bool ambitocorrecto = false;
                            string nombreallamar = raizop.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                            string ambitmp = ambito2 + "/" + nombreallamar;
                            switch (raizop.ChildNodes[0].ChildNodes.Count)
                            {
                                case 1:
                                    {
                                        int nm = 0;
                                 
                                        foreach (Clase item in listaclases)
                                        {
                                            if (item.getNombre() == ambito2)
                                            {
                                                foreach (Metodo ex in item.GetMetodos())
                                                {
                                                    if (ex.getNombre() == nombreallamar)
                                                    {
                                                        nm++;
                                                    }
                                                }
                                                foreach (Metodo itemm in item.GetMetodos())
                                                {
                                                    if (itemm.getNombre() == nombreallamar && itemm.getTipo() != "void")
                                                    {
                                                        existe = true;

                                                        if (nm == 1)
                                                        {
                                                            compilarmetodo(itemm.getnodo(), ambitmp);
                                                            if (ambitoaux == "buscandoarreglo")
                                                            {
                                                                return nombreallamar;
                                                            }

                                                        }
                                                        else
                                                        {
                                                            if (itemm.getover() == true)
                                                            {
                                                                compilarmetodo(itemm.getnodo(), ambitmp);

                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombreallamar)
                                            {
                                                if (item.GetAmbito() == bam || item.GetAmbito() == ambito2)//cambio
                                                {
                                                    ambitocorrecto = true;
                                                    return item.GetValor();
                                                }
                                            }
                                        }

                                        if (!existe)
                                        {
                                            Console.WriteLine("MARCAR ERROR EL METODO A LLAMAR NO EXISTE");
                                        }
                                        if (!ambitocorrecto)
                                        {
                                            Console.WriteLine("LLAMANDO METODO DESDE AMBITO INCORRECTO");

                                        }
                                        break;
                                    }
                                case 2:
                                    {

                                        int nm = 0;
                                        if (raizop.ChildNodes[0].ChildNodes[1].ToString().Equals("LISTAENVIADA"))
                                        {
                                            foreach (Clase item in listaclases)
                                            {
                                                if (item.getNombre() == ambito2)//cambio+
                                                {
                                                    foreach (Metodo ex in item.GetMetodos())
                                                    {
                                                        if (ex.getNombre() == nombreallamar)
                                                        {
                                                            nm++;
                                                        }
                                                    }
                                                    foreach (Metodo itemm in item.GetMetodos())
                                                    {
                                                        if (itemm.getNombre() == nombreallamar && itemm.getTipo() != "void")
                                                        {
                                                            existe = true;
                                                            int contador = 0;
                                                            foreach (ParseTreeNode nodo in raizop.ChildNodes[0].ChildNodes[1].ChildNodes)
                                                            {

                                                                object valortmp = Operacion(nodo, bam);
                                                                Type tipotmp = valortmp.GetType();
                                                                int numero_parametros = itemm.getParams().Count;

                                                                string tipoabuscar = "";
                                                                if (tipotmp.Equals(typeof(int)))
                                                                {
                                                                    tipoabuscar = "int/parametro";
                                                                    foreach (Simbolo items in listasimbolos)
                                                                    {
                                                                        for (int i = 0; i < numero_parametros; i++)
                                                                        {
                                                                            if (i == contador)
                                                                            {
                                                                                if (items.GetAmbito() == ambitmp && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contador).getnombre())
                                                                                {
                                                                                    
                                                                                    items.SetValor(valortmp);
                                                                                }
                                                                            }

                                                                        }

                                                                    }//cierre foreach

                                                                }
                                                                else if (tipotmp.Equals(typeof(string)))
                                                                {
                                                                    tipoabuscar = "string/parametro";
                                                                    foreach (Simbolo items in listasimbolos)
                                                                    {
                                                                        for (int i = 0; i < numero_parametros; i++)
                                                                        {
                                                                            if (i == contador)
                                                                            {
                                                                                if (items.GetAmbito() == ambitmp && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contador).getnombre())
                                                                                {
                                                                           

                                                                                    items.SetValor(valortmp);
                                                                                }
                                                                            }

                                                                        }

                                                                    }//cierre foreach
                                                                }
                                                                else if (tipotmp.Equals(typeof(char)))
                                                                {
                                                                    tipoabuscar = "char/parametro";
                                                                    foreach (Simbolo items in listasimbolos)
                                                                    {
                                                                        for (int i = 0; i < numero_parametros; i++)
                                                                        {
                                                                            if (i == contador)
                                                                            {
                                                                                if (items.GetAmbito() == ambitmp && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contador).getnombre())
                                                                                {
                                                                               

                                                                                    items.SetValor(valortmp);
                                                                                }
                                                                            }

                                                                        }

                                                                    }//cierre foreach
                                                                }
                                                                else if (tipotmp.Equals(typeof(bool)))
                                                                {
                                                                    tipoabuscar = "bool/parametro";
                                                                    foreach (Simbolo items in listasimbolos)
                                                                    {
                                                                        for (int i = 0; i < numero_parametros; i++)
                                                                        {
                                                                            if (i == contador)
                                                                            {
                                                                                if (items.GetAmbito() == ambitmp && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contador).getnombre())
                                                                                {
                                                                                 

                                                                                    items.SetValor(valortmp);
                                                                                }
                                                                            }

                                                                        }

                                                                    }//cierre foreach
                                                                }
                                                                else if (tipotmp.Equals(typeof(double)))
                                                                {
                                                                    tipoabuscar = "double/parametro";
                                                                    foreach (Simbolo items in listasimbolos)
                                                                    {
                                                                        for (int i = 0; i < numero_parametros; i++)
                                                                        {
                                                                            if (i == contador)
                                                                            {
                                                                                if (items.GetAmbito() == ambitmp || items.GetAmbito().Contains(ambitosimportados))
                                                                                {
                                                                                    if (items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);

                                                                                    }

                                                                                }
                                                                            }

                                                                        }

                                                                    }//cierre foreach
                                                                }
                                                                contador++;
                                                            }
                                                            if (nm == 1)
                                                            {
                                                                compilarmetodo(itemm.getnodo(), ambitmp);
                                                                if (ambitoaux == "buscandoarreglo")
                                                                {
                                                                    return nombreallamar;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (itemm.getover() == true)
                                                                {
                                                                    compilarmetodo(itemm.getnodo(), ambitmp);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            foreach (Simbolo item in listasimbolos)
                                            {
                                                if (item.GetNombre() == nombreallamar)
                                                {
                                                    if (item.GetAmbito() == bam || item.GetAmbito() == ambito2)//<------------------------------------CAMBIO IMPORTATNTE?
                                                    {
                                                        ambitocorrecto = true;

                                                        return item.GetValor();
                                                    }
                                                }
                                            }
                                            if (!existe)
                                            {
                                                Console.WriteLine("MARCAR ERROR EL METODO A LLAMAR NO EXISTE");
                                            }
                                            if (!ambitocorrecto)
                                            {
                                                Console.WriteLine("LLAMANDO METODO DESDE AMBITO INCORRECTO");

                                            }
                                        }
                                        else
                                        {
                                            int nmm = 0;
                                            string nombreobjeto = raizop.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                            string metodoallamar = raizop.ChildNodes[0].ChildNodes[1].ToString().Replace(" (id)", "");
                                            foreach (Simbolo item in listasimbolos)
                                            {
                                                if (item.GetNombre() == nombreobjeto && item.GetTipo().Contains("objeto") && !item.GetTipo().Contains("off"))
                                                {
                                                    string[] lista = item.GetTipo().Split('/');
                                                    string tmpambit = lista[1] + "/" + metodoallamar;
                                                    foreach (Clase citem in listaclases)
                                                    {
                                                        if (citem.getNombre() == lista[1])
                                                        {
                                                            foreach (Metodo ex in citem.GetMetodos())
                                                            {
                                                                if (ex.getNombre() == metodoallamar)
                                                                {
                                                                    nmm++;
                                                                }
                                                            }
                                                            foreach (Metodo itemm in citem.GetMetodos())
                                                            {
                                                                if (itemm.getNombre() == metodoallamar && itemm.getTipo() != "void")
                                                                {
                                                                    existe = true;
                                                                    if (nmm == 1)
                                                                    {
                                                                        compilarmetodo(itemm.getnodo(), tmpambit);

                                                                    }
                                                                    else
                                                                    {
                                                                        if (itemm.getover() == true)
                                                                        {
                                                                            compilarmetodo(itemm.getnodo(), tmpambit);
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }
                                                    foreach (Simbolo sitem in listasimbolos)
                                                    {
                                                        if (sitem.GetNombre() == metodoallamar)
                                                        {
                                                            if (sitem.GetAmbito() == bam || sitem.GetAmbito() == lista[1])
                                                            {
                                                                ambitocorrecto = true;
                                                                return sitem.GetValor();

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (!existe)
                                            {
                                                Console.WriteLine("MARCAR ERROR EL METODO A LLAMAR NO EXISTE");
                                            }
                                            if (!ambitocorrecto)
                                            {
                                                Console.WriteLine("LLAMANDO METODO DESDE AMBITO INCORRECTO");

                                            }
                                        }

                                        break;
                                    }
                                case 3:
                                    {
                                        int nm = 0;
                                        string nombreobjeto = raizop.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                        string metodoabuscar = raizop.ChildNodes[0].ChildNodes[1].ToString().Replace(" (id)", "");

                                        foreach (Simbolo simbolo in listasimbolos)
                                        {
                                            if (simbolo.GetNombre() == nombreobjeto && simbolo.GetTipo().Contains("objeto") && !simbolo.GetTipo().Contains("off"))
                                            {
                                                string[] lista = simbolo.GetTipo().Split('/');
                                                string nombreclasebuscar = lista[1];
                                                string exambito = nombreclasebuscar + "/" + metodoabuscar;
                                          
                                                string tmpex = ambitoaux;
                                                ambitoaux = ambito2;
                                                foreach (Clase clase in listaclases)
                                                {
                                                    if (clase.getNombre() == nombreclasebuscar)
                                                    {
                                                        foreach (Metodo ex in clase.GetMetodos())
                                                        {
                                                            if (ex.getNombre() == metodoabuscar)
                                                            {
                                                                nm++;
                                                            }
                                                        }
                                                        foreach (Metodo met in clase.GetMetodos())
                                                        {
                                                            if (met.getNombre() == metodoabuscar && met.getTipo() != "void")
                                                            {
                                                                existe = true;
                                                                int contador = 0;
                                                                foreach (ParseTreeNode nodo in raizop.ChildNodes[0].ChildNodes[2].ChildNodes)
                                                                {
                                                                    object valortmp = Operacion(nodo, bam);
                                                                    Type tipotmp = valortmp.GetType();
                                                                    int numero_parametros = met.getParams().Count;
                                                                    string tipoabuscar = "";
                                                                    if (tipotmp.Equals(typeof(int)))
                                                                    {
                                                                        tipoabuscar = "int/parametro";
                                                                        foreach (Simbolo items in listasimbolos)
                                                                        {
                                                                            for (int i = 0; i < numero_parametros; i++)
                                                                            {
                                                                                if (i == contador)
                                                                                {
                                                                                    if (items.GetAmbito() == exambito && items.GetTipo() == tipoabuscar && items.GetNombre() == met.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }//cierre foreach
                                                                    }
                                                                    else if (tipotmp.Equals(typeof(string)))
                                                                    {
                                                                        tipoabuscar = "string/parametro";
                                                                        foreach (Simbolo items in listasimbolos)
                                                                        {
                                                                            for (int i = 0; i < numero_parametros; i++)
                                                                            {
                                                                                if (i == contador)
                                                                                {
                                                                                    if (items.GetAmbito() == exambito && items.GetTipo() == tipoabuscar && items.GetNombre() == met.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }//cierre foreach
                                                                    }
                                                                    else if (tipotmp.Equals(typeof(char)))
                                                                    {
                                                                        tipoabuscar = "char/parametro";
                                                                        foreach (Simbolo items in listasimbolos)
                                                                        {
                                                                            for (int i = 0; i < numero_parametros; i++)
                                                                            {
                                                                                if (i == contador)
                                                                                {
                                                                                    if (items.GetAmbito() == exambito && items.GetTipo() == tipoabuscar && items.GetNombre() == met.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }//cierre foreach
                                                                    }
                                                                    else if (tipotmp.Equals(typeof(bool)))
                                                                    {
                                                                        tipoabuscar = "bool/parametro";
                                                                        foreach (Simbolo items in listasimbolos)
                                                                        {
                                                                            for (int i = 0; i < numero_parametros; i++)
                                                                            {
                                                                                if (i == contador)
                                                                                {
                                                                                    if (items.GetAmbito() == exambito && items.GetTipo() == tipoabuscar && items.GetNombre() == met.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }//cierre foreach
                                                                    }
                                                                    else if (tipotmp.Equals(typeof(double)))
                                                                    {
                                                                        tipoabuscar = "double/parametro";
                                                                        foreach (Simbolo items in listasimbolos)
                                                                        {
                                                                            for (int i = 0; i < numero_parametros; i++)
                                                                            {
                                                                                if (i == contador)
                                                                                {
                                                                                    if (items.GetAmbito() == exambito && items.GetTipo() == tipoabuscar && items.GetNombre() == met.getParams().ElementAt(contador).getnombre())
                                                                                    {
                                                                                        items.SetValor(valortmp);
                                                                                        Console.WriteLine("cambie:" + items.GetAmbito()+items.GetTipo()+items.GetNombre() + items.GetValor()+"----"+valortmp);
                                                                                     
                                                                                    }
                                                                                }
                                                                            }
                                                                        }//cierre foreach
                                                                    }
                                                                    contador++;
                                                                }
                                                                if (nm == 1)
                                                                {
                                                                    compilarmetodo(met.getnodo(), exambito);
                                                                    if (ambitoaux == "buscandoarreglo")
                                                                    {
                                                                        return nombreallamar;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (met.getover() == true)
                                                                    {
                                                                        compilarmetodo(met.getnodo(), exambito);

                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                                foreach (Simbolo item in listasimbolos)
                                                {
                                                    if (item.GetNombre() == metodoabuscar)
                                                    {
                                                        if (item.GetAmbito() == bam || item.GetAmbito() == nombreclasebuscar)
                                                        {
                                                            ambitocorrecto = true;
                                                            return item.GetValor();
                                                        }
                                                    }
                                                }
                                                ambitoaux = tmpex;
                                            }
                                        }
                                        if (!existe)
                                        {
                                            Console.WriteLine("MARCAR ERROR EL METODO A LLAMAR NO EXISTE");
                                        }
                                        if (!ambitocorrecto)
                                        {
                                            Console.WriteLine("LLAMANDO METODO DESDE AMBITO INCORRECTO");

                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }

                        }
                        else if (valor.Equals("ACCESOVAR"))
                        {
                            bool existeobjeto = false;
                            bool existevar = false;
                            bool ambitobien = false;
                            string nombreobjeto = raizop.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                            string nombrevar = raizop.ChildNodes[0].ChildNodes[1].ToString().Replace(" (id)", "");
                            foreach (Simbolo item in listasimbolos)
                            {
                                if (item.GetNombre() == nombreobjeto && !item.GetTipo().Contains("off"))
                                {
                                    existeobjeto = true;
                                    string[] contenido = item.GetTipo().Split('/');
                                    foreach (Simbolo var in listasimbolos)
                                    {
                                        if (var.Getvis().Equals("publico") && var.GetAmbito() == contenido[1] && var.GetNombre() == nombrevar)
                                        {
                                            existevar = true;
                                            ambitobien = true;
                                            return var.GetValor();
                                        }
                                    }
                                }
                            }
                            if (!existeobjeto)
                            {
                                Console.WriteLine("MARCAR ERROR EL OBJETO NO EXISTE");
                            }
                            if (!existevar)
                            {
                                Console.WriteLine("SE INTENDA ACCEDER A UNA VARIABLE QUE NO EXISTE");

                            }
                            if (!ambitobien)
                            {
                                Console.WriteLine("ACCESOVAR DESDE AMBITO INCORRECTO");

                            }
                        }
                        else if (valor.Equals("TOARRAY"))
                        {
                            bool existe = false;
                            bool posicionmal = false;
                            bool ambitobien = false;
                            string nombreabuscar = raizop.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)","");
                            int num_dimensiones = raizop.ChildNodes[0].ChildNodes[1].ChildNodes.Count();
                            if (num_dimensiones == 1)
                            {
                                object pos1 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[0], bam);
                                if (pos1.GetType().Equals(typeof(int)))
                                {
                                    int posicionabuscar = int.Parse(pos1.ToString());
                                    foreach (Simbolo sim in listasimbolos)
                                    {
                                        string contiene = nombreabuscar +"["+posicionabuscar+"]";
                                        if (sim.GetAmbito() == bam || sim.GetAmbito() == ambito2)
                                        {
                                            ambitobien = true;
                                            if (sim.GetNombre() == nombreabuscar)
                                            {
                                                existe = true;
                                                if (sim.GetTipo().Equals(contiene))
                                                {
                                                    return sim.GetValor();
                                                }
                                                else
                                                {
                                                    posicionmal = true;
                                                }
                                                
                                            }
                                            
                                        }
                                        
                                    }
                                }
                                if (!existe)
                                {
                                    Console.WriteLine("MARCAR ERROR EL ARREGLO NO EXISTE");
                                }
                                if (posicionmal)
                                {
                                    Console.WriteLine("SE INTENDA ACCEDER A UNA POSICION QUE NO EXISTE EN EL ARREGLO");

                                }
                                if (!ambitobien)
                                {
                                    Console.WriteLine("ACCESO A ARREGLO DESDE AMBITO INCORRECTO");

                                }
                            }
                            else if (num_dimensiones == 2)
                            {
                                object pos1 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[0],bam);
                                object pos2 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[1], bam);
                                if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)))
                                {
                                    int posicionabuscar = int.Parse(pos1.ToString());
                                    int posicion2abuscar = int.Parse(pos2.ToString());
                                    foreach (Simbolo sim in listasimbolos)
                                    {
                                        string contiene = nombreabuscar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]";
                                        if (sim.GetAmbito() == bam || sim.GetAmbito() == ambito2)
                                        {
                                            ambitobien = true;
                                            if (sim.GetNombre() == nombreabuscar)
                                            {
                                                existe = true;
                                                if (sim.GetTipo().Equals(contiene))
                                                {
                                                    return sim.GetValor();
                                                }
                                                else
                                                {
                                                    posicionmal = true;
                                                }
                                                
                                            }
                                        }

                                    }
                                }
                                if (!existe)
                                {
                                    Console.WriteLine("MARCAR ERROR EL ARREGLO NO EXISTE");
                                }
                                if (posicionmal)
                                {
                                    Console.WriteLine("SE INTENDA ACCEDER A UNA POSICION QUE NO EXISTE EN EL ARREGLO");

                                }
                                if (!ambitobien)
                                {
                                    Console.WriteLine("ACCESO A ARREGLO DESDE AMBITO INCORRECTO");

                                }
                            }
                            else if (num_dimensiones == 3)
                            {
                                object pos1 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[0], bam);
                                object pos2 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[1], bam);
                                object pos3 = Operacion(raizop.ChildNodes[0].ChildNodes[1].ChildNodes[2], bam);
                                if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)) && pos3.GetType().Equals(typeof(int)))
                                {
                                    int posicionabuscar = int.Parse(pos1.ToString());
                                    int posicion2abuscar = int.Parse(pos2.ToString());
                                    int posicion3abuscar = int.Parse(pos3.ToString());
                                    foreach (Simbolo sim in listasimbolos)
                                    {
                                        string contiene = nombreabuscar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]" + "[" + posicion3abuscar + "]";
                                        if (sim.GetAmbito() == bam || sim.GetAmbito() == ambito2)
                                        {
                                            ambitobien = true;
                                            if (sim.GetNombre() == nombreabuscar && sim.GetTipo().Equals(contiene))
                                            {
                                                existe = true;
                                                if (sim.GetTipo().Equals(contiene))
                                                {
                                                    return sim.GetValor();
                                                }
                                                else
                                                {
                                                    posicionmal = true;
                                                }
                                                
                                            }
                                        }

                                    }
                                }
                                if (!existe)
                                {
                                    Console.WriteLine("MARCAR ERROR EL ARREGLO NO EXISTE");
                                }
                                if (posicionmal)
                                {
                                    Console.WriteLine("SE INTENDA ACCEDER A UNA POSICION QUE NO EXISTE EN EL ARREGLO");

                                }
                                if (!ambitobien)
                                {
                                    Console.WriteLine("ACCESO A ARREGLO DESDE AMBITO INCORRECTO");

                                }
                            }
                        }
                        else
                        {
                            return Operacion(raizop.ChildNodes[0], bam);
                        }
                        break;
                    }
                case 2:
                    {
                        Object nevalor = Operacion(raizop.ChildNodes[1], bam);
                        Type tn = nevalor.GetType();
                        Object nresutl = "Error";
                        bool sepuede = false;
                        string test = raizop.ChildNodes[0].ToString();
                        if (raizop.ChildNodes[0].ToString().Contains("(Key symbol)"))
                        {
                            
                            switch (raizop.ChildNodes[0].ToString().Replace(" (Key symbol)", ""))
                            {
                                case "-":
                                    if (tn.Equals(typeof(int)))
                                    {
                                        sepuede = true;
                                        int val1 = int.Parse(nevalor.ToString());
                                        nresutl = -val1;
                                    }
                                    else if (tn.Equals(typeof(double)))
                                    {
                                        sepuede = true;
                                        double vald = double.Parse(nevalor.ToString());
                                        nresutl = -vald;
                                    }
                                    if (!sepuede)
                                    {
                                        Console.WriteLine("MARCAR ERROR SE INTENTO NEGAR UN TIPO NO PERMITIDO");
                                    }
                                    return nresutl;
                                case "!":
                                    if (tn.Equals(typeof(Boolean)))
                                    {
                                        sepuede = true;
                                        Boolean val = Boolean.Parse(nevalor.ToString());
                                        if (val == false)
                                        {
                                            nresutl = true;
                                        }
                                        else if (val == true)
                                        {
                                            nresutl = false;
                                        }


                                    }
                                    if (!sepuede)
                                    {
                                        Console.WriteLine("MARCAR ERROR SE INTENTO NEGAR UN TIPO NO PERMITIDO");
                                    }
                                    return nresutl;
                                default: return nresutl;
                            }


                        }
                        
                        else if (raizop.ChildNodes[0].ToString().Contains("new"))
                        {
                            bool existe = false;
                            string nombre = raizop.ChildNodes[1].ToString().Replace(" (id)", "");
                            foreach (Simbolo item in listasimbolos)
                            {
                                if (item.GetTipo().Contains(nombre) && item.GetTipo().Contains("off"))
                                {
                                    existe = true;
                                    item.SetTipo("objeto/" + nombre);
                                    nresutl = "----";
                                }
                            }
                            if (!existe)
                            {
                                Console.WriteLine("MARCAR ERROR SE INTENTA CREAR UN NEW OBJECT QUE NO EXISTE");
                            }
                        }
                        else
                        {
                            bool sepuedecambiar = false;
                            Object extra = Operacion(raizop.ChildNodes[0], bam);
                            Type ex = extra.GetType();
                            switch (raizop.ChildNodes[1].ToString().Replace(" (Key symbol)", ""))
                            {
                                case "++":
                                    if (ex.Equals(typeof(int)))
                                    {
                                        sepuedecambiar = true;
                                        int val1 = int.Parse(extra.ToString());
                                        nresutl = val1 + 1;
                                    }
                                    else if (ex.Equals(typeof(double)))
                                    {
                                        sepuedecambiar = true;
                                        double vald = double.Parse(extra.ToString());
                                        nresutl = vald + 1;
                                    }
                                    else if (ex.Equals(typeof(char)))
                                    {
                                        sepuedecambiar = true;
                                        int valc = char.Parse(extra.ToString());
                                        nresutl = valc + 1;
                                    }
                                    return nresutl;
                                case "--":
                                    if (ex.Equals(typeof(int)))
                                    {
                                        sepuedecambiar = true;
                                        int val1 = int.Parse(extra.ToString());
                                        nresutl = val1 - 1;
                                    }
                                    else if (ex.Equals(typeof(double)))
                                    {
                                        sepuedecambiar = true;
                                        double vald = double.Parse(extra.ToString());
                                        nresutl = vald - 1;
                                    }
                                    else if (ex.Equals(typeof(char)))
                                    {
                                        sepuedecambiar = true;
                                        int valc = char.Parse(extra.ToString());
                                        nresutl = valc - 1;
                                    }
                                    return nresutl;
                            }
                            if (!sepuedecambiar)
                            {
                                Console.WriteLine("MARCAR ERROR SE INTENTO AUMENTAR O DISMINUIR UN TIPO NO PERMITDO");
                            }
                        }

                        return nresutl;
                    }
                case 3:
                    {
                        Object valor1 = Operacion(raizop.ChildNodes[0], bam);
                        Type t1 = valor1.GetType();
                        Object valor2 = Operacion(raizop.ChildNodes[2], bam);
                        Type t2 = valor2.GetType();
                        Object resultado = "Error";
                        switch (raizop.ChildNodes[1].ToString().Replace(" (Key symbol)", ""))
                        {
                            /*----------------------------------------------------ARITMETICAS---------------------------------------------------------------------*/

                            #region suma
                            case "+":

                                //SI EL PRIMER TERMINO ES ENTERO
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(String)))
                                {
                                    string val1 = valor1.ToString();
                                    string val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 + 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1;
                                    }

                                }

                                //SI EL PRIMER TERMINO ES STRING

                                else if (t1.Equals(typeof(String)) && t2.Equals(typeof(int)))
                                {
                                    String val1 = valor1.ToString();
                                    String val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(String)) && t2.Equals(typeof(String)))
                                {
                                    String val1 = valor1.ToString();
                                    String val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(String)) && t2.Equals(typeof(Double)))
                                {
                                    String val1 = valor1.ToString();
                                    String val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(String)) && t2.Equals(typeof(char)))
                                {
                                    String val1 = valor1.ToString();
                                    String val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;

                                }
                                //SI EL PRIMER TERMINO ES DOUBLE
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(String)))
                                {
                                    string val1 = valor1.ToString();
                                    string val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Boolean)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 + 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES CHAR
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Char.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(String)))
                                {
                                    string val1 = valor1.ToString();
                                    string val2 = valor2.ToString();
                                    resultado = val1 + " " + val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Char.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 + 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES BOOL
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(int)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(double)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    double val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Char)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 + val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Boolean)))
                                {
                                    Boolean val1 = Boolean.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val1 || val2)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }

                                }
                                else
                                {
                                    return "Error";
                                }
                                return resultado;
                            #endregion
                            #region resta
                            case "-":

                                //SI EL PRIMER TERMINO ES ENTERO
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 - 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1 - 0;
                                    }

                                }

                                //SI EL PRIMER TERMINO ES DOUBLE
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Boolean)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 - 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES CHAR
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Char.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Char.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                //SI EL PRIMER TERMINO ES BOOL
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(int)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(double)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    double val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 - val2;
                                }
                                else
                                {
                                    return "Error";
                                }
                                return resultado;
                            #endregion
                            #region multiplicacion
                            case "*":
                                //SI EL PRIMER TERMINO ES ENTERO
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 * 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1 * 0;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES DOUBLE
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Boolean)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 * 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1 * 0;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES CHAR
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Char.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Char.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = val1 * 1;
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = val1 * 0;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES BOOL
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(int)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(double)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    double val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Char)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = val1 * val2;
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Boolean)))
                                {
                                    Boolean val1 = Boolean.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val1 && val2)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }

                                }
                                else
                                {
                                    return "Error";
                                }
                                return resultado;
                            #endregion
                            #region division
                            case "/":
                                if (valor2.ToString() != "0")
                                {
                                    //SI EL PRIMER TERMINO ES ENTERO
                                    if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        double val2 = double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                    {
                                        double val1 = Double.Parse(valor1.ToString());
                                        double val2 = Double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Char)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        double val2 = char.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Boolean)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        Boolean val2 = Boolean.Parse(valor2.ToString());
                                        if (val2.Equals(true))
                                        {
                                            resultado = val1 / 1;
                                        }
                                        else if (val2.Equals(false))
                                        {
                                            MessageBox.Show("MATH ERROR!");
                                            return "Error";
                                        }

                                    }
                                    //SI EL PRIMER TERMINO ES DOUBLE
                                    else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        double val2 = double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                    {
                                        double val1 = Double.Parse(valor1.ToString());
                                        double val2 = Double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Char)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        int val2 = Char.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Boolean)))
                                    {
                                        double val1 = double.Parse(valor1.ToString());
                                        Boolean val2 = Boolean.Parse(valor2.ToString());
                                        if (val2.Equals(true))
                                        {
                                            resultado = val1 / 1;
                                        }
                                        else if (val2.Equals(false))
                                        {
                                            MessageBox.Show("MATH ERROR!");
                                            return "Error";
                                        }

                                    }
                                    //SI EL PRIMER TERMINO ES CHAR
                                    else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                    {
                                        double val1 = Char.Parse(valor1.ToString());
                                        double val2 = double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                    {
                                        double val1 = Char.Parse(valor1.ToString());
                                        double val2 = Double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Char)))
                                    {
                                        double val1 = char.Parse(valor1.ToString());
                                        double val2 = Char.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Boolean)))
                                    {
                                        double val1 = char.Parse(valor1.ToString());
                                        Boolean val2 = Boolean.Parse(valor2.ToString());
                                        if (val2.Equals(true))
                                        {
                                            resultado = val1 / 1;
                                        }
                                        else if (val2.Equals(false))
                                        {
                                            MessageBox.Show("MATH ERROR!");
                                            return "Error";
                                        }

                                    }
                                    //SI EL PRIMER TERMINO ES BOOL
                                    else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(int)))
                                    {
                                        Boolean res = Boolean.Parse(valor1.ToString());
                                        int val1 = 0;
                                        if (res.Equals(true))
                                        {
                                            val1 = 1;
                                        }
                                        int val2 = Int32.Parse(valor2.ToString());
                                        if (val1 != 0)
                                        {
                                            resultado = val1 / val2;
                                        }
                                        else
                                        {
                                            MessageBox.Show("MATH ERROR!");
                                            return "Error";
                                        }
                                    }
                                    else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(double)))
                                    {
                                        Boolean res = Boolean.Parse(valor1.ToString());
                                        double val1 = 0;
                                        if (res.Equals(true))
                                        {
                                            val1 = 1;
                                        }
                                        double val2 = Double.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Char)))
                                    {
                                        Boolean res = Boolean.Parse(valor1.ToString());
                                        int val1 = 0;
                                        if (res.Equals(true))
                                        {
                                            val1 = 1;
                                        }
                                        int val2 = Char.Parse(valor2.ToString());
                                        resultado = val1 / val2;
                                    }
                                    else
                                    {
                                        return "Error";
                                    }
                                    return resultado;
                                }
                                else
                                {
                                    MessageBox.Show("MATH ERROR!");
                                    return "Error";
                                }
                            #endregion
                            #region potencia
                            case "^":

                                //SI EL PRIMER TERMINO ES ENTERO
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = (int)Math.Pow(val1, 1);
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = 1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES DOUBLE
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Double.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(Boolean)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = Math.Pow(val1, 1);
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = 1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES CHAR
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Char.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = Char.Parse(valor1.ToString());
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(Boolean)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    Boolean val2 = Boolean.Parse(valor2.ToString());
                                    if (val2.Equals(true))
                                    {
                                        resultado = (int)Math.Pow(val1, 1);
                                    }
                                    else if (val2.Equals(false))
                                    {
                                        resultado = 1;
                                    }

                                }
                                //SI EL PRIMER TERMINO ES BOOL
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(int)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Int32.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(double)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    double val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    double val2 = Double.Parse(valor2.ToString());
                                    resultado = Math.Pow(val1, val2);
                                }
                                else if (t1.Equals(typeof(Boolean)) && t2.Equals(typeof(Char)))
                                {
                                    Boolean res = Boolean.Parse(valor1.ToString());
                                    int val1 = 0;
                                    if (res.Equals(true))
                                    {
                                        val1 = 1;
                                    }
                                    int val2 = Char.Parse(valor2.ToString());
                                    resultado = (int)Math.Pow(val1, val2);
                                }
                                else
                                {
                                    return "Error";
                                }
                                return resultado;
                            #endregion
                            /*---------------------------------------------------- LOGICAS---------------------------------------------------------------------*/
                            #region or
                            case "||":
                                Boolean v1 = Boolean.Parse(valor1.ToString());
                                Boolean v2 = Boolean.Parse(valor2.ToString());
                                if (v1 || v2)
                                {
                                    resultado = true;
                                }
                                else
                                {
                                    resultado = false;
                                }
                                return resultado;
                            #endregion
                            #region and
                            case "&&":
                                Boolean av1 = Boolean.Parse(valor1.ToString());
                                Boolean av2 = Boolean.Parse(valor2.ToString());
                                if (av1 && av2)
                                {
                                    resultado = true;
                                }
                                else
                                {
                                    resultado = false;
                                }
                                return resultado;
                            #endregion
                            /*---------------------------------------------------- RELACIONALES---------------------------------------------------------------------*/
                            #region igualigual
                            case "==":
                                string vars1 = valor1.ToString();
                                string vars2 = valor2.ToString();
                                if (vars1.Equals(vars2))
                                {
                                    resultado = true;
                                }
                                else
                                {
                                    resultado = false;
                                }
                                return resultado;
                            #endregion
                            #region menorque
                            case "<":
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(bool)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 < 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    } else if (val2 == false)
                                    {
                                        if (val1 < 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }

                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = int.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(bool)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 < 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 < 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }

                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 < val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }

                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(int)))
                                {
                                    int val2 = int.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 < val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 < val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if(t1.Equals(typeof(bool)) && t2.Equals(typeof(double))){
                                    double val2 = double.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 < val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 < val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                return resultado;
                            #endregion
                            #region mayorque
                            case ">":
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(bool)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 > 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 > 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = int.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(bool)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 > 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 > 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 > val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(int)))
                                {
                                    int val2 = int.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 > val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 > val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(double)))
                                {
                                    double val2 = double.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 > val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 > val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                return resultado;
                            #endregion
                            #region menorigual
                            case "<=":
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = int.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 <= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(bool)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 <= 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 <= 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(bool)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 <= 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 <= 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(double)))
                                {
                                    double val2 = double.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 <= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 <= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(int)))
                                {
                                    int val2 = int.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 <= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 <= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                return resultado;
                            #endregion
                            #region mayorigual
                            case ">=":
                                if (t1.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    int val2 = Int32.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(double)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = Int32.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(char)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(char)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(int)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    int val2 = int.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(int)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    int val2 = char.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(char)) && t2.Equals(typeof(double)))
                                {
                                    int val1 = char.Parse(valor1.ToString());
                                    double val2 = double.Parse(valor2.ToString());
                                    if (val1 >= val2)
                                    {
                                        resultado = true;
                                    }
                                    else
                                    {
                                        resultado = false;
                                    }
                                }
                                else if (t1.Equals(typeof(int)) && t2.Equals(typeof(bool)))
                                {
                                    int val1 = int.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 >= 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 >= 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(double)) && t2.Equals(typeof(bool)))
                                {
                                    double val1 = double.Parse(valor1.ToString());
                                    bool val2 = bool.Parse(valor2.ToString());
                                    if (val2 == true)
                                    {
                                        if (val1 >= 1)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val2 == false)
                                    {
                                        if (val1 >= 0)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(double)))
                                {
                                    double val2 = double.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 >= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 >= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                else if (t1.Equals(typeof(bool)) && t2.Equals(typeof(int)))
                                {
                                    int val2 = int.Parse(valor2.ToString());
                                    bool val1 = bool.Parse(valor1.ToString());
                                    if (val1 == true)
                                    {
                                        if (1 >= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                    else if (val1 == false)
                                    {
                                        if (0 >= val2)
                                        {
                                            resultado = true;
                                        }
                                        else
                                        {
                                            resultado = false;
                                        }
                                    }
                                }
                                return resultado;
                            #endregion
                            #region diferente
                            case "!=":
                                string varsd1 = valor1.ToString();
                                string varsd2 = valor2.ToString();
                                if (varsd1 != varsd2)
                                {
                                    resultado = true;
                                }
                                else
                                {
                                    resultado = false;
                                }
                                return resultado;
                            #endregion
                            default:
                                return Operacion(raizop.ChildNodes[1], bam);
                        }

                    }
            }
            return "Error";
        }

        //METODO PARA AGREGAR UN SIMBOLO A LA TABLA DE SIMBOLOS
        public static void agregarSimbolo(String visi, String ambi, String nombre, string tipo, object valor)
        {
            Simbolo nuevo = new Simbolo(visi, ambi, nombre, tipo, valor);
            listasimbolos.Add(nuevo);
        }

       
        public static void agregarSimbolo2(String visi, String ambi, String nombre, string tipo, object valor)
        {
            Simbolo nuevo = new Simbolo(visi, ambi, nombre, tipo, valor);
            extra.Add(nuevo);
        }


        static string ambitosimportados = "";
        //SEGUNADA PASADA PARA HACER LA EXTENSION Y OPERACIONES ENTRE CLASES
        public static void compilar2(ParseTreeNode root)
        {
            switch (root.Term.Name.ToString())
            {
                case "INICIO":
                    {
                        compilar2(root.ChildNodes[0]);
                        break;
                    }
                case "LISTACLASES":
                    {
                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            compilar2(root.ChildNodes[i]);
                        }
                        break;
                    }
                case "CLASE":
                    {
                        if (root.ChildNodes.Count > 1)
                        {
                            string nombreclaseanalizando = root.ChildNodes[0].ToString().Replace(" (id)", "");
                            ambito2 = nombreclaseanalizando;
                            bool importar = false;
                            List<string> nombresimportados = new List<string>();
                            if (root.ChildNodes[1].ToString().Equals("LISTAIDES"))
                            {
                                Clase clasequerecibe = null;
                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                {
                                    bool existe = false;
                                    bool cambio = false;
                                    string nombreclaseaimportar = item.ToString().Replace(" (id)", "");
                                    foreach (Clase clase in listaclases)
                                    {
                                        if (clase.getNombre() == nombreclaseanalizando)
                                        {
                                            clasequerecibe = clase;
                                            cambio = true;
                                        }
                                        if (clase.getNombre() == nombreclaseaimportar && cambio == true)
                                        {
                                            nombresimportados.Add(nombreclaseaimportar);
                                            foreach (Variable var in clase.GetVariables())
                                            {
                                                clasequerecibe.addVariable(var);
                                                try
                                                {
                                                    foreach (Simbolo sims in listasimbolos)
                                                    {
                                                        if (sims.GetAmbito().Contains(clase.getNombre()) && sims.Getvis() == "publico" && sims.GetNombre() == var.getnombre())
                                                        {
                                                            agregarSimbolo2(sims.Getvis(), clasequerecibe.getNombre(), sims.GetNombre(), sims.GetTipo(), sims.GetValor());
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("ERROR FATAL AL CAMBIAR LA TABLA?");
                                                }
                                            }
                                            foreach (Metodo met in clase.GetMetodos())
                                            {
                                                if (met.getNombre() != "main")
                                                {
                                                    clasequerecibe.addMetodo(met);
                                                }
                                                try
                                                {
                                                    foreach (Simbolo sim in listasimbolos)
                                                    {
                                                        if (sim.GetAmbito().Contains(clase.getNombre()) && sim.Getvis() == "publico" && sim.GetNombre() == met.getNombre())
                                                        {
                                                            agregarSimbolo2(sim.Getvis(), clasequerecibe.getNombre(), sim.GetNombre(), sim.GetTipo(), sim.GetValor());
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {

                                                    Console.WriteLine("ERROR FATAL AL QUERER CAMBIAR LA TABLAX2?" + e);
                                                }
                                            }
                                            existe = true;
                                        }
                                    }
                                    if (existe == false)
                                    {
                                        Console.WriteLine("MARCAR ERROR SE INTENTO IMPORTAR UNA CLASE QUE NO EXISTE");
                                    }
                                }
                                importar = true;

                            }
                            if (importar == true && nombresimportados.Count != 0)
                            {
                                foreach (string x in nombresimportados)
                                {
                                    ambitosimportados += x + "/";
                                    try
                                    {
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetAmbito().Contains('/') && item.GetAmbito().Contains(x))
                                            {
                                                string exxx = item.GetAmbito().Replace(x, nombreclaseanalizando);
                                                Console.WriteLine(exxx);
                                                agregarSimbolo2(item.Getvis(), exxx, item.GetNombre(), item.GetTipo(), item.GetValor());
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Console.WriteLine("QUE PASA¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡");
                                    }

                                }
                            }

                            foreach (ParseTreeNode item in root.ChildNodes)
                            {
                                compilar2(item);
                            }
                        }

                        break;
                    }
                case "OBJETO":
                    {
                        switch (root.ChildNodes.Count)
                        {
                            case 4:
                                {
                                    string nombreclaseabuscar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    string identificador = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    string verificar = root.ChildNodes[3].ToString().Replace(" (id)", "");
                                    bool confirmar_nombre = false;
                                    bool confirmar_clase = false;
                                    foreach (Clase item in listaclases)
                                    {
                                        if (nombreclaseabuscar == item.getNombre())
                                        {
                                            confirmar_clase = true;
                                            if (nombreclaseabuscar == verificar)
                                            {
                                                string ti = "objeto/" + nombreclaseabuscar;
                                                agregarSimbolo("publico", ambito2, identificador, ti, "-----");
                                                confirmar_nombre = true;
                                            }
                                        }
                                    }
                                    if (confirmar_nombre == false)
                                    {
                                        Console.WriteLine("Marcar error mala declaracion");
                                    }
                                    else if (confirmar_clase == false)
                                    {
                                        Console.WriteLine("Marcar error la clase no existe");
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nombreclaseabuscar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    string iden = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    bool confirmar = false;
                                    foreach (Clase item in listaclases)
                                    {
                                        if (item.getNombre() == nombreclaseabuscar)
                                        {
                                            string ti = "objeto/" + nombreclaseabuscar + "/off";
                                            agregarSimbolo("publico", ambito2, iden, ti, "-----");
                                            confirmar = true;
                                        }
                                    }
                                    if (confirmar == false)
                                    {
                                        Console.WriteLine("La clse no exixte");
                                    }
                                    break;
                                }

                        }
                        break;
                    }
                case "LISTAINSTRUCCIONES":
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            compilar2(hijo);
                        }
                        break;
                    }
                case "ASIGNACIONARRAY":
                    {
                        string nombreacambiar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        string tipoas = root.ChildNodes[1].ToString();
                        foreach (Clase clase in listaclases)
                        {
                            if (clase.getNombre() == ambito2)
                            {
                                foreach (Variable var in clase.GetVariables())
                                {
                                    if (var.getnombre() == nombreacambiar)
                                    {
                                        if (tipoas == "AS1")
                                        {
                                            if (var.getDimen() == 1)
                                            {
                                                int num1 = var.getdim1();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    foreach (Simbolo sim in listasimbolos)
                                                    {
                                                        string contiene = "[" + i + "]";
                                                        if (sim.GetAmbito() == ambito2 && sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                        {
                                                            if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num1)
                                                            {
                                                                object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[i], ambito2);
                                                                sim.SetValor(valornuevo);
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (tipoas == "AS2")
                                        {
                                            if (var.getDimen() == 2)
                                            {
                                                int num1 = var.getdim1();
                                                int num2 = var.getdim2();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    for (int j = 0; j < num2; j++)
                                                    {
                                                        foreach (Simbolo sim in listasimbolos)
                                                        {
                                                            string contiene = "[" + i + "]" + "[" + j + "]";
                                                            if (sim.GetAmbito() == ambito2 && sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                            {
                                                                if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num2 && root.ChildNodes[1].ChildNodes[0].ChildNodes[j].ChildNodes.Count == num1)
                                                                {
                                                                    object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[j].ChildNodes[i], ambito2);
                                                                    sim.SetValor(valornuevo);
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                }

                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                        else if (tipoas == "AS3")
                                        {
                                            if (var.getDimen() == 3)
                                            {
                                                int num1 = var.getdim1();
                                                int num2 = var.getdim2();
                                                int num3 = var.getdim3();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    for (int j = 0; j < num2; j++)
                                                    {
                                                        for (int k = 0; k < num3; k++)
                                                        {
                                                            foreach (Simbolo sim in listasimbolos)
                                                            {
                                                                string contiene = "[" + i + "]" + "[" + j + "]" + "[" + k + "]";
                                                                if (sim.GetAmbito() == ambito2 && sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                                {
                                                                    if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num1 && root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count == num2 && root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes.Count == num3)
                                                                    {
                                                                        object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes[k], ambito2);
                                                                        sim.SetValor(valornuevo);
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                    }

                                                                }
                                                            }
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR LA VARIABLE NO EXISTE EN ESTE AMBITO");
                                    }
                                }
                            }
                        }

                        break;
                    }
                case "ASIGNACION":
                    {
                        String iden = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        Object valor = Operacion(root.ChildNodes[1], ambito2);
                        Type vty = valor.GetType();
                        foreach (Simbolo sym in listasimbolos)
                        {
                            if (sym.GetNombre() == iden && sym.GetAmbito() == ambito2)
                            {
                                sym.SetValor(valor);
                            }
                        }
                        foreach (Clase clase in listaclases)
                        {
                            if (clase.getNombre() == ambito2)
                            {
                                foreach (Variable var in clase.GetVariables())
                                {
                                    if (var.getnombre() == iden)
                                    {
                                        string tip = var.gettipo();
                                        if (tip.Equals("int") && vty.Equals(typeof(int)))
                                        {
                                            int nuevo = int.Parse(valor.ToString());
                                            var.setvi(nuevo);
                                        }
                                        else if (tip.Equals("string") && vty.Equals(typeof(string)))
                                        {
                                            var.setvs(valor.ToString());
                                        }
                                        else if (tip.Equals("char") && vty.Equals(typeof(char)))
                                        {
                                            char val = char.Parse(valor.ToString());
                                            var.setvc(val);
                                        }
                                        else if (tip.Equals("double") && vty.Equals(typeof(double)))
                                        {
                                            double num = double.Parse(valor.ToString());
                                            var.setvd(num);
                                        }
                                        else if (tip.Equals("bool") && vty.Equals(typeof(Boolean)))
                                        {
                                            Boolean vari = Boolean.Parse(valor.ToString());
                                            var.setvb(vari);
                                        }
                                        else
                                        {
                                            String lexema = "Asignacion";
                                            String mensaje = "No se puede asignar un valor si el tipo no coincide o no se ha declarado previamente";
                                            String tipoerror = "Semantico";
                                            Error nuevo = new Error(lexema, mensaje, tipoerror, 0, 0);
                                            listaerrores.Add(nuevo);
                                        }
                                    }
                                }
                            }

                        }

                        break;
                    }
                case "LISTAMETODOS":
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            compilar2(hijo);
                        }
                        break;
                    }
                case "METODOVOID":
                    {
                        switch (root.ChildNodes.Count)
                        {
                            case 2:
                                {
                                    String nombrem = root.ChildNodes[0].ToString().Replace(" (Keyword)", "");
                                    if (nombrem == "main")
                                    {
                                        nummain++;
                                        foreach (Clase item in listaclases)
                                        {
                                            if (ambito2 == item.getNombre())
                                            {
                                                foreach (Metodo item2 in item.GetMetodos())
                                                {
                                                    if (nombrem == item2.getNombre() && item2.getTipo() == "void" && nummain == 1)
                                                    {
                                                        String ambi = ambito2 + "/" + nombrem;
                                                        compilarmetodo(item2.getnodo(), ambi);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
            }//FIN DEL SWITCH
        }//FIN DE COMPILAR2


        static string nombreaux = "";
        //METODO PARA EJECUTAR EL NODO DE UN METODO
        public static void compilarmetodo(ParseTreeNode root, string ambitom)
        {

            switch (root.Term.Name.ToString())
            {
                case "METODOVOID":
                    {
                        foreach (ParseTreeNode item in root.ChildNodes)
                        {
                            compilarmetodo(item, ambitom);
                        }
                        break;
                    }
                case "METODORETURN":
                    {
                            
                        foreach (ParseTreeNode item in root.ChildNodes)
                        {
                            if (item.ToString() == "LISTAINSTRUCCIONES")
                            {
                                foreach (ParseTreeNode items in item.ChildNodes)
                                {
                                    compilarmetodo(items, ambitom);
                                    if (items.ToString() == "RETURN")
                                    {
                                        break;
                                    }


                                }
                            }else if (item.ToString() == "RETURNARREGLO")
                            {
                               
                                foreach (ParseTreeNode nodo in root.ChildNodes[0].ChildNodes)
                                {
                                    if (nodo.ToString().Contains("(id)"))
                                    {
                                        nombreaux = nodo.ToString().Replace(" (id)","");
                                    }
                                    compilarmetodo(nodo,ambitom);
                                    if (nodo.ToString() == "RETURN")
                                    {
                                        nombreaux = "";
                                        break;
                                    }

                                }
                            }
                            
                        }
                        break;
                    }
                case "RETURN":
                    {
                        if (!nombreaux.Equals(""))
                        {
                            string nombreabuscar = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)","");
                                foreach (Simbolo buscar in listasimbolos)
                                {
                                    if (buscar.GetNombre() == nombreabuscar && buscar.GetTipo().Contains("["))
                                    {
                                        foreach (Simbolo colocar in listasimbolos)
                                        {
                                            string aux = buscar.GetTipo().Replace(buscar.GetNombre(), nombreaux);
                                            if (colocar.GetNombre() == nombreaux && colocar.GetTipo() == aux)
                                            {
                                                colocar.SetValor(buscar.GetValor());

                                            colocar.SetNombre(nombreaux+" ");
                                           
                                            
                                            //Console.WriteLine(colocar.Getvis()+","+ colocar.GetAmbito()+","+ colocar.GetNombre()+","+ colocar.GetTipo()+","+ buscar.GetValor());
                                                break;
                                            }
                                        }
                                    }
                                }
                            

                        }
                        object valoreturn = Operacion(root.ChildNodes[0], ambitom);
                        string[] test = ambitom.Split('/');
                        if (!valoreturn.ToString().Contains("Error"))
                        {
                            foreach (Simbolo item in listasimbolos)
                            {
                                if (item.GetAmbito() == ambito2 || item.GetAmbito() == test[0])
                                {
                                    if (item.GetNombre() == test[1])
                                    {
                                        item.SetValor(valoreturn);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("MARCAR ERROR DE MAL VALOR DE RETURN");
                        }


                        break;
                    }
                case "LISTAINSTRUCCIONES":
                    {
                        foreach (ParseTreeNode hijo in root.ChildNodes)
                        {
                            compilarmetodo(hijo, ambitom);
                        }
                        break;
                    }
                case "DECLARACION":
                    {
                        Declarar(root, ambitom);
                        break;
                    }
                case "DARREGLO":
                    {
                        DeclararArreglo(root, ambitom);
                        break;
                    }
                case "ASIGNACIONARRAY":
                    {
                        string nombreacambiar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        string tipoas = root.ChildNodes[1].ToString();
                        foreach (Clase clase in listaclases)
                        {
                            if (clase.getNombre() == ambito2)
                            {
                                foreach (Variable var in clase.GetVariables())
                                {
                                    if (var.getnombre() == nombreacambiar)
                                    {
                                        if (tipoas == "AS1")
                                        {
                                            if (var.getDimen() == 1)
                                            {
                                                int num1 = var.getdim1();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    foreach (Simbolo sim in listasimbolos)
                                                    {
                                                        string contiene = "[" + i + "]";
                                                        if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                        {
                                                            if (sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                            {
                                                                if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num1)
                                                                {
                                                                    object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[i], ambitom);
                                                                    sim.SetValor(valornuevo);
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                }
                                                            }


                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (tipoas == "AS2")
                                        {
                                            if (var.getDimen() == 2)
                                            {
                                                int num1 = var.getdim1();
                                                int num2 = var.getdim2();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    for (int j = 0; j < num2; j++)
                                                    {
                                                        foreach (Simbolo sim in listasimbolos)
                                                        {
                                                            string contiene = "[" + i + "]" + "[" + j + "]";
                                                            if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                            {
                                                                if (sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                                {
                                                                    if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num2 && root.ChildNodes[1].ChildNodes[0].ChildNodes[j].ChildNodes.Count == num1)
                                                                    {
                                                                        object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[j].ChildNodes[i], ambitom);
                                                                        sim.SetValor(valornuevo);
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                        else if (tipoas == "AS3")
                                        {
                                            if (var.getDimen() == 3)
                                            {
                                                int num1 = var.getdim1();
                                                int num2 = var.getdim2();
                                                int num3 = var.getdim3();
                                                for (int i = 0; i < num1; i++)
                                                {
                                                    for (int j = 0; j < num2; j++)
                                                    {
                                                        for (int k = 0; k < num3; k++)
                                                        {
                                                            foreach (Simbolo sim in listasimbolos)
                                                            {
                                                                string contiene = "[" + i + "]" + "[" + j + "]" + "[" + k + "]";
                                                                if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                                {
                                                                    if (sim.GetNombre() == var.getnombre() && sim.GetTipo().Contains(contiene))
                                                                    {
                                                                        if (root.ChildNodes[1].ChildNodes[0].ChildNodes.Count == num1 && root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count == num2 && root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes.Count == num3)
                                                                        {
                                                                            object valornuevo = Operacion(root.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes[k], ambitom);
                                                                            sim.SetValor(valornuevo);
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                        }
                                                                    }

                                                                }
                                                            }
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR LA VARIABLE NO EXISTE EN ESTE AMBITO");
                                    }
                                }
                            }
                        }

                        break;
                    }
                case "ASIGNACION":
                    {
                        switch (root.ChildNodes.Count)
                        {
                            case 2:
                                {
                                    if (root.ChildNodes[0].ToString().Equals("TOARRAY"))
                                    {
                                        object valornuevo = Operacion(root.ChildNodes[1],ambitom);
                                        string nombreabuscar = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                        bool existe = false;
                                        bool dimok = false;
                                        bool ambitok = false;
                                        int num_dimensiones = root.ChildNodes[0].ChildNodes[1].ChildNodes.Count();
                                        if (num_dimensiones == 1)
                                        {
                                            object pos1 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[0], ambitom);
                                            if (pos1.GetType().Equals(typeof(int)))
                                            {
                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                foreach (Simbolo sim in listasimbolos)
                                                {
                                                    string contiene = nombreabuscar + "[" + posicionabuscar + "]";
                                                    if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                    {
                                                        ambitok = true;
                                                        if (sim.GetNombre() == nombreabuscar)
                                                        {
                                                            existe = true;
                                                            if (sim.GetTipo().Equals(contiene))
                                                            {
                                                                dimok = true;
                                                                sim.SetValor(valornuevo);
                                                            }
                                                            
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR UNA DIMENSION DEBE SER UN NUMERO ENTERO");
                                            }
                                            if (!existe)
                                            {
                                                Console.WriteLine("MARCAR ERROR LA VARIABLE A ASIGNAR NO EXISTE");
                                            }
                                            if (!dimok)
                                            {
                                                Console.WriteLine("LA POSICION QUE SE INTENTA ACCEDER NO EXISTE");
                                            }
                                            if (!ambitok)
                                            {
                                                Console.WriteLine("MARCAR ERROR ASIGNANDO UN AMBITO INCORRECTO");
                                            }
                                        }
                                        else if (num_dimensiones == 2)
                                        {
                                            object pos1 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[0], ambitom);
                                            object pos2 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[1],ambitom);
                                            if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)))
                                            {
                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                int posicion2abuscar = int.Parse(pos2.ToString());
                                                foreach (Simbolo sim in listasimbolos)
                                                {
                                                    string contiene = nombreabuscar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]";
                                                    if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                    {
                                                        ambitok = true;
                                                        if (sim.GetNombre() == nombreabuscar)
                                                        {
                                                            existe = true;
                                                            if (sim.GetTipo().Equals(contiene))
                                                            {
                                                                dimok = true;
                                                                sim.SetValor(valornuevo);
                                                            }
                                                            
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR UNA DIMENSION DEBE SER UN NUMERO ENTERO");
                                            }
                                            if (!existe)
                                            {
                                                Console.WriteLine("MARCAR ERROR LA VARIABLE A ASIGNAR NO EXISTE");
                                            }
                                            if (!dimok)
                                            {
                                                Console.WriteLine("LA POSICION QUE SE INTENTA ACCEDER NO EXISTE");
                                            }
                                            if (!ambitok)
                                            {
                                                Console.WriteLine("MARCAR ERROR ASIGNANDO UN AMBITO INCORRECTO");
                                            }
                                        }
                                        else if (num_dimensiones == 3)
                                        {
                                            object pos1 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[0], ambitom);
                                            object pos2 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[1], ambitom);
                                            object pos3 = Operacion(root.ChildNodes[0].ChildNodes[1].ChildNodes[2], ambitom);
                                            if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)) && pos3.GetType().Equals(typeof(int)))
                                            {
                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                int posicion2abuscar = int.Parse(pos2.ToString());
                                                int posicion3abuscar = int.Parse(pos3.ToString());
                                                foreach (Simbolo sim in listasimbolos)
                                                {
                                                    string contiene = nombreabuscar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]" + "[" + posicion3abuscar + "]";
                                                    if (sim.GetAmbito() == ambitom || sim.GetAmbito() == ambito2)
                                                    {
                                                        ambitok = true;
                                                        if (sim.GetNombre() == nombreabuscar )
                                                        {
                                                            existe = true;
                                                            if (sim.GetTipo().Equals(contiene))
                                                            {
                                                                dimok = true;
                                                                sim.SetValor(valornuevo);
                                                            }
                                                            
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR UNA DIMENSION DEBE SER UN NUMERO ENTERO");
                                            }
                                            if (!existe)
                                            {
                                                Console.WriteLine("MARCAR ERROR LA VARIABLE A ASIGNAR NO EXISTE");
                                            }
                                            if (!dimok)
                                            {
                                                Console.WriteLine("LA POSICION QUE SE INTENTA ACCEDER NO EXISTE");
                                            }
                                            if (!ambitok)
                                            {
                                                Console.WriteLine("MARCAR ERROR ASIGNANDO UN AMBITO INCORRECTO");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        String iden = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                        Object valor = Operacion(root.ChildNodes[1], ambitom);
                                        Type vty = valor.GetType();
                                        bool sepuedecambiar = false;
                                        bool existe = false;
                                        bool ambitobien = false;

                                        foreach (Clase clase in listaclases)
                                        {
                                            if (clase.getNombre() == ambito2)
                                            {
                                                foreach (Variable var in clase.GetVariables())
                                                {
                                                    if (var.getnombre() == iden)
                                                    {
                                                        existe = true;
                                                        string tip = var.gettipo();
                                                        if (tip.Equals("int") && vty.Equals(typeof(int)))
                                                        {
                                                            sepuedecambiar = true;
                                                            int nuevo = int.Parse(valor.ToString());
                                                            var.setvi(nuevo);
                                                        }
                                                        else if (tip.Equals("string") && vty.Equals(typeof(string)))
                                                        {
                                                            sepuedecambiar = true;
                                                            var.setvs(valor.ToString());
                                                        }
                                                        else if (tip.Equals("char") && vty.Equals(typeof(char)))
                                                        {
                                                            sepuedecambiar = true;
                                                            char valc = char.Parse(valor.ToString());
                                                            var.setvc(valc);
                                                        }
                                                        else if (tip.Equals("double") && vty.Equals(typeof(double)))
                                                        {
                                                            sepuedecambiar = true;
                                                            double num = double.Parse(valor.ToString());
                                                            var.setvd(num);
                                                        }
                                                        else if (tip.Equals("bool") && vty.Equals(typeof(Boolean)))
                                                        {
                                                            sepuedecambiar = true;
                                                            Boolean vari = Boolean.Parse(valor.ToString());
                                                            var.setvb(vari);
                                                        }
                                                        else
                                                        {
                                                            String lexema = "Asignacion";
                                                            String mensaje = "No se puede asignar un valor si el tipo no coincide o no se ha declarado previamente";
                                                            String tipoerror = "Semantico";
                                                            int lin = 0;
                                                            int col = 0;
                                                            Error nuevo = new Error(lexema, mensaje, tipoerror, lin, col);
                                                            listaerrores.Add(nuevo);
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        if (existe == true && sepuedecambiar == true)
                                        {
                                            foreach (Simbolo sym in listasimbolos)
                                            {
                                                if (sym.GetNombre() == iden)
                                                {
                                                    if (sym.GetAmbito() == ambito2 || sym.GetAmbito() == ambitom)
                                                    {
                                                        ambitobien = true;
                                                        if (ambitobien)
                                                        {
                                                            sym.SetValor(valor);

                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("MARCAR ERROR ASIGNAR EN AMBITO INCORRECTO");
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ASIGNANDO UNA VARIABLE QUE NO EXISTE O NO ES DEL TIPO CORRECTO");
                                        }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (root.ChildNodes[1].ToString().Equals("TOARRAY"))
                                    {
                                        string nombreobjeto = root.ChildNodes[0].ToString().Replace(" (id)","");
                     
                                        string nombrevariableacambiar = root.ChildNodes[1].ChildNodes[0].ToString().Replace(" (id)","");
                                        int num_dimensiones = root.ChildNodes[1].ChildNodes[1].ChildNodes.Count();
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombreobjeto && item.GetTipo().Contains("objeto") && !item.GetTipo().Contains("off"))
                                            {
                                                string[] lista = item.GetTipo().Split('/');
                                                foreach (Simbolo sim2 in listasimbolos)
                                                {
                                                    if (sim2.GetNombre() == nombrevariableacambiar && sim2.GetAmbito() == lista[1])
                                                    {
                                                        object valornuevo = Operacion(root.ChildNodes[2], lista[1]);
                                                        if (num_dimensiones == 1)
                                                        {
                                                            object pos1 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[0], lista[1]);
                                                            
                                                            if (pos1.GetType().Equals(typeof(int)))
                                                            {
                                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                                
                                                                    string contiene = nombrevariableacambiar + "[" + posicionabuscar + "]";
                                                                    
                                                                        if (sim2.GetNombre() == nombrevariableacambiar && sim2.GetTipo().Equals(contiene))
                                                                        {
                                                                            sim2.SetValor(valornuevo);
                                                                        } 
                                                            }
                                                        }
                                                        else if (num_dimensiones == 2)
                                                        {
                                                            object pos1 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[0], lista[1]);
                                                            object pos2 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[1], lista[1]);
                                                            if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)))
                                                            {
                                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                                int posicion2abuscar = int.Parse(pos2.ToString());
                                                                
                                                                    string contiene = nombrevariableacambiar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]";
                                                                    
                                                                        if (sim2.GetNombre() == nombrevariableacambiar && sim2.GetTipo().Equals(contiene))
                                                                        {
                                                                            sim2.SetValor(valornuevo);
                                                                        }
                                                                    

                                                                
                                                            }
                                                        }
                                                        else if (num_dimensiones == 3)
                                                        {
                                                            object pos1 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[0], lista[1]);
                                                            object pos2 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[1], lista[1]);
                                                            object pos3 = Operacion(root.ChildNodes[1].ChildNodes[1].ChildNodes[2], lista[1] );
                                                            if (pos1.GetType().Equals(typeof(int)) && pos2.GetType().Equals(typeof(int)) && pos3.GetType().Equals(typeof(int)))
                                                            {
                                                                int posicionabuscar = int.Parse(pos1.ToString());
                                                                int posicion2abuscar = int.Parse(pos2.ToString());
                                                                int posicion3abuscar = int.Parse(pos3.ToString());
                                                                
                                                                    string contiene = nombrevariableacambiar + "[" + posicionabuscar + "]" + "[" + posicion2abuscar + "]" + "[" + posicion3abuscar + "]";
                                                                    
                                                                        if (sim2.GetNombre() == nombrevariableacambiar && sim2.GetTipo().Equals(contiene))
                                                                        {
                                                                            sim2.SetValor(valornuevo);
                                                                        }
                                                                    

                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string nombreobjeto = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                        string nombrevariablecambiar = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombreobjeto && item.GetTipo().Contains("objeto") && !item.GetTipo().Contains("off"))
                                            {
                                                string[] lista = item.GetTipo().Split('/');
                                                foreach (Simbolo sim2 in listasimbolos)
                                                {
                                                    if (sim2.GetNombre() == nombrevariablecambiar && sim2.GetAmbito() == lista[1])
                                                    {
                                                        object valor = Operacion(root.ChildNodes[2], lista[1]);
                                                        sim2.SetValor(valor);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case "PRINT":
                    {
                        Object val = Operacion(root.ChildNodes[0], ambitom);
                        if (val.ToString() != "Error")
                        {
                            resultado += val.ToString() + "\n";
                        }
                        break;
                    }
                case "OBJETO":
                    {
                        switch (root.ChildNodes.Count)
                        {
                            case 4:
                                {
                                    string nombreclaseabuscar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    string identificador = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    string verificar = root.ChildNodes[3].ToString().Replace(" (id)", "");
                                    bool confirmar_nombre = false;
                                    bool confirmar_clase = false;
                                    foreach (Clase item in listaclases)
                                    {
                                        if (nombreclaseabuscar == item.getNombre())
                                        {
                                            confirmar_clase = true;
                                            if (nombreclaseabuscar == verificar)
                                            {
                                                string ti = "objeto/" + nombreclaseabuscar;
                                                agregarSimbolo("publico", ambitom, identificador, ti, "-----");
                                                confirmar_nombre = true;
                                            }
                                        }
                                    }
                                    if (confirmar_nombre == false)
                                    {
                                        Console.WriteLine("Marcar error mala declaracion");
                                    }
                                    else if (confirmar_clase == false)
                                    {
                                        Console.WriteLine("Marcar error la clase no existe");
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    string nombreclaseabuscar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    string iden = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    bool confirmar = false;
                                    foreach (Clase item in listaclases)
                                    {
                                        if (item.getNombre() == nombreclaseabuscar)
                                        {
                                            string ti = "objeto/" + nombreclaseabuscar + "/off";
                                            agregarSimbolo("publico", ambitom, iden, ti, "-----");
                                            confirmar = true;
                                        }
                                    }
                                    if (confirmar == false)
                                    {
                                        Console.WriteLine("La clse no exixte");
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "SHOW":
                    {
                        Object titulo = Operacion(root.ChildNodes[0], ambitom);
                        Object contenido = Operacion(root.ChildNodes[1], ambitom);
                        if (titulo.ToString() != "Error" && contenido.ToString() != "Error")
                        {
                            MessageBox.Show(contenido.ToString(), titulo.ToString());
                        }
                        break;
                    }
                case "IF":
                    {
                        ambitoaux = ambitom;
                        ambitom = ambitom + "/" + "if";
                        Object valorif = Operacion(root.ChildNodes[0], ambitom);
                        bool condicion = bool.Parse(valorif.ToString());
                        switch (root.ChildNodes.Count)
                        {
                            case 2:
                                if (condicion == true)
                                {
                                    compilarmetodo(root.ChildNodes[1], ambitom);
                                }
                                break;
                            case 3:
                                if (condicion == true)
                                {
                                    compilarmetodo(root.ChildNodes[1], ambitom);
                                }
                                else
                                {
                                    compilarmetodo(root.ChildNodes[2], ambitom);
                                }
                                break;
                            case 4:
                                if (condicion == true)
                                {
                                    compilarmetodo(root.ChildNodes[1], ambitom);
                                }
                                else
                                {
                                    compilarmetodo(root.ChildNodes[2], ambitom);
                                    if (secumple == false)
                                    {
                                        compilarmetodo(root.ChildNodes[3], ambitom);
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                        ambitom = ambitoaux;
                        reset();
                        break;
                    }
                case "LISTAIFELSE":
                    {
                        foreach (ParseTreeNode item in root.ChildNodes)
                        {
                            compilarmetodo(item, ambitom);
                        }
                        break;
                    }
                case "IF ELSE":
                    {
                        object valorifelse = Operacion(root.ChildNodes[0], ambitom);
                        bool condicionifelse = bool.Parse(valorifelse.ToString());
                        if (condicionifelse == true && secumple == false)
                        {
                            compilarmetodo(root.ChildNodes[1], ambitom);
                            secumple = true;
                        }
                        break;
                    }
                case "FOR":
                    {
                        ambitoaux = ambitom;
                        ambitom = ambitom + "/" + "for";
                        for (int x = 0; x < root.ChildNodes.Count - 1; x++)
                        {
                            compilarmetodo(root.ChildNodes[x], ambitom);
                            switch (operador)
                            {
                                case "<":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i < valorf; i++)
                                            {
                                                foreach (Simbolo item in listasimbolos)
                                                {
                                                    if (item.GetNombre() == id && item.GetAmbito() == ambitom || item.GetAmbito() == ambitoaux)
                                                    {
                                                        item.SetValor(i);
                                                    }
                                                }
                                                compilarmetodo(root.ChildNodes[3], ambitom);

                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i < valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ">":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i > valorf; i++)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i > valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ">=":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i >= valorf; i++)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i >= valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case "<=":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i <= valorf; i++)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i <= valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case "==":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i == valorf; i++)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i == valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case "!=":
                                    switch (cambio)
                                    {
                                        case "++":
                                            for (int i = valor; i != valorf; i++)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        case "--":
                                            for (int i = valor; i != valorf; i--)
                                            {
                                                compilarmetodo(root.ChildNodes[3], ambitom);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                        ambitom = ambitoaux;
                        reset();
                        break;
                    }
                case "OPCION":
                    {
                        compilarmetodo(root.ChildNodes[0], ambitom);
                        string next = root.ChildNodes[0].ToString();
                        if (next.Equals("DECLARACION"))
                        {
                            id = root.ChildNodes[0].ChildNodes[1].ChildNodes[0].ToString().Replace(" (id)", "");
                        }
                        else if (next.Equals("ASIGNACION"))
                        {
                            id = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                        }
                        break;
                    }
                case "E":
                    {
                        switch (root.ChildNodes.Count)
                        {
                            case 3:
                                object valorfor = Operacion(root.ChildNodes[0], ambitom);
                                Type t = valorfor.GetType();
                                // int valor = 0;
                                //double valord = 0.0;
                                if (t.Equals(typeof(int)))
                                {
                                    valor = int.Parse(valorfor.ToString());
                                }
                                else if (t.Equals(typeof(double)))
                                {
                                    valord = double.Parse(valorfor.ToString());
                                }
                                operador = root.ChildNodes[1].ToString().Replace(" (Key symbol)", "");
                                object valorfinal = Operacion(root.ChildNodes[2], ambitom);
                                valorf = int.Parse(valorfinal.ToString());
                                break;
                            case 2:
                                cambio = root.ChildNodes[1].ToString().Replace(" (Key symbol)", "");
                                break;
                            default:
                                break;

                        }
                        break;
                    }
                case "REPEAT":
                    {
                        ambitoaux = ambitom;
                        ambitom = ambitom + "/" + "repeat";
                        object valre = Operacion(root.ChildNodes[0], ambitom);
                        int vr = int.Parse(valre.ToString());
                        for (int i = 0; i < vr; i++)
                        {
                            compilarmetodo(root.ChildNodes[1], ambitom);
                        }
                        ambitom = ambitoaux;
                        reset();
                        break;
                    }
                case "CAMBIO":
                    {
                        compilarmetodo(root.ChildNodes[0], ambitom);
                        break;
                    }
                case "AUMENTO":
                    {
                        string ide = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        foreach (Simbolo item in listasimbolos)
                        {
                            if (item.GetNombre() == ide)
                            {
                                if (item.GetTipo() == "int")
                                {
                                    int valorantes = int.Parse(item.GetValor().ToString());
                                    int suma = valorantes + 1;
                                    item.SetValor(suma);
                                }
                                else if (item.GetTipo() == "double")
                                {
                                    double vad = double.Parse(item.GetValor().ToString());
                                    double sd = vad + 1;
                                    item.SetValor(sd);
                                }
                                else if (item.GetTipo() == "char")
                                {
                                    int vac = Char.Parse(item.GetValor().ToString());
                                    int sc = vac + 1;
                                    item.SetValor(sc);
                                }
                            }
                        }
                        break;
                    }
                case "DECREMENTO":
                    {
                        string ided = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        foreach (Simbolo item in listasimbolos)
                        {
                            if (item.GetNombre() == ided)
                            {
                                if (item.GetTipo() == "int")
                                {
                                    int valorantes = int.Parse(item.GetValor().ToString());
                                    int resta = valorantes - 1;
                                    item.SetValor(resta);
                                }
                                else if (item.GetTipo() == "double")
                                {
                                    double vad = double.Parse(item.GetValor().ToString());
                                    double sd = vad - 1;
                                    item.SetValor(sd);
                                }
                                else if (item.GetTipo() == "char")
                                {
                                    int vac = Char.Parse(item.GetValor().ToString());
                                    int sc = vac - 1;
                                    item.SetValor(sc);
                                }
                            }
                        }
                        break;
                    }
                case "WHILE":
                    {
                        ambitoaux = ambitom;
                        ambitom = ambitom + "/" + "while";
                        switch (root.ChildNodes[0].ChildNodes.Count)
                        {
                            case 1:
                                {
                                    if (root.ChildNodes[0].ChildNodes[0].ToString().Contains("(bool)"))
                                    {
                                        object otra = Operacion(root.ChildNodes[0], ambitom);
                                        bool pruebaa = bool.Parse(otra.ToString());
                                        while (pruebaa)
                                        {
                                            compilarmetodo(root.ChildNodes[1], ambitom);
                                        }
                                    }
                                    else
                                    {
                                        object condicionInicial = Operacion(root.ChildNodes[0], ambitom);
                                        bool prueba = bool.Parse(condicionInicial.ToString());
                                        string nombretemp = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (id)", "");
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombretemp)
                                            {
                                                while (item.GetValor().ToString().Equals("True"))
                                                {
                                                    compilarmetodo(root.ChildNodes[1], ambitom);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    bool condicionw = bool.Parse(Operacion(root.ChildNodes[0], ambitom).ToString());
                                    while (condicionw)
                                    {
                                        compilarmetodo(root.ChildNodes[1], ambitom);
                                        condicionw = bool.Parse(Operacion(root.ChildNodes[0], ambitom).ToString());
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        ambitom = ambitoaux;
                        reset();
                        break;
                    }
                case "DOWHILE":
                    {
                        ambitoaux = ambitom;
                        ambitom = ambitom + "/" + "dowhile";
                        compilarmetodo(root.ChildNodes[0], ambitom);
                        switch (root.ChildNodes[1].ChildNodes.Count)
                        {
                            case 1:
                                {
                                    if (root.ChildNodes[1].ChildNodes[0].ToString().Contains("(bool)"))
                                    {
                                        object otra = Operacion(root.ChildNodes[0], ambitom);
                                        bool pruebaa = bool.Parse(otra.ToString());
                                        while (pruebaa)
                                        {
                                            compilarmetodo(root.ChildNodes[0], ambitom);
                                        }
                                    }
                                    else
                                    {
                                        object condicionInicial = Operacion(root.ChildNodes[1], ambitom);
                                        bool prueba = bool.Parse(condicionInicial.ToString());
                                        string nombretemp = root.ChildNodes[1].ChildNodes[0].ToString().Replace(" (id)", "");
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombretemp)
                                            {
                                                while (item.GetValor().ToString().Equals("True"))
                                                {
                                                    compilarmetodo(root.ChildNodes[0], ambitom);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    bool condicionw = bool.Parse(Operacion(root.ChildNodes[1], ambitom).ToString());
                                    while (condicionw)
                                    {
                                        compilarmetodo(root.ChildNodes[0], ambitom);
                                        condicionw = bool.Parse(Operacion(root.ChildNodes[1], ambitom).ToString());
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        ambitom = ambitoaux;
                        reset();
                        break;
                    }
                case "FUNCION":
                    string name = root.ChildNodes[0].ToString().Replace(" (id)", "");
                    string ambitoabuscar = ambito2 + "/" + name;//cambio
                    switch (root.ChildNodes.Count)
                    {
                        case 1:
                            {
                                int nummet = 0;
                                foreach (Clase item in listaclases)
                                {
                                    if (item.getNombre() == ambito2)
                                    {
                                        foreach (Metodo itemm in item.GetMetodos())
                                        {
                                            if (itemm.getNombre() == name)
                                            {
                                                nummet++;
                                            }
                                        }
                                        foreach (Metodo encontrado in item.GetMetodos())
                                        {
                                            if (encontrado.getNombre() == name && encontrado.getTipo() == "void")
                                            {
                                                string test = ambito2 + "/" + name;//cambio
                                                if (nummet == 1)
                                                {

                                                    compilarmetodo(encontrado.getnodo(), test);
                                                }
                                                else
                                                {
                                                    if (encontrado.getover() == true)
                                                    {
                                                        compilarmetodo(encontrado.getnodo(), test);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case 2:
                            {
                                int nummet = 0;
                                if (root.ChildNodes[1].ToString().Equals("LISTAENVIADA"))
                                {
                                    int contadorparametros = 0;
                                    foreach (Clase item in listaclases)
                                    {
                                        if (item.getNombre() == ambito2)
                                        {
                                            foreach (Metodo met in item.GetMetodos())
                                            {
                                                if (met.getNombre() == name)
                                                {
                                                    nummet++;
                                                }
                                            }
                                            foreach (Metodo itemm in item.GetMetodos())
                                            {

                                                if (itemm.getNombre() == name && itemm.getTipo() == "void")
                                                {

                                                    foreach (ParseTreeNode nodo in root.ChildNodes[1].ChildNodes)
                                                    {
                                                        object valortmp = Operacion(nodo, ambitom);
                                                        Type tipotmp = valortmp.GetType();

                                                        string tipoabuscar = "";
                                                        if (tipotmp.Equals(typeof(int)))
                                                        {
                                                            tipoabuscar = "int/parametro";
                                                            foreach (Simbolo items in listasimbolos)
                                                            {
                                                                if (items.GetAmbito() == ambitoabuscar && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                {
                                                                    items.SetValor(valortmp);
                                                                    if (contadorparametros < itemm.getParams().Count - 1)
                                                                    {
                                                                        contadorparametros++;
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        else if (tipotmp.Equals(typeof(string)))
                                                        {
                                                            tipoabuscar = "string/parametro";
                                                            foreach (Simbolo items in listasimbolos)
                                                            {
                                                                if (items.GetAmbito() == ambitoabuscar && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                {
                                                                    items.SetValor(valortmp);
                                                                    if (contadorparametros < itemm.getParams().Count - 1)
                                                                    {
                                                                        contadorparametros++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (tipotmp.Equals(typeof(char)))
                                                        {
                                                            tipoabuscar = "char/parametro";
                                                            foreach (Simbolo items in listasimbolos)
                                                            {
                                                                if (items.GetAmbito() == ambitoabuscar && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                {
                                                                    items.SetValor(valortmp);
                                                                    if (contadorparametros < itemm.getParams().Count - 1)
                                                                    {
                                                                        contadorparametros++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (tipotmp.Equals(typeof(bool)))
                                                        {
                                                            tipoabuscar = "bool/parametro";
                                                            foreach (Simbolo items in listasimbolos)
                                                            {
                                                                if (items.GetAmbito() == ambitoabuscar && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                {
                                                                    items.SetValor(valortmp);
                                                                    if (contadorparametros < itemm.getParams().Count - 1)
                                                                    {
                                                                        contadorparametros++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (tipotmp.Equals(typeof(double)))
                                                        {
                                                            tipoabuscar = "double/parametro";
                                                            foreach (Simbolo items in listasimbolos)
                                                            {
                                                                if (items.GetAmbito() == ambitoabuscar && items.GetTipo() == tipoabuscar && items.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                {
                                                                    items.SetValor(valortmp);
                                                                    if (contadorparametros < itemm.getParams().Count - 1)
                                                                    {
                                                                        contadorparametros++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (nummet == 1)
                                                    {
                                                        compilarmetodo(itemm.getnodo(), ambitoabuscar);
                                                    }
                                                    else
                                                    {
                                                        if (itemm.getover() == true)
                                                        {
                                                            compilarmetodo(itemm.getnodo(), ambitoabuscar);
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    int nummeto = 0;
                                    string nombreobjeto = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                    string nombremetodoabuscar = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                    try
                                    {
                                        foreach (Simbolo item in listasimbolos)
                                        {
                                            if (item.GetNombre() == nombreobjeto && item.GetTipo().Contains("objeto"))
                                            {
                                                string[] lista = item.GetTipo().Split('/');
                                                foreach (Clase bclase in listaclases)
                                                {
                                                    if (bclase.getNombre() == lista[1])
                                                    {
                                                        foreach (Metodo extra in bclase.GetMetodos())
                                                        {
                                                            if (extra.getNombre() == nombremetodoabuscar)
                                                            {
                                                                nummeto++;
                                                            }
                                                        }
                                                        foreach (Metodo bme in bclase.GetMetodos())
                                                        {
                                                            if (bme.getNombre() == nombremetodoabuscar && bme.getTipo() == "void")
                                                            {

                                                                string tmp = ambitoaux;
                                                                ambitoaux = bclase.getNombre();
                                                                string test = bclase.getNombre() + "/" + nombremetodoabuscar;
                                                                if (nummeto == 1 && bme.getvisi() == "publico")
                                                                {
                                                                    compilarmetodo(bme.getnodo(), test);
                                                                }
                                                                else
                                                                {
                                                                    if (bme.getover() == true && bme.getvisi() == "publico")
                                                                    {
                                                                        compilarmetodo(bme.getnodo(), test);
                                                                    }
                                                                }
                                                                ambitoaux = tmp;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Console.WriteLine("ERROR FATAL AQUI");
                                    }

                                }
                                break;
                            }
                        case 3:
                            {
                                int nummet = 0;
                                string nombreobjeto = root.ChildNodes[0].ToString().Replace(" (id)", "");
                                string nombremetodobuscar = root.ChildNodes[1].ToString().Replace(" (id)", "");
                                try
                                {
                                    foreach (Simbolo items in listasimbolos)
                                    {
                                        if (items.GetNombre() == nombreobjeto && items.GetTipo().Contains("objeto"))
                                        {
                                            string[] lista = items.GetTipo().Split('/');
                                            int contadorparametros = 0;
                                            foreach (Clase item in listaclases)
                                            {
                                                if (item.getNombre() == lista[1])
                                                {
                                                    string ambitoextra = item.getNombre() + "/" + nombremetodobuscar;
                                                    foreach (Metodo ex in item.GetMetodos())
                                                    {
                                                        if (ex.getNombre() == nombremetodobuscar)
                                                        {
                                                            nummet++;
                                                        }
                                                    }
                                                    foreach (Metodo itemm in item.GetMetodos())
                                                    {
                                                        if (itemm.getNombre() == nombremetodobuscar && itemm.getTipo() == "void")
                                                        {

                                                            foreach (ParseTreeNode nodo in root.ChildNodes[2].ChildNodes)
                                                            {
                                                                object valortmp = Operacion(nodo, ambitom);
                                                                Type tipotmp = valortmp.GetType();

                                                                string tipoabuscar = "";
                                                                if (tipotmp.Equals(typeof(int)))
                                                                {
                                                                    tipoabuscar = "int/parametro";
                                                                    foreach (Simbolo itemss in listasimbolos)
                                                                    {
                                                                        if (itemss.GetAmbito() == ambitoextra && itemss.GetTipo() == tipoabuscar && itemss.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                        {
                                                                            itemss.SetValor(valortmp);
                                                                            if (contadorparametros < itemm.getParams().Count - 1)
                                                                            {
                                                                                contadorparametros++;
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                                else if (tipotmp.Equals(typeof(string)))
                                                                {
                                                                    tipoabuscar = "string/parametro";
                                                                    foreach (Simbolo itemss in listasimbolos)
                                                                    {
                                                                        if (itemss.GetAmbito() == ambitoextra && itemss.GetTipo() == tipoabuscar && itemss.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                        {
                                                                            itemss.SetValor(valortmp);
                                                                            if (contadorparametros < itemm.getParams().Count - 1)
                                                                            {
                                                                                contadorparametros++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else if (tipotmp.Equals(typeof(char)))
                                                                {
                                                                    tipoabuscar = "char/parametro";
                                                                    foreach (Simbolo itemss in listasimbolos)
                                                                    {
                                                                        if (itemss.GetAmbito() == ambitoextra && itemss.GetTipo() == tipoabuscar && itemss.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                        {
                                                                            itemss.SetValor(valortmp);
                                                                            if (contadorparametros < itemm.getParams().Count - 1)
                                                                            {
                                                                                contadorparametros++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else if (tipotmp.Equals(typeof(bool)))
                                                                {
                                                                    tipoabuscar = "bool/parametro";
                                                                    foreach (Simbolo itemss in listasimbolos)
                                                                    {
                                                                        if (itemss.GetAmbito() == ambitoextra && itemss.GetTipo() == tipoabuscar && itemss.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                        {
                                                                            itemss.SetValor(valortmp);
                                                                            if (contadorparametros < itemm.getParams().Count - 1)
                                                                            {
                                                                                contadorparametros++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else if (tipotmp.Equals(typeof(double)))
                                                                {
                                                                    tipoabuscar = "double/parametro";
                                                                    foreach (Simbolo itemss in listasimbolos)
                                                                    {
                                                                        if (itemss.GetAmbito() == ambitoextra && itemss.GetTipo() == tipoabuscar && itemss.GetNombre() == itemm.getParams().ElementAt(contadorparametros).getnombre())
                                                                        {
                                                                            itemss.SetValor(valortmp);
                                                                            if (contadorparametros < itemm.getParams().Count - 1)
                                                                            {
                                                                                contadorparametros++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (nummet == 1 && itemm.getvisi() == "publico")
                                                            {
                                                                compilarmetodo(itemm.getnodo(), ambitoextra);
                                                            }
                                                            else
                                                            {
                                                                if (itemm.getover() == true && itemm.getvisi() == "publico")
                                                                {
                                                                    compilarmetodo(itemm.getnodo(), ambitoextra);
                                                                }
                                                            }

                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("EROR FALTAL 2 AQUI");
                                }
                                break;
                            }

                        default:
                            break;
                    }
                    break;
                case "MROBJECT":
                    {
                        int numeto = 0;
                        string nombreclaseabuscar = root.ChildNodes[0].ToString().Replace(" (id)", "");
                        string nombreobjeto = root.ChildNodes[1].ToString().Replace(" (id)", "");
                        string metodoallamar = root.ChildNodes[2].ToString().Replace(" (id)", "");
                        bool validar = false;
                        bool valm = false;
                        string[] prueba = ambitom.Split('/');
                        foreach (Clase item in listaclases)
                        {
                            if (item.getNombre() == nombreclaseabuscar)
                            {
                                validar = true;
                            }
                            if (item.getNombre() == prueba[0])
                            {
                                foreach (Metodo ex in item.GetMetodos())
                                {
                                    if (ex.getNombre() == metodoallamar)
                                    {
                                        numeto++;
                                    }
                                }
                                foreach (Metodo r in item.GetMetodos())
                                {
                                    if (r.getNombre() == metodoallamar)
                                    {
                                        if (numeto == 1)
                                        {
                                            compilarmetodo(r.getnodo(), ambitom);
                                        }
                                        else
                                        {
                                            if (r.getover() == true)
                                            {
                                                compilarmetodo(r.getnodo(), ambitom);
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Metodo sim in item.GetMetodos())
                            {
                                if (sim.getNombre() == metodoallamar)
                                {
                                    valm = true;
                                }
                            }
                        }
                        if (validar == true && valm == true)
                        {
                            agregarSimbolo("publico", ambitom, nombreobjeto, "objeto/" + nombreclaseabuscar, "-----");
                        }

                        break;
                    }
            }
        }

        //METODO PARA DEVOLVER LAS VARIABLES A SU VALOR INICIAL
        public static void reset()
        {
            valor = 0;
            valord = 0.0;
            operador = "";
            valorf = 0;
            cambio = "";
            nummain = 0;
            secumple = false;
            ambitoaux = "";
        }

        //METODO PARA DECLARAR UN ARREGLO
        public static void DeclararArreglo(ParseTreeNode root, string ambito)
        {
            switch (root.ChildNodes.Count)
            {
                case 3: // TIPO LID DIM
                    {
                        string tipo = root.ChildNodes[0].ToString().Replace(" (Keyword)", "");
                        int dimensiones = 0;
                        List<string> nombres = new List<string>();
                        switch (root.ChildNodes[2].ChildNodes.Count)
                        {
                            case 1:
                                {
                                    int primera = 0;
                                    dimensiones = 1;
                                    object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                    Type t = dim1.GetType();
                                    if (t.Equals(typeof(int)))
                                    {
                                        primera = int.Parse(dim1.ToString());
                                        foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                        {
                                            string nombre = item.ToString().Replace(" (id)", "");
                                            nombres.Add(nombre);
                                        }

                                        foreach (string nueva in nombres)
                                        {
                                            Variable var = new Variable("publico", nueva, tipo, dimensiones, primera);
                                            listavar.Add(var);
                                            agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                            for (int i = 0; i < primera; i++)
                                            {
                                                agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]", "----");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                    }

                                    break;
                                }
                            case 2:
                                {
                                    dimensiones = 2;
                                    int primera = 0;
                                    int segunda = 0;
                                    object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                    Type t = dim1.GetType();
                                    object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                    Type t2 = dim2.GetType();
                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                    {
                                        primera = int.Parse(dim1.ToString());
                                        segunda = int.Parse(dim2.ToString());
                                        foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                        {
                                            string nombre = item.ToString().Replace(" (id)", "");
                                            nombres.Add(nombre);
                                        }
                                        foreach (string nueva in nombres)
                                        {
                                            Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda);
                                            listavar.Add(var);
                                            agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                            for (int i = 0; i < primera; i++)
                                            {
                                                for (int j = 0; j < segunda; j++)
                                                {
                                                    agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    dimensiones = 3;
                                    int primera = 0;
                                    int segunda = 0;
                                    int tercera = 0;
                                    object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                    Type t = dim1.GetType();
                                    object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                    Type t2 = dim2.GetType();
                                    object dim3 = Operacion(root.ChildNodes[2].ChildNodes[2], ambito);
                                    Type t3 = dim3.GetType();
                                    if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                    {
                                        primera = int.Parse(dim1.ToString());
                                        segunda = int.Parse(dim2.ToString());
                                        tercera = int.Parse(dim3.ToString());
                                        foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                        {
                                            string nombre = item.ToString().Replace(" (id)", "");
                                            nombres.Add(nombre);
                                        }
                                        foreach (string nueva in nombres)
                                        {
                                            Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda, tercera);
                                            listavar.Add(var);
                                            agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                            for (int i = 0; i < primera; i++)
                                            {
                                                for (int j = 0; j < segunda; j++)
                                                {
                                                    for (int k = 0; k < tercera; k++)
                                                    {
                                                        agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                    }

                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case 4:
                    {
                        //VISI TIPO LID DIM
                        if (root.ChildNodes[0].ToString().Equals("VISIBILIDAD"))
                        {
                            string visis = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                            string tipo = root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                            int dimensiones = 0;
                            List<string> nombres = new List<string>();
                            switch (root.ChildNodes[3].ChildNodes.Count)
                            {
                                case 1:
                                    {
                                        int primera = 0;
                                        dimensiones = 1;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        if (t.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }

                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]", "----");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }

                                        break;
                                    }
                                case 2:
                                    {
                                        dimensiones = 2;
                                        int primera = 0;
                                        int segunda = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }


                                        break;
                                    }
                                case 3:
                                    {
                                        dimensiones = 3;
                                        int primera = 0;
                                        int segunda = 0;
                                        int tercera = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        object dim3 = Operacion(root.ChildNodes[3].ChildNodes[2], ambito);
                                        Type t3 = dim3.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            tercera = int.Parse(dim3.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda, tercera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        for (int k = 0; k < tercera; k++)
                                                        {
                                                            agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        // TIPO LD DIM ASIG
                        else
                        {
                            string tipo = root.ChildNodes[0].ToString().Replace(" (Keyword)", "");
                            List<string> nombres = new List<string>();
                            int dimensiones = 0;
                            if (root.ChildNodes[3].ToString().Equals("E"))
                            {

                            
                                switch (root.ChildNodes[2].ChildNodes.Count)
                                {
                                    case 1:
                                        {
                                            int primera = 0;
                                            dimensiones = 1;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            if (t.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]", "----");
                                                    }
                                                }
                                            }
                                                    break;
                                        }
                                    case 2:
                                        {
                                            dimensiones = 2;
                                            int primera = 0;
                                            int segunda = 0;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                            Type t2 = dim2.GetType();
                                            if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                segunda = int.Parse(dim2.ToString());
                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        for (int j = 0; j < segunda; j++)
                                                        {
                                                            agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                        }
                                                    }
                                                }
                                            }
                                                    break;
                                        }
                                    case 3:
                                        {
                                            dimensiones = 3;
                                            int primera = 0;
                                            int segunda = 0;
                                            int tercera = 0;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                            Type t2 = dim2.GetType();
                                            object dim3 = Operacion(root.ChildNodes[2].ChildNodes[2], ambito);
                                            Type t3 = dim3.GetType();
                                            if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                segunda = int.Parse(dim2.ToString());
                                                tercera = int.Parse(dim3.ToString());

                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda, tercera);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        for (int j = 0; j < segunda; j++)
                                                        {
                                                            for (int k = 0; k < tercera; k++)
                                                            {
                                                                agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                                    break;
                                        }
                                }
                                string re = ambitoaux;
                                ambitoaux = "buscandoarreglo";
                                object nombreparacambiar = Operacion(root.ChildNodes[3], ambito);
                                foreach (string nom in nombres)
                                {
                                    foreach (Simbolo buscar in listasimbolos)
                                    {
                                        if (buscar.GetNombre().Replace(" ","") == nombreparacambiar.ToString() && buscar.GetTipo().Contains("["))
                                        {
                                            buscar.SetNombre(buscar.GetNombre().Replace(" ", ""));
                                            foreach (Simbolo colocar in listasimbolos)
                                            {
                                                string aux = buscar.GetTipo().Replace(buscar.GetNombre(), nom);
                                                if (colocar.GetNombre() == nom && colocar.GetTipo() == aux)
                                                {
                                                    colocar.SetValor(buscar.GetValor());

                                                    //colocar.SetNombre(nombreaux + " ");


                                                    //Console.WriteLine(colocar.Getvis()+","+ colocar.GetAmbito()+","+ colocar.GetNombre()+","+ colocar.GetTipo()+","+ buscar.GetValor());
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                

                            }
                            else
                            {
                                switch (root.ChildNodes[2].ChildNodes.Count)
                                {
                                    case 1:
                                        {
                                            int primera = 0;
                                            dimensiones = 1;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            if (t.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]", "----");
                                                    }

                                                    //asignando
                                                    foreach (Variable variable in listavar)
                                                    {
                                                        if (variable.getnombre() == nueva)
                                                        {
                                                            int num1 = variable.getdim1();
                                                            for (int i = 0; i < num1; i++)
                                                            {
                                                                foreach (Simbolo sim in listasimbolos)
                                                                {
                                                                    string contiene = "[" + i + "]";
                                                                    if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                    {
                                                                        if (root.ChildNodes[3].ChildNodes[0].ChildNodes.Count == num1)
                                                                        {
                                                                            object valornuevo = Operacion(root.ChildNodes[3].ChildNodes[0].ChildNodes[i], ambito);
                                                                            sim.SetValor(valornuevo);
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                        }

                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }

                                                    //asignando
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            dimensiones = 2;
                                            int primera = 0;
                                            int segunda = 0;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                            Type t2 = dim2.GetType();
                                            if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                segunda = int.Parse(dim2.ToString());
                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        for (int j = 0; j < segunda; j++)
                                                        {
                                                            agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                        }
                                                    }
                                                    //asignando
                                                    foreach (Variable variable in listavar)
                                                    {
                                                        if (variable.getnombre() == nueva)
                                                        {
                                                            int num1 = variable.getdim1();
                                                            int num2 = variable.getdim2();
                                                            for (int i = 0; i < num1; i++)
                                                            {
                                                                for (int j = 0; j < num2; j++)
                                                                {
                                                                    foreach (Simbolo sim in listasimbolos)
                                                                    {
                                                                        string contiene = "[" + i + "]" + "[" + j + "]";
                                                                        if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                        {
                                                                            if (root.ChildNodes[3].ChildNodes[0].ChildNodes.Count == num2 && root.ChildNodes[3].ChildNodes[0].ChildNodes[j].ChildNodes.Count == num1)
                                                                            {
                                                                                object valornuevo = Operacion(root.ChildNodes[3].ChildNodes[0].ChildNodes[j].ChildNodes[i], ambito);
                                                                                sim.SetValor(valornuevo);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                            }

                                                                        }
                                                                    }
                                                                }

                                                            }

                                                        }
                                                    }

                                                    //asignando
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            dimensiones = 3;
                                            int primera = 0;
                                            int segunda = 0;
                                            int tercera = 0;
                                            object dim1 = Operacion(root.ChildNodes[2].ChildNodes[0], ambito);
                                            Type t = dim1.GetType();
                                            object dim2 = Operacion(root.ChildNodes[2].ChildNodes[1], ambito);
                                            Type t2 = dim2.GetType();
                                            object dim3 = Operacion(root.ChildNodes[2].ChildNodes[2], ambito);
                                            Type t3 = dim3.GetType();
                                            if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                            {
                                                primera = int.Parse(dim1.ToString());
                                                segunda = int.Parse(dim2.ToString());
                                                tercera = int.Parse(dim3.ToString());

                                                foreach (ParseTreeNode item in root.ChildNodes[1].ChildNodes)
                                                {
                                                    string nombre = item.ToString().Replace(" (id)", "");
                                                    nombres.Add(nombre);
                                                }
                                                foreach (string nueva in nombres)
                                                {
                                                    Variable var = new Variable("publico", nueva, tipo, dimensiones, primera, segunda, tercera);
                                                    listavar.Add(var);
                                                    agregarSimbolo("publico", ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                    for (int i = 0; i < primera; i++)
                                                    {
                                                        for (int j = 0; j < segunda; j++)
                                                        {
                                                            for (int k = 0; k < tercera; k++)
                                                            {
                                                                agregarSimbolo("publico", ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                            }
                                                        }
                                                    }
                                                    //asignando
                                                    foreach (Variable variable in listavar)
                                                    {
                                                        if (variable.getnombre() == nueva)
                                                        {
                                                            int num1 = variable.getdim1();
                                                            int num2 = variable.getdim2();
                                                            int num3 = variable.getdim3();
                                                            for (int i = 0; i < num1; i++)
                                                            {
                                                                for (int j = 0; j < num2; j++)
                                                                {
                                                                    for (int k = 0; k < num3; k++)
                                                                    {
                                                                        foreach (Simbolo sim in listasimbolos)
                                                                        {
                                                                            string contiene = "[" + i + "]" + "[" + j + "]" + "[" + k + "]";
                                                                            if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                            {
                                                                                if (root.ChildNodes[3].ChildNodes[0].ChildNodes.Count == num1 && root.ChildNodes[3].ChildNodes[0].ChildNodes[i].ChildNodes.Count == num2 && root.ChildNodes[3].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes.Count == num3)
                                                                                {
                                                                                    object valornuevo = Operacion(root.ChildNodes[3].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes[k], ambito);
                                                                                    sim.SetValor(valornuevo);
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                                }

                                                                            }
                                                                        }
                                                                    }

                                                                }

                                                            }

                                                        }
                                                    }

                                                    //asignando
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                            }
                                        }

                                        break;
                                }

                            }


                        }
                        break;
                    }
                case 5://   VISI TIPO LID DIM ASIG
                    {
                        string visis = root.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                        string tipo = root.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                        int dimensiones = 0;
                        List<string> nombres = new List<string>();
                        if (root.ChildNodes[4].ToString().Equals("E"))
                        {
                            switch (root.ChildNodes[3].ChildNodes.Count)
                            {
                                case 1:
                                    {
                                        int primera = 0;
                                        dimensiones = 1;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        if (t.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }

                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]", "----");
                                                }
                                            }
                                        }
                                                break;
                                    }
                                case 2:
                                    {
                                        dimensiones = 2;
                                        int primera = 0;
                                        int segunda = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                    }
                                                }
                                            }
                                        }
                                                break;
                                    }
                                case 3:
                                    {
                                        dimensiones = 3;
                                        int primera = 0;
                                        int segunda = 0;
                                        int tercera = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        object dim3 = Operacion(root.ChildNodes[3].ChildNodes[2], ambito);
                                        Type t3 = dim3.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            tercera = int.Parse(dim3.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda, tercera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        for (int k = 0; k < tercera; k++)
                                                        {
                                                            agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                                break;
                                    }

                            }
                            string re = ambitoaux;
                            ambitoaux = "buscandoarreglo";
                            object nombreparacambiar =  Operacion(root.ChildNodes[4],ambito);
                            foreach (string nom in nombres)
                            {
                                foreach (Simbolo buscar in listasimbolos)
                                {
                                    if (buscar.GetNombre().Replace(" ", "") == nombreparacambiar.ToString() && buscar.GetTipo().Contains("["))
                                    {
                                        buscar.SetNombre(buscar.GetNombre().Replace(" ", ""));
                                        foreach (Simbolo colocar in listasimbolos)
                                        {
                                            string aux = buscar.GetTipo().Replace(buscar.GetNombre(), nom);
                                            if (colocar.GetNombre() == nom && colocar.GetTipo() == aux)
                                            {
                                                colocar.SetValor(buscar.GetValor());

                                                //colocar.SetNombre(nombreaux + " ");


                                                //Console.WriteLine(colocar.Getvis()+","+ colocar.GetAmbito()+","+ colocar.GetNombre()+","+ colocar.GetTipo()+","+ buscar.GetValor());
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            ambitoaux = re;
                        }
                        else
                        {
                            switch (root.ChildNodes[3].ChildNodes.Count)
                            {
                                case 1:
                                    {
                                        int primera = 0;
                                        dimensiones = 1;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        if (t.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }

                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]", "----");
                                                }
                                                //asignando
                                                foreach (Variable variable in listavar)
                                                {
                                                    if (variable.getnombre() == nueva)
                                                    {
                                                        int num1 = variable.getdim1();
                                                        for (int i = 0; i < num1; i++)
                                                        {
                                                            foreach (Simbolo sim in listasimbolos)
                                                            {
                                                                string contiene = "[" + i + "]";
                                                                if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                {
                                                                    if (root.ChildNodes[4].ChildNodes[0].ChildNodes.Count == num1)
                                                                    {
                                                                        object valornuevo = Operacion(root.ChildNodes[4].ChildNodes[0].ChildNodes[i], ambito);
                                                                        sim.SetValor(valornuevo);
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                    }

                                                                }
                                                            }
                                                        }

                                                    }
                                                }

                                                //asignando
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }

                                        break;
                                    }
                                case 2:
                                    {
                                        dimensiones = 2;
                                        int primera = 0;
                                        int segunda = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]", "----");
                                                    }
                                                }
                                                //asignando
                                                foreach (Variable variable in listavar)
                                                {
                                                    if (variable.getnombre() == nueva)
                                                    {
                                                        int num1 = variable.getdim1();
                                                        int num2 = variable.getdim2();
                                                        for (int i = 0; i < num1; i++)
                                                        {
                                                            for (int j = 0; j < num2; j++)
                                                            {
                                                                foreach (Simbolo sim in listasimbolos)
                                                                {
                                                                    string contiene = "[" + i + "]" + "[" + j + "]";
                                                                    if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                    {
                                                                        if (root.ChildNodes[4].ChildNodes[0].ChildNodes.Count == num2 && root.ChildNodes[4].ChildNodes[0].ChildNodes[j].ChildNodes.Count == num1)
                                                                        {
                                                                            object valornuevo = Operacion(root.ChildNodes[4].ChildNodes[0].ChildNodes[j].ChildNodes[i], ambito);
                                                                            sim.SetValor(valornuevo);
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                        }

                                                                    }
                                                                }
                                                            }

                                                        }

                                                    }
                                                }

                                                //asignando
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }


                                        break;
                                    }
                                case 3:
                                    {
                                        dimensiones = 3;
                                        int primera = 0;
                                        int segunda = 0;
                                        int tercera = 0;
                                        object dim1 = Operacion(root.ChildNodes[3].ChildNodes[0], ambito);
                                        Type t = dim1.GetType();
                                        object dim2 = Operacion(root.ChildNodes[3].ChildNodes[1], ambito);
                                        Type t2 = dim2.GetType();
                                        object dim3 = Operacion(root.ChildNodes[3].ChildNodes[2], ambito);
                                        Type t3 = dim3.GetType();
                                        if (t.Equals(typeof(int)) && t2.Equals(typeof(int)) && t3.Equals(typeof(int)))
                                        {
                                            primera = int.Parse(dim1.ToString());
                                            segunda = int.Parse(dim2.ToString());
                                            tercera = int.Parse(dim3.ToString());
                                            foreach (ParseTreeNode item in root.ChildNodes[2].ChildNodes)
                                            {
                                                string nombre = item.ToString().Replace(" (id)", "");
                                                nombres.Add(nombre);
                                            }
                                            foreach (string nueva in nombres)
                                            {
                                                Variable var = new Variable(visis, nueva, tipo, dimensiones, primera, segunda, tercera);
                                                listavar.Add(var);
                                                agregarSimbolo(visis, ambito, nueva, tipo + "/arreglo" + "/" + dimensiones, "------");
                                                for (int i = 0; i < primera; i++)
                                                {
                                                    for (int j = 0; j < segunda; j++)
                                                    {
                                                        for (int k = 0; k < tercera; k++)
                                                        {
                                                            agregarSimbolo(visis, ambito, nueva, nueva + "[" + i + "]" + "[" + j + "]" + "[" + k + "]", "----");
                                                        }
                                                    }
                                                }
                                                //asignando
                                                foreach (Variable variable in listavar)
                                                {
                                                    if (variable.getnombre() == nueva)
                                                    {
                                                        int num1 = variable.getdim1();
                                                        int num2 = variable.getdim2();
                                                        int num3 = variable.getdim3();
                                                        for (int i = 0; i < num1; i++)
                                                        {
                                                            for (int j = 0; j < num2; j++)
                                                            {
                                                                for (int k = 0; k < num3; k++)
                                                                {
                                                                    foreach (Simbolo sim in listasimbolos)
                                                                    {
                                                                        string contiene = "[" + i + "]" + "[" + j + "]" + "[" + k + "]";
                                                                        if (sim.GetAmbito() == ambito && sim.GetNombre() == nueva && sim.GetTipo().Contains(contiene))
                                                                        {
                                                                            if (root.ChildNodes[4].ChildNodes[0].ChildNodes.Count == num1 && root.ChildNodes[4].ChildNodes[0].ChildNodes[i].ChildNodes.Count == num2 && root.ChildNodes[4].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes.Count == num3)
                                                                            {
                                                                                object valornuevo = Operacion(root.ChildNodes[4].ChildNodes[0].ChildNodes[i].ChildNodes[j].ChildNodes[k], ambito);
                                                                                sim.SetValor(valornuevo);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("MARCARERRROR SE INTENTO ASIGNAR MAS VARIABLES QUE EL ARREGLO");
                                                                            }

                                                                        }
                                                                    }
                                                                }

                                                            }

                                                        }

                                                    }
                                                }

                                                //asignando
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("MARCAR ERROR ADENTRO DE UNA DIMENSION SOLO PUEDEN VENIR NUMERO ENTEROS");
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }

                        }


                        break;
                    }
            }
        }

        //METODO PARA DECLARAR UNA VARIABLE
        public static void Declarar(ParseTreeNode nodo, string ambito)
        {
            //COMPONENTES DE LA VARIVABLE
            String VISIVILIDAD = "publico";
            String NOMBRE = "";
            String VALORSTRING = "";
            String TIPO = "";
            int VINT = 0;
            double VDOUBLE = 0.0;
            char VCHAR = ' ';
            Boolean VBOOL = false;
            object valorGeneral = "-----";
            string revisar = "";

            // DECLARACION-> VISIVILIDADBILIDAD TIPO IDES VALOR
            if (nodo.ChildNodes.Count == 4)
            {
                VISIVILIDAD = nodo.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                TIPO = nodo.ChildNodes[1].ToString().Replace(" (Keyword)", "");
                valorGeneral = Operacion(nodo.ChildNodes[3], ambito);
                Type t = valorGeneral.GetType();

                //VERIFICAR TIPO
                if (t.Equals(typeof(int)) && TIPO.Equals("int"))
                {
                    VINT = int.Parse(valorGeneral.ToString());
                    revisar = "int";
                }
                else if (t.Equals(typeof(int)) && TIPO.Equals("bool"))
                {
                    VINT = int.Parse(valorGeneral.ToString());
                    if (VINT == 1)
                    {
                        VBOOL = true;
                        revisar = "bool";

                    }
                    else if (VINT == 0)
                    {
                        VBOOL = false;
                        revisar = "bool";
                    }
                }
                else if (t.Equals(typeof(String)) && TIPO.Equals("string") && valorGeneral.ToString() != "Error")
                {
                    VALORSTRING = valorGeneral.ToString();
                    revisar = "string";
                }
                else if (t.Equals(typeof(double)) && TIPO.Equals("double"))
                {
                    VDOUBLE = Double.Parse(valorGeneral.ToString());
                    revisar = "double";
                }
                else if (t.Equals(typeof(char)) && TIPO.Equals("char"))
                {
                    VCHAR = Char.Parse(valorGeneral.ToString());
                    revisar = "char";
                }
                else if (t.Equals(typeof(double)) && TIPO.Equals("int"))
                {
                    double ex = double.Parse(valorGeneral.ToString());
                    VINT = Convert.ToInt32(ex);
                    revisar = "int";
                }
                else if (t.Equals(typeof(Boolean)) && TIPO.Equals("bool"))
                {
                    VBOOL = Boolean.Parse(valorGeneral.ToString());
                    revisar = "bool";
                }
                else if (t.Equals(typeof(bool)) && TIPO.Equals("int"))
                {
                    VBOOL = bool.Parse(valorGeneral.ToString());
                    if (VBOOL == true)
                    {
                        VINT = 1;
                    }
                    else
                    {
                        VINT = 0;
                    }
                    revisar = "int";
                }
                else if (valorGeneral.ToString().Equals("Error"))
                {
                    String lexema = "operador";
                    String mensaje = "Los tipos al realizar la operacion son invalidos";
                    String tipoerror = "Semantico";
                    int lin = 0;
                    int col = 0;
                    Error nuevo = new Error(lexema, mensaje, tipoerror, lin, col);
                    listaerrores.Add(nuevo);
                }

                //CREAR LAS VARIABLES POR ID
                if (!valorGeneral.ToString().Contains("Error"))
                {
                    if (revisar != "")
                    {
                        for (int i = 0; i < nodo.ChildNodes[2].ChildNodes.Count; i++)
                        {
                            NOMBRE = nodo.ChildNodes[2].ChildNodes[i].ToString().Replace(" (id)", "");
                            Variable nueva = new Variable(VISIVILIDAD, NOMBRE, TIPO, VALORSTRING, VINT, VDOUBLE, VCHAR, VBOOL);
                            listavar.Add(nueva);
                            switch (TIPO)
                            {
                                case "int":
                                    if (revisar.Equals("int"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VINT);
                                    }
                                    break;
                                case "string":
                                    if (revisar.Equals("string"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VALORSTRING);
                                    }
                                    break;
                                case "double":
                                    if (revisar.Equals("double"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VDOUBLE);
                                    }
                                    break;
                                case "char":
                                    if (revisar.Equals("char"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VCHAR);
                                    }
                                    break;
                                case "bool":
                                    if (revisar.Equals("bool"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VBOOL);

                                    }
                                    break;
                                default:
                                    Console.WriteLine("TIPO INCORRECTO");
                                    break;
                            } 
                    }
                    }
                    else
                    {
                        Console.WriteLine("MARCAR ERROR LOS TIPO NO COINCIDEN AL DECLARAR");

                    }

                }
                //DECLARACION-> VISIBILIDAD TIPO IDES
            }
            //VISIBILIDAD TIPO IDES
            else if (nodo.ChildNodes.Count == 3 && nodo.ChildNodes[0].ToString().Equals("VISIBILIDAD"))
            {
                VISIVILIDAD = nodo.ChildNodes[0].ChildNodes[0].ToString().Replace(" (Keyword)", "");
                TIPO = nodo.ChildNodes[1].ToString().Replace(" (Keyword)", "");

                //CREAR LAS VARIABLES POR ID
                for (int i = 0; i < nodo.ChildNodes[2].ChildNodes.Count; i++)
                {
                    NOMBRE = nodo.ChildNodes[2].ChildNodes[i].ToString().Replace(" (id)", "");
                    Variable nueva = new Variable(VISIVILIDAD, NOMBRE, TIPO, VALORSTRING, VINT, VDOUBLE, VCHAR, VBOOL);
                    listavar.Add(nueva);
                    agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, valorGeneral);
                }

            }
            //TIPO IDES
            else if (nodo.ChildNodes.Count == 2)
            {

                TIPO = nodo.ChildNodes[0].ToString().Replace(" (Keyword)", "");

                //CREAR LAS VARIABLES POR ID
                for (int i = 0; i < nodo.ChildNodes[1].ChildNodes.Count; i++)
                {
                    NOMBRE = nodo.ChildNodes[1].ChildNodes[i].ToString().Replace(" (id)", "");
                    Variable nueva = new Variable(VISIVILIDAD, NOMBRE, TIPO, VALORSTRING, VINT, VDOUBLE, VCHAR, VBOOL);
                    listavar.Add(nueva);
                    agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, valorGeneral);
                }

            }
            // TIPO IDES VALOR
            else if (nodo.ChildNodes.Count == 3)
            {
                TIPO = nodo.ChildNodes[0].ToString().Replace(" (Keyword)", "");
                valorGeneral = Operacion(nodo.ChildNodes[2], ambito);
                Type t = valorGeneral.GetType();

                //VERIFICAR TIPO
                if (t.Equals(typeof(int)) && TIPO.Equals("int"))
                {
                    VINT = int.Parse(valorGeneral.ToString());
                    revisar = "int";
                }
                else if(t.Equals(typeof(int)) && TIPO.Equals("bool"))
                {
                    VINT = int.Parse(valorGeneral.ToString());
                    if (VINT == 1)
                    {
                        VBOOL = true;
                        revisar = "bool";

                    }else if (VINT == 0)
                    {
                        VBOOL = false;
                        revisar = "bool";
                    }
                }
                else if (t.Equals(typeof(string)) && TIPO.Equals("string") && valorGeneral.ToString() != "Error")
                {
                    VALORSTRING = valorGeneral.ToString();
                    revisar = "string";
                }
                else if (t.Equals(typeof(double)) && TIPO.Equals("double"))
                {
                    VDOUBLE = double.Parse(valorGeneral.ToString());
                    revisar = "double";
                }
                else if (t.Equals(typeof(char)) && TIPO.Equals("char"))
                {
                    VCHAR = char.Parse(valorGeneral.ToString());
                    revisar = "char";
                }
                else if (t.Equals(typeof(double)) && TIPO.Equals("int"))
                {
                    double ex = double.Parse(valorGeneral.ToString());
                    VINT = Convert.ToInt32(ex);
                    revisar = "int";
                }
                else if (t.Equals(typeof(Boolean)) && TIPO.Equals("bool"))
                {
                    VBOOL = Boolean.Parse(valorGeneral.ToString());
                    revisar = "bool";
                }else if (t.Equals(typeof(bool)) && TIPO.Equals("int"))
                {
                    VBOOL = bool.Parse(valorGeneral.ToString());
                    if (VBOOL == true)
                    {
                        VINT = 1;
                    }
                    else
                    {
                        VINT = 0;
                    }
                    revisar = "int";
                }
                else if (valorGeneral.ToString().Equals("Error"))
                {
                    String lexema = "operador";
                    String mensaje = "Los tipos al realizar la operacion son invalidos";
                    String tipoerror = "Semantico";
                    int lin = 0;
                    int col = 0;
                    Error nuevo = new Error(lexema, mensaje, tipoerror, lin, col);
                    listaerrores.Add(nuevo);
                }
                //CREAR LAS VARIABLES POR ID
                if (!valorGeneral.ToString().Contains("Error"))
                {
                    if (revisar != "")
                    {
                        for (int i = 0; i < nodo.ChildNodes[1].ChildNodes.Count; i++)
                        {
                            NOMBRE = nodo.ChildNodes[1].ChildNodes[i].ToString().Replace(" (id)", "");
                            Variable nueva = new Variable(VISIVILIDAD, NOMBRE, TIPO, VALORSTRING, VINT, VDOUBLE, VCHAR, VBOOL);
                            listavar.Add(nueva);

                            switch (TIPO)
                            {
                                case "int":
                                    if (revisar.Equals("int"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VINT);
                                    }
                                    break;
                                case "string":
                                    if (revisar.Equals("string"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VALORSTRING);
                                    }
                                    break;
                                case "double":
                                    if (revisar.Equals("double"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VDOUBLE);
                                    }
                                    break;
                                case "char":
                                    if (revisar.Equals("char"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VCHAR);
                                    }
                                    break;
                                case "bool":
                                    if (revisar.Equals("bool"))
                                    {
                                        agregarSimbolo(VISIVILIDAD, ambito, NOMBRE, TIPO, VBOOL);

                                    }
                                    break;
                                default:
                                    Console.WriteLine("TIPO INCORRECTO");
                                    break;
                            }

                        }
                    }else
                    {
                        Console.WriteLine("MARCAR ERROR LOS TIPO NO COINCIDEN AL DECLARAR");
                    }
                    
                }
            }
        }//FIN METODO

    }//FIN DE CLASE
}//FIN DEL NAMESPACE