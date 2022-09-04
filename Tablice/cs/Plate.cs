using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Tablice.cs
{
    class Plate
    {
        readonly string Path;

        private string text;
        public string Text
        {
            get { return text; }
            private set { text = value; }
        }

        readonly Mat OriginalPhoto;

        private List<Mark> Marks = new List<Mark>();

        internal void DetectPlate()
        {
            Mat HSVPic;
            Matrix<byte> TablicaSzara;
            Matrix<Byte> NowaBiel;
            Matrix<byte> Tablica;
            Matrix<Byte> PicPlate;
            Matrix<Byte> White2;
            Matrix<Byte> Blue2;

            #region funkcje lokalne

            //Funkcja znajduje białe pole tablicy na którym umieszczone sa znaki skladajace sie na numer rejestracyjny
            void WyznaczBialeObszaryObokBlekitu()
            {
                White2 = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols, 1);
                Blue2 = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols, 1);

                CvInvoke.CvtColor(OriginalPhoto, HSVPic, ColorConversion.Rgb2Hsv);

                CvInvoke.InRange(HSVPic, new ScalarArray(new MCvScalar(8, 150, 50)), new ScalarArray(new MCvScalar(16, 255, 255)), Blue2);
                CvInvoke.InRange(HSVPic, new ScalarArray(new MCvScalar(0, 0, 163)), new ScalarArray(new MCvScalar(255, 70, 255)), White2);

                Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(2, 2), new Point(-1, -1));

                CvInvoke.Dilate(White2, White2, element, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(255));

                CvInvoke.Dilate(Blue2, Blue2, element, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(255));

#if DEBUG
                CvInvoke.Imshow("Biel", White2);
                CvInvoke.Imshow("Blekit", Blue2);
                CvInvoke.WaitKey(1);
#endif

                PicPlate = new Matrix<Byte>(OriginalPhoto.Rows, OriginalPhoto.Cols);

                for (int a = 2; a < Blue2.Cols - 2; ++a)
                {
                    for (int b = 2; b < Blue2.Rows - 2; ++b)
                    {
                        if ((Blue2[b, a - 1] == 255 && White2[b, a] == 255) // ◨ granica
                            || (PicPlate[b, a - 1] == 255 && White2[b, a] == 255) // ⭢ w prawo
                            || (PicPlate[b - 1, a - 1] == 255 && White2[b, a] == 255) // ⭨ w prawo w dó³
                            || (PicPlate[b + 1, a - 1] == 255 && White2[b, a] == 255) // ⭧ w prawo do góry
                            || (PicPlate[b - 2, a] == 255 && White2[b, a] == 255) // ⭣ w dół
                            )
                        {
                            PicPlate[b, a] = 255;
                        }
                    }
                }

#if DEBUG
                CvInvoke.Imshow("PicPlate", PicPlate);
                CvInvoke.WaitKey(1);
#endif
            }

            //Funkcja przekształca perspektywicznie częsc obrazu oryginalnego bedaca odpowiednikiem najwiekszego Bialego Obszaru Obok Blekitu
            void WyodrebnijPoleTablicy()
            {
                for (int a = PicPlate.Mat.Rows - 3; a >= 3; --a)
                {
                    for (int b = PicPlate.Mat.Cols - 6; b >= 3; --b)
                    {
                        if (PicPlate[a, b] == 255 && White2[a - 1, b] == 255)
                        {
                            PicPlate[a - 1, b] = 255;
                        }

                        //bialy obszar na lewo od niebieskiego
                        if (PicPlate[a, b] == 255 && Blue2[a, b + 5] == 255 && b + 4 < Blue2.Mat.Cols)
                        {
                            while (PicPlate[a, b] == 255 && b > 1)
                            {
                                PicPlate[a, b] = 255;
                                --b;
                            }
                        }
                    }
                }

#if DEBUG
                CvInvoke.Imshow("PicPlate", PicPlate);
                CvInvoke.WaitKey(1);
#endif
                //TODO zabezpieczyć na wypadek pustego poniżej
                Tablica = Helper.SzukajWierzcholkow(PicPlate, OriginalPhoto, 94, 407); //sama tablica, choć jeszcze wykrzywiona
                NowaBiel = new Matrix<Byte>(Tablica.Rows, Tablica.Cols, 1);
                Mat TablicaHsv = new Mat();
                CvInvoke.CvtColor(Tablica, TablicaHsv, ColorConversion.Bgr2Hsv);
                CvInvoke.InRange(TablicaHsv, new ScalarArray(new MCvScalar(0, 0, 70)), new ScalarArray(new MCvScalar(255, 70, 255)), NowaBiel);

#if DEBUG
                CvInvoke.Imshow("NowaBiel", NowaBiel);
                CvInvoke.WaitKey(1);
#endif
            }

            //Funcja prostuje ewentualne odksztalcenie tablicy w kształcie litery "U"
            void UsunKrzywizneTablicy()
            {
                //zmienna przechowuje ilosc pikseli pomiedzy tablica a gorna krawedzia obrazu
                int Hmax = 0;
                for (int a = 0; a != NowaBiel.Cols / 2; ++a)
                {
                    //Szukanie wartosci Hmax idac od lewej str. - eliminowanie bledu szumów, ramek tablicy itp.
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

                TablicaSzara = new Matrix<Byte>(Tablica.Rows, Tablica.Cols, 1);
                CvInvoke.CvtColor(Tablica, TablicaSzara, ColorConversion.Bgr2Gray);

                //W przypadku zbyt małego odkształcenia nie warto prostować && w przypadku zbyt dużego można podejrzewać bład w wyznaczaniu wartosci
                if (Hmax > 2 && Hmax < 35)
                {
                    //Wyznaczenie promienia okregu, którego odcinek jest dodany powyzej tablicy
                    double R = (Math.Pow(NowaBiel.Cols, 2) / (8 * Hmax)) + Hmax / 2;

                    //Przesuwanie o odcinek koła do gory
                    Parallel.For(0, NowaBiel.Cols / 2 + 1, i =>
                    {
                        //Wyznaczenie odleglosci pomiedzy cieciwa, a punktem lezacym na okregu
                        int h = Convert.ToInt16(Math.Sqrt(Math.Pow(R, 2) - Math.Pow(NowaBiel.Cols / 2 - i, 2)) - R + Hmax) + 3;

                        for (int a = 0; a != 96; ++a)
                        {
                            //odksztalcenie powinno byc symetryczne wzgladem pionowej linii środka tablicy stad 2 linijki ponizej
                            TablicaSzara[a, i] = TablicaSzara[a + h, i];
                            TablicaSzara[a, NowaBiel.Cols - i - 1] = TablicaSzara[a + h, NowaBiel.Cols - i - 1];
                        }
                    });
                }
            }

            //Funkcja znajduje kontury znaków
            void ZnajdzKonturyZnakow()
            {
                CvInvoke.Threshold(TablicaSzara, NowaBiel, 100, 255, ThresholdType.Otsu);

                Matrix<byte> temp = new Matrix<byte>(NowaBiel.Rows + 20, NowaBiel.Cols + 20);
                temp.SetValue(255);

                CvInvoke.CopyMakeBorder(NowaBiel, NowaBiel, 2, 2, 2, 2, BorderType.Constant, new MCvScalar(255));

#if DEBUG
                CvInvoke.Imshow("tutaj szukam konturów", NowaBiel);
                CvInvoke.WaitKey(5);
#endif

                Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(NowaBiel, contours, hierarchy, RetrType.External, ChainApproxMethod.LinkRuns);

                Matrix<byte> kontury = new Matrix<byte>(NowaBiel.Rows + 4, NowaBiel.Cols + 4);


                for (int aa = 0; aa != contours.Size; ++aa)
                    for (int b = 0; b != contours[aa].Size; ++b)
                        kontury[contours[aa][b].Y, contours[aa][b].X] = 255;

#if DEBUG
                CvInvoke.Imshow("foc", kontury);
                CvInvoke.WaitKey(1);
#endif

                kontury = new Matrix<byte>(NowaBiel.Rows + 4, NowaBiel.Cols + 4);

                for (int aa = 0; aa != contours.Size; ++aa)
                {
                    if (CvInvoke.ContourArea(contours[aa]) >= 80)
                    {
                        int up = 94;
                        int dw = 0;
                        int lf = 407;
                        int rg = 0;

                        for (int bb = 0; bb != contours[aa].Size; ++bb)
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

                        if (rg - lf > 5 && rg - lf < 65 && up < 25 && lf > 2)
                        {
                            Rectangle newMark = new Rectangle(lf - 1, up - 1, rg - lf, dw - up);

                            Matrix<byte> NormSource = TablicaSzara.GetSubRect(newMark);

                            Mark mark = new Mark(TablicaSzara.GetSubRect(newMark), (uint)lf);

                            Marks.Add(mark);
                        }
                    }
                }
            }

            //funkcja porzadkuje kontury znakow i odczytuje numer rejestracyjny
            void OdczytajTabliceRej()
            {
                //sortowanie kontórów od lewej do prawej
                Marks.Sort();

                //oznaczenie wewnętrznych kontórów (np. kontur będący wnątrzem litery "O")
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

                //usunięcie wewnętrznych konturów
                for (int c = Marks.Count - 1; c >= 0; c--)
                {
                    if (Marks[c].toDel)
                    {
                        Marks.RemoveAt(c);
                    }
                }

                //sprawdzenie czy tablica ma odpowiednia ilosc znakow
                if (Marks.Count < 4 || Marks.Count > 8)
                    throw new ApplicationException("Number of found marks is incorrect");

                //odczytanie znaków
                Parallel.ForEach(Marks, (m) =>
                {
                    m.Read();
                });

                //łączenie poszczególnych znaków w numer rejestracyjny
                for (int a = 0; a != Marks.Count; ++a)
                {
                    Text = Text + Marks[a].Odczytana;
                }
                #endregion
            }

            //Zmniejszenie rozdzielczosci w celu przyspieszenia dzialania progranu
            CvInvoke.Resize(OriginalPhoto, OriginalPhoto, new Size(OriginalPhoto.Width/4, OriginalPhoto.Height/4), 0, 0, Inter.Linear);

#if DEBUG
            String WinName = "Orig";
            CvInvoke.NamedWindow(WinName);
            CvInvoke.Imshow(WinName, OriginalPhoto);
            CvInvoke.WaitKey(1);
#endif

            HSVPic = OriginalPhoto.Clone();

            WyznaczBialeObszaryObokBlekitu();

            WyodrebnijPoleTablicy();

            UsunKrzywizneTablicy();


            Rectangle w = new Rectangle(0, 0, 407, 94);
            TablicaSzara = TablicaSzara.GetSubRect(w);

            ZnajdzKonturyZnakow();

            OdczytajTabliceRej();

            Console.Write(Environment.NewLine);
            Console.Write(Text);
        }

        public Plate (string Path)
        {
            this.Path = Path;

            if (!File.Exists(Path))
                throw new ApplicationException("File with plate to read doesn't exist");

            OriginalPhoto = CvInvoke.Imread(Path, Emgu.CV.CvEnum.ImreadModes.Color); //TODO zabezpieczyć
        }
    }
}
