using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Zoggr;
using ReplayTv;

namespace ZoggrService
{
    public partial class ZoggrUpdater : ServiceBase
    {
        private DateTime workStartTime;
        private Timer serviceTimer;
        private ZoggrUploader uploader;

        public ZoggrUpdater()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            uploader = new ZoggrUploader();
            //int _interval = uploader.options.Options.updateFrequency;
            int _interval = 240000;  // 4 minutes

            TimerCallback timerDelegate = new TimerCallback(UpdateZoggr);

            // create timer and attach our method delegate to it
            serviceTimer = new Timer(timerDelegate, null, 1000, _interval);
        }

        protected override void OnStop()
        {
            serviceTimer.Dispose();
        }

        protected void UpdateZoggr(object stateObjec)
        {
            if (workStartTime != DateTime.MinValue)
            {
                // Previous UpdateZoggr is still running.  Log warning and stop.
                EventLog.WriteEntry("UpdateZoggr busy since " + workStartTime.ToLongTimeString(), System.Diagnostics.EventLogEntryType.Warning);
            }
            else
            {
                // set work start time
                workStartTime = DateTime.Now;

                // Note: Exception handling is very important here
                // if you don't, the error will vanish along with your worker thread
                try
                {
                    EventLog.WriteEntry("Timer Service Tick :" + DateTime.Now.ToString());

                    // Auto-Add ReplayTv's if none found
                    if (uploader.options.Options.rtvList.ReplayDevices.Length < 1)
                    {
                        uploader.AutoAdd();
                    }

                    // Refresh Guides
                    uploader.RefreshGuides();

                    // Send Data To Zoggr
                    uploader.UpdateZoggr();

                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry("UpdateZoggr error. " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
                finally
                {
                    // reset work start time
                    workStartTime = DateTime.MinValue;
                }
            }
        }
    }
}
