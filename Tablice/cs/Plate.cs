using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Tablice.cs
{
    class Plate
    {
        protected string Path;
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        readonly Mat OriginalPhoto;
        Point[] CuspOfPlate = new Point[4];

        List<Mark> Marks = new List<Mark>();

        internal void DetectPlate()
        {
            CvInvoke.Resize(OriginalPhoto, OriginalPhoto, new Size(OriginalPhoto.Width/4, OriginalPhoto.Height/4), 0, 0, Inter.Linear);

#if DEBUG
                String WinName = "Orig";
                CvInvoke.NamedWindow(WinName);
                CvInvoke.Imshow(WinName, OriginalPhoto);
                CvInvoke.WaitKey(1);
#endif

            Mat HSVPic = OriginalPhoto.Clone();

            Matrix<Byte> White2 = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols, 1);
            Matrix<Byte> Blue2 = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols, 1);

            CvInvoke.CvtColor(OriginalPhoto, HSVPic, ColorConversion.Rgb2Hsv);

            CvInvoke.InRange(HSVPic, new ScalarArray(new MCvScalar(8, 150, 50)), new ScalarArray(new MCvScalar(16, 255, 255)), Blue2);
            CvInvoke.InRange(HSVPic, new ScalarArray(new MCvScalar (0, 0, 163)), new ScalarArray(new MCvScalar(255, 70, 255)), White2);

            Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(2, 2), new Point(-1, -1));

            CvInvoke.Dilate(White2, White2, element, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(255));

            CvInvoke.Dilate(Blue2, Blue2, element, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(255));

#if DEBUG
            CvInvoke.Imshow("Biel", White2);
            CvInvoke.Imshow("Blekit", Blue2);
            CvInvoke.WaitKey(1);
#endif

            Matrix<Byte> PicPlate = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols);
            
            for (int a = 2; a < Blue2.Cols - 2; ++a)
            {
                for (int b = 2; b < Blue2.Rows - 2; ++b)
                {
                    if (   (Blue2[b,a-1]      == 255 && White2[b,a] == 255) //granica
                        || (PicPlate[b,a-1]   == 255 && White2[b,a] == 255) //w prawo
                        || (PicPlate[b-1,a-1] == 255 && White2[b,a] == 255) //w prawo w dó³
                        || (PicPlate[b+1,a-1] == 255 && White2[b,a] == 255) //w prawo do gógy
                        || (PicPlate[b-2, a] == 255 && White2[b, a] == 255) //w dół możliwe źródło błędów
                        )
                    {
                        PicPlate[b,a] = 255;
                    }
                }
            }

#if DEBUG
                CvInvoke.Imshow("PicPlate", PicPlate);
                CvInvoke.WaitKey(1);
#endif
            
            for (int a = PicPlate.Mat.Rows - 3; a >= 3; --a)
            {
                for (int b = PicPlate.Mat.Cols - 6; b >= 3; --b)
                {
                    if (a == 376 && b == 0) //TODO skad ta liczba
                    {
                        CvInvoke.Imshow("drawing", PicPlate);
                        CvInvoke.WaitKey(1);
                    }

                    if (PicPlate[a,b] == 255 && White2[a - 1,b] == 255)
                    {
                        PicPlate[a-1,b] = 255;
                    }

                    //bialy obszar na lewo od niebieskiego
                    if (PicPlate[a,b] == 255 && Blue2[a,b+5] == 255 && b + 4 < Blue2.Mat.Cols)// || 
                                                                                                                               //(drawing.row(a).col(b).data[0] == 255 && blekit.row(a).col(b).data[0] == 255))
                    {
                        while (PicPlate[a,b] == 255 && b > 1)
                        {
                            PicPlate[a,b] = 255;
                            --b;
                        }
                    }
                }
            }
            
#if DEBUG
            CvInvoke.Imshow("PicPlate", PicPlate);
            CvInvoke.WaitKey(1);
