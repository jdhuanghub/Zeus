using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;


using OpenNIWrapper;

namespace LaserPreHeat
{
    public partial class MainForm : Form
    {
        private int secondsToExecute  = new int();//20 seconds
        private int[] PgBarValue = new int[6];
        private int[] PgBarPercent = new int[6];
        private bool BGexecuteState = true;
        private static string[] displayIDqueue = new string[6];
        private string[] connectedUriList;
        private string[] previousPortIDList;
        private string[] connectedPortIDList;
        
        public MainForm()
        {
            InitializeComponent();
            programStart();
        }

        private void programStart()
        {
            secondsToExecute = Int32.Parse(ConfigurationManager.AppSettings["SetTime"]);
            label8.Text = "预热时间：" + secondsToExecute + "(秒)";
            backgroundWorker1.RunWorkerAsync();
        }

        #region BackgroundThread
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker BGoperation = sender as BackgroundWorker;
            //main operation following...
            displayOperation(BGoperation);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            renewProgressBarDisplay(progressBarEx1, label1, 1, PgBarPercent[0]);
            renewProgressBarDisplay(progressBarEx2, label2, 2, PgBarPercent[1]);
            renewProgressBarDisplay(progressBarEx3, label3, 3, PgBarPercent[2]);
            renewProgressBarDisplay(progressBarEx4, label4, 4, PgBarPercent[3]);
            renewProgressBarDisplay(progressBarEx5, label5, 5, PgBarPercent[4]);
            renewProgressBarDisplay(progressBarEx6, label6, 6, PgBarPercent[5]);
        }

        private void renewProgressBarDisplay(ProgressBarEx.ProgressBarEx probressBarObject, System.Windows.Forms.Label labelObject, int labelText, int percentComplete)
        {
            probressBarObject.Value = percentComplete;
            if (percentComplete != 0)
            {
                labelObject.Text = "端口 " + labelText + ": 监控中";
                if (percentComplete != 100)
                {
                    probressBarObject.ProgressColor = System.Drawing.Color.Red;
                }
                if (percentComplete == 100)
                {
                    probressBarObject.ProgressColor = System.Drawing.Color.Lime;
                }
            }
            else { labelObject.Text = "端口 " + labelText + ": "; }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion //BackgroundThread

        #region OpenNIapi
        private void HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.Ok)
            {
                return;
            }

            MessageBox.Show(
                string.Format(@"Error: {0} - {1}", status, OpenNI.LastError),
                @"Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
        }

        private void uriParsing(string[] InputUri,int uriIndex, out string uniquePortID)
        {
            uniquePortID = null;
            if (string.IsNullOrEmpty(InputUri[0]))
            {
                uniquePortID = null;
            }
            //get rid of first "&"
            int startIndex = InputUri[uriIndex].IndexOf("&") + "&".Length;
            string subInputUri = InputUri[uriIndex].Substring(startIndex, InputUri[uriIndex].Length - startIndex);
            //get rid of second "&"
            int secondIndex = subInputUri.IndexOf("&") + "&".Length;
            string theRestUri = subInputUri.Substring(secondIndex, subInputUri.Length - secondIndex);
            //now get the id string
            int first = theRestUri.IndexOf("&") + "&".Length;
            int last = theRestUri.LastIndexOf("&");

            uniquePortID = theRestUri.Substring(first, last - first - 2);
        }

        private void UpdateDevicesList()
        {
            previousPortIDList = connectedPortIDList;
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            connectedUriList = new string[devices.Length];
            connectedPortIDList = new string[devices.Length];

            int listIndex = 0;
            foreach (DeviceInfo onlineDevice in devices)
            {
                connectedUriList[listIndex] = onlineDevice.Uri;
                //Enable the laser as soon as the device plug in
                DevFunction.EnableLaser(connectedUriList[listIndex], 1);
                uriParsing(connectedUriList, listIndex,out connectedPortIDList[listIndex]);
                listIndex++;
            }
        }

        private void OpenNiOnDeviceConnectionStateChanged(DeviceInfo device)
        {
            this.BeginInvoke(new MethodInvoker(this.UpdateDevicesList));
        }
        #endregion //OpenNIapi

