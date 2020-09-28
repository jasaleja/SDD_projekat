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
                    new double[] { 0.6, 0.4 },
                    defaultSeed
                );

            // Vreme dolaska
            UnifDist arrival = new UnifDist(6, 15, defaultSeed);

            // U kom delu raskrsnice se pojavljuje
            DiscreteDistr<IntersectionPart> intersectionPart = new DiscreteDistr<IntersectionPart>
                (
                    new IntersectionPart[] { IntersectionPart.North, IntersectionPart.South, IntersectionPart.East, IntersectionPart.West },
                    new double[] { 0.25, 0.25, 0.25, 0.25 },
                    defaultSeed / 2
                );

            // Prosecno vreme prolaska kroz raskrsnicu za automobil
            NormalDist vehicleService = new NormalDist(6.94, 1, defaultSeed / 3);

            // Prosecna vreme prelaksa prelaza za pesaka
            NormalDist pedestrianService = new NormalDist(7, 1, defaultSeed / 4);

            // U kom delu raskrsnice se pojavljuje novi ucesnik
            DiscreteDistr<VehicleDirection> vehicleDirection = new DiscreteDistr<VehicleDirection>
                (
                    new VehicleDirection[] { VehicleDirection.Straight, VehicleDirection.Right, VehicleDirection.Left },
                    new double[] { 0.5, 0.3, 0.2 },
                    defaultSeed / 5
                );

            FEL_template<Event> template = new FEL_template<Event>();
            Intersection sim = new Intersection(travelers, arrival, intersectionPart, vehicleService, pedestrianService, vehicleDirection);
            // Trajanje simulacije je 3600 sekundi
            template.Run(sim, 3600);

            // Crtanje grafova za sve 4 strane
            for (int i = 0; i < 4; i++)
            {
                // Pesaci
                Plot pedestrianQue = new Plot()
                {
                    Title = $"Red cekanja pesaka na \"{(IntersectionPart)i}\" prelazu",
                    YLabel = "Kolicina pesaka",
                    XLabel = "Vreme u sekundama"
                };

                pedestrianQue.AddStairPlot(sim.pedestrianQueLength[i].Select((t) => t.Item1).ToArray(), sim.pedestrianQueLength[i].Select((t) => t.Item2).ToArray(), $"{(IntersectionPart)i}");
                pedestrianQue.Draw();

                // Vozila
                Plot vehicleQue = new Plot()
                {
                    Title = $"Red cekanja vozila na \"{(IntersectionPart)i}\" delu raskrsnice",
                    YLabel = "Kolicina vozila",
                    XLabel = "Vreme u sekundama"
                };

                vehicleQue.AddStairPlot(sim.vehicleQueLength[i].Select((t) => t.Item1).ToArray(), sim.vehicleQueLength[i].Select((t) => t.Item2).ToArray(), $"{(IntersectionPart)i}");
                vehicleQue.Draw();

                //Console.ReadKey();
            }

            // Ostatak statistike pisan
            Console.WriteLine($"Prosecno vreme cekanja pesaka na bilo kom pesackim prelazima je {sim.pedestrianWait.Average():0.00} s.");
            Console.WriteLine($"Najveci red cekanja na bilo kom pesackom prelazu je {sim.largestPedestrianQue} pesaka.");
            Console.WriteLine($"Prosecno vreme cekanja vozila na bilo kom ulazu u raskrsnicu je {sim.vehicleWait.Average():0.00} s.");
            Console.WriteLine($"Najveci red cekanja na bilo kom ulazu u raskrsnicu je {sim.largestVehicleQue} vozila.");
            
            Console.ReadLine();
        }
    }
}
