using System.Linq;
using System;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.Generic;

namespace SDDLibrary.Plotting
{
    partial class PlotForm : Form
    {
        public PlotModel PlotModel { get; private set; }

        public PlotForm()
        {
            InitializeComponent();
            //this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            PlotModel = new PlotModel
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendSymbolLength = 24,
                PlotType = PlotType.XY,
                //Background = OxyColors.AntiqueWhite
            };

            var xAxis = new LinearAxis();
            xAxis.Position = AxisPosition.Bottom;
            var yAxis = new LinearAxis();

            PlotModel.Axes.Add(xAxis);
            PlotModel.Axes.Add(yAxis);
            plotView.Model = PlotModel;
        }

        public void Draw()
        {
            double max = double.MinValue;
            double min = double.MaxValue;
            bool containsStemSerie = false;

            foreach (LineSeries serie in PlotModel.Series)
            {
                foreach (DataPoint point in serie.Points)
                {
                    MinMaxFinder(ref min, ref max, point);
                }

                if (serie is StemSeries)
                {
                    containsStemSerie = true;
                }
            }

            if (containsStemSerie)
            {
                if (max < 0)
                {
                    max = 0;
                }

                else if (min > 0)
                {
                    min = 0;
                }

                PlotModel.Axes[1].Zoom(min, max);
            }

            PlotModel.InvalidatePlot(true);
        }

        public void AddLinePlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null, LineStyle style = LineStyle.Automatic)
        {
            if (xVal.Count() != yVal.Count())
                throw new Exception("X and Y values count must be equal.");

            //new FunctionSeries(Math.Sin, -10, 10, 0.1, "sin(x)")
            LineSeries line = new LineSeries();
            line.Title = name;
            line.LineStyle = style;

            var enumX = xVal.GetEnumerator();
            var enumY = yVal.GetEnumerator();

            while(enumX.MoveNext() && enumY.MoveNext())
            {
                line.Points.Add(new DataPoint(enumX.Current, enumY.Current));
            }

            PlotModel.Series.Add(line);
        }

        public void AddStemPlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null, LineStyle style = LineStyle.Automatic)
        {
            if (xVal.Count() != yVal.Count())
                throw new Exception("X and Y values count must be equal.");

            var stem = new StemSeries();
            stem.Title = name;
            stem.Color = OxyColors.DeepSkyBlue;
            stem.MarkerFill = OxyColors.DeepSkyBlue;
            stem.MarkerSize = 6;
            stem.MarkerStroke = OxyColors.White;
            stem.MarkerStrokeThickness = 1.5;
            stem.MarkerType = MarkerType.Circle;
            stem.LineStyle = style;

            var enumX = xVal.GetEnumerator();
            var enumY = yVal.GetEnumerator();

            while (enumX.MoveNext() && enumY.MoveNext())
            {
                stem.Points.Add(new DataPoint(enumX.Current, enumY.Current));
            }

            PlotModel.Series.Add(stem);
        }

        public void AddStairPlot(IEnumerable<double> xVal, IEnumerable<double> yVal, string name = null, LineStyle style = LineStyle.Automatic)
        {
            if (xVal.Count() != yVal.Count())
                throw new Exception("X and Y values count must be equal.");

            StairStepSeries step = new StairStepSeries();
            step.Title = name;
            step.LineStyle = style;

            var enumX = xVal.GetEnumerator();
            var enumY = yVal.GetEnumerator();

            while (enumX.MoveNext() && enumY.MoveNext())
            {
                step.Points.Add(new DataPoint(enumX.Current, enumY.Current));
            }

            PlotModel.Series.Add(step);
        }

        public void ClearSeries()
        {
            PlotModel.Series.Clear();
            PlotModel.InvalidatePlot(true);
        }

        private void MinMaxFinder(ref double min, ref double max, DataPoint point)
        {
            if (point.Y > max)
            {
                max = point.Y;
            }

            if (point.Y < min)
            {
                min = point.Y;
            }
        }
    }
}