        #region UImainform
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.HandleError(OpenNI.Initialize());
//             OpenNI.OnDeviceConnected += this.OpenNiOnDeviceConnectionStateChanged;
//             OpenNI.OnDeviceDisconnected += this.OpenNiOnDeviceConnectionStateChanged;
//             this.UpdateDevicesList();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BGexecuteState = false;
            OpenNI.Shutdown();
        }

        private void FindoutDeviceStatusChange(string[] previousIDarray, string[] currentIDarray,out string theDifferenceArray, out int newDeviceFlag)
        {
            theDifferenceArray = null;
            newDeviceFlag = 0;
            if (previousIDarray.Length < currentIDarray.Length)
            {
                foreach (string IDarray in currentIDarray)
                {
                    if (!previousIDarray.Contains(IDarray))
                    {
                        theDifferenceArray = IDarray;
                        newDeviceFlag = 1;
                    }
                }
            }
            if (previousIDarray.Length > currentIDarray.Length)
            {
                foreach (string IDarray2 in previousIDarray)
                {
                    if (!currentIDarray.Contains(IDarray2))
                    {
                        theDifferenceArray = IDarray2;
                        newDeviceFlag = -1;
                    }
                }
            }
        }

        private void pushItemInIDqueue(string InputItemID)
        {
            for (int searchIndex = 0; searchIndex < displayIDqueue.Length;searchIndex++ )
            {
                string disConnectedmark = "&";
                if (displayIDqueue[searchIndex] == InputItemID)
                {
                    break;
                }
                if (displayIDqueue[searchIndex] == (disConnectedmark + InputItemID))
                {
                    displayIDqueue[searchIndex] = InputItemID;
                    break;
                }
                if (displayIDqueue[searchIndex] == null)
                {
                    displayIDqueue[searchIndex] = InputItemID;
                    break;
                }
            }
        }

        private void clearValueInIDqueue(string ItemToBeSet)
        {
            for (int searchIndex = 0; searchIndex < displayIDqueue.Length;searchIndex++ )
            {
                string disConnectedmark = "&";
                if (displayIDqueue[searchIndex] == ItemToBeSet)
                {
                    displayIDqueue[searchIndex] = (disConnectedmark + ItemToBeSet);
                }
            }
        }

        private void renewProgressBarValue()
        {
            for (int PgBarIndex = 0; PgBarIndex < displayIDqueue.Length;PgBarIndex++ )
            {
                if (string.IsNullOrEmpty(displayIDqueue[PgBarIndex]))
                {
                    PgBarValue[PgBarIndex] = 0;
                    PgBarPercent[PgBarIndex] = 0;
                    break;
                }
                int indexOfmark = displayIDqueue[PgBarIndex].IndexOf("&");
                if (indexOfmark == 0)
                {
                    PgBarValue[PgBarIndex] = 0;
                    PgBarPercent[PgBarIndex] = 0;
                }
                if (indexOfmark == -1)
                {
                    PgBarValue[PgBarIndex]++;
                }
                PgBarPercent[PgBarIndex] = (int)((float)PgBarValue[PgBarIndex] / (float)secondsToExecute * 100);
                if (PgBarPercent[PgBarIndex] > 100)
                {
                    PgBarPercent[PgBarIndex] = 100;
                }
            }
        }

        private void displayOperation(BackgroundWorker bgOperation)
        {
            string theChangingInputID;
            int DeviceStatusFlag;
            while (BGexecuteState)
            {   
                UpdateDevicesList();
                if (previousPortIDList == null)
                {
                    UpdateDevicesList();//No devices detected, assign the initial value to prevent failure.
                }
                
                FindoutDeviceStatusChange(previousPortIDList, connectedPortIDList, out theChangingInputID, out  DeviceStatusFlag);
                if (DeviceStatusFlag == 1)
                {
                    pushItemInIDqueue(theChangingInputID);
                    //MessageBox.Show(theChangingInputID, "New Device connected");
                }
                if (DeviceStatusFlag == -1)
                {
                    clearValueInIDqueue(theChangingInputID);
                    //MessageBox.Show(theChangingInputID, "Device connection Lost");
                }

                //now continue to display on the main form
                renewProgressBarValue();

                bgOperation.ReportProgress(PgBarPercent[0]);
                bgOperation.ReportProgress(PgBarPercent[1]);
                bgOperation.ReportProgress(PgBarPercent[2]);
                bgOperation.ReportProgress(PgBarPercent[3]);
                bgOperation.ReportProgress(PgBarPercent[4]);
                bgOperation.ReportProgress(PgBarPercent[5]);

                System.Threading.Thread.Sleep(1000);
            }
        }

        #endregion //UImainform
    }
}
