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
    using System.Runtime.CompilerServices;
    using System.Security.AccessControl;
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

    // Svaki deo rasrsnice moze biti slobodan ili imati automobil koji ide negde
    enum VehicleDirection
    {
        Free,
        Straight,
        Left,
        Right
    }

    class Intersection : IEventHandler<Event>
    {
        // Statistika
        public List<DoublePair>[] pedestrianQueLength = new List<DoublePair>[4];
        public int largestPedestrianQue = 0;
        public List<double> pedestrianWait = new List<double>();
        public List<DoublePair>[] vehicleQueLength = new List<DoublePair>[4];
        public int largestVehicleQue = 0;
        public List<double> vehicleWait = new List<double>();

        // Raspodele
        Distribution<TravelType> travelers;
        Distribution<double> arrival;
        DiscreteDistr<IntersectionPart> intersectionPart;
        NormalDist vehicleService;
        NormalDist pedestrianService;
        DiscreteDistr<VehicleDirection> vehicleDirection;

        // Pomocne promenljive
        TrafficLight trafficLight = TrafficLight.EW;
        Queue<double>[] pedestrianQue = new Queue<double>[4];
        Queue<double>[] vehicleQue = new Queue<double>[4];
        int[] peopleOnCrossing = new int[] { 0, 0, 0, 0 };
        bool[] crossingInUse = new bool[] { false, false, false, false };
        List<Tuple<double, int>> peopleCrossing = new List<Tuple<double, int>>();
        List<Tuple<double, int>> vehicleCrossing = new List<Tuple<double, int>>();
        VehicleDirection[] intersectionInUse = new VehicleDirection[] { VehicleDirection.Free, VehicleDirection.Free, VehicleDirection.Free, VehicleDirection.Free };

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
            // Funkcija da bi se umela da se sortira ova lista
            peopleCrossing.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            vehicleCrossing.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            // Inicijalizuje ove promenljive
            for (int i = 0; i < 4; i++)
            {
                pedestrianQueLength[i] = new List<DoublePair>();
                vehicleQueLength[i] = new List<DoublePair>();
                pedestrianQue[i] = new Queue<double>();
                vehicleQue[i] = new Queue<double>();
            }

            // Ubaci prvu promenu semafora koja ce ubuduce sama sebe da zakazuje
            AddEvent(new Event(EventType.TrafficLightChange, 0.0));

            // Ubaci prvi dolazak vozila ili pesaka i svaki dolazak ce da zakaze i sledeci
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

                        // Dodaj dogadjaj za vozila
                        AddEvent(new Event(EventType.EnterIntersection, time));

                        // Promeni svetlo na semaforu
                        if (trafficLight == TrafficLight.EW)
                        {
                            trafficLight = TrafficLight.NS;
                        }
                        else
                        {
                            trafficLight = TrafficLight.EW;
                        }
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

                            // Proveri da li je novi red veci od poslednjeg najveceg
                            if (pedestrianQue[(int)direction].Count > largestPedestrianQue)
                            {
                                largestPedestrianQue = pedestrianQue[(int)direction].Count;
                            }

                            // Zabelezi statistiku reda cekanja
                            pedestrianQueLength[(int)direction].Add(new DoublePair(time, pedestrianQue[(int)direction].Count));

                            // Proveri semafor, zatim gledaj suprotne pesacke
                            if ((int)trafficLight != (int)direction / 2)
                            {
                                // Dodaj dogadjaj za prelazenje
                                AddEvent(new Event(EventType.StartCrossing, time));
                            }
                        }
                        else
                        {
                            // Dodaj vozila u red cekanja
                            vehicleQue[(int)direction].Enqueue(time);

                            // Zabelezi statistiku reda cekanja
                            vehicleQueLength[(int)direction].Add(new DoublePair(time, vehicleQue[(int)direction].Count));

                            // Proveri da li je novi red veci od poslednjeg najveceg
                            if (vehicleQue[(int)direction].Count > largestVehicleQue)
                            {
                                largestVehicleQue = vehicleQue[(int)direction].Count;
                            }

                            // Proveri semafor
                            if ((int)trafficLight == (int)direction / 2)
                            {
                                if (intersectionInUse[(int)direction] == VehicleDirection.Free)
                                {
                                    // Dodaj dogadjaj za prelazenje
                                    AddEvent(new Event(EventType.EnterIntersection, time));
                                }
                            }
                        }
                    }
                    break;
                case EventType.StartCrossing:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            // Zapad i istok su otvoreni pesacki kada je semafor za automobile sever i jug
                            int direction = trafficLight == TrafficLight.NS ? 2 + i : i;

                            // Pokreni sve pesake za North tj. East
                            while (pedestrianQue[direction].Count != 0)
                            {
                                double timeOfArrival = pedestrianQue[direction].Dequeue();

                                // Zabelezi koliko je cekao pesak
                                pedestrianWait.Add(time - timeOfArrival);

                                // Vreme da predje
                                double timeToCross = time + pedestrianService.NextValue();

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

                            // Ako se oslobodio pesacki, obavesti vozila ako postoje, prvo levo iza, ako nema, onda desno iza
                            int left = WhichWayIsLeft(direction);
                            // Proveri da li vozilo ide desno
                            if (intersectionInUse[left] == VehicleDirection.Right)
                            {
                                // Pronadji da li lokacija postoji u listi servisiranja
                                if (vehicleCrossing.Select((t) => t.Item2).ToArray().Contains(left) == false)
                                {
                                    AddEvent(new Event(EventType.ExitIntersection, time));
                                    vehicleCrossing.Add(new Tuple<double, int>(time, left));
                                    vehicleCrossing.Sort();
                                }
                            }
                            // Ako nema vozila koje skrece desno, proveri da li je ta pozicija prazna
                            else if (intersectionInUse[left] == VehicleDirection.Free)
                            {
                                int right = WhichWayIsRight(direction);
                                // Proveri da li vozilo ide levo
                                if (intersectionInUse[right] == VehicleDirection.Left)
                                {
                                    // Pronadji da li lokacija postoji u listi servisiranja
                                    if (vehicleCrossing.Select((t) => t.Item2).ToArray().Contains(right) == false)
                                    {
                                        AddEvent(new Event(EventType.ExitIntersection, time));
                                        vehicleCrossing.Add(new Tuple<double, int>(time, right));
                                        vehicleCrossing.Sort();
                                    }
                                }
                            }
                        }

                        // Ukloni tog pesaka iz liste
                        peopleCrossing.RemoveAt(0);
                    }
                    break;
                case EventType.EnterIntersection:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            // Sever/Jug(0) -> Sever(0), Jug(1), Istok/Zapad(1) -> Istok(2), Zapad(3)
                            int direction = ((int)trafficLight * 2) + i;

                            if (intersectionInUse[direction] == VehicleDirection.Free)
                            {
                                if (vehicleQue[direction].Count != 0)
                                {
                                    double timeToCross = vehicleService.NextValue();
                                    double timeOfArrival = vehicleQue[direction].Dequeue();

                                    // Zabelezi koliko je cekao pesak
                                    vehicleWait.Add(time - timeOfArrival);

                                    // Obelezio deo raskrsnice kao zauzet
                                    intersectionInUse[direction] = vehicleDirection.NextValue();

                                    // Zabelezi statistiku reda cekanja
                                    vehicleQueLength[(int)direction].Add(new DoublePair(time, vehicleQue[(int)direction].Count));

                                    // Dodaj dogadjaj za izlazak iz raskrsnice
                                    AddEvent(new Event(EventType.ExitIntersection, time + timeToCross));

                                    // Kada se desi dogadjaj za izlazak iz raskrsnice, treba da se zna koji je najraniji za izlazak
                                    vehicleCrossing.Add(new Tuple<double, int>(timeToCross, (int)direction));
                                    vehicleCrossing.Sort();
                                }
                            }
                        }
                        
                    }
                    break;
                case EventType.ExitIntersection:
                    {
                        int direction = vehicleCrossing[0].Item2;
                        vehicleCrossing.RemoveAt(0);
                        int across = WhichWayIsAcross(direction);

                        // Za slucaj da se semafor promenio u medjuvremenu,
                        // oslobodi ovaj deo raskrsnice kome je crveno svetlo, a usli su u raskrsnicu
                        if ((int)trafficLight != direction / 2)
                        {
                            intersectionInUse[direction] = VehicleDirection.Free;
                        }
                        else
                        {
                            // Proveri u kom pravcu ide vozilo
                            if (intersectionInUse[direction] == VehicleDirection.Straight)
                            {
                                intersectionInUse[direction] = VehicleDirection.Free;
                                if (vehicleQue[direction].Count != 0)
                                {
                                    AddEvent(new Event(EventType.EnterIntersection, time));
                                }
                                else
                                {
                                    if (intersectionInUse[across] == VehicleDirection.Left)
                                    {
                                        if (vehicleCrossing.Select((t) => t.Item2).ToArray().Contains(across) == false)
                                        {
                                            AddEvent(new Event(EventType.ExitIntersection, time));
                                            vehicleCrossing.Add(new Tuple<double, int>(time, across));
                                            vehicleCrossing.Sort();
                                        }
                                    }
                                }
                            }
                            else if (intersectionInUse[direction] == VehicleDirection.Right)
                            {
                                int right = WhichWayIsRight(direction);
                                if (crossingInUse[right] == false)
                                {
                                    intersectionInUse[direction] = VehicleDirection.Free;
                                    if (vehicleQue[direction].Count != 0)
                                    {
                                        AddEvent(new Event(EventType.EnterIntersection, time));
                                    }
                                    else
                                    {
                                        if (intersectionInUse[across] == VehicleDirection.Left)
                                        {
                                            if (vehicleCrossing.Select((t) => t.Item2).ToArray().Contains(across) == false)
                                            {
                                                AddEvent(new Event(EventType.ExitIntersection, time));
                                                vehicleCrossing.Add(new Tuple<double, int>(time, across));
                                                vehicleCrossing.Sort();
                                            }
                                        }
                                    }

                                }
                            }
                            else if (intersectionInUse[direction] == VehicleDirection.Left)
                            {
                                int left = WhichWayIsLeft(direction);

                                // Vozilo moze da skrene levo ako vozilo prekoputa skrece levo
                                // ili ako je prekoputa prazno i niko ne ide u susret
                                if ((intersectionInUse[across] == VehicleDirection.Left)
                                    || ((intersectionInUse[across] == VehicleDirection.Free) && (vehicleQue[across].Count == 0)))
                                {
                                    if (crossingInUse[left] == false)
                                    {
                                        intersectionInUse[direction] = VehicleDirection.Free;
                                        if (vehicleQue[direction].Count != 0)
                                        {
                                            AddEvent(new Event(EventType.EnterIntersection, time));
                                        }
                                        else
                                        {
                                            if (intersectionInUse[across] == VehicleDirection.Left)
                                            {
                                                if (vehicleCrossing.Select((t) => t.Item2).ToArray().Contains(across) == false)
                                                {
                                                    AddEvent(new Event(EventType.ExitIntersection, time));
                                                    vehicleCrossing.Add(new Tuple<double, int>(time, across));
                                                    vehicleCrossing.Sort();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private int WhichWayIsRight(int direction)
        {
            // Ove vrednosti su po enumeraciji IntersectionPart
            switch (direction)
            {
                case 0: // Sever
                    return (int)IntersectionPart.West;
                case 1: // Jug
                    return (int)IntersectionPart.East;
                case 2: // Istok
                    return (int)IntersectionPart.North;
                case 3: // Zapad
                    return (int)IntersectionPart.East;
            }
            // Za kompajler
            return 0;
        }

        private int WhichWayIsLeft(int direction)
        {
            // Ove vrednosti su po enumeraciji IntersectionPart
            switch (direction)
            {
                case 0: // Sever
                    return (int)IntersectionPart.East;
                case 1: // Jug
                    return (int)IntersectionPart.West;
                case 2: // Istok
                    return (int)IntersectionPart.South;
                case 3: // Zapad
                    return (int)IntersectionPart.North;
            }
            // Za kompajler
            return 0;
        }

        private int WhichWayIsAcross(int direction)
        {
            // Ove vrednosti su po enumeraciji IntersectionPart
            switch (direction)
            {
                case 0: // Sever
                    return (int)IntersectionPart.South;
                case 1: // Jug
                    return (int)IntersectionPart.North;
                case 2: // Istok
                    return (int)IntersectionPart.West;
                case 3: // Zapad
                    return (int)IntersectionPart.East;
            }
            // Za kompajler
            return 0;
        }

        public void Finish(double endTime)
        {
            //TODO: Add finish if needed
        }
    }
}
