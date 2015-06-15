using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace AssemblyLinePreheatGUI_Csharp
{
    public partial class Form1 : Form
    {

        public string[] IDarray = new string[10];//support 10 devices

        public Form1()
        {
            InitializeComponent();
            BackgroundWorker.work
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Startbtn.Enabled = false;
            Exitbtn.Enabled = true;
            //Guid GUID_CLASS_OBDRV_USB = new Guid("c3b5f0225a4219801909ea72095601b1");
            Guid GUID_CLASS_OBDRV_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);
            
            bool RetVal = false;
            const int Inquiry_Intervals = 100;
            DevFunction.DevDetail SensorData = new DevFunction.DevDetail();
            SensorData.DevPathArray = new string[10];

            //before the loop search the connect device and save the id string...
            RetVal = DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
            for (uint i = 0; i <= SensorData.DeviceIndex; i++)
            {
                DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
            }
            DetectedDeviceCount.Text = "检测到设备数：" + (SensorData.DeviceIndex+1);

            //Start the loop
            int LoopCounter = 0;

            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(loopKeyPress);

            while(true)
            {
                //See if there are new devices connecting
                uint CurrentDevCount = 0;
                RetVal = DevFunction.DeviceConnectInquiry(ref SensorData.DeviceIndex, ref CurrentDevCount, GUID_CLASS_OBDRV_USB);
                if (RetVal == true)
                {
                    DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
                    System.Windows.Forms.MessageBox.Show("New device plug in!", "New Device");
                }
                //Check if a device is disconnect
                string DeviceUniqueID = null;
                for (uint i = 0; i <= SensorData.DeviceIndex; i++ )
                {
                    RetVal = DevFunction.DevicePathValidate(ref SensorData,i);
                    if (RetVal == false)
                    {
                        //Parsing the device ID to see which one is unplug
                        DevFunction.DevicePathIDParsing(ref SensorData, i, ref DeviceUniqueID);
                        //System.Windows.Forms.MessageBox.Show(DeviceUniqueID,"DeviceID");
                    }
                    
                    if (IDarray[i].Equals(DeviceUniqueID) == true)
                    {
                        string displayLostID = "Port: " + i +" Lost connection";
                        //System.Windows.Forms.MessageBox.Show(displayLostID);
                        
                    }
                }
                LoopCounter++;
                //1 minutes time reached.
                if (LoopCounter >= 61)
                {
                    LoopCounter = 0;
                    progressBar1.Value = 0;
                    //System.Windows.Forms.MessageBox.Show("1 Minutes time reached! reseting timer", "Timer event");
                }

                //system sleep for particular intervals, 1seconds here.
                System.Threading.Thread.Sleep(Inquiry_Intervals);
            }

        }

        private void loopKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                System.Windows.Forms.MessageBox.Show("Form.KeyPress: '" + e.KeyChar.ToString() + "' pressed.");
                //q is pressed
                
            }
        }

        private void Exitbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
