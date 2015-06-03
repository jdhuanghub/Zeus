
namespace DepthSensorTest
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Xml;

    using OpenNIWrapper;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;
    using Emgu.Util;
    using Emgu.CV.UI;

    #endregion

    public partial class TestProgramWindow : Form
    {
        #region  globalFields

        private VideoStream depthStream;
        
        private Device currentDevice;

        bool programRunning;

        string CUSTOM_XML_FILE = null;

        const String DISPLAY_IMAGE = "Image(Press Esc to exit)";

        #endregion

        #region XML Data

        public class CustomXmlData
        {
            public double testDistance;
            public double TestDistance
            {
                get { return testDistance; }
                set { testDistance = value; }
            }

            public double errorRange;
            public double ErrorRange
            {
                get { return errorRange; }
                set { errorRange = value; }
            }

            public int xRes;
            public int XRes
            {
                get { return xRes; }
                set { xRes = value; }
            }

            public int yRes;
            public int YRes
            {
                get { return yRes; }
                set { yRes = value; }
            }

            public int fps;
            public int FPS
            {
                get { return fps; }
                set { fps = value; }
            }
        }

        private double customDistance;
        private double customRange;

        private int customXRes;
        private int customYRes;
        private int customFPS;

        #endregion

        public TestProgramWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            programRunning = true;
            if (this.cBoxDevices.SelectedItem == null || CUSTOM_XML_FILE == null)
            {
                MessageBox.Show("Please check the device and load XML file", "Test parameters NOT set");
                programRunning = false;
                return;
            }
            this.cBoxDevices.Enabled = false;
            this.StartBtn.Enabled = false;
            this.LoadXmlBtn.Enabled = false;
            depthStream = currentDevice.CreateVideoStream(Device.SensorType.Depth);
            depthStream.VideoMode = new VideoMode
            {
                DataPixelFormat = VideoMode.PixelFormat.Depth1Mm,
                Fps = customFPS,
                Resolution = new Size(customXRes, customYRes)
            };
            depthStream.Start();

            Image<Bgr, Byte> Dis_image;
            //String imgshow = "Image(Press Esc to exit)";
            CvInvoke.cvNamedWindow(DISPLAY_IMAGE);

            int distanceSet = (int)(customDistance * 1000);
            int rangeSet = (int)(customRange * 1000);
            int dUpperValue = distanceSet + rangeSet;
            int dLowerValue = distanceSet - rangeSet;
            programRunning = true;
            while (programRunning)
            {
                if (depthStream.IsValid && depthStream.IsFrameAvailable())
                {
                    using (VideoFrameRef depthframe = depthStream.ReadFrame())
                    {
                        if (depthframe.IsValid)
                        {
                            Dis_image = new Image<Bgr, Byte>((int)depthframe.FrameSize.Width,(int)depthframe.FrameSize.Height);

                            int count0 = 0;
                            int count1 = 0;
                            int count2 = 0;

                            int depthWidth = depthframe.FrameSize.Width;
                            int depthHeight = depthframe.FrameSize.Height;
                            Int16[] depth = new Int16[depthWidth*depthHeight];
                            Marshal.Copy(depthframe.Data, depth, 0, depth.Length);

                            if (this.cb_mirror.Checked)
                            {
                                int count00 = 0;
                                int count01 = 0;
                                int count02 = 0;
                                int z = 0;
                                for (int y = 0; y < depthHeight; ++y)
                                {
                                    for (int x = depthWidth - 1; x >= 0; --x)
                                    {
                                        int pz = depth[z];
                                        if (pz == 0)
                                        {
                                            //pixels with zero value
                                            Dis_image.Data[y, x, 0] = 250;
                                            count00++;
                                        }
                                        else if (pz < dLowerValue || pz > dUpperValue)
                                        {
                                            //pixel values outside range
                                            Dis_image.Data[y, x, 2] = 250;
                                            count01++;
                                        }
                                        else
                                        {
                                            //pixel values inside range
                                            Dis_image.Data[y, x, 1] = 250;
                                            count02++;
                                        }
                                        ++z;
                                    }
                                }
                                count0 = count00;
                                count1 = count01;
                                count2 = count02;
                            }
                            else
                            {
                                int count10 = 0;
                                int count11 = 0;
                                int count12 = 0;
                                int z = 0;
                                for (int y = 0; y < depthHeight; ++y)
                                {
                                    for (int x = 0; x < depthWidth; ++x)
                                    {
                                        int pz = depth[z];
                                        if (pz == 0)
                                        {
                                            //pixels with zero value
                                            Dis_image.Data[y, x, 0] = 250;
                                            count10++;
                                        }
                                        else if (pz < dLowerValue || pz > dUpperValue)
                                        {
                                            //pixel values outside range
                                            Dis_image.Data[y, x, 2] = 250;
                                            count11++;
                                        }
                                        else
                                        {
                                            //pixel values inside range
                                            Dis_image.Data[y, x, 1] = 250;
                                            count12++;
                                        }
                                        ++z;
                                    }
                                }
                                count0 = count10;
                                count1 = count11;
                                count2 = count12;
                            }

                            double totalPiexls = depthWidth*depthHeight;
                            int middlePixel = (int)(totalPiexls/2 - depthWidth/2); 
                            zeropixel.Text = "Pixel value of 0: " + count0;
                            invalidpixel.Text = "Pixel value outside range: " + count1;
                            validpixel.Text = "Pixel value inside range: " + count2;
                            centerDistance.Text = "Center Distance: " + depth[middlePixel];
                            percentage1.Text = "percentage(%): " + ((double)(100 * count0 / totalPiexls)).ToString("0.00") + "%";
                            percentage2.Text = "percentage(%): " + ((double)(100 * count1 / totalPiexls)).ToString("0.00") + "%";
                            percentage3.Text = "percentage(%): " + ((double)(100 * count2 / totalPiexls)).ToString("0.00") + "%";
                            Frame.Text = "NO.Frame: " + depthframe.FrameIndex;
                            CvInvoke.cvShowImage(DISPLAY_IMAGE, Dis_image);
                            if (CvInvoke.cvWaitKey(10) == 27)
                            {
                                break;
                            }

                        }
                    }
                }
            }
            CvInvoke.cvDestroyWindow(DISPLAY_IMAGE);
            this.cBoxDevices.Enabled = true;
            this.StartBtn.Enabled = true;
            this.LoadXmlBtn.Enabled = true;
        }

        private void LoadXmlBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                //Load Xml File
                CUSTOM_XML_FILE = Openfile.FileName;
                XmlDocument TestParam = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader readxml = XmlReader.Create(CUSTOM_XML_FILE, settings);
                TestParam.Load(readxml);

                CustomXmlData customSetData = new CustomXmlData();
                XmlNode rootNode = TestParam.SelectSingleNode("OpenNI2");

                XmlNodeList DisplayNode = rootNode.SelectNodes("DisplayResolution");
                //XmlNodeList DisplayNodeList = DisplayNode;
                foreach (XmlNode MapParam in DisplayNode)
                {
                    XmlElement DisplayParam = (XmlElement)MapParam;
                    customSetData.XRes = Convert.ToInt32(DisplayParam.GetAttribute("xRes"));
                    customSetData.YRes = Convert.ToInt32(DisplayParam.GetAttribute("yRes"));
                    customSetData.FPS = Convert.ToInt32(DisplayParam.GetAttribute("FPS"));
                }

                XmlNodeList PositionNode = rootNode.SelectNodes("SensorPosition");
                //XmlNodeList PositionNodeList = PositionNode.ChildNodes;
                foreach (XmlNode PositionParam in PositionNode)
                {
                    XmlElement SetPlace = (XmlElement)PositionParam;
                    customSetData.TestDistance = Convert.ToDouble(SetPlace.GetAttribute("Distance"));
                    customSetData.ErrorRange = Convert.ToDouble(SetPlace.GetAttribute("ErrorRange"));
                }

                SetDistance.Text = "Target Distance: " + customSetData.testDistance.ToString() + " m";
                ErrorRange.Text = "Error Range: " + customSetData.errorRange.ToString() + " m";
                this.customDistance = customSetData.testDistance;
                this.customRange = customSetData.errorRange;
                this.customXRes = customSetData.xRes;
                this.customYRes = customSetData.yRes;
                this.customFPS = customSetData.fps;
                readxml.Close();
            }
        }

        #region Program Init&Ending

        private void cBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*this.cBoxSensors.Items.Clear();*/
            if (this.cBoxDevices.SelectedItem != null)
            {
                if (this.currentDevice != null)
                {
                    this.currentDevice.Dispose();
                }

                this.currentDevice = ((DeviceInfo)this.cBoxDevices.SelectedItem).OpenDevice();
            }
        }

        private void HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.Ok)
            {
                return;
            }

            MessageBox.Show(string.Format(@"Error: {0} - {1}", status, OpenNI.LastError),
                            @"Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
        }

        private void OpenNIOnDeviceConnectionStateChanged(DeviceInfo device)
        {
            this.BeginInvoke(new MethodInvoker(this.UpdateDevicesList));
        }

        private void UpdateDevicesList()
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            this.cBoxDevices.Items.Clear();
            foreach (DeviceInfo device in devices)
            {
                this.cBoxDevices.Items.Add(device);
            }
        }

        private void TestProgramWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            programRunning = false;
            CvInvoke.cvDestroyWindow(DISPLAY_IMAGE);
            OpenNI.Shutdown();
        }

        private void TestProgramWindow_Load(object sender, EventArgs e)
        {
            this.HandleError(OpenNI.Initialize());
            OpenNI.OnDeviceConnected += this.OpenNIOnDeviceConnectionStateChanged;
            OpenNI.OnDeviceDisconnected -= this.OpenNIOnDeviceConnectionStateChanged;
            this.UpdateDevicesList();
        }

        #endregion

    }
}
