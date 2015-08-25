namespace ReplayTv
{
    using System;

    public class DeviceNotExistException : ApplicationException
    {
        public DeviceNotExistException(string message) : base(message)
        {
        }
    }
}
