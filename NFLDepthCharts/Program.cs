using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NFLDepthCharts.Models;
using NFLDepthCharts.Services;

namespace NFLDepthCharts
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleUsage();
            TestFromRequirement();
        }

        static void SampleUsage()
        {
            try
            {
                var positions = new List<string>() { "QB", "LWR" };

                var service = new DepthChartService(positions, 5);

                var p1 = new PlayerModel { Number = 1, Name = "p1" };
                var p2 = new PlayerModel { Number = 2, Name = "p2" };
                var p3 = new PlayerModel { Number = 3, Name = "p3" };
                var p4 = new PlayerModel { Number = 4, Name = "p4" };
                var p5 = new PlayerModel { Number = 5, Name = "p5" };
                var p6 = new PlayerModel { Number = 6, Name = "p6" };
                var p7 = new PlayerModel { Number = 7, Name = "p7" };
                var p8 = new PlayerModel { Number = 8, Name = "p8" };
                var p9 = new PlayerModel { Number = 9, Name = "p9" };
                var p10 = new PlayerModel { Number = 10, Name = "p10" };

                service.AddPlayerToChart("lwr", p1);
                service.AddPlayerToChart("lwr", p2);
                service.AddPlayerToChart("qb", p3);
                service.AddPlayerToChart("qb", p4);
                service.AddPlayerToChart("lwr", p5);
                service.AddPlayerToChart("qb", p6, 0);

                service.GetFullDepthChart();
                service.GetBackups("qb", p6);
                service.GetBackups("qb", p3);
                service.GetBackups("qb", p4);
                service.GetBackups("lwr", p6);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void TestFromRequirement()
        {
            try
            {
                var positions = new List<string>() { "QB", "LWR" };

                var service = new DepthChartService(positions, 5);

                var TomBrady = new PlayerModel { Number = 12, Name = "Tom Brady" };
                var BlaineGabbert = new PlayerModel { Number = 11, Name = "Blaine Gabbert" };
                var KyleTrask = new PlayerModel { Number = 2, Name = "Kyle Trask" };

                var MikeEvans = new PlayerModel { Number = 13, Name = "Mike Evans" };
                var JaelonDarden = new PlayerModel { Number = 1, Name = "Jaelon Darden" };
                var ScottMiller = new PlayerModel { Number = 10, Name = "Scott Miller" };

                service.AddPlayerToChart("QB", TomBrady, 0);
                service.AddPlayerToChart("QB", BlaineGabbert, 1);
                service.AddPlayerToChart("QB", KyleTrask, 2);

                service.AddPlayerToChart("LWR", MikeEvans, 0);
                service.AddPlayerToChart("LWR", JaelonDarden, 1);
                service.AddPlayerToChart("LWR", ScottMiller, 2);

                service.GetBackups("QB", TomBrady);
                /* Output */
                //#11 – Blaine Gabbert
                //#2 – Kyle Trask

                service.GetBackups("LWR", JaelonDarden);
                /* Output */
                //#10 – Scott Miller

                service.GetBackups("QB", MikeEvans);
                /* Output */
                //< NO LIST >

                service.GetBackups("QB", BlaineGabbert);
                /* Output */
                //#2 - Kyle Trask

                service.GetBackups("QB", KyleTrask);
                /* Output */
                //< NO LIST >

                service.GetFullDepthChart();
                /* Output */
                //QB – (#12, Tom Brady), (#11, Blaine Gabbert), (#2, Kyle Trask)
                //LWR – (#13, Mike Evans), (#1, Jaelon Darden), (#10, Scott Miller)

                service.RemovePlayerFromChart("LWR", MikeEvans);
                /* Output */
                //#13 – MikeEvans

                service.GetFullDepthChart();
                /* Output */
                //QB – (#12, Tom Brady), (#11, Blaine Gabbert), (#2, Kyle Trask)
                //LWR - (#1, JaelonDarden), (#10, Scott Miller)
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
