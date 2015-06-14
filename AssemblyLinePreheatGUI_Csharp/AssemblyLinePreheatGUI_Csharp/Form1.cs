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
        private string mFolder;
        private Thread mWorkingThread;
        private Process mProcess;
        private CUSBDevices mDevice;

        public Form1()
        {
            InitializeComponent();
            //mFolder = string.Empty;
            //mWorkingThread = new Thread(new ThreadStart(StartUSBDeviceEnum));
            mProcess = new Process();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {

            Startbtn.Enabled = false;
            Exitbtn.Enabled = false;

            mWorkingThread.Start();
        }
// 
//         private void StartUSBDeviceEnum()
//         {
//             mDevice = new CUSBDevices();
//             mDevice.StartProgress += new EventHandler(mDevice_StartProgress);
//             mDevice.EndProgress += new EventHandler(mDevice_EndProgress);
// 
// 
//             mDevice.Start(mFolder);
//         }
// 
//         private void mDevice_StartProgress(object sender, EventArgs e)
//         {
//             if (Startbtn.InvokeRequired)
//                 this.Invoke(new EventHandler(mDevice_StartProgress), new object[] { sender, e });
//             else
//             {
//                 Startbtn.Enabled = false;
//                 Exitbtn.Enabled = false;
//             }
//         }
// 
//         private void mDevice_EndProgress(object sender, EventArgs e)
//         {
//             if (Exitbtn.InvokeRequired)
//                 this.Invoke(new EventHandler(mDevice_EndProgress), new object[] { sender, e });
//             else
//             {
//                 Exitbtn.Enabled = true;
//                 mFolder = string.Empty;
//                 mProcess.Start();
//             }
//         }

        private void Exitbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
