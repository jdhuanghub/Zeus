using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace AssemblyLinePreheatGUI_Csharp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker BWLoop = sender as BackgroundWorker;

            //Extract the argument.
            int arg = (int)e.Argument;

            //Start the time consuming loop
            e.Result = BGoperation(BWLoop, arg);

            //If the operation was canceled by the user,
            //set the DoWorkEventArgs  Cancel property to true.
            if (BWLoop.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //Manual canceled by user
                MessageBox.Show("Operation was canceled");
            }
            else
            {
                //The operation completed normally
//                 string msg = string.Format("Result = {0}", e.Result);
//                 MessageBox.Show(msg);
            }
        }

        private int BGoperation(BackgroundWorker BWLoop, int Inquiry_Intervals)
        {
            
            int result = 0;

            Startbtn.Enabled = false;
            Exitbtn.Enabled = true;
            bool loopControl = true;

            //Guid GUID_CLASS_OBDRV_USB = new Guid("c3b5f0225a4219801909ea72095601b1");
            Guid GUID_CLASS_OBDRV_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);

            string[] IDarray = new string[10];//support 10 devices
            bool RetVal = false;
            //int Inquiry_Intervals = 120;
            DevFunction.DevDetail SensorData = new DevFunction.DevDetail();
            SensorData.DevPathArray = new string[10];

            //before the loop search the connect device and save the id string...
            RetVal = DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
            if (string.IsNullOrEmpty(SensorData.DevPathArray[0]))
            {
                loopControl = false;
                SensorData.DeviceIndex = 0;
                IDarray[0] = string.Empty;
                MessageBox.Show("未检测到设备！", "错误");
            }
            else
            {
                for (uint i = 0; i <= SensorData.DeviceIndex; i++)
                {
                    DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
                }

            }

            //Start the loop
            int LoopCounter = 0;

            while (loopControl)
            {
                string DeviceUniqueID = null;
                //See if there are new devices connecting
                uint CurrentDevCount = 0;
                RetVal = DevFunction.DeviceConnectInquiry(ref SensorData.DeviceIndex, ref CurrentDevCount, GUID_CLASS_OBDRV_USB);
                if (RetVal == true)
                {
                    DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
                    //System.Windows.Forms.MessageBox.Show("New device plug in!", "New Device");
                }

                //Check if a device is disconnect
                for (uint i = 0; i <= SensorData.DeviceIndex; i++)
                {
                    RetVal = DevFunction.DevicePathValidate(ref SensorData, i);
                    if (RetVal == false)
                    {
                        //Parsing the device ID to see which one is unplug
                        DevFunction.DevicePathIDParsing(ref SensorData, i, ref DeviceUniqueID);
                        //System.Windows.Forms.MessageBox.Show(DeviceUniqueID,"DeviceID");
                    }

                    if (IDarray[i].Equals(DeviceUniqueID) == true)
                    {
                        //string displayLostID = "Port: " + i +" Lost connection";
                        IDarray[i] = string.Empty;
                        //System.Windows.Forms.MessageBox.Show(displayLostID);

                    }
                    else
                    {
                        DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
                    }
                }
                LoopCounter++;
                //1 minutes time reached.
                if (LoopCounter >= 62)
                {
                    LoopCounter = 0;
                    //loopControl = false;
                    //System.Windows.Forms.MessageBox.Show("1 Minutes time reached! reseting timer", "Timer event");
                }
                #region Progressbar display

                bool[] pbCtl = new bool[] { false, false, false, false, false };//4 usb port for now
                if (progressBar1.Value == 60)
                {
                    pbCtl[0] = true;
                }
                if (progressBar2.Value == 60)
                {
                    pbCtl[1] = true;
                }
                if (progressBar3.Value == 60)
                {
                    pbCtl[2] = true;
                }
                if (progressBar4.Value == 60)
                {
                    pbCtl[3] = true;
                }
                if (progressBar5.Value == 60)
                {
                    pbCtl[4] = true;
                }
                if (pbCtl[0] /*& pbCtl[1] & pbCtl[2] & pbCtl[3] & pbCtl[4]*/)
                {
                    loopControl = false;
                }

                if (!string.IsNullOrEmpty(IDarray[0]))
                {
                    progressBar1.Increment(1);
                }
                else { progressBar1.Value = 1; }
                if (!string.IsNullOrEmpty(IDarray[1]))
                {
                    progressBar2.Increment(1);
                }
                else { progressBar2.Value = 1; }
                if (!string.IsNullOrEmpty(IDarray[2]))
                {
                    progressBar3.Increment(1);
                }
                else { progressBar3.Value = 1; }
                if (!string.IsNullOrEmpty(IDarray[3]))
                {
                    progressBar4.Increment(1);
                }
                else { progressBar4.Value = 1; }
                if (!string.IsNullOrEmpty(IDarray[4]))
                {
                    progressBar5.Increment(1);
                }
                else { progressBar5.Value = 1; }

                #endregion// Progress bar display


                //system sleep for particular intervals, 1seconds here.
                System.Threading.Thread.Sleep(Inquiry_Intervals);
                Exitbtn.Enabled = true;
            }// While Loop
            Startbtn.Enabled = true;
            Exitbtn.Enabled = true;

            return result;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync(200);

            #region previous code
            //             Startbtn.Enabled = false;
