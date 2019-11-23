using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Parsing;
using Irony.Ast;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;

namespace Proyecto2
{
    public partial class Ventana : Form
    {

        private string archivorutacompleta;
        private string FILENAME = "Nueva Pestaña";
        Analizador analizar = new Analizador();
        int numeroerror = 1;


        //CONSTRUCTOR
        public Ventana()
        {
            InitializeComponent();
        }

        //NUEVA PESTAÑA
        private void nuevaPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tpage = new TabPage(FILENAME);

            FastColoredTextBox rt = new FastColoredTextBox();
            //RichTextBox rt = new RichTextBox();
            rt.Dock = System.Windows.Forms.DockStyle.Fill;
            //rt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tpage.Controls.Add(rt);
            tabControl1.TabPages.Add(tpage);
        }

        //ELIMINAR PESTAÑA
        private void eliminarPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control box;
            if (tabControl1.SelectedTab.HasChildren)
            {
                foreach (Control item in tabControl1.SelectedTab.Controls)
                {
                    box = item;
                    if (box is FastColoredTextBox)
                    {
                        string nombre = tabControl1.SelectedTab.Text;
                        if (nombre.Equals(FILENAME))
                        {
                            var result = MessageBox.Show("El archivo no ha sido guardado, Continuar?", "Nuevo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (result == DialogResult.OK)
                            {
                                tabControl1.TabPages.Remove(tabControl1.SelectedTab);

                            }
                        }
                    }
                }
            }
        }

