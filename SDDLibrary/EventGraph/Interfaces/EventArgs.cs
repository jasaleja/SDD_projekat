using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.Interfaces
{
    public class DataPairArgs : EventArgs
    {
        public readonly Tuple<double, double> DataPair;

        public DataPairArgs(double time, double value)
        {
            DataPair = new Tuple<double, double>(time, value);
        }
    }

    public class DataValueArgs : EventArgs
    {
        public readonly double DataValue;

        public DataValueArgs(double value)
        {
            DataValue = value;
        }
    }
}
