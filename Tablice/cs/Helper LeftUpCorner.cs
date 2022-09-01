using System;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;

public partial class Helper
{
    /// <summary>
    /// Zwraca Task, którego wynikiem jest punkt będący lewym górnym rogiem białego obszaru argumentu funkcji
    /// </summary>
    /// <param name="contour">Binarny obraz konturu tablicy</param>
    /// <returns>Task, którego rezultatem jest punkt będącegy współrządnymi lewego górnego wierzchołka tabl. rej. lub null w przypadku nizdnalezienia</returns>

    static async Task<Point?> LeftUpCorner(Matrix<Byte> contour)
    // ┌────┐
    // │⭨⭨⭨│
    // │⭨◲⭨│
    // │⭨⭨⭨│
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
        // │⭨⭨⭨│
        // │■■■ │
        // │⭨⭨⭨│
        // └────┘
        {
            for (int line = 0; line != smallerDim + biggerDim; ++line)
            {
                if (line < smallerDim)
                // ┌───┐
                // │⭨  │
                // │◢■■│
                // │   │
                // └───┘
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                    {
                        if (contour[line - pixel, pixel] == 255)
                            return new Point(pixel, line - pixel);
#if DEBUG
                        else
                            contour[line - pixel, pixel] = 100;
#endif
                    }
                }
                else if (line <= biggerDim)
                // ┌───┐
                // │ ⭨⭨│
                // │□◢■│
                // │⭨⭨ │
                // └───┘
                {
                    for (int pixel = 0; pixel != smallerDim; ++pixel)
                    {
                        if (contour[smallerDim - pixel - 1, -smallerDim + line + pixel] == 255)
                            return new Point(-smallerDim + line + pixel, smallerDim - pixel - 1);
#if DEBUG
                        else
                            contour[smallerDim - pixel - 1, -smallerDim + line + pixel] = 100;
#endif
                    }
                }
                else
                // ┌───┐
                // │   │
                // │□□◢│
                // │  ⭨│
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
        // ┌────┐
        // │⭨■⭨│
        // │⭨■⭨│
        // │⭨■⭨│
        // └────┘
        {
            for (int line = 0; line != smallerDim + biggerDim; ++line)
            {
                if (line <= smallerDim)
                // ┌───┐
                // │⭨◢ │
                // │ ■ │
                // │ ■ │ 
                // └───┘
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                    {
                        if (contour[line - pixel, pixel] == 255)
                            return new Point(pixel, line - pixel);
#if DEBUG
                        else
                            contour[line - pixel, pixel] = 100;
#endif

                    }
                }
                else if (line < biggerDim)
                // ┌───┐
                // │ □⭨│
                // │⭨◢⭨│
                // │⭨■ │
                // └───┘
                {
                    for (int pixel = 0; pixel != smallerDim; ++pixel)
                    {
                        if (contour[line - pixel, pixel] == 255)
                            return new Point(pixel, line - pixel);
#if DEBUG
                        else
                            contour[line - pixel, pixel] = 100;
#endif
                    }
                }
                else
                // ┌───┐
                // │ □ │
                // │ □ │
                // │ ◢⭨│
                // └───┘
                {
                    for (int pixel = 0; pixel != smallerDim + biggerDim - line; ++pixel)
                    {
                        if (contour[biggerDim - pixel - 1, -biggerDim + line + pixel] == 255)
                            return new Point(-biggerDim + line + pixel, biggerDim - pixel - 1);
#if DEBUG
                        else
                            contour[biggerDim - pixel - 1, -biggerDim + line + pixel] = 100;
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