#endif

            Matrix<byte> Tablica = Helper.SzukajWierzcholkow(PicPlate, OriginalPhoto, 94, 407); //sama tablica, choć jeszcze wykrzywiona
            Matrix<Byte> NowaBiel = new Matrix<Byte>(Tablica.Rows, Tablica.Cols, 1);
            Mat TablicaHsv = new Mat();
            CvInvoke.CvtColor(Tablica, TablicaHsv, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(TablicaHsv, new ScalarArray(new MCvScalar(0, 0, 70)), new ScalarArray(new MCvScalar(255, 70, 255)), NowaBiel); //male hsv, duze hsv

#if DEBUG
            CvInvoke.Imshow("NowaBiel", NowaBiel);
            CvInvoke.WaitKey(1);
#endif

            int Hmax = 0;
            for (int a = 0; a != NowaBiel.Cols / 2; ++a)
            {
                int delta;
                if (Hmax - 2 <= 0)
                {
                    delta = 2;
                }
                else
                {
                    Hmax = Hmax - 2;
                    delta = 4;
                }

                int HmaxP = Hmax;

                if (HmaxP < 40)
                {
                    for (int b = Hmax; b != HmaxP + delta; ++b)
                    {
                        Hmax = b;
                        if (NowaBiel[b, a] == 255)
                            break;
                    }
                }
            }

            Matrix<byte> TablicaSzara = new Matrix<Byte>(Tablica.Rows, Tablica.Cols, 1);
            CvInvoke.CvtColor(Tablica, TablicaSzara, ColorConversion.Bgr2Gray);

            if (Hmax > 2 && Hmax < 35)
            {
                double R = (Math.Pow(NowaBiel.Cols, 2) / (8*Hmax)) + Hmax/2;

                Parallel.For(0, NowaBiel.Cols / 2 + 1, i =>
                {
                    int h = Convert.ToInt16(Math.Sqrt(Math.Pow(R, 2) - Math.Pow(NowaBiel.Cols / 2 - i, 2)) - R + Hmax) + 3 ;

                    for (int a = 0; a != 96; ++a)
                    {
                        TablicaSzara[a, i] = TablicaSzara[a + h, i];
                        TablicaSzara[a, NowaBiel.Cols - i -1 ] = TablicaSzara[a + h, NowaBiel.Cols - i - 1];
                    }
                });
            }

            Rectangle w = new Rectangle(0, 0, 407, 94);
            TablicaSzara = TablicaSzara.GetSubRect(w);

            CvInvoke.Threshold(TablicaSzara, NowaBiel, 100, 255,ThresholdType.Otsu);

            Matrix<byte> temp = new Matrix<byte>(NowaBiel.Rows + 20, NowaBiel.Cols + 20);
            temp.SetValue(255);

            CvInvoke.CopyMakeBorder(NowaBiel, NowaBiel, 2, 2, 2, 2, BorderType.Constant, new MCvScalar(255));

            CvInvoke.Imshow("tutaj szukam konturów", NowaBiel);
            CvInvoke.WaitKey(5);

            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(NowaBiel, contours, hierarchy, RetrType.External, ChainApproxMethod.LinkRuns);

            Matrix<byte> kontury = new Matrix<byte>(NowaBiel.Rows+4, NowaBiel.Cols+4);


            for (int aa = 0; aa!= contours.Size; ++aa)
                for (int b = 0; b != contours[aa].Size; ++b)
                    kontury[contours[aa][b].Y, contours[aa][b].X] = 255;

#if DEBUG
            CvInvoke.Imshow("foc", kontury);
            CvInvoke.WaitKey(1);
#endif

            kontury = new Matrix<byte>(NowaBiel.Rows+4, NowaBiel.Cols+4);

            for (int aa = 0; aa != contours.Size; ++aa)
            {
                //if ( contours[aa].Size >= 40 && contours[aa].Size <= 250)
                if (CvInvoke.ContourArea(contours[aa]) >= 80)
                {
                    //if (contours[aa])
                    //CvInvoke.DrawContours(TablicaSzara, contours, aa, new MCvScalar(255, 255, 255), 1, LineType.AntiAlias, hierarchy, 1);

                    int up = 94;
                    int dw = 0;
                    int lf = 407;
                    int rg = 0;

                    for(int bb = 0; bb != contours[aa].Size; ++bb)
                    {
                        if (contours[aa][bb].X < lf && contours[aa][bb].X > 0)
                            lf = contours[aa][bb].X;

                        if (contours[aa][bb].X > rg && contours[aa][bb].X < 407)
                            rg = contours[aa][bb].X;

                        if (contours[aa][bb].Y < up && contours[aa][bb].Y > 0)
                            up = contours[aa][bb].Y;

                        if (contours[aa][bb].Y > dw && contours[aa][bb].Y < 94)
                            dw = contours[aa][bb].Y;
                    }

                    if (rg-lf > 5 && rg - lf < 65 && up < 25 && lf > 2)
                    {
                        Rectangle newMark = new Rectangle(lf-1, up-1, rg - lf, dw - up);

                        Matrix<byte> NormSource = TablicaSzara.GetSubRect(newMark);

                        Mark mark = new Mark(TablicaSzara.GetSubRect(newMark), (uint)lf);

                        Marks.Add(mark);
                    }
                }
            }

            Marks.Sort();

            if (Marks.Count > 1)
            {
                for (int a = 0; a != Marks.Count - 1; ++a)
                {
                    for (int b = a + 1; b != Marks.Count; ++b)
                    {
                        if (Marks[a].left + 0.7 * Marks[a].OrygWi > Marks[b].left)
                        {
                            Marks[b].toDel = true;
                        }
                    }
                }
            }

            for (int c = Marks.Count -1; c>=0; c--)
            {
                if (Marks[c].toDel)
                {
                    Marks.RemoveAt(c);
                }
            }

            Parallel.ForEach(Marks, (m) =>
            {
                m.Read();
            });

            for (int a = 0; a!=Marks.Count; ++a)
            {
                text = text + Marks[a].spodziewana;
            }

            Console.Write(Environment.NewLine);
            Console.Write(text);
        }

        public void SaveDataToLearn()
        {
            char[] chars = new char[text.Length];
            chars = text.ToCharArray();
            int size = Math.Min(text.Length, Marks.Count);
        
            for (int a=0; a!=size; ++a)
            {
                Marks[a].Save(chars[a]);
            }
        }

        public Plate (string Path)
        {
            this.Path = Path;
            OriginalPhoto = CvInvoke.Imread(Path, Emgu.CV.CvEnum.ImreadModes.Color); //TODO zabezpieczyć
        }

        public Plate (string Path, string text)
        {
            this.Path = Path;
            OriginalPhoto = CvInvoke.Imread(Path, Emgu.CV.CvEnum.ImreadModes.Color);
            this.text = text;
        }
    }
}
