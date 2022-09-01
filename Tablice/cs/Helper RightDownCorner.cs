using System;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;

public partial class Helper
{
    /// <summary>
    /// Zwraca Task, którego wynikiem jest punkt będący prawy dolnym rogiem białego obszaru argumentu funkcji
    /// </summary>
    /// <param name="contour">Binarny obraz konturu tablicy</param>
    /// <returns>Task, którego rezultatem jest punkt będącegy współrządnymi prawego dolnego wierzchołka tabl. rej. lub null w przypadku nizdnalezienia</returns>

    static async Task<Point?> RightDnCorner(Matrix<Byte> contour)
    // ┌────┐
    // │⭦⭦⭦│
    // │⭦◰⭦│
    // │⭦⭦⭦│
    // └────┘
    {
        bool horizontal;
            int smallerDim;
            int biggerDim;

            if (contour.Cols > contour.Rows)
            {
                horizontal = true;
                smallerDim = contour.Rows;
                biggerDim = contour.Cols;
            }
            else
            {
                horizontal = false;
                smallerDim = contour.Cols;
                biggerDim = contour.Rows;
            }

        if (horizontal)
        // ┌────┐
        // │⭦⭦⭦│
        // │■■■ │
        // │⭦⭦⭦│
        // └────┘
        {
            for (int line = smallerDim + biggerDim; line != 0; --line)
            {
                if (line <= smallerDim)
                // ┌───┐
                // │   │
                // │■■◤│
                // │  ⭦│
                // └───┘
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                    {
                        if (contour[line - pixel - 1, pixel] == 255)
                            return new Point(pixel, line - pixel - 1);
#if DEBUG
                        else
                            contour[line - pixel - 1, pixel] = 100;
#endif

                    }
                }
                else if (line <= biggerDim)
                // ┌────┐
                // │ ⭦⭦│
                // │■◤□ │
                // │⭦⭦ │
                // └────┘
                {
                    for (int pixel = 0; pixel != smallerDim; ++pixel)
                    {
                        if (contour[smallerDim - pixel - 1, -smallerDim + line + pixel] == 255) //smallerDim - pixel
                            return new Point(-smallerDim + line + pixel, smallerDim - pixel - 1);
#if DEBUG
                         else
                            contour[smallerDim - pixel - 1, -smallerDim + line + pixel] = 100; //smallerDim - pixel, -smallerDim + line + pixel
#endif
                    }
                }
                else
                // ┌───┐
                // │⭦  │
                // │◤□□│
                // │   │
                // └───┘
                {
                    for (int pixel = 0; pixel != smallerDim + biggerDim - line; ++pixel)
                        {
                            if (contour[smallerDim - pixel - 1, -smallerDim + line + pixel] == 255)
                                return new Point(-smallerDim + line + pixel, smallerDim - pixel - 1);
#if DEBUG
                            else
                                contour[smallerDim - pixel - 1, -smallerDim + line + pixel] = 100;
#endif
                        }
                    }
#if DEBUG
                    CvInvoke.Imshow("contour", contour);
                    CvInvoke.WaitKey(2);
#endif
                }
            }
            else
        // ┌───┐
        // │⭦■⭦│
        // │⭦■⭦│
        // │⭦■⭦│
        // └───┘
        {
            for (int line = smallerDim + biggerDim; line != 0; --line)
                {
                    if (line >= biggerDim)
                // ┌───┐
                // │ ■ │
                // │ ■ │
                // │ ◤⭦│
                // └───┘
                {
                    for (int pixel = 0; pixel != smallerDim + biggerDim - line; ++pixel)
                        {
                            if (contour[-smallerDim + line + pixel, smallerDim - pixel - 1] == 255)
                                return new Point(smallerDim - pixel - 1, -smallerDim + line + pixel);
#if DEBUG
                            else
                                contour[-smallerDim + line + pixel, smallerDim - pixel - 1] = 100;
#endif
                        }
                    }
                    else if (line >= smallerDim)
                // ┌───┐
                // │ ■⭦│
                // │⭦◤⭦│
                // │⭦□ │
                // └───┘
                {
                    for (int pixel = 1; pixel != smallerDim; ++pixel)
                        {
                            if (contour[line - smallerDim + pixel, smallerDim - pixel] == 255)
                                return new Point(smallerDim - pixel, line - smallerDim + pixel);
#if DEBUG
                            else
                                contour[line - smallerDim + pixel, smallerDim - pixel] = 100;
#endif
                        }
                    }
                    else
                // ┌───┐
                // │⭦◤ │
                // │ □ │
                // │ □ │
                // └───┘
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                        {
                            if (contour[line - pixel - 1, pixel] == 255)
                                return new Point(pixel, line - pixel - 1);
#if DEBUG
                            else
                                contour[line - pixel - 1, pixel] = 100;
#endif
                        }
                    }

#if DEBUG
                    CvInvoke.Imshow("contour", contour);
                    CvInvoke.WaitKey(2);
#endif
                }
            }

        //Zrwacany w przypadku nieznalezienia żadnego białego punktu
        return null;

    }
}