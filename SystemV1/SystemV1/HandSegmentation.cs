using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;  
using System.Drawing; 

namespace SystemV1
{
    public class HandSegmentation
    {
        //:::::::::::::::::Variables::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private Seq<System.Drawing.Point> Hull;
        private MCvBox2D box; 
        //:::::::::::::::::fin variables::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        //:::::::::::::Methods for make the image binary::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //esta binarizacion usa el histograma para ontener el valor mas grande
        private Image<Gray, Byte> binaryThreshold(Image<Gray, Byte> handImage)
        {
            float maxValue;
            float minValue;
            int[] maxPos;
            int[] minPos;

            DenseHistogram histograma = new DenseHistogram(255, new RangeF(1, 255));

            histograma.Calculate(new Image<Gray, Byte>[] { handImage }, true, null);
            histograma.MinMax(out minValue, out maxValue, out minPos, out maxPos);

            handImage = handImage.ThresholdBinary(new Gray(maxPos[0] - 20), new Gray(255));

            return handImage;
        }//end binaryThreshold 


        private Image<Gray, Byte> binaryThresholdNiBlack(Image<Gray, Byte> handImage)
        {
            Gray media;
            MCvScalar desest;

            handImage.AvgSdv(out media, out desest);
            handImage = handImage.ThresholdBinary(media, new Gray(255));

            return handImage;
        }
        

        //::::::::::::Method to calculate the convex hull:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public Image<Gray, Byte> HandConvexHull(Image<Gray, Byte> frame, Rectangle Roi)
        {
            Image<Gray, Byte> BinaryImage;

            frame.ROI = Roi;
            BinaryImage = frame; 
            //BinaryImage = binaryThresholdNiBlack(frame);

            /*
            using (MemStorage storage = new MemStorage())
            {
                box = new MCvBox2D();

                Contour<System.Drawing.Point> contours = BinaryImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage);
                Contour<System.Drawing.Point> biggestContour = null;

                Double result1 = 0;
                Double result2 = 0;

                while (contours != null)
                {
                    result1 = contours.Area;
                    if (result1 > result2)
                    {
                        result2 = result1;
                        biggestContour = contours;
                    }
                    contours = contours.HNext;
                }

                if (biggestContour != null)
                {
                    Contour<System.Drawing.Point> concurrentContour = biggestContour.ApproxPoly(biggestContour.Perimeter * 0.0025, storage);
                    biggestContour = concurrentContour;
                    Hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_COUNTER_CLOCKWISE);
                }

                //handSegmentation.Draw(biggestContour, new Gray(155), 3);
                //BinaryImage.Draw(Hull, new Gray(155), 3);*/

                return BinaryImage;
        }//end HandConvexHull

    }//end class
}//end namespace
