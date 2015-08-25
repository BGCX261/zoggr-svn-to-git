namespace Zoggr
{
    using ReplayTv;
    using System;
    using System.Collections;

    public class ZoggrUploader
    {
        public ZoggrOptions options;

        public ZoggrUploader()
        {
            ZoggrLogger.Log(AppInfo.GetTitle() + " v" + AppInfo.GetVersion() + " - (c) 2008 Zoggr");
            ZoggrLogger.LogThanks();
            this.options = new ZoggrOptions();
            this.options.LoadConfiguration();
        }

        public void AutoAdd()
        {
            ZoggrLogger.Log("Attempting to auto-discover all ReplayTVs...");
            SortedList list = null;
            try
            {
                list = ReplayUPnP.DiscoverRTVs(this.options.Options.allowWiRNS);
                ZoggrLogger.DebugLog("Successful auto-discovery of all ReplayTVs...");
            }
            catch (Exception exception)
            {
                ZoggrLogger.Log(exception.Message);
            }
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        this.options.Options.rtvList.AddDevice((ReplayDevice)list.GetByIndex(i));
                    }
                    catch (DuplicateIPException exception2)
                    {
                        ZoggrLogger.Log(exception2.Message);
                    }
                    catch (Exception exception3)
                    {
                        ZoggrLogger.Log(exception3.Message);
                    }
                }
            }
            this.options.SaveConfiguration();
            ZoggrLogger.Log(string.Format("Found {0} ReplayTV{1} on the local area network.", list.Count, (list.Count <= 1) ? "" : "s"));
        }

        public void ManualAdd(string ip_address)
        {
            ZoggrLogger.Log("Attempting to manually add a ReplayTV...");
            try
            {
                ReplayDevice theDevice = new ReplayDevice(ip_address, this.options.Options.allowWiRNS);
                this.options.Options.rtvList.AddDevice(theDevice);
                this.options.SaveConfiguration();
                ZoggrLogger.Log(string.Format("Successfully added ReplayTV at {0}.", ip_address));
            }
            catch (Exception exception)
            {
                ZoggrLogger.Log(exception.Message);
            }
        }

        public void RefreshGuides()
        {
            ZoggrLogger.DebugLog("Refreshing guide(s)...");
            ReplayDevice[] replayDevices = this.options.Options.rtvList.ReplayDevices;
            int length = replayDevices.Length;
            for (int i = 0; i < length; i++)
            {
                ReplayDevice rd = replayDevices[i];
                ReplayGuide guide = new ReplayGuide(rd);
                if (!ZoggrDriver.noGetGuide)
                {
                    guide.GetGuide();
                }
                else
                {
                    ZoggrLogger.DebugLog("noGetGuide");
                }
                bool showHDTV = true;
                guide.SaveGuideAsXML(this.options.Options.shareReceived, showHDTV);
            }
        }

        public void Remove(string serial_number)
        {
            ZoggrLogger.Log(string.Format("Removing ReplayTV {0}...", serial_number));
            try
            {
                this.options.Options.rtvList.RemoveDevice(serial_number);
                this.options.SaveConfiguration();
                ZoggrLogger.Log("Successfully removed ReplayTV.");
            }
            catch (Exception exception)
            {
                ZoggrLogger.Log(exception.Message);
            }
        }

        public void Run(bool auto_add, string manual_add, string remove_device, bool offlineMode)
        {
            ZoggrLogger.Log("Starting Zoggr Update...");
            try
            {
                if ((manual_add != null) && (manual_add.Length != 0))
                {
                    this.ManualAdd(manual_add);
                }
                if ((remove_device != null) && (remove_device.Length != 0))
                {
                    this.Remove(remove_device);
                }
                if (auto_add)
                {
                    this.AutoAdd();
                }
                if (!ZoggrDriver.noRefreshGuide)
                {
                    this.RefreshGuides();
                }
                else
                {
                    ZoggrLogger.DebugLog("noFreshGuide");
                }
                if (!offlineMode)
                {
                    this.UpdateZoggr();
                }
            }
            catch (Exception exception)
            {
                ZoggrLogger.Log(exception.Message);
            }
            ZoggrLogger.Log("Finished Zoggr Update!");
        }

        public void UpdateZoggr()
        {
            ZoggrLogger.Log("Updating your show database on Zoggr...");
            ReplayDevice[] replayDevices = this.options.Options.rtvList.ReplayDevices;
            int length = replayDevices.Length;
            for (int i = 0; i < length; i++)
            {
                ZoggrLogger.DebugLog("UpdateZoggr loop");
                ReplayDevice rd = replayDevices[i];
                if (!ZoggrDriver.noUpload)
                {
                    string filename = rd.serialNumber + ".xml";
                    int status = ZoggrWorker.UploadToZoggr(rd.serialNumber, filename);
                    ZoggrLogger.Log("UploadToZoggr status = {0}", status);
                    if (status == 1)    // if Success
                    {
                        ZoggrWorker.DeleteFile(filename);
                    }
                }
                ZoggrLogger.Log("Upload to Zoggr successful!");
            }
        }
    }
}