//             Exitbtn.Enabled = true;
//             bool loopControl = true;
// 
//             //Guid GUID_CLASS_OBDRV_USB = new Guid("c3b5f0225a4219801909ea72095601b1");
//             Guid GUID_CLASS_OBDRV_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);
// 
//             string[] IDarray = new string[10];//support 10 devices
//             bool RetVal = false;
//             const int Inquiry_Intervals = 120;
//             DevFunction.DevDetail SensorData = new DevFunction.DevDetail();
//             SensorData.DevPathArray = new string[10];
// 
//             //before the loop search the connect device and save the id string...
//             RetVal = DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
//             if (string.IsNullOrEmpty(SensorData.DevPathArray[0]))
//             {
//                 loopControl = false;
//                 SensorData.DeviceIndex = 0;
//                 IDarray[0] = string.Empty;
//                 MessageBox.Show("未检测到设备！", "错误");
//             }
//             else
//             {
//                 for (uint i = 0; i <= SensorData.DeviceIndex; i++)
//                 {
//                     DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
//                 }
// 
//             }
// 
//             //Start the loop
//             int LoopCounter = 0;
//             
//             while (loopControl)
//             {
//                 string DeviceUniqueID = null;
//                 //See if there are new devices connecting
//                 uint CurrentDevCount = 0;
//                 RetVal = DevFunction.DeviceConnectInquiry(ref SensorData.DeviceIndex, ref CurrentDevCount, GUID_CLASS_OBDRV_USB);
//                 if (RetVal == true)
//                 {
//                     DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
//                     //System.Windows.Forms.MessageBox.Show("New device plug in!", "New Device");
//                 }
// 
//                 //Check if a device is disconnect
//                 for (uint i = 0; i <= SensorData.DeviceIndex; i++ )
//                 {
//                     RetVal = DevFunction.DevicePathValidate(ref SensorData,i);
//                     if (RetVal == false)
//                     {
//                         //Parsing the device ID to see which one is unplug
//                         DevFunction.DevicePathIDParsing(ref SensorData, i, ref DeviceUniqueID);
//                         //System.Windows.Forms.MessageBox.Show(DeviceUniqueID,"DeviceID");
//                     }
//                     
//                     if (IDarray[i].Equals(DeviceUniqueID) == true)
//                     {
//                         //string displayLostID = "Port: " + i +" Lost connection";
//                         IDarray[i] = string.Empty;
//                         //System.Windows.Forms.MessageBox.Show(displayLostID);
// 
//                     }
//                     else
//                     {
//                         DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
//                     }
//                 }
//                 LoopCounter++;
//                 //1 minutes time reached.
//                 if (LoopCounter >= 62)
//                 {
//                     LoopCounter = 0;
//                     //loopControl = false;
//                     //System.Windows.Forms.MessageBox.Show("1 Minutes time reached! reseting timer", "Timer event");
//                 }
//                 #region Progressbar display
// 
//                 bool[] pbCtl = new bool[] {false,false,false,false,false};//4 usb port for now
//                 if (progressBar1.Value == 60 )
//                 {
//                     pbCtl[0] = true;
//                 }
//                 if (progressBar2.Value == 60 )
//                 {
//                     pbCtl[1] = true;
//                 }
//                 if (progressBar3.Value == 60 )
//                 {
//                     pbCtl[2] = true;
//                 }
//                 if (progressBar4.Value == 60 )
//                 {
//                     pbCtl[3] = true;
//                 }
//                 if (progressBar5.Value == 60 )
//                 {
//                     pbCtl[4] = true;
//                 }
//                 if (pbCtl[0] /*& pbCtl[1] & pbCtl[2] & pbCtl[3] & pbCtl[4]*/)
//                 {
//                     loopControl = false;
//                 }
//                 
//                 if (!string.IsNullOrEmpty(IDarray[0]))
//                 {
//                     progressBar1.Increment(1);
//                 }
//                 else { progressBar1.Value = 1; }
//                 if (!string.IsNullOrEmpty(IDarray[1]))
//                 {
//                     progressBar2.Increment(1);
//                 }
//                 else { progressBar2.Value = 1; }
//                 if (!string.IsNullOrEmpty(IDarray[2]))
//                 {
//                     progressBar3.Increment(1);
//                 }
//                 else { progressBar3.Value = 1; }
//                 if (!string.IsNullOrEmpty(IDarray[3]))
//                 {
//                     progressBar4.Increment(1);
//                 }
//                 else { progressBar4.Value = 1; }
//                 if (!string.IsNullOrEmpty(IDarray[4]))
//                 {
//                     progressBar5.Increment(1);
//                 }
//                 else { progressBar5.Value = 1; }
// 
//                 #endregion// Progress bar display
// 
// 
//                 //system sleep for particular intervals, 1seconds here.
//                 System.Threading.Thread.Sleep(Inquiry_Intervals);
//                 Exitbtn.Enabled = true;
//             }// While Loop
//             Startbtn.Enabled = true;
            //             Exitbtn.Enabled = true;
            #endregion 

        }

        private void Exitbtn_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            this.Exitbtn.Enabled = false;
            //this.Close();
        }
    }
}
