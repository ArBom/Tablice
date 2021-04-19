using System;
using System.Collections.Generic;
using Emgu.CV;
using Tablice.cs;

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
                Console.WriteLine("Error of EmguCV library: " + Environment.NewLine + e.ToString());
                Console.ReadKey();
                return;
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Drag and drop picture to read a plate next time.");
                Console.ReadKey();

                plates.Add(new Plate("../../Data/ZK0714E.jpg"));
            }
            else
                foreach (string path in args)
                {
                    plates.Add(new Plate(path));
                }

            foreach (Plate plate in plates)
            {
                plate.DetectPlate();
            }

            Console.ReadKey();
        }
    }
}
