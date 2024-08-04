using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using AuthServer.Game.Packets.PacketHandler;
using AuthServer.Network;

using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using Framework.Serialization;

#if !PUBLIC
public enum ReplayOpcodes
{
}

#endif


[Serializable()]
[DataContract]
public class ReplaySettings
{
}

namespace AuthServer.WorldServer.Managers
{
    
    public sealed class ReplayManager : Singleton<ReplayManager>
    {
        public bool Playing { get; internal set; }
        public IEnumerable<object> QueryPlayerNamePackets { get; internal set; }
        public bool ReplayMode { get; internal set; }

        public Dictionary<int, object> CreatureCache;
        public Dictionary<int, object> GameobjectCache;

        ReplayManager()
        {
           
        }

        public void Assign(ref WorldClass session)
        {
        }

        public void Assign2(ref WorldClass2 session2)
        {
        }
#if !PUBLIC

        public bool Load(string path)
        {
            

            return true;
        }

        public bool LoadAdditionalSniff(string path)
        {
           
            return true;
        }

#endif
        public void Play()
        {
#if !PUBLIC
            
#endif
        }
        public void PlayInstance()
        {
#if !PUBLIC
            
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WaitThread(double durationSeconds)
        {
            var durationTicks = (durationSeconds * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks)
            {

            }
        }
    }

    public class HotfixMessage
    {
        public uint Count { get; set; }
        public BinaryWriter HeaderData { get; } = new BinaryWriter(new MemoryStream());
        public BinaryWriter ContentData { get; } = new BinaryWriter(new MemoryStream());
    }
}
