namespace ReplayTv
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    [XmlRoot("replayDeviceList")]
    public class ReplayDeviceList
    {
        private ArrayList listReplayDevices = new ArrayList();

        public void AddDevice(ReplayDevice theDevice)
        {
            bool flag = false;
            bool flag2 = false;
            foreach (ReplayDevice device in this.listReplayDevices)
            {
                if (device.serialNumber == theDevice.serialNumber)
                {
                    flag = true;
                }
                if (device.ip == theDevice.ip)
                {
                    flag2 = true;
                }
            }
            if (!flag && !flag2)
            {
                this.listReplayDevices.Add(theDevice);
            }
            else
            {
                if (flag)
                {
                    throw new DuplicateDeviceException(string.Format("Another device with serial {0} is already listed!", theDevice.serialNumber));
                }
                throw new DuplicateIPException(string.Format("Another device with IP {0} is already listed!", theDevice.ip));
            }
        }

        public ReplayDevice GetDevice(string serialNumber)
        {
            foreach (ReplayDevice device in this.listReplayDevices)
            {
                if (device.serialNumber == serialNumber)
                {
                    return device;
                }
            }
            throw new DeviceNotExistException(string.Format("There is no device with serial number {0}!", serialNumber));
        }

        public void RemoveDevice(string serialNumber)
        {
            this.listReplayDevices.Remove(this.GetDevice(serialNumber));
        }

        [XmlElement("unit")]
        public ReplayDevice[] ReplayDevices
        {
            get
            {
                ReplayDevice[] array = new ReplayDevice[this.listReplayDevices.Count];
                this.listReplayDevices.CopyTo(array);
                return array;
            }
            set
            {
                if (value != null)
                {
                    ReplayDevice[] deviceArray = value;
                    this.listReplayDevices.Clear();
                    ReplayDevice[] deviceArray2 = deviceArray;
                    int length = deviceArray2.Length;
                    for (int i = 0; i < length; i++)
                    {
                        ReplayDevice device = deviceArray2[i];
                        this.listReplayDevices.Add(device);
                    }
                }
            }
        }
    }
}
