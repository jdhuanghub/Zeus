using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

using OpenNI;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;

namespace SensorTest.net
{
    public class CustomXmlData
    {
        public double testDistance;
        public double TestDistance
        {
            get{ return testDistance; }
            set{ testDistance = value; }
        }

        public double errorRange;
        public double ErrorRange
        {
            get { return errorRange; }
            set { errorRange = value; }
        }
    }

    class Program
    {
        static void Run(double customDistance, double customRange)
        {

            string SAMPLE_XML_FILE = @"D:/Documents/visual studio 2010/Projects/cSharpTraining2/OpenNIConfig.xml";
            
            ScriptNode scriptNode;
            Context context = Context.CreateFromXmlFile(SAMPLE_XML_FILE, out scriptNode);

            DepthGenerator depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (depth == null)
            {
                Console.WriteLine("Sample must have a depth generator!");
                return;
            }

            MapOutputMode mapMode = depth.MapOutputMode;

            DepthMetaData depthMD = new DepthMetaData();

            Image<Bgr, Byte> Dis_image;

            Bgr colorR = new Bgr(0, 0, 250);
            Bgr colorG = new Bgr(0, 250, 0);
            Bgr colorB = new Bgr(250, 0, 0);

            String imgshow = "Image";
            CvInvoke.cvNamedWindow(imgshow);

            int cDistance = (int)(customDistance*1000);
            int cRange = (int)(customRange * 1000);
            int cUpperValue = cDistance + cRange;
            int cLowerValue = cDistance - cRange;
            while (true)
            {

                context.WaitOneUpdateAll(depth);

                depth.GetMetaData(depthMD);
                Dis_image = new Image<Bgr, Byte>((int)mapMode.XRes, (int)mapMode.YRes);
                int count0 = 0;
                int count1 = 0;
                int count2 = 0;
                for (int y = 0; y < depthMD.YRes; ++y )
                {
                    for (int x = 0; x < depthMD.XRes; ++x )
                    {
                        int z = depthMD[x,y];
                        if (z == 0)
                        {
                            //pixels with zero value
                            Dis_image[y, x] = colorB;
                            count0++;
                        }
                        else if (z < cLowerValue || z > cUpperValue)
                        {
                            //pixel values outside range
                            Dis_image[y, x] = colorR;
                            count1++;
                        }
                        else
                        {
                            //pixel values inside range
                            Dis_image[y, x] = colorG;
                            count2++;
                        }
                    }
                }
                CvInvoke.cvShowImage(imgshow, Dis_image);
                if (CvInvoke.cvWaitKey(10) == 27)
                {
                    break;
                }
                double totalPixels = mapMode.XRes * mapMode.YRes;
                Console.WriteLine("Frame {0} Middle point is: {1}.", depthMD.FrameID, depthMD[(int)mapMode.XRes / 2, (int)mapMode.YRes / 2]);
                Console.WriteLine("numbers of pixels with value of 0: {0}\t[{1:p1}]", count0, (double)(count0 / totalPixels));
                Console.WriteLine("numbers of pixels out of default range: {0}\t[{1:p1}]", count1, (double)(count1 / totalPixels));
                Console.WriteLine("numbers of pixels within the range: {0}\t[{1:p1}]", count2, (double)(count2 / totalPixels));
            }

           CvInvoke.cvDestroyWindow(imgshow);
        }

        static void Main(string[] args)
        {
            //Read from Xml file to configure the test param.
            XmlDocument TestParam = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;

            TestParam.Load(@"D:/Documents/visual studio 2010/Projects/cSharpTraining2/OpenNIConfig.xml");

            XmlNode rootNode = TestParam.SelectSingleNode("OpenNI");
            XmlNode DepthSensorTestNode = rootNode.SelectSingleNode("DepthSensorTest");
            XmlNodeList testDetailList = DepthSensorTestNode.ChildNodes;
            CustomXmlData paramData = new CustomXmlData();

            foreach (XmlNode param1 in testDetailList)
            {

                 XmlElement paramElement = (XmlElement)param1;
                 paramData.TestDistance = Convert.ToDouble(paramElement.GetAttribute("Distance").ToString());
                 paramData.ErrorRange = Convert.ToDouble(paramElement.GetAttribute("ErrorRange").ToString()); 
            }

            Console.WriteLine("display the param{0}", paramData.TestDistance);
            Console.WriteLine("display the param{0}", paramData.ErrorRange);

            try
            {
                Run(paramData.TestDistance,paramData.ErrorRange);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}