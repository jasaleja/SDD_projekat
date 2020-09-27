using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SDDLibrary.Utils;
using OxyPlot;
using System.Linq;

namespace SDDLibrary.Plotting
{
    using DoublePair = Tuple<double, double>;

    public class Plot
    {
        private PlotForm plotForm;        
        private Thread plotThread;
        private bool formLoaded;
        private object syncObj;

        public string Title
        {
            get { return plotForm.SafeInvoke(d => d.PlotModel.Title); }
            set { plotForm.SafeInvokeAsync(d => d.PlotModel.Title = value); }
        }

        public string XLabel
        {
            get { return plotForm.SafeInvoke(d => d.PlotModel.Axes[0].Title); }
            set { plotForm.SafeInvokeAsync(d => d.PlotModel.Axes[0].Title = value); }
        }

        public string YLabel
        {
            get { return plotForm.SafeInvoke(d => d.PlotModel.Axes[1].Title ); }
            set { plotForm.SafeInvokeAsync(d => d.PlotModel.Axes[1].Title = value); }
        }  

        public Plot()
        {
            syncObj = new object();
            formLoaded = false;
            plotThread = new Thread(FormStarter);
            //plotThread.IsBackground = true;
            plotThread.Start();

            lock (syncObj)
                while (!formLoaded)
                    Monitor.Wait(syncObj);
        }

        private void FormStarter()
        {
            plotForm = new PlotForm();
            plotForm.Load += PlotForm_Load;
            plotForm.ShowDialog();
        }

        private void PlotForm_Load(object sender, EventArgs e)
        {
            lock (syncObj)
            {
                formLoaded = true;
                Monitor.Pulse(syncObj);
            }
        }

        public void AddStemPlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddStemPlot(xVal, yVal, name));
        }

        public void AddStemPlot(IEnumerable<DoublePair> points, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddStemPlot(points.Select(p => p.Item1), points.Select(p => p.Item2), name));
        }

        public void AddLinePlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddLinePlot(xVal, yVal, name));
        }

        public void AddLinePlot(IEnumerable<DoublePair> points, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddLinePlot(points.Select(p => p.Item1), points.Select(p => p.Item2), name));
        }

        public void AddStairPlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddStairPlot(xVal, yVal, name));
        }

        public void AddStairPlot(IEnumerable<DoublePair> points, string name = null)
        {
            plotForm.SafeInvokeAsync(d => d.AddStairPlot(points.Select(p => p.Item1), points.Select(p => p.Item2), name));
        }

        public void Draw()
        {
            plotForm.SafeInvokeAsync(d => d.Draw());
        }

        public void ClearSeries()
        {
            plotForm.SafeInvokeAsync(d => d.ClearSeries());
        }
    }
}
