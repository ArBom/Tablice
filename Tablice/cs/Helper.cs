using System;
using System.Drawing;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

public partial class Helper
{
    /// <summary>
    /// Zwraca wycinek zdjęcia zawierający białą część tablicy rejetracyjnej
    /// </summary>
    /// <param name="Powierzchnia">Binarny odpowiednik źródłowego obrazu, gdzie pole tablicy jest białe</param>
    /// <param name="zrudlo">Obraz źródłowy zawierający tablicę</param>
    /// <param name="WysTab">Wysokość Zwracanego obrazu (bez tzw. nadmiarowości)</param>
    /// <param name="SzeTab">Szerokość Zwrcanego obrazu</param>
    /// <returns>Przekształcony perspektywicznie obszar obrazu źródłowego zawierający tabl. rej.</returns>
    public static Matrix<Byte> SzukajWierzcholkow(Matrix<Byte> Powierzchnia, Mat zrudlo, int WysTab, int SzeTab)
    // ┌────┐
    // │⭨ ⭩│
    // │ ⧈ │
    // │⭧ ⭦│
    // └────┘
    {
        MCvScalar color = new MCvScalar(0, 0, 255);

        //Ilość pikseli dodawana do rządanej wysokoćsi zwracanego obrazu (z uwagi na możliwość wykrzywienia tablicy)
        int nadmiatowosc = 50;
        Matrix<Byte> zwrotka = new Matrix<byte>(WysTab + nadmiatowosc, SzeTab);

        VectorOfPoint co_image = new VectorOfPoint();
        VectorOfPoint gdzie_image = new VectorOfPoint();

#if DEBUG
        Mat Bis = zrudlo.Clone();
#endif
        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
        Mat hierarchy = new Mat();

        Emgu.CV.Matrix<int> m = new Emgu.CV.Matrix<int>(10,10);

        zrudlo.ConvertTo(zrudlo, DepthType.Cv8U);
        CvInvoke.FindContours(Powierzchnia, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

        int biggest = 0;
        for (int a = 0; a!=contours.Size; a++)
        {
            if (CvInvoke.ContourArea(contours[a]) > CvInvoke.ContourArea(contours[biggest]))
            {
                biggest = a;
            }
        }

        Matrix<byte> Powierzchnia2 = new Matrix<byte>(Powierzchnia.Size.Height, Powierzchnia.Size.Width, 1);

        if (contours.Size > 0)
        {
            for (int a = 0; a != contours[biggest].Size; a++)
            {
                Powierzchnia2[contours[biggest][a].Y, contours[biggest][a].X] = 255;
            }
        }

#if DEBUG
        CvInvoke.Imshow("Powierzchnia2", Powierzchnia2);
        CvInvoke.WaitKey(1);
#endif

        Task<Point?>[] coUporz_tabl = 
        {
            Task<Point?>.Factory.StartNew(() => LeftUpCorner(Powierzchnia2).Result),
            Task<Point?>.Factory.StartNew(() => RightUpCorner(Powierzchnia2).Result),
            Task<Point?>.Factory.StartNew(() => LeftDnCorner(Powierzchnia2).Result),
            Task<Point?>.Factory.StartNew(() => RightDnCorner(Powierzchnia2).Result)
        };
        Task.WaitAll(coUporz_tabl);

        if (!(coUporz_tabl[0].Result.HasValue & coUporz_tabl[0].Result.HasValue & coUporz_tabl[0].Result.HasValue & coUporz_tabl[0].Result.HasValue))
        {
            Console.WriteLine("It cannot to find a plate in the picture");
            return null;
        }

        Point[] temp = new Point[1];

        temp[0] = new Point(0, 0);
        gdzie_image.Push(temp);

        temp[0] = new Point(0, WysTab);
        gdzie_image.Push(temp);

        temp[0] = new Point(zwrotka.Cols, WysTab);
        gdzie_image.Push(temp);

        temp[0] = new Point(zwrotka.Cols, 0);
        gdzie_image.Push(temp);


        temp[0] = coUporz_tabl[0].Result.Value;
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(105, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[2].Result.Value;
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(155, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[3].Result.Value;
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(205, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[1].Result.Value;
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(255, 0, 255), 5);
#endif
        co_image.Push(temp);

        Mat H = CvInvoke.FindHomography(co_image, gdzie_image, 0);
        CvInvoke.WarpPerspective(zrudlo, zwrotka, H, zwrotka.Size);

#if DEBUG
        CvInvoke.Imshow("tablica", zwrotka);
        CvInvoke.Imshow("founf", Bis);
        CvInvoke.WaitKey(1);
#endif
          return zwrotka;
    }

    public static char Mark2Char (int number)
    {
        if (number >= 0 && number <= 9) //numbers
        {
            return Convert.ToChar(number + 48);
        }
        else if (number >= 10 && number <= 35) //letters
        {
            return Convert.ToChar(number + 55); 
        }
        else
        { return '?'; }
    }
}
    