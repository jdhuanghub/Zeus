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
using System.Configuration;

namespace AssemblyLinePreheatGUI_Csharp
{
    public partial class Form1 : Form
    {
        #region variables
        private bool loopControl = false;
        private const int Inquiry_vals = 1000;
        private int secondsToExecute = 0;
        private int totalDeviceCount = 0;

        private int prograeeBarCount1 = 0;
        private int prograeeBarCount2 = 0;
        private int prograeeBarCount3 = 0;
        private int prograeeBarCount4 = 0;
        private int prograeeBarCount5 = 0;

        private int percentComplete1 = 0;
        private int percentComplete2 = 0;
        private int percentComplete3 = 0;
        private int percentComplete4 = 0;
        private int percentComplete5 = 0;
        #endregion//variables

        public Form1()
        {
            InitializeComponent();
            programStart();
        }

        #region backgroundWorker thread

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker timeWorker = sender as BackgroundWorker;
            PreHeatoperation(ref totalDeviceCount ,timeWorker);
        }

        //update the progress bar
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DevFunction.progressBarStatus(progressBar1, label1, 1, percentComplete1);

            DevFunction.progressBarStatus(progressBar2, label2, 2, percentComplete2);

            DevFunction.progressBarStatus(progressBar3, label3, 3, percentComplete3);

            DevFunction.progressBarStatus(progressBar4, label4, 4, percentComplete4);

            DevFunction.progressBarStatus(progressBar5, label5, 5, percentComplete5);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (totalDeviceCount == 0)
            {
                panel1.BackColor = System.Drawing.Color.Red;
            }
        }

        #endregion //backgroundWorker thread

        private void programStart()
        {
            //Get the time value from the app.config file
            secondsToExecute = Int32.Parse(ConfigurationManager.AppSettings["SetTime"]);
            label7.Text = "预热时间：" + secondsToExecute + "(秒)";
            //reset the progress bar count to 0
            prograeeBarCount1 = 0;
            prograeeBarCount2 = 0;
            prograeeBarCount3 = 0;
            prograeeBarCount4 = 0;
            prograeeBarCount5 = 0;

            percentComplete1 = 0;
            percentComplete2 = 0;
            percentComplete3 = 0;
            percentComplete4 = 0;
            percentComplete5 = 0;

            //Start the time consuming operation
            loopControl = true;
            backgroundWorker1.RunWorkerAsync(Inquiry_vals);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            loopControl = false;
            this.backgroundWorker1.CancelAsync();
            MessageBox.Show("再次启动预热程序之前，先将对应端口设备都连接上！", "提示：");
        }

        private void PreHeatoperation(ref int devicesTotal, BackgroundWorker timeWorker)
        {
            //Orbbec Driver
            //ClassGuid={6bdd1fc6-810f-11d0-bec7-08002be2092f}
            //Guid GUID_CLASS_OBDRV_USB = new Guid(0x6bdd1fc6, 0x810f, 0x11d0, 0xbe, 0xc7, 0x08, 0x00, 0x2b, 0xe2, 0x09, 0x2f);
            //
            //Common driver for Orbbec and PS
            Guid GUID_CLASS_OBDRV_USB = new Guid(0xc3b5f022, 0x5a42, 0x1980, 0x19, 0x09, 0xea, 0x72, 0x09, 0x56, 0x01, 0xb1);

            string[] IDarray = new string[10];//support 10 devices
            bool RetVal = false;
            DevFunction.DevDetail SensorData = new DevFunction.DevDetail();
            SensorData.DevPathArray = new string[10];

            //before the loop search the connect device and save the id string...
            RetVal = DevFunction.DevPathSearching(ref SensorData, GUID_CLASS_OBDRV_USB);
            if (string.IsNullOrEmpty(SensorData.DevPathArray[0]))
            {
                loopControl = false;
                SensorData.DeviceIndex = 0;
                IDarray[0] = string.Empty;
                devicesTotal = 0;
                //MessageBox.Show("未检测到设备！", "错误");
            }
            else
            {
                for (uint i = 0; i <= SensorData.DeviceIndex; i++)
                {
                    DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
                }
                devicesTotal = (int)SensorData.DeviceIndex;
            }
            
            while (loopControl)
            {
                #region Turn on laser emitter

                int laserInitialValue = 1;
                for (uint i = 0; i <= SensorData.DeviceIndex;i++ )
                {
                    DevFunction.EnableLaser(ref SensorData, i, laserInitialValue);
                }

                #endregion

                #region Checking devices

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
                #endregion //Checking devices

                #region Progressbar display

                DevFunction.progressSetValue(IDarray[0], ref percentComplete1, ref prograeeBarCount1, secondsToExecute);

                DevFunction.progressSetValue(IDarray[1], ref percentComplete2, ref prograeeBarCount2, secondsToExecute);

                DevFunction.progressSetValue(IDarray[2], ref percentComplete3, ref prograeeBarCount3, secondsToExecute);

                DevFunction.progressSetValue(IDarray[3], ref percentComplete4, ref prograeeBarCount4, secondsToExecute);

                DevFunction.progressSetValue(IDarray[4], ref percentComplete5, ref prograeeBarCount5, secondsToExecute);
                
                timeWorker.ReportProgress(percentComplete1);
                timeWorker.ReportProgress(percentComplete2);
                timeWorker.ReportProgress(percentComplete3);
                timeWorker.ReportProgress(percentComplete4);
                timeWorker.ReportProgress(percentComplete5);
                
                #endregion// Progress bar display

                //system sleep for particular intervals, 1seconds here.
                System.Threading.Thread.Sleep(Inquiry_vals);

            }// While Loop

        }

    }
}
