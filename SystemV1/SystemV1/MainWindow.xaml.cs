using System;
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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util; 


namespace SystemV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
 
    public partial class MainWindow : Window
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        
        //:::::Declare Class::::::::::::::::::::::::::;:::::
        public GetKinectData GettingKinectData;
        public HandDetector HandDetection; 
        
        //:::::Variables::::::::::::::::::::::::::::::::::::
        private int FrameWidth = 640;
        private int FrameHeigth = 480;

        private WriteableBitmap ImagenWriteablebitmap;
        private Int32Rect WriteablebitmapRect;
        private int WriteablebitmapStride;
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //::::::Constructor::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public MainWindow()
        {
            InitializeComponent();
            GettingKinectData = new GetKinectData();
            HandDetection = new HandDetector();
        } 
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //::::::Call methods:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GettingKinectData.FindKinect();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //:::::::Display the stuff:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            List<Object> returnHandDetectorK1 = new List<object>(2);
            List<Object> returnHandDetectorK2 = new List<object>(2); 
            Image<Gray, Byte> imagenKinectGray1;
            Image<Gray, Byte> imagenKinectGray2;

            imagenKinectGray1 = GettingKinectData.PollDepth(0); 
            imagenKinectGray2 = GettingKinectData.PollDepth(1);
            
            returnHandDetectorK1 = HandDetection.Detection(imagenKinectGray1);
            returnHandDetectorK2 = HandDetection.Detection(imagenKinectGray2);

            imagenKinectGray1 = (Image<Gray,Byte>)returnHandDetectorK1[1];
            imagenKinectGray2 = (Image<Gray, Byte>)returnHandDetectorK2[1];

            DepthImageK1.Source = imagetoWriteablebitmap(imagenKinectGray1);
            DepthImageK2.Source = imagetoWriteablebitmap(imagenKinectGray2);
        }
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //:::::::::::::Method to convert a byte[] of the gray image to a writeablebitmap::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private WriteableBitmap imagetoWriteablebitmap(Image<Gray, Byte> frameHand)
        {
            Image<Bgra, Byte> frameBGR = new Image<Bgra, Byte>(FrameWidth, FrameHeigth);
            byte[] imagenPixels = new byte[FrameWidth * FrameHeigth];

            this.ImagenWriteablebitmap = new WriteableBitmap(FrameWidth, FrameHeigth, 96, 96, PixelFormats.Bgr32, null);
            this.WriteablebitmapRect = new Int32Rect(0, 0, FrameWidth, FrameHeigth);
            this.WriteablebitmapStride = FrameWidth * 4;

            frameBGR = frameHand.Convert<Bgra, Byte>();
            imagenPixels = frameBGR.Bytes;

            ImagenWriteablebitmap.WritePixels(WriteablebitmapRect, imagenPixels, WriteablebitmapStride, 0);

            return ImagenWriteablebitmap;
        }  
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



        //:::::::::::::::Stop the kinect stream::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            GettingKinectData.StopKinect();
        }
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



    }//end class
}// end namespace
