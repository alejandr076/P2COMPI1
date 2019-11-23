using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Ast;
using Irony.Parsing;
using WINGRAPHVIZLib;

namespace Proyecto2
{
    class Analizador:Grammar
    {

        public List<Error> listaerrores = new List<Error>();
        

        public ParseTreeNode analiza(string cadena) {

            Gramatica gram = new Gramatica();
            LanguageData lenguaje = new LanguageData(gram);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;
            if (raiz == null)
            {
                int numerrores = 1;
               
                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {
                    String tip = "Lexico";
                    MessageBox.Show("encontre un error");
                    if (arbol.ParserMessages.ElementAt(i).Message.Contains("Syntax"))
                    {
                        tip = "Sintactico";
                    }
                    Error nuevo = new Error(arbol.ParserMessages.ElementAt(i).Level.ToString(), arbol.ParserMessages.ElementAt(i).Message,tip, arbol.ParserMessages.ElementAt(i).Location.Line, arbol.ParserMessages.ElementAt(i).Location.Column);
                    listaerrores.Add(nuevo);
                    numerrores++;
                }
                return null;
              
            }
            else {
                generarImagen(raiz);
                int nume = 1;
                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {
                    Error nuevo = new Error(arbol.ParserMessages.ElementAt(i).Level.ToString(), arbol.ParserMessages.ElementAt(i).Message, "Sintactico", arbol.ParserMessages.ElementAt(i).Location.Line, arbol.ParserMessages.ElementAt(i).Location.Column);
                    listaerrores.Add(nuevo);
                    nume++;
                }
                return raiz;
            }

        }


        private static void generarImagen(ParseTreeNode raiz) {
            String grafoDOT = Graficas.Grafica.getDOT(raiz);
            WINGRAPHVIZLib.DOT dot = new WINGRAPHVIZLib.DOT();
            WINGRAPHVIZLib.BinaryImage img = dot.ToPNG(grafoDOT);
            img.Save("AST.png");
        }

    }
}
