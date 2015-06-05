﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util; 


 

namespace GetKinectData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //::::::::::::::Variables:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private KinectSensor Kinect;
        private KinectSensorCollection Sensores = KinectSensor.KinectSensors;
        private List<KinectSensor> Sensor = new List<KinectSensor>();
        
        private WriteableBitmap ImagenWriteablebitmap;
        private Int32Rect WriteablebitmapRect;
        private DepthImagePixel[] DepthPixels;
        private DepthImageStream DepthStream;
        private int WriteablebitmapStride;
        private byte[] DepthImagenPixeles;
        private Image<Gray, Byte> depthFrameKinect;

        private int minDepth=400;
        private int maxDepth=2000;
        //:::::::::::::fin variables::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public MainWindow()
        {
            InitializeComponent();
        } 
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FindKinect();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        } 
       //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void FindKinect()
        {
            foreach (KinectSensor Kinect in Sensores)
            {
                if (Kinect.Status == KinectStatus.Connected)
                {
                    Sensor.Add(Kinect);
                }
            }

            try
            {
                for (int i = 0; i < Sensor.Count; i++)
                {
                    Kinect = Sensor[i];
                    if (this.Kinect != null)
                    {
                        this.Kinect.DepthStream.Enable();
                        this.Kinect.DepthStream.Range = DepthRange.Near;
                        this.Kinect.Start();
                    }
                }
            }
            catch
            {
                MessageBox.Show("El dispositivo Kinect no se encuentra conectado", "Error Kinect");
            }
        }


        private Image<Gray, Byte> PollDepth(int numKinect)
        {
            Image<Bgra, Byte> depthFrameKinectBGR = new Image<Bgra, Byte>(640, 480);
            Kinect = Sensor[numKinect]; 


            if (this.Kinect != null)
            {
                this.DepthStream = this.Kinect.DepthStream;
                //this.DepthValoresStream = new short[DepthStream.FramePixelDataLength];
                this.DepthPixels = new DepthImagePixel[DepthStream.FramePixelDataLength];
                this.DepthImagenPixeles = new byte[DepthStream.FramePixelDataLength * 4];
                this.depthFrameKinect = new Image<Gray, Byte>(DepthStream.FrameWidth, DepthStream.FrameHeight);

                Array.Clear(DepthImagenPixeles, 0, DepthImagenPixeles.Length);

                try
                {
                    using (DepthImageFrame frame = this.Kinect.DepthStream.OpenNextFrame(100))
                    {
                        if (frame != null)
                        {
                            frame.CopyDepthImagePixelDataTo(this.DepthPixels);


                            int index = 0;
                            for (int i = 0; i < DepthPixels.Length; ++i)
                            {
                                short depth = DepthPixels[i].Depth;

                                byte intensity = (byte)((depth >= minDepth) && (depth <= maxDepth) ? depth : 0);

                                DepthImagenPixeles[index++] = intensity;
                                DepthImagenPixeles[index++] = intensity;
                                DepthImagenPixeles[index++] = intensity;

                                ++index;
                            }

                            depthFrameKinectBGR.Bytes = DepthImagenPixeles; //The bytes are converted to a Imagen(Emgu). This to work with the functions of opencv. 
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("No se pueden leer los datos del sensor", "Error");
                }
            }

            depthFrameKinect = depthFrameKinectBGR.Convert<Gray, Byte>();
            depthFrameKinect = removeNoise(depthFrameKinect, 3);

            return depthFrameKinect;
        }//fin PollDepth() 


        private Image<Gray, Byte> removeNoise(Image<Gray, Byte> imagenKinet, int sizeWindow)
        {
            Image<Gray, Byte> imagenSinRuido;

            imagenSinRuido = imagenKinet.SmoothMedian(sizeWindow);

            return imagenSinRuido;
        }//endremoveNoise  

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //::::::::::::::This part of the code, is just for see the results of this program::::::::::::::::::::::::::::::::::::::::::::::::::::
        
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Image<Gray, Byte> imagenKinectGray1;
            Image<Gray, Byte> imagenKinectGray2; 

            imagenKinectGray1 = PollDepth(0);
            imagenKinectGray2 = PollDepth(1);
            DepthImageK1.Source = imagetoWriteablebitmap(imagenKinectGray1);
            DepthImageK2.Source = imagetoWriteablebitmap(imagenKinectGray2);
            
        } //fin CompositionTarget_Rendering()   

        
        //:::::::::::::Method to convert a byte[] of the gray image to a writeablebitmap::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private WriteableBitmap imagetoWriteablebitmap(Image<Gray, Byte> frameHand)
        {
            Image<Bgra, Byte> frameBGR = new Image<Bgra, Byte>(DepthStream.FrameWidth, DepthStream.FrameHeight);
            byte[] imagenPixels = new byte[DepthStream.FrameWidth * DepthStream.FrameHeight];

            this.ImagenWriteablebitmap = new WriteableBitmap(DepthStream.FrameWidth, DepthStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
            this.WriteablebitmapRect = new Int32Rect(0, 0, DepthStream.FrameWidth, DepthStream.FrameHeight);
            this.WriteablebitmapStride = DepthStream.FrameWidth * 4;

            frameBGR = frameHand.Convert<Bgra, Byte>();
            imagenPixels = frameBGR.Bytes;

            ImagenWriteablebitmap.WritePixels(WriteablebitmapRect, imagenPixels, WriteablebitmapStride, 0);

            return ImagenWriteablebitmap;
        }//end  
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Kinect.Stop(); 
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


    }//end class
}//end namespace 