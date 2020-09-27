using EventGraphLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventGraphLib.EventGraph
{
    /// <summary>
    /// Implementacija IEventObserver interfejsa, omogucava pracenje stanja blokova
    /// koji implementiraju IEventObservable interfejs.
    /// </summary>
    public class EventObserver: IEventObserver<DataPairArgs>, IEventObserver<DataValueArgs>
    {
        public Dictionary<object, List<Tuple<double, double>>> DataPairs { get; private set; }
        public Dictionary<object, List<double>> DataValues { get; private set; }

        public EventObserver()
        {
            DataPairs = new Dictionary<object, List<Tuple<double, double>>>();
            DataValues = new Dictionary<object, List<double>>();
        }

        public void OnNotify(object sender, DataValueArgs args)
        {
            if (DataValues.ContainsKey(sender))
                DataValues[sender].Add(args.DataValue);
            else
                DataValues[sender] = new List<double>() { args.DataValue };
        }

        public void OnNotify(object sender, DataPairArgs args)
        {
            if (DataPairs.ContainsKey(sender))
                DataPairs[sender].Add(args.DataPair);
            else
                DataPairs[sender] = new List<Tuple<double, double>>() { args.DataPair };
        }

        public void Clear()
        {
            //Tokom oslobadjanja resursa, ostaje informacija o vezanim event-ima
            foreach (var kvp in DataPairs)
                kvp.Value.Clear();

            foreach (var kvp in DataValues)
                kvp.Value.Clear();
        }
    }
}
