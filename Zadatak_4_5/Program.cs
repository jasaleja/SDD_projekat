using SDDLibrary.Distributions;
using SDDLibrary.FEL_template;
using SDDLibrary.Plotting;
using SDDLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace klasicna_raskrsnica
{

    class Program
    {
        static void Main(string[] args)
        {
            // Potrebno ako lokalna podesavanja ukljucuju (,) kao decimalni simbol
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // Postavljanje seed-a
            int defaultSeed = DateTime.Now.Millisecond;

            // Postavljanje raspodela

            // Tip ucesnika u raskrsnici
            DiscreteDistr<TravelType> travelers = new DiscreteDistr<TravelType>
                (
                    new TravelType[] { TravelType.Vehicle, TravelType.Pedestrian },
                    new double[] { 0.7, 0.3 },
                    defaultSeed/2
                );

            // Vreme dolaska
            UnifDist arrival = new UnifDist(6, 15, defaultSeed/3);

            // U kom delu raskrsnice se pojavljuje
            DiscreteDistr<IntersectionPart> intersectionPart = new DiscreteDistr<IntersectionPart>
                (
                    new IntersectionPart[] { IntersectionPart.North, IntersectionPart.South, IntersectionPart.East, IntersectionPart.West },
                    new double[] { 0.25, 0.25, 0.25, 0.25 },
                    defaultSeed/4
                );

            // Prosecno vreme prolaska kroz raskrsnicu za automobil
            NormalDist vehicleService = new NormalDist(25, 4, defaultSeed/5);

            // Prosecna vreme prelaksa prelaza za pesaka
            NormalDist pedestrianService = new NormalDist(7, 1, defaultSeed/6);

            // U kom delu raskrsnice se pojavljuje novi ucesnik
            DiscreteDistr<VehicleDirection> vehicleDirection = new DiscreteDistr<VehicleDirection>
                (
                    new VehicleDirection[] { VehicleDirection.Straight, VehicleDirection.Right, VehicleDirection.Left },
                    new double[] { 0.5, 0.3, 0.2 },
                    defaultSeed/7
                );

            FEL_template<Event> template = new FEL_template<Event>();
            Intersection sim = new Intersection(travelers, arrival, intersectionPart, vehicleService, pedestrianService, vehicleDirection);
            // Trajanje simulacije je 3600 sekundi
            template.Run(sim, 3600);

            // Crtanje grafova za pesake na sva 4 prelaza
            Plot pedestrianQueN = new Plot()
            {
                Title = "Red cekanja pesaka na severnom prelazu",
                YLabel = "Velicina reda",
                XLabel = "Vreme u sekundama"
            };

            pedestrianQueN.AddStairPlot(sim.pedestrianQueLength[0].Select((t) => t.Item1).ToArray(), sim.pedestrianQueLength[0].Select((t) => t.Item2).ToArray(), "Sever");
            pedestrianQueN.Draw();

            
            Plot pedestrianQueS = new Plot()
            {
                Title = "Red cekanja pesaka na juznom prelazu",
                YLabel = "Velicina reda",
                XLabel = "Vreme u sekundama"
            };

            pedestrianQueS.AddStairPlot(sim.pedestrianQueLength[1].Select((t) => t.Item1).ToArray(), sim.pedestrianQueLength[1].Select((t) => t.Item2).ToArray(), "Jug");
            pedestrianQueS.Draw();

            Plot pedestrianQueE = new Plot()
            {
                Title = "Red cekanja pesaka na istocnom prelazu",
                YLabel = "Velicina reda",
                XLabel = "Vreme u sekundama"
            };

            pedestrianQueE.AddStairPlot(sim.pedestrianQueLength[2].Select((t) => t.Item1).ToArray(), sim.pedestrianQueLength[2].Select((t) => t.Item2).ToArray(), "Istok");
            pedestrianQueE.Draw();

            Plot pedestrianQueW = new Plot()
            {
                Title = "Red cekanja pesaka na zapadnom prelazu",
                YLabel = "Velicina reda",
                XLabel = "Vreme u sekundama"
            };

            pedestrianQueW.AddStairPlot(sim.pedestrianQueLength[3].Select((t) => t.Item1).ToArray(), sim.pedestrianQueLength[3].Select((t) => t.Item2).ToArray(), "Zapad");
            pedestrianQueW.Draw();

            Console.ReadLine();
        }
    }
}
