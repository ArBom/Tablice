using System;
using System.Collections.Generic;

using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using Tablice.cs;
using Emgu.CV.ML;



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

                switch ("OnPl")
                {
                    case "OnPl":
                        //plates.Add(new Plate("E:/Wszystkie/PZ417HA.jpg"));
                        //plates.Add(new Plate("C:/Users/arkad/Desktop/ZKO7FX5a.jpg"));
                        plates.Add(new Plate("C:/Users/arkad/Desktop/ZK0714E.jpg"));
                        break;

                    case "RdAll":
                        {
                            using (var sr = new StreamReader("E:/Wszystkie/tensor.txt"))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    // Read the stream as a string, and write the string to the console.
                                    string all = sr.ReadLine();
                                    Plate plate = new Plate("E:/Wszystkie/" + all);
                                    plate.DetectPlate();
                                }
                            }
                        }
                        break;

                    case "tens":
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Are you sure? Delete old data and then press any key");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();

                            using (var sr = new StreamReader("E:/Wszystkie/tensor.txt"))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    // Read the stream as a string, and write the string to the console.
                                    string all = sr.ReadLine();
                                    Plate plate = new Plate("E:/Wszystkie/" + all);
                                    plate.DetectPlate();

                                    bool folderExist = System.IO.Directory.Exists("Marks");
                                    if (!folderExist)
                                        System.IO.Directory.CreateDirectory("Marks");

                                    int dot = all.IndexOf(".");
                                    plate.Text = all.Substring(0, dot);
                                    plate.SaveDataToLearn();
                                }
                            }

                            String[] directory = System.IO.Directory.GetDirectories("Marks");

                            List<Mat> skladowe = new List<Mat>();

                            object _lock = new object();
                            Matrix<float> Bars = new Matrix<float>(0, 54 + 80, 1);
                            Matrix<int> Vals = new Matrix<int>(0, 1, 1);

                            foreach(var path in directory)
                            
                            {
                                int position = path.LastIndexOf("\\");
                                position = position + 1;
                                string mark = path.Substring(position);

                                String[] files = System.IO.Directory.GetFiles(path);
                                List<string> files2 = new List<string>();

                                foreach (string s in files)
                                {
                                    if (!s.Contains(" "))
                                    {
                                        files2.Add(s);
                                    }
                                }

                                Matrix<float> Bar = new Matrix<float>(files2.Count, 54 + 80, 1);
                                Matrix<int> Val = new Matrix<int>(files2.Count, 1, 1);

                                for (int a = 0; a != files2.Count; ++a)
                                {
                                    Matrix<byte> TempBar = new Matrix<byte>(1, 54 + 80);
                                    CvInvoke.Imread(files2[a], ImreadModes.Grayscale).CopyTo(TempBar);

                                    for (int b = 0; b != TempBar.Cols; ++b)
                                    {
                                        Bar[a, b] = TempBar[0, b];
                                    }

                                    char c = mark[0];
                                    Val[a, 0] = Helper.Char2Mark(c);
                                }

                                lock (_lock)
                                {
                                    Matrix<int> NoweVals = new Matrix<int>(Vals.Rows + Val.Rows, 1, 1);
                                    Matrix<int> NoweVal = new Matrix<int>(Vals.Rows + Val.Rows, 1, 1);
                                    CvInvoke.CopyMakeBorder(Vals, NoweVals, 0, Val.Rows, 0, 0, BorderType.Constant, new MCvScalar(0));
                                    CvInvoke.CopyMakeBorder(Val, NoweVal, Vals.Rows, 0, 0, 0, BorderType.Constant, new MCvScalar(0));
                                    Vals = new Matrix<int>(NoweVals.Rows, NoweVals.Cols, 1);
                                    CvInvoke.Add(NoweVal, NoweVals, Vals);

                                    Matrix<float> NoweBars = new Matrix<float>(Bars.Rows + Bar.Rows, 54 + 80, 1);
                                    Matrix<float> NoweBar = new Matrix<float>(Bars.Rows + Bar.Rows, 54 + 80, 1);
                                    CvInvoke.CopyMakeBorder(Bars, NoweBars, 0, Bar.Rows, 0, 0, BorderType.Constant, new MCvScalar(0));
                                    CvInvoke.CopyMakeBorder(Bar, NoweBar, Bars.Rows, 0, 0, 0, BorderType.Constant, new MCvScalar(0));
                                    Bars = new Matrix<float>(NoweBar.Rows, NoweBar.Cols, 1);
                                    CvInvoke.Add(NoweBar, NoweBars, Bars);
                                }
                            }

#if DEBUG
                            CvInvoke.Imwrite("Vals.jpg", Vals);
                            CvInvoke.Imwrite("Bars.jpg", Bars);
                            CvInvoke.WaitKey(50);
#endif

                            Console.WriteLine("Make you sure, marks are correct. The press any key.");
                            Console.ReadKey();


                            //TODO nauka
                            TrainData trainData = new TrainData(Bars, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, Vals);

                            //https://stackoverflow.com/questions/35732489/load-trained-svm-emgu-cv

                            SVM model = new SVM();
                            model.SetKernel(Emgu.CV.ML.SVM.SvmKernelType.Linear);
                            model.Type = Emgu.CV.ML.SVM.SvmType.CSvc;
                            model.C = 1;
                            model.TermCriteria = new MCvTermCriteria(100, 0.00001);

                            if (model.Train(trainData))
                            {
                                using (FileStorage fileStorage = new FileStorage("SVM.txt", FileStorage.Mode.Write))
                                {
                                    model.Save("SVM.xml");
                                    model.Write(fileStorage);
                                }                               
                            }
                        }
                        break;
                }
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
