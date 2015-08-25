namespace ReplayTv
{
    using System;

    public class DuplicateDeviceException : ApplicationException
    {
        public DuplicateDeviceException(string message) : base(message)
        {
        }
    }
}
