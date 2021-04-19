using System;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

public partial class Helper
{
    public static Matrix<Byte> SzukajWierzcholkow(Matrix<Byte> Powierzchnia, Mat zrudlo, int WysTab, int SzeTab)
    {
        bool[] wierzcholki = { false, false, false, false };
        MCvScalar color = new MCvScalar(0, 0, 255);
        int nadmiatowosc = 50;
        Matrix<Byte> zwrotka = new Matrix<byte>(WysTab + nadmiatowosc, SzeTab);//, DepthType.Cv8U , 3);
        Point[] coUporz_tabl = new Point[4];

        VectorOfPoint co_image = new VectorOfPoint();
        VectorOfPoint gdzie_image = new VectorOfPoint();

#if DEBUG
        Mat Bis = zrudlo.Clone();
#endif
        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
        Mat hierarchy = new Mat();

        Emgu.CV.Matrix<int> m = new Emgu.CV.Matrix<int>(10,10);

        zrudlo.ConvertTo(zrudlo, DepthType.Cv8U);
        CvInvoke.FindContours(Powierzchnia, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone); //ChainApproxMethod.ChainApproxSimple

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
        
        Point? wlu = LeftUpCorner(Powierzchnia2);
        Point? wru = RightUpCorner(Powierzchnia2);
        Point? wld = LeftDnCorner(Powierzchnia2);
        Point? wrd = RightDnCorner(Powierzchnia2);

        if (wlu.HasValue)
            coUporz_tabl[0] = wlu.Value;

        if (wru.HasValue)
            coUporz_tabl[1] = wru.Value;

        if (wld.HasValue)
            coUporz_tabl[2] = wld.Value;

        if (wrd.HasValue)
            coUporz_tabl[3] = wrd.Value;           

        Point[] temp = new Point[1];

        temp[0] = new Point(0, 0);
        gdzie_image.Push(temp);

        temp[0] = new Point(0, WysTab);
        gdzie_image.Push(temp);

        temp[0] = new Point(zwrotka.Cols, WysTab);
        gdzie_image.Push(temp);

        temp[0] = new Point(zwrotka.Cols, 0);
        gdzie_image.Push(temp);


        temp[0] = coUporz_tabl[0];
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(105, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[2];
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(155, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[3];
#if DEBUG
        CvInvoke.Circle(Bis, temp[0], 2, new MCvScalar(205, 0, 255), 5);
#endif
        co_image.Push(temp);

        temp[0] = coUporz_tabl[1];
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
        if (number >= 0 && number <= 9)
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

    public static byte Char2Mark(char mark)
    {
        if (mark >= 48 && mark <= 57)
        {
            return Convert.ToByte(mark-48); //letters
        }
        else if (mark >= 65 && mark <= 90)
        {
            return Convert.ToByte(mark - 55); //numbers
        }
        else
        { return 63; } //ASCII(63) = '?'
    }
}
    