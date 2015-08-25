namespace ReplayTv
{
    using System;

    public class DuplicateIPException : ApplicationException
    {
        public DuplicateIPException(string message) : base(message)
        {
        }
    }
}
