using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.ML;

namespace Tablice.cs
{
    class Mark : IComparable
    {
        public Matrix<byte> NormSource = new Matrix<byte>(80, 54, 1);
        public readonly uint left;
        public readonly uint OrygWi;
        public bool toDel = false;

        private char odczytana;
        public char Odczytana
        {
            get { return odczytana; }
            private set { odczytana = value; }
        }

        readonly Matrix<float> Obraz1d2;

        static SVM svm;

        /// <summary>
        /// Funkcja wykorzystując SVM przyporządkowuje NormSource najlepiej pasujący znak alfanumeryczny
        /// </summary>
        public void Read()
        {
            UInt64[] Obraz1d = new UInt64[54 + 80];

            for (int pomocnyKopiujacy1 = 0; pomocnyKopiujacy1 < Obraz1d.Length; ++pomocnyKopiujacy1)
            {
                if (pomocnyKopiujacy1 < 54) //pion
                {
                    for (int pomocnyKopiujacy2 = 0; pomocnyKopiujacy2 < 80; ++pomocnyKopiujacy2)
                    {
                        Obraz1d[pomocnyKopiujacy1] = Obraz1d[pomocnyKopiujacy1] + Convert.ToUInt64(NormSource[pomocnyKopiujacy2, pomocnyKopiujacy1]);
                    }
                    Obraz1d[pomocnyKopiujacy1] = Obraz1d[pomocnyKopiujacy1] / 80;
                }
                else //poziom
                {
                    for (int pomocnyKopiujacy2 = 0; pomocnyKopiujacy2 < 54; pomocnyKopiujacy2++)
                    {
                        Obraz1d[pomocnyKopiujacy1] = Obraz1d[pomocnyKopiujacy1] + Convert.ToUInt64(NormSource[pomocnyKopiujacy1 - 54, pomocnyKopiujacy2]);
                    } 
                    Obraz1d[pomocnyKopiujacy1] = Obraz1d[pomocnyKopiujacy1] / 54;
                }
            }

            for(int a = 0; a != Obraz1d.Length; ++a)
            {
                Obraz1d2[0, a] = (byte)Obraz1d[a];
            }

            int r = (int)svm.Predict(Obraz1d2);
            Odczytana = Helper.Mark2Char(r);

#if DEBUG
            CvInvoke.Imshow("Char" + this.left, NormSource);
            CvInvoke.WaitKey(1500);
#endif
        }

        //implementacja sortowania IComparable
        public int CompareTo(object obj)
        {
            Mark m = (Mark)obj;

            if (this.left > m.left)
                return 1;

            if (this.left < m.left)
                return -1;

            else
                return 0;
        }

        /// <param name="Src">Obraz odczytywanego znaku</param>
        /// <param name="left">Pozycja znaku - odleglosc od lewej krawedzi</param>
        public Mark(Matrix<byte> Src, uint left)
        {
            odczytana = '?';

            if (Src.Cols > 0 && Src.Rows > 0)
            {
                this.left = left;
                this.OrygWi = (uint)Src.Cols;

                NormSource = new Matrix<byte>(80, 54, 1);
                CvInvoke.Resize(Src, this.NormSource, new System.Drawing.Size(54, 80));
                this.Obraz1d2 = new Matrix<float>(1, 54 + 80, 1);
#if DEBUG
                CvInvoke.Imshow("NormSource", this.NormSource);
                CvInvoke.WaitKey(10);
#endif
                //svm jest zmienna statyczną; wczytywanie z pliku, jesli żadna inna instancja klasy Mark tego dotychczas nie zrobiła
                if (svm == null)
                {
                    svm = new SVM();

                    const string SVMpath = @"..\..\Data\SVM.xml";

                    if (! File.Exists(SVMpath))
                        throw new ApplicationException("SVM file cannot be read");

                    using (FileStorage fsr = new FileStorage(SVMpath, FileStorage.Mode.Read))
                    {
                        svm.Read(fsr.GetFirstTopLevelNode());
                    }
                }
            }
            else
                this.toDel = true;
        }
    }
}