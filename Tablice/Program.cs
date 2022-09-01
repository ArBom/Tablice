using System;
using System.Collections.Generic;
using Emgu.CV;
using Tablice.cs;

using System.Diagnostics;


namespace Tablice
{
    class Program
    {
        static List<Plate> plates = new List<Plate>();

        static void Main(string[] args)
        {
            try //Checking EmguCV
            {
                CvInvoke.CheckLibraryLoaded();
            }
            catch (Exception e)
            {

                Console.WriteLine("Error of EmguCV library (try to reinstall Emgu.CV): \n Update-Package -Id Emgu.CV –reinstall \n" + Environment.NewLine + e.ToString());
                Console.ReadKey();
                Process.Start("https://docs.microsoft.com/pl-pl/nuget/consume-packages/reinstalling-and-updating-packages");
                return;
            }

            Stopwatch sw = new Stopwatch();
            if (args.Length == 0)
            {
                Console.WriteLine("Drag and drop picture to read a plate next time. Press any key to continue now.");
                Console.ReadKey();

                sw.Start();
                plates.Add(new Plate("../../Data/ZK0714E.jpg"));
            }
            else
            {
                sw.Start();
                foreach (string path in args)
                {
                    plates.Add(new Plate(path));
                }
            }
            foreach (Plate plate in plates)
            {
                plate.DetectPlate();
            }

            sw.Stop();
            Console.WriteLine(Environment.NewLine + "Time of read the plates: {0}", sw.Elapsed);

            Console.ReadKey();
        }
    }
}