        //GUARDAR COMO
        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                Control box;
                if (tabControl1.SelectedTab.HasChildren)
                {
                    foreach (Control item in tabControl1.SelectedTab.Controls)
                    {
                        box = item;
                        if (box is FastColoredTextBox)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Title = "Guadar";
                            sfd.ShowDialog();
                            StreamWriter wit = new StreamWriter(sfd.FileName);
                            wit.Write(box.Text);
                            wit.Close();

                            tabControl1.SelectedTab.Text = sfd.FileName;
                            MessageBox.Show("El archivo a sido guardado con exito");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un problema al guardar el archivo "+ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


        }
        
        //GUARDAR
        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Control box;
                string filename = tabControl1.SelectedTab.Text;

                if (tabControl1.SelectedTab.HasChildren)
                {
                    foreach (Control item in tabControl1.SelectedTab.Controls)
                    {
                        box = item;
                        if (box is FastColoredTextBox)
                        {
                            if (filename.Equals(FILENAME))
                            {
                                SaveFileDialog sfd = new SaveFileDialog();
                                sfd.Title = "Guadar";
                                sfd.ShowDialog();
                                filename = sfd.FileName;
                            }
                            StreamWriter wit = new StreamWriter(filename);
                            wit.Write(box.Text);
                            wit.Close();
                            tabControl1.SelectedTab.Text = filename;
                            MessageBox.Show("El archivo a sido guardado con exito");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un problema al guardar el archivo "+ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        //ABRIR
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Control box;
                if (tabControl1.SelectedTab.HasChildren)
                {
                    foreach (Control item in tabControl1.SelectedTab.Controls)
                    {
                        box = item;
                        if (box is FastColoredTextBox)
                        {
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.Title = "Abrir";
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                archivorutacompleta = ofd.FileName.ToString();
                                string texto = File.ReadAllText(archivorutacompleta);
                                box.Text = texto;
                                tabControl1.SelectedTab.Text = ofd.FileName;
                                
                            }
                        }
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Hubo un problema al abrir el archivo","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        //NUEVO
        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control box;
            if (tabControl1.SelectedTab.HasChildren)
            {
                foreach(Control item in tabControl1.SelectedTab.Controls)
                {
                    box = item;
                    if(box is FastColoredTextBox)
                    {
                        string nombre = tabControl1.SelectedTab.Text;
                        if (nombre.Equals(FILENAME))
                        {
                            var result = MessageBox.Show("El archivo no ha sido guardado, Continuar?", "Nuevo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (result == DialogResult.OK)
                            {
                                box.Text = "";
                            }
                        }
                        else {
                            tabControl1.SelectedTab.Text = FILENAME;
                            box.Text = "";
                        }
                    }
                }
            }
        }

        //COMPILAR
        private void compilarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            ParseTreeNode raiz = analizar.analiza(txtcodigo.Text.ToLower());

            if (raiz != null)
            {
                //MessageBox.Show("Analisis realizado correctamente");
                //PRIMERA PASADA
                Acciones.compilar(raiz);
                //SEGUNADA PASADA
                Acciones.compilar2(raiz);
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
                foreach (Clase cla in Acciones.listaclases)
                {
                    Console.WriteLine("CLASE-> " + cla.getNombre() + "\n");
                    foreach (Variable var in cla.GetVariables())
                    {
                        Console.WriteLine("      Variable-> " + var.getnombre() +" "+var.getvi()+ "\n");
                    }
                    foreach (Metodo met in cla.GetMetodos())
                    {
                        Console.WriteLine("             Metodo-> " + met.getNombre() +"      "+met.getvisi()+ "\n");
                    }
                }
                foreach (Simbolo sym in Acciones.listasimbolos)
                {
                   dataGridView1.Rows.Add(sym.Getvis(), sym.GetAmbito(), sym.GetNombre(), sym.GetTipo(), sym.GetValor());
                }
                txtconsola.Text = Acciones.resultado;       
            }
            else {
               MessageBox.Show("Existen Errores: Ver reportes");
            }

            reiniciar();
        }

        //REINICIAR
        private void reiniciar()
        {
            try
            {
                Acciones.listasimbolos.Clear();
                Acciones.listametodos.Clear();
                Acciones.listaerrores.Clear();
                Acciones.listavar.Clear();
                Acciones.listaclases.Clear();
                Acciones.resultado = "";
                Acciones.claseactual = null;
                Acciones.nombreclaseactual = "";
                Acciones.ambito = "global";
                Acciones.ambito2 = "";
                Acciones.ambitoaux = "";
                Acciones.reset();
            }
            catch
            {
                Console.WriteLine("PROBLEMA AL HACER RESET");
            }
        }

        //GRAFICAR
        private void aSTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("AST.png");
            }
            catch {
                MessageBox.Show("No se puede obtener la imagen");
            }
            
        }
 
        //ERRORES
        private void erroresToolStripMenuItem_Click(object sender, EventArgs e)
        {

            String html = "";
            if (analizar.listaerrores.Count == 0 && Acciones.listaerrores.Count == 0)
            {
                html += "<html>\n";
                html += "<head>\n";
                html += "<title>Errores</title>\n";
                html += "</head>\n";
                html += "<h1>NO HAY ERRORES</h1>\n";
                html += "<body>\n";
                html += "</body>\n";
                html += "</html>\n";
            }
            else
            {
                html += "<!DOCTYPE html>\n";
                html += "<html lang=\"en\" >\n";
                html += "<head>\n";
                html += "<meta charset=\"UTF-8\">\n";
                html += "<title>Errores</title>\n";
                html += "<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/meyer-reset/2.0/reset.min.css\">\n";
                html += "<link rel=\"stylesheet\" href=\"css/style.css\">";
                html += "</head>\n";
                html += "<body>\n";
                html += "<h1>REPORTE DE ERRORES</h1>\n";
                html += "  <div class=\"tbl-header\">\n";
                html += "    <table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\n";
                html += "<thead>\n";
                html += "<tr>\n";
                html += "<td>NUMERO</td>\n";
                html += "<td>ERROR</td>\n";
                html += "<td>MENSAJE</td>\n";
                html += "<td>TIPO</td>\n";
                html += "<td>FILA</td>\n";
                html += "<td>COLUMNA</td>\n";
                html += "</tr>\n";
                html += "</thead>\n";
                html += "</table>\n";
                html += "</div>\n";
                html += "<div class=\"tbl-content\">\n";
                html += "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\n";
                html += "<tbody>\n";

                foreach (Error er in analizar.listaerrores)
                {
                    html += "<td>" + numeroerror + "</td>\n";
                    html += "<td>" + er.getLevel() + "</td>\n";
                    html += "<td>" + er.getMensaje() + "</td>\n";
                    html += "<td>" + er.getTipo() + "</td>\n";
                    html += "<td>" + er.getLinea() + "</td>\n";
                    html += "<td>" + er.getColumna() + "</td>\n";
                    html += "</tr><tr>";
                    numeroerror++;
                }
                foreach (Error er in Acciones.listaerrores)
                {
                    html += "<td>" + numeroerror + "</td>\n";
                    html += "<td>" + er.getLevel() + "</td>\n";
                    html += "<td>" + er.getMensaje() + "</td>\n";
                    html += "<td>" + er.getTipo() + "</td>\n";
                    html += "<td>" + er.getLinea() + "</td>\n";
                    html += "<td>" + er.getColumna() + "</td>\n";
                    html += "</tr><tr>";
                    numeroerror++;
                }
                html += "</tbody>\n";
                html += "</table>\n";
                html += "</div>\n";
                html += "  <script src='http://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js'></script>\n";
                html += "    <script  src=\"js/index.js\"></script>\n";
                html += "</body>\n";
                html += "</html>\n";

            }


            String docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            String camino = Path.Combine(docPath,"Errores.html");
            String ruta = "C:\\Users\\aleja\\Desktop\\Recursos\\index.html";
            File.WriteAllText(ruta,html);
            try
            {
                System.Diagnostics.Process.Start(ruta);

            }
            catch
            {
                MessageBox.Show("no se pudo abrir el documento de errores");
            }

        }

        //TABLA DE SIMBOLOS
        private void tabControl2_Click(object sender, EventArgs e)
        {
            //foreach (Simbolo sym in Acciones.listasimbolos)
           // {
            //    dataGridView1.Rows.Add(sym.Getvis(),sym.GetAmbito(),sym.GetNombre(),sym.GetTipo(),sym.GetValor());
           // }
            
        }

        Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        Style BlueStyle = new TextStyle(Brushes.Blue,null,FontStyle.Regular);
        Style RedStyle = new TextStyle(Brushes.Red,null,FontStyle.Regular);
        Style BoldStyle = new TextStyle(Brushes.Black,null,FontStyle.Bold);
        Style other = new TextStyle(Brushes.DarkCyan,null,FontStyle.Bold);

        private void txtcodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(GreenStyle,BlueStyle,RedStyle,BoldStyle);
            e.ChangedRange.SetStyle(GreenStyle, @">>.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle,@"\A(<-).*(->)\Z",RegexOptions.Singleline);
            e.ChangedRange.SetStyle(BoldStyle, "clase|array|main|if|else|repeat|print|show|hacer|mientras|comprobar|caso|defecto|salir|continuar|for|while|return|importar", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(other,@"\(|\)|\[|\]|\{|\}");
            e.ChangedRange.SetStyle(BlueStyle, "(int)|string|bool|char|double|publico|privado|new", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(RedStyle,@"(\+|-|\*|/|\||=|!|&|\^|<|>)");
        }
    }
}
