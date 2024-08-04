using System;
using Framework.Constants.Net;

namespace AuthServer.AuthServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BnetServiceAttribute : Attribute
    {
        public BnetServiceHash Hash { get; set; }

        public BnetServiceAttribute(BnetServiceHash hash)
        {
            Hash = hash;
        }
    }
}
