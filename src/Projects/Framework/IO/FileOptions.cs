using System;

namespace Arctium_WoW_ClientDB_Viewer.ClientDB
{
    [Flags]
    enum FileOptions : int
    {
        None       = 0,
        DataOffset = 1,
        Unknown    = 2, // Some Index data stuff?!
        Index      = 4,
    }
}
