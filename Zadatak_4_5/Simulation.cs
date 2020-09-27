using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDDLibrary.Heap;
using SDDLibrary.Distributions;
using SDDLibrary.Utils;

namespace klasicna_raskrsnica
{
    using SDDLibrary.FEL_template;
    using System.IO;
    // Sintaksicka skracenica za par (time, value)
    using DoublePair = Tuple<double, double>;

    // Tip ucesnika u saobracaju
    enum TravelType
    {
        Vehicle,
        Pedestrian
    }

    // Da li je zeleno vertikalno ili horizontalno ukljuceno
    enum TrafficLight
    {
        NS = 0,
        EW = 1
    }

    enum IntersectionPart
    {
        North = 0,
        South = 1,
        East = 2,
        West = 3
    }

    enum VehicleDirection
    {
        Straight,
        Left,
        Right
    }

    class Intersection : IEventHandler<Event>
    {
        // Statistika
        public List<DoublePair>[] pedestrianQueLength = new List<DoublePair>[4];
        List<DoublePair>[] vehicleQue = new List<DoublePair>[4];
        List<double> vehicleWait = new List<double>();
        List<double> pedestrianWait = new List<double>();

        // Raspodele
        Distribution<TravelType> travelers;
        Distribution<double> arrival;
        DiscreteDistr<IntersectionPart> intersectionPart;
        NormalDist vehicleService;
        NormalDist pedestrianService;
        DiscreteDistr<VehicleDirection> vehicleDirection;

        TrafficLight trafficLight = TrafficLight.EW;
        Queue<double>[] pedestrianQue = new Queue<double>[4];
        int[] vehicleQueLength = new int[] { 0, 0, 0, 0 };
        int[] peopleOnCrossing = new int[] { 0, 0, 0, 0 };
        bool[] crossingInUse = new bool[] { false, false, false, false };
        List<Tuple<double, int>> peopleCrossing = new List<Tuple<double, int>>();

        bool[] intersectionInUse = new bool[] { false, false, false, false };

        public event NewEvent<Event> AddEvent;


        public Intersection(Distribution<TravelType> travelers, UnifDist arrival, DiscreteDistr<IntersectionPart> intersectionPart, NormalDist vehicleService, NormalDist pedestrianService, DiscreteDistr<VehicleDirection> vehicleDirection)
        {
            this.travelers = travelers;
            this.arrival = arrival;
            this.intersectionPart = intersectionPart;
            this.vehicleService = vehicleService;
            this.pedestrianService = pedestrianService;
            this.vehicleDirection = vehicleDirection;
        }

        public void Initialize()
        {
            peopleCrossing.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            for (int i = 0; i < 4; i++)
            {
                pedestrianQueLength[i] = new List<DoublePair>();
                vehicleQue[i] = new List<DoublePair>();
                pedestrianQue[i] = new Queue<double>();
            }

            AddEvent(new Event(EventType.TrafficLightChange, 0.0));
            AddEvent(new Event(EventType.Arrival, arrival.NextValue()));
        }

        public void ProcessEvent(Event nextEvent, double time)
        {
            switch (nextEvent.EventType)
            {
                case EventType.TrafficLightChange:
                    {
                        // Dodaj sledecu promenu semafora
                        AddEvent(new Event(EventType.TrafficLightChange, time + 60.0));

                        // Dodaj dogadjaj za pesake
                        AddEvent(new Event(EventType.StartCrossing, time));

                        if (trafficLight == TrafficLight.EW)
                            trafficLight = TrafficLight.NS;
                        else
                            trafficLight = TrafficLight.EW;
                    }
                    break;
                case EventType.Arrival:
                    {
                        // Uvek dodaj novi dolazak
                        AddEvent(new Event(EventType.Arrival, time + arrival.NextValue()));

                        TravelType traveler = travelers.NextValue();
                        IntersectionPart direction = intersectionPart.NextValue();

                        if (traveler == TravelType.Pedestrian)
                        {
                            // Dodaj ga u red cekanja
                            pedestrianQue[(int)direction].Enqueue(time);

                            // Zabelezi statistiku
                            pedestrianQueLength[(int)direction].Add(new DoublePair(time, pedestrianQue[(int)direction].Count));

                            // Proveri pesakci prelaz
                            if ((int)trafficLight != (int)direction / 2)
                            {
                                // Dodaj dogadjaj za prelazenje
                                AddEvent(new Event(EventType.StartCrossing, time));
                            }
                        }
                        else
                        {
                            //TODO: Vehicle
                        }
                    }
                    break;
                case EventType.StartCrossing:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int direction = ((int)trafficLight * 2) + i;
                            double timeOfArrival;

                            // Pokreni sve pesake za North tj. East
                            while (pedestrianQue[direction].Count != 0)
                            {
                                // Vreme da predje
                                double timeToCross = time + pedestrianService.NextValue();

                                timeOfArrival = pedestrianQue[direction].Dequeue();

                                // Zabelezi koliko je cekao pesak
                                pedestrianWait.Add(time - timeOfArrival);

                                // Dodaj dogadjaj da je pesak zavrsio prelazenje
                                AddEvent(new Event(EventType.EndCrossing, timeToCross));

                                // Zauzmi pesacki
                                peopleOnCrossing[direction]++;
                                peopleCrossing.Add(new Tuple<double, int>(timeToCross, direction));
                                crossingInUse[direction] = true;
                            }
                            // Zabelezi da su svi pesaci prosli
                            pedestrianQueLength[direction].Add(new DoublePair(time, 0));
                        }
                        // Sortiraj pesake od najranije do najkasnijeg
                        peopleCrossing.Sort();
                    }
                    break;
                case EventType.EndCrossing:
                    {
                        // Pronadji koji prelaz je presao pesak
                        int direction = peopleCrossing[0].Item2;

                        // Oslobodi prelaz ako nema pesaka na njemu
                        peopleOnCrossing[direction]--;
                        if (peopleOnCrossing[direction] == 0)
                        {
                            crossingInUse[direction] = false;
                        }

                        // Ukloni tog pesaka iz liste
                        peopleCrossing.RemoveAt(0);
                    }
                    break;
            }
        }

        public void Finish(double endTime)
        {
            //TODO: Add finish if needed
        }
    }
}
