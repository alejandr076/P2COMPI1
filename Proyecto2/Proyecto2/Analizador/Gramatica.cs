using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2
{
    class Gramatica:Grammar
    {
        public Gramatica() : base(caseSensitive: false)
        {
            #region ER
            //RegexBasedTerminal numero = new RegexBasedTerminal("int", "[0-9]+");
            RegexBasedTerminal dec = new RegexBasedTerminal("double", "[0-9]+[.][0-9]+");
            RegexBasedTerminal booleano = new RegexBasedTerminal("bool", "verdadero|falso|true|false");

            NumberLiteral numero = TerminalFactory.CreateCSharpNumber("int");
            

            IdentifierTerminal id = new IdentifierTerminal("id");

            StringLiteral cadena = TerminalFactory.CreateCSharpString("string");
            StringLiteral caracter = TerminalFactory.CreateCSharpChar("char");

            CommentTerminal simple = new CommentTerminal("simple", ">>", "\n", "\r\n");
            CommentTerminal multi = new CommentTerminal("multi", "<-", "->");



            #endregion


            #region Terminales
            var mas = ToTerm("+");
            var menos = ToTerm("-");
            var div = ToTerm("/");
            var por = ToTerm("*");
            var para = ToTerm("(");
            var parc = ToTerm(")");
            var elevar = ToTerm("^");
            var aumento = ToTerm("++");
            var decremento = ToTerm("--");
            var igual = ToTerm("=");
            var not = ToTerm("!");
            var diferente = ToTerm("!=");
            var equivalente = ToTerm("==");
            var or = ToTerm("||");
            var and = ToTerm("&&");
            var menorque = ToTerm("<");
            var mayorque = ToTerm(">");
            var menorigual = ToTerm("<=");
            var mayorigual = ToTerm(">=");
            var puntocoma = ToTerm(";");
            var coma = ToTerm(",");
            var punto = ToTerm(".");
            var dospuntos = ToTerm(":");
            var llavea = ToTerm("{");
            var llavec = ToTerm("}");
            var corchetea = ToTerm("[");
            var corchetec = ToTerm("]");
            var tint = ToTerm("int");
            var tdouble = ToTerm("double");
            var tstring = ToTerm("string");
            var tchar = ToTerm("char");
            var tbool = ToTerm("bool");
            var array = ToTerm("array");
            var clase = ToTerm("clase");
            var importar = ToTerm("importar");
            var publico = ToTerm("publico");
            var privado = ToTerm("privado");
            var tvoid = ToTerm("void");
            var tnew = ToTerm("new");
            var toverride = ToTerm("override");
            var main = ToTerm("main");
            var treturn = ToTerm("return");
            var print = ToTerm("print");
            var show = ToTerm("show");
            var tif = ToTerm("if");
            var telse = ToTerm("else");
            var tfor = ToTerm("for");
            var repeat = ToTerm("repeat");
            var twhile = ToTerm("while");
            var salir = ToTerm("salir");
            var comprobar = ToTerm("comprobar");
            var caso = ToTerm("caso");
            var defecto = ToTerm("defecto");
            var hacer = ToTerm("hacer");
            var mientras = ToTerm("mientras");
            var continuar = ToTerm("continuar");
            var addfigure = ToTerm("addfigure");
            var line = ToTerm("line");
            var triangle = ToTerm("triangle");
            var square = ToTerm("square");
            var circle = ToTerm("circle");
            var figure = ToTerm("figure");

            #endregion


            #region No Terminales
            NonTerminal INICIO = new NonTerminal("INICIO"),
                        LISTACLASES = new NonTerminal("LISTACLASES"),
                        CLASE = new NonTerminal("CLASE"),
                        OBJETO = new NonTerminal("OBJETO"),
                        LISTAINSTRUCCIONES = new NonTerminal("LISTAINSTRUCCIONES"),
                        LISTAMETODOS = new NonTerminal("LISTAMETODOS"),
                        INSTRUCCION = new NonTerminal("INSTRUCCION"),
                        METODO = new NonTerminal("METODO"),
                        LISTAPARAM = new NonTerminal("LISTAPARAM"),
                        LISTAENVIADA = new NonTerminal("LISTAENVIADA"),
                        LISTAIDES = new NonTerminal("LISTAIDES"),
                        DECLARACION = new NonTerminal("DECLARACION"),
                        ASIGNACION = new NonTerminal("ASIGNACION"),
                        VISIBILIDAD = new NonTerminal("VISIBILIDAD"),
                        METODOVOID = new NonTerminal("METODOVOID"),
                        METODORETURN = new NonTerminal("METODORETURN"),
                        FUNCION = new NonTerminal("FUNCION"),
                        RETURN = new NonTerminal("RETURN"),
                        PRINT = new NonTerminal("PRINT"),
                        IF = new NonTerminal("IF"),
                        FOR = new NonTerminal("FOR"),
                        REPEAT = new NonTerminal("REPEAT"),
                        WHILE = new NonTerminal("WHILE"),
                        COMPROBAR = new NonTerminal("COMPROBAR"),
                        OPCION = new NonTerminal("OPCION"),
                        DOWHILE = new NonTerminal("DOWHILE"),
                        ADDFIGURA = new NonTerminal("ADDFIGURA"),
                        FIGURA = new NonTerminal("FIGURA"),
                        IFELSE = new NonTerminal("IF ELSE"),
                        SHOW = new NonTerminal("SHOW"),
                        LISTAIFELSE = new NonTerminal("LISTAIFELSE"),
                        TIPO = new NonTerminal("TIPO"),
                        LISTACASOS = new NonTerminal("LISTACASOS"),
                        CONTINUAR = new NonTerminal("CONTINUAR"),
                        CASO = new NonTerminal("CASO"),
                        SALIR = new NonTerminal("SALIR"),
                        PARAMETRO = new NonTerminal("PARAMETRO"),
                        E = new NonTerminal("E"),
                        MROBJECT = new NonTerminal("MROBJECT"),
                        CALL = new NonTerminal("CALL"),
                        AUMENTO = new NonTerminal("AUMENTO"),
                        ACCESOVAR = new NonTerminal("ACCESOVAR"),
                        DECREMENTO = new NonTerminal("DECREMENTO"),
                        CAMBIO = new NonTerminal("CAMBIO"),
                        DIMENSION = new NonTerminal("DIMENSION"),
                        ASARREGLO = new NonTerminal("ASARREGLO"),
                        AS1 = new NonTerminal("AS1"),
                        AS2 = new NonTerminal("AS2"),
                        AS3 = new NonTerminal("AS3"),
                        RETURNARREGLO = new NonTerminal("RETURNARREGLO"),
                        LISTAEXTRA = new NonTerminal("LISTAEXTRA"),
                        EXTRA = new NonTerminal("EXTRA"),
                        PRUEBA = new NonTerminal("PRUEBA"),
                        LISTAPRUEBA = new NonTerminal("LISTAPRUEBA"),
                        ASIGNACIONARRAY = new NonTerminal("ASIGNACIONARRAY"),
                        TOARRAY = new NonTerminal("TOARRAY"),
                        DARREGLO = new NonTerminal("DARREGLO");
            #endregion


            #region Gramatica
            //precedencia
            this.RegisterOperators(0, Associativity.Left, or);
            this.RegisterOperators(1, Associativity.Left, and);
            this.RegisterOperators(2, Associativity.Left, not);
            this.RegisterOperators(3, Associativity.Left, equivalente,diferente,mayorigual,mayorque,menorque,menorigual);
            this.RegisterOperators(4, Associativity.Left, mas,menos);
            this.RegisterOperators(5, Associativity.Left, por,div);
            this.RegisterOperators(6, Associativity.Left, elevar);

            //marcar como puntuacion para que no aparezca en el arbol
            this.MarkPunctuation(para,array,treturn,hacer,mientras,punto,twhile,parc,puntocoma,coma,dospuntos,llavea,llavec,corchetea,corchetec,igual,clase,importar,tvoid,print,show,tif,telse,tfor,repeat);

            //no tomar para el analisis
            this.NonGrammarTerminals.Add(simple);
            this.NonGrammarTerminals.Add(multi);
 
            //hacer intrasendentes para optimizar el arbol
            this.MarkTransient(INSTRUCCION,TIPO,METODO,ASARREGLO,EXTRA,PRUEBA);

            //palabras reservadas para no confundir con ids
            this.MarkReservedWords("array", "clase", "importar", "publico", "privado", "void", "new","override", "main","return","print","show","if","else","for","repeat","while","salir","comprobar","caso","defecto","hacer","mientras","continuar","addfigure","circle","triangle","line","square","figure");


            INICIO.Rule = LISTACLASES;

            //SOLO UN NODO ACTUANDO COMO LISTA
            LISTACLASES.Rule = MakePlusRule(LISTACLASES,CLASE);
                

            CLASE.Rule = clase + id + llavea + LISTAINSTRUCCIONES + LISTAMETODOS +llavec
                        |clase + id + llavea + LISTAINSTRUCCIONES+llavec
                        |clase + id + llavea + LISTAMETODOS + llavec
                        |clase + id + llavea + llavec
                        |clase + id + importar + LISTAIDES + llavea + LISTAINSTRUCCIONES + LISTAMETODOS + llavec
                        |clase + id + importar + LISTAIDES + llavea + LISTAINSTRUCCIONES + llavec
                        |clase + id + importar + LISTAIDES + llavea + LISTAMETODOS + llavec
                        |clase + id + importar + LISTAIDES + llavea + llavec;

            LISTAIDES.Rule = MakePlusRule(LISTAIDES, coma, id);

            LISTAINSTRUCCIONES.Rule = MakePlusRule(LISTAINSTRUCCIONES, INSTRUCCION);

            LISTAMETODOS.Rule = MakePlusRule(LISTAMETODOS, METODO);

            INSTRUCCION.Rule = DECLARACION
                |ASIGNACION
                |FUNCION
                |PRINT
                |SHOW
                |IF
                |FOR
                |COMPROBAR
                |SALIR
                |CONTINUAR
                |WHILE
                |DOWHILE
                |REPEAT
                |RETURN
                |E
                |CAMBIO
                |OBJETO
                |MROBJECT
                |DARREGLO
                |ASIGNACIONARRAY
                ;

            DARREGLO.Rule = TIPO + array + LISTAIDES + DIMENSION + puntocoma
                    | VISIBILIDAD + TIPO + array + LISTAIDES + DIMENSION + puntocoma
                    | TIPO + array + LISTAIDES + DIMENSION+ igual +ASARREGLO + puntocoma
                    | VISIBILIDAD + TIPO + array + LISTAIDES + DIMENSION +igual+ASARREGLO+ puntocoma
                    ;

            ASARREGLO.Rule = AS1|AS2|AS3|E;

            AS1.Rule = llavea + LISTAENVIADA + llavec;

            AS2.Rule = llavea +LISTAEXTRA+ llavec;

            AS3.Rule = llavea + LISTAPRUEBA +llavec;

            LISTAPRUEBA.Rule = MakePlusRule(LISTAPRUEBA,coma,PRUEBA);

            PRUEBA.Rule = llavea + LISTAEXTRA + llavec;

            LISTAEXTRA.Rule = MakePlusRule(LISTAEXTRA,coma,EXTRA);

            EXTRA.Rule = llavea + LISTAENVIADA + llavec;

            DIMENSION.Rule = corchetea + E + corchetec
                | corchetea + E + corchetec+ corchetea + E + corchetec
                | corchetea + E + corchetec+ corchetea + E + corchetec+ corchetea + E + corchetec;

            ASIGNACIONARRAY.Rule = id + igual + ASARREGLO + puntocoma;

            MROBJECT.Rule = id + id + igual + id + para + parc + puntocoma;

            OBJETO.Rule = id + id + puntocoma
                | id+id+igual+ tnew+ id+ para+parc+puntocoma;

            CAMBIO.Rule = AUMENTO
                | DECREMENTO;

            AUMENTO.Rule = id + aumento + puntocoma;

            DECREMENTO.Rule = id + decremento + puntocoma;

            METODO.Rule = METODOVOID
                | METODORETURN;

            METODOVOID.Rule = main + para + parc + llavea + LISTAINSTRUCCIONES + llavec//MAIN
                |VISIBILIDAD + id + tvoid + toverride + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                |id + tvoid + toverride + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                |VISIBILIDAD + id + tvoid + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                |id + tvoid + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                | VISIBILIDAD + id + tvoid + toverride + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                | id + tvoid + toverride + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                | VISIBILIDAD + id + tvoid + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                | id + tvoid + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                ;

            METODORETURN.Rule = VISIBILIDAD + id + TIPO + toverride + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | id + TIPO + toverride + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | VISIBILIDAD + id + TIPO + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | id + TIPO + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | VISIBILIDAD + id + TIPO + toverride + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | id + TIPO + toverride + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | VISIBILIDAD + id + TIPO + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | id + TIPO + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                               | RETURNARREGLO;
                ;

            RETURNARREGLO.Rule = VISIBILIDAD + id + array + TIPO + DIMENSION + para + parc + llavea + LISTAINSTRUCCIONES + llavec
                | id + array + TIPO +DIMENSION+ para +parc + llavea + LISTAINSTRUCCIONES +llavec
                |VISIBILIDAD + id + array + TIPO + DIMENSION + para+LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec
                | id + array + TIPO + DIMENSION + para + LISTAPARAM + parc + llavea + LISTAINSTRUCCIONES + llavec;


            DECLARACION.Rule = VISIBILIDAD + TIPO + LISTAIDES + igual + E + puntocoma
                | TIPO + LISTAIDES + igual + E + puntocoma
                | TIPO + LISTAIDES + puntocoma
                | VISIBILIDAD + TIPO + LISTAIDES + puntocoma
                ;

            LISTAPARAM.Rule = MakePlusRule(LISTAPARAM, coma, PARAMETRO);

            PARAMETRO.Rule = TIPO + E
                |TIPO + TOARRAY;

            ASIGNACION.Rule = id + igual + E + puntocoma
                |TOARRAY + igual + E + puntocoma
                |id +punto+id+igual+E+puntocoma
                |id + punto + TOARRAY + igual + E + puntocoma;

            VISIBILIDAD.Rule = publico
                | privado;

            TIPO.Rule = tint
                |tdouble
                |tstring
                |tchar
                |tbool
                |id
                |TOARRAY;

            RETURN.Rule = treturn + E+puntocoma;

            FUNCION.Rule = id + para + LISTAENVIADA + parc+ puntocoma
                | id + para + parc+puntocoma
                | id + punto + id + para + parc + puntocoma
                |id+punto+id+para+LISTAENVIADA+parc+puntocoma;

            LISTAENVIADA.Rule = MakePlusRule(LISTAENVIADA,coma, E);

            PRINT.Rule = print + para + E + parc + puntocoma;

            SHOW.Rule = show + para + E + coma + E + parc + puntocoma;

            IF.Rule = tif + para + E + parc + llavea + LISTAINSTRUCCIONES + llavec
                | tif + para + E + parc + llavea+ LISTAINSTRUCCIONES+ llavec + telse +llavea+ LISTAINSTRUCCIONES + llavec
                | tif + para + E + parc + llavea+ LISTAINSTRUCCIONES+ llavec + LISTAIFELSE + telse + llavea+ LISTAINSTRUCCIONES + llavec
                ;

            LISTAIFELSE.Rule = MakePlusRule(LISTAIFELSE,IFELSE);

            IFELSE.Rule = telse + tif + para + E + parc + llavea + LISTAINSTRUCCIONES + llavec;

            FOR.Rule = tfor + para + OPCION + E + puntocoma + E + parc + llavea + LISTAINSTRUCCIONES + llavec;

            OPCION.Rule = ASIGNACION | DECLARACION;

            REPEAT.Rule = repeat + para + E + parc + llavea + LISTAINSTRUCCIONES + llavec;

            WHILE.Rule = twhile + para + E + parc + llavea + LISTAINSTRUCCIONES + llavec;

            COMPROBAR.Rule = comprobar + para + E + parc + llavea + LISTACASOS + llavec;

            LISTACASOS.Rule = MakePlusRule(LISTACASOS,CASO);

            CASO.Rule = caso + E + dospuntos + LISTAINSTRUCCIONES
                |defecto + dospuntos+LISTAINSTRUCCIONES;

            SALIR.Rule = salir + puntocoma;

            DOWHILE.Rule = hacer + llavea + LISTAINSTRUCCIONES + llavec + mientras + para + E + parc+puntocoma;

            CONTINUAR.Rule = continuar + puntocoma;

            E.Rule = E + mas + E
                    | E + menos + E
                    | E + por + E
                    | E + div + E
                    | E + elevar + E
                    | E + equivalente + E
                    | E + diferente + E
                    | E + mayorque + E
                    | E + mayorigual + E
                    | E + menorigual + E
                    | E + menorque + E
                    | menos + E
                    | E + aumento
                    | E + decremento
                    | E + or + E
                    | E + and + E
                    | not + E
                    | para + E + parc
                    | numero
                    | dec
                    | id
                    | booleano
                    | cadena
                    | caracter
                    | CALL
                    | tnew + id + para + parc
                    | ACCESOVAR
                    | TOARRAY
                    ;

            TOARRAY.Rule = id +DIMENSION;

            CALL.Rule = id + para + parc
                | id + para + LISTAENVIADA + parc
                | id + punto + id + para + parc
                | id + punto + id + para + LISTAENVIADA + parc;

            ACCESOVAR.Rule = id + punto + id;


            #endregion


            #region Raiz
            this.Root = INICIO;
            #endregion
        }
    }
}
