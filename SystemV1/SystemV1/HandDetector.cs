using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing; 
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util; 



namespace SystemV1
{
    public class HandDetector
    { 
        //:::::::::::::::::Variables:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private CascadeClassifier haar;   
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


        //::::::::::::Detection of the hand in a gray image::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public List<Object> Detection(Image<Gray, Byte> frame)
        {   
            List<Object> listReturn = new List<object>(2);
            haar = new CascadeClassifier(@"C:\Users\America\Documents\NewHandClassifier\OpenPalm100pos\classifier\cascade.xml");
            

            if (frame != null)
            {
                System.Drawing.Rectangle[] hands = haar.DetectMultiScale(frame, 1.1, 2, new System.Drawing.Size(frame.Width / 8, frame.Height / 8), new System.Drawing.Size(frame.Width / 3, frame.Height / 3));

                foreach (System.Drawing.Rectangle roi in hands)
                {
                    Gray colorcillo = new Gray(double.MaxValue);
                    frame.Draw(roi, colorcillo, 5);
                }

                listReturn.Add(hands);
            }

            listReturn.Add(frame);

            return listReturn;
            //Regresa los dos valores si el frame es diferente de null, lo cual se supone que siempre es cierto, por que eso se toma en cuenta desde data poll
        }//finaliza detection()   

    }//end class
}//end namespace

