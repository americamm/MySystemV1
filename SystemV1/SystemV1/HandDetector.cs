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

                if (hands.Count() == 0)
                {
                    listReturn.Add(Rectangle.Empty); 
                }
                else
                {
                    listReturn.Add(hands[0]);
                }
                
            }

            listReturn.Add(frame);

            return listReturn;
            //Regresa los dos valores si el frame es diferente de null, lo cual se supone que siempre es cierto, por que eso se toma en cuenta desde data poll
        }//finaliza detection()   
        


        //Acomodar para que solo regrese un rectangulo
        private List<List<System.Drawing.Rectangle>> GetIntersectedRectangles(System.Drawing.Rectangle[] DepthRA)
        {
            int startI = 0;
            bool alwaysIntersected = true;
            List<int> listIndex = new List<int>();
            List<System.Drawing.Rectangle> lista = new List<System.Drawing.Rectangle>();
            List<List<System.Drawing.Rectangle>> intesectedRect = new List<List<System.Drawing.Rectangle>>();



            if (DepthRA.Length > 2)
            {
                for (int i = startI; i < DepthRA.Length; i++)
                {
                    for (int j = i + 1; j < DepthRA.Length; j++)
                    {
                        if (DepthRA[i].IntersectsWith(DepthRA[j]))
                        {
                            if (lista.Count == 0)
                                lista.Add(DepthRA[i]);

                            lista.Add(DepthRA[j]);
                        }
                        else
                        {
                            listIndex.Add(j);
                            alwaysIntersected = false;
                        }
                    }

                    if (alwaysIntersected)
                        break;
                    else
                        startI = listIndex[0];

                    if (lista.Count > 2)
                        intesectedRect.Add(lista);
                    else
                        lista.Clear();
                }
            }

            /*if (intesectedRect.Count == 0)
            { 
                lista.Add(System.Drawing.Rectangle.Empty);
                intesectedRect.Add(lista); 
            }*/
            return intesectedRect;
        }//end method

    }//end class
}//end namespace

