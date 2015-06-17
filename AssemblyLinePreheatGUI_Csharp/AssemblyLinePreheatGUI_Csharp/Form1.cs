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
        private const int Inquiry_vals = 1000;
        private int prograeeBarCount1 = 0;
        private int prograeeBarCount2 = 0;
        private int prograeeBarCount3 = 0;
        private int prograeeBarCount4 = 0;
        private int prograeeBarCount5 = 0;
        private bool loopControl = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker timeWorker = sender as BackgroundWorker;
            PreHeatoperation(timeWorker);
        }

        //update the progress bar
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = prograeeBarCount1;
            this.progressBar2.Value = prograeeBarCount2;
            this.progressBar3.Value = prograeeBarCount3;
            this.progressBar4.Value = prograeeBarCount4;
            this.progressBar5.Value = prograeeBarCount5;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.Startbtn.Enabled = false;
            this.Exitbtn.Enabled = true;

            //reset the progress bar count to 0
            prograeeBarCount1 = 0;
            prograeeBarCount2 = 0;
            prograeeBarCount3 = 0;
            prograeeBarCount4 = 0;
            prograeeBarCount5 = 0;

            //Start the time consuming operation
            loopControl = true;
            backgroundWorker1.RunWorkerAsync(Inquiry_vals);

        }

        private void Exitbtn_Click(object sender, EventArgs e)
        {
            loopControl = false;
            this.backgroundWorker1.CancelAsync();
            this.Startbtn.Enabled = true;
            MessageBox.Show("再次按下“开始”按钮之前，先将5个端口设备都连接上！", "提示：");
            //this.Close();
        }

        private void PreHeatoperation(BackgroundWorker timeWorker)
        {
            //Orbbec Driver
            //Class=Orbbec
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
                MessageBox.Show("未检测到设备！", "错误");
            }
            else
            {
                for (uint i = 0; i <= SensorData.DeviceIndex; i++)
                {
                    DevFunction.DevicePathIDParsing(ref SensorData, i, ref IDarray[i]);
                }

            }

            while (loopControl)
            {
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

                if (!string.IsNullOrEmpty(IDarray[0]))
                {
                    if (prograeeBarCount1 > 60)
                    { prograeeBarCount1 = 60; }
                    else { prograeeBarCount1++; }
                }else { prograeeBarCount1 = 0; }

                if (!string.IsNullOrEmpty(IDarray[1]))
                {
                    if (prograeeBarCount2 > 60)
                    { prograeeBarCount2 = 60; }
                    else { prograeeBarCount2++; }
                }else { prograeeBarCount2 = 0; }

                if (!string.IsNullOrEmpty(IDarray[2]))
                {
                    if (prograeeBarCount3 > 60)
                    { prograeeBarCount3 = 60; }
                    else { prograeeBarCount3++; }
                }else { prograeeBarCount3 = 0; }

                if (!string.IsNullOrEmpty(IDarray[3]))
                {
                    if (prograeeBarCount4 > 60)
                    { prograeeBarCount4 = 60; }
                    else { prograeeBarCount4++; }
                }else { prograeeBarCount4 = 0; }

                if (!string.IsNullOrEmpty(IDarray[4]))
                {
                    if (prograeeBarCount5 > 60)
                    { prograeeBarCount5 = 60; }
                    else { prograeeBarCount5++; }
                } else { prograeeBarCount5 = 0; }

                timeWorker.ReportProgress(prograeeBarCount1);
                timeWorker.ReportProgress(prograeeBarCount2);
                timeWorker.ReportProgress(prograeeBarCount3);
                timeWorker.ReportProgress(prograeeBarCount4);
                timeWorker.ReportProgress(prograeeBarCount5);

                #endregion// Progress bar display

                //system sleep for particular intervals, 1seconds here.
                System.Threading.Thread.Sleep(Inquiry_vals);

            }// While Loop

        }

    }
}
