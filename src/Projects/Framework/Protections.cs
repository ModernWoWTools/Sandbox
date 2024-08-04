using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


[assembly: Obfuscation(Feature = "Apply to type *: apply to member * when method or constructor: virtualization", Exclude = true)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
[assembly: Obfuscation(Feature = "rename serializable symbols", Exclude = false)]
[assembly: Obfuscation(Feature = "rename symbols", Exclude = false)]
[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = false)]
[assembly: Obfuscation(Feature = "encrypt resources", Exclude = false)]
//[assembly: Obfuscation(Feature = "sanitize resources", Exclude = false)]


[assembly: Obfuscation(Feature = "Apply to type Framework.ObjectDefine*: renaming", Exclude = true)]
[assembly: Obfuscation(Feature = "Apply to type Framework.Misc.HttpHeader: all", Exclude = true, ApplyToMembers = false)]

namespace Arctium.WoW.Launcher
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class Protection
    {
        public static void Run()
        {
        }

        [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
        public static void Run2()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct UNICODE_STRING
    {
        internal ushort Length;
        internal ushort MaximumLength;
        internal IntPtr Buffer;
    }

    internal enum ObjectAttributes : uint
    {
        OBJ_CASE_INSENSITIVE = 0x40
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OBJECT_ATTRIBUTES
    {
        internal uint Length;
        internal IntPtr RootDirectory;
        internal IntPtr ObjectName;
        internal uint Attributes;
        internal IntPtr SecurityDescriptor;
        internal IntPtr SecurityQualityOfService;
    }

    internal enum _KEY_VALUE_INFORMATION_CLASS
    {
        KeyValueBasicInformation,
        KeyValueFullInformation,
        KeyValuePartialInformation,
        KeyValueFullInformationAlign64,
        KeyValuePartialInformationAlign64,
        KeyValueLayerInformation,
        MaxKeyValueInfoClass
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _KEY_VALUE_PARTIAL_INFORMATION
    {
        internal uint TitleIndex;
        internal uint Type;
        internal uint DataLength;
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProcessBasicInformation
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;

        public int Size => Marshal.SizeOf(typeof(ProcessBasicInformation));
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct LargeInteger
    {
        [FieldOffset(0)]
        public int Low;
        [FieldOffset(4)]
        public int High;
        [FieldOffset(0)]
        public long QuadPart;
    }
}
