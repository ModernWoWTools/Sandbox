using System;

namespace AuthServer.AuthServer.Attributes
{
    public abstract class BnetServiceBase
    {
        [AttributeUsage(AttributeTargets.Method)]
        public sealed class BnetMethodAttribute : Attribute
        {
            public uint MethodId { get; set; }

            public BnetMethodAttribute(uint methodId)
            {
                MethodId = methodId;
            }
        }
    }
}
