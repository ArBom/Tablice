﻿using System;
using System.Drawing;
using Emgu.CV;

public partial class Helper
{
    static Point? LeftDnCorner(Matrix<Byte> contour)
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
        {
            for (int line = 0; line != smallerDim + biggerDim; ++line)
            {
                if (line <= smallerDim)
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                    {
                        if (contour[smallerDim - line + pixel, pixel] == 255)
                            return new Point(pixel, smallerDim - line + pixel);
#if DEBUG
                        else
                            contour[smallerDim - line + pixel, pixel] = 100;
#endif
                    }
                }
                else if (line <= biggerDim)
                {
                    for (int pixel = 0; pixel != smallerDim; ++pixel)
                    {
                        if (contour[pixel, -smallerDim + line + pixel] == 255)
                            return new Point(-smallerDim + line + pixel, pixel);
#if DEBUG
                        else
                            contour[pixel, -smallerDim + line + pixel] = 100;
#endif
                    }
                }
                else
                {
                    for (int pixel = 0; pixel != smallerDim + biggerDim - line; ++pixel)
                    {
                        if (contour[pixel, -smallerDim + line + pixel] == 255)
                            return new Point(-smallerDim + line + pixel, pixel);
#if DEBUG
                        else
                            contour[pixel, -smallerDim + line + pixel] = 100;
#endif
                    }
                }

#if DEBUG
                CvInvoke.Imshow("contour", contour);
                CvInvoke.WaitKey(5);
#endif
            }
        }
        else //vertical left down
        {
            for (int line = 0; line != smallerDim + biggerDim; ++line)
            {
                if (line <= smallerDim)
                {
                    for (int pixel = 0; pixel != line; ++pixel)
                    {
                        if (contour[biggerDim - line + pixel, pixel] == 255)
                            return new Point(pixel, biggerDim - line + pixel);
#if DEBUG
                        else
                            contour[biggerDim - line + pixel, pixel] = 100;
#endif
                    }
                }
                else if (line <= biggerDim)
                {
                    for (int pixel = 0; pixel != smallerDim-1; ++pixel)
                    {
                        if (contour[pixel - line + biggerDim, pixel] == 255)
                            return new Point(pixel, pixel - line + biggerDim);
#if DEBUG
                        else
                            contour[pixel-line+biggerDim, pixel] = 100;
#endif
                    }
                }
                else
                {
                    for (int pixel = 0; pixel != smallerDim + biggerDim - line; ++pixel)
                    {
                        if (contour[pixel, -biggerDim + line + pixel] == 255)
                            return new Point(-biggerDim + line + pixel, pixel);
#if DEBUG
                        else
                            contour[pixel, -biggerDim + line + pixel] = 100;
#endif
                    }
                }
#if DEBUG
                CvInvoke.Imshow("contour", contour);
                CvInvoke.WaitKey(1);
#endif
            }
        }

        return null;
    }
}