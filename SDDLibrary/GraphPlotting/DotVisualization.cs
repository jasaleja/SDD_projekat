using Microsoft.Win32;
using SDDLibrary.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SDDLibrary.GraphPlotting
{
    /// <summary>
    /// Za vise detalja o .dot formatu pogledati na netu.
    /// </summary>
    public static class DotVisualization
    {
        public static string DotExportPath = @".\Dot\";
        public static string GraphVizPath = "";
        private static int counter;

        public static StringBuilder BuildDOT<TVertex, TEdge>(IGraph<TVertex, TEdge> graph, Func<TVertex, string> vertex2Text,
            Func<TEdge, string> edge2Text = null) where TEdge : IEdge<TVertex>
        {
            StringBuilder dot = new StringBuilder();
            Dictionary<TVertex, int> vert2Label = new Dictionary<TVertex, int>();

            //Adding vertices
            int counter = 0;
            foreach(var vert in graph.Vertices)
            {
                vert2Label.Add(vert, counter);
                counter++;
            }

            //1. Otvaranje zaglavlja grafa
            dot.AppendLine((graph.IsDirected ? "digraph" : "graph") + " G { ");

            //2. Dodavanje globalnih atributa, npr. izgled nod-ova i grana
            dot.AppendLine("node [shape=oval];"); //oval, circle, box, plaintext...

            //3. Informacije o nod-ovima, pojedinacni izgled i sadrzaj ako je potrebno
            foreach (var kvp in vert2Label)
            {
                dot.AppendFormat("{0} [label=\"{1}\"];\n", kvp.Value, vertex2Text(kvp.Key));
            }

            //4. Informacije o povezanosti nod-ova
            string association = graph.IsDirected ? "->" : "--";
            string edgeFormat1 = "{0} " + association + " {1};\n";
            string edgeFormat2 = "{0} " + association + " {1} " + "[label=\"{2}\"];\n";

            foreach (TEdge e in graph.Edges)
            {
                if(edge2Text != null)
                {
                    dot.AppendFormat(edgeFormat2, vert2Label[e.Source], vert2Label[e.Target], edge2Text(e));
                }
                else
                {
                    dot.AppendFormat(edgeFormat1, vert2Label[e.Source], vert2Label[e.Target]);
                }
            }

            //5. Zatvaranje zaglavlja grafa
            dot.AppendLine("}");
            return dot;
        }

        public static void PlotSVG<TVertex, TEdge>(IGraph<TVertex, TEdge> graph, Func<TVertex, string> vertex2Text,
            Func<TEdge, string> edge2Text = null) where TEdge : IEdge<TVertex>
        {
            StringBuilder dotBuilder = BuildDOT(graph, vertex2Text, edge2Text);

            if (!Directory.Exists(DotExportPath))
                Directory.CreateDirectory(DotExportPath);

            string dotFilename = DotExportPath + "temp" + counter++ + ".dot";
            string svgFilename = dotFilename.Replace(".dot", ".svg");

            File.WriteAllText(dotFilename, dotBuilder.ToString());

            //Parsiranje u .dot i prikazivanje u prozoru
            RunCmdApp("\"" + GraphVizPath + "\\dot.exe\" -Tsvg " + dotFilename + " -o " + svgFilename, "\"" + svgFilename);
        }

        /// <summary>
        /// Pokretanje spoljnjeg procesa preko komandne konzole.
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="argument"></param>
        public static void RunCmdApp(params string[] commands)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = false,
                RedirectStandardInput = true,
                FileName = "cmd.exe",
            };

            using (var p = Process.Start(startInfo))
            {
                if (p == null)
                    return;

                using (StreamWriter sw = p.StandardInput)
                {
                    if (!sw.BaseStream.CanWrite)
                        return;

                    foreach (string command in commands)
                        sw.WriteLine(command);                        

                    //Zatvaranje komandne konzole
                    sw.WriteLine("exit");

                    string errorMsg = p.StandardError.ReadToEnd();

                    //Ako su postojale greske, bice prikazane u konzoli.
                    if (!string.IsNullOrEmpty(errorMsg))
                        Console.WriteLine(errorMsg);
                }

                p.WaitForExit();
                p.Close();
            }
        }
    }
